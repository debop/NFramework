using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using NSoft.NFramework.Threading;

namespace NSoft.NFramework.Collections.Concurrent {
    /// <summary>
    ///  SkipList 방식의 내부저장소를 가진 <see cref="BlockingCollection{T}"/>입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ConcurrentSkipList<T> : IProducerConsumerCollection<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        #region << Node Class >>

        private class Node {
            public readonly int Key;
            public readonly T Value;
            public readonly int TopLayer;
            public readonly Node[] Nexts;
            public volatile bool Marked;
            public volatile bool FullyLinked;
            public readonly SpinLock SpinLock;

            public Node(int key, T value, int heightValue) {
                Key = key;
                Value = value;
                TopLayer = heightValue;
                Nexts = new Node[heightValue + 1];
                SpinLock = new SpinLock(false);
                Marked = FullyLinked = false;
            }
        }

        #endregion

        private const int MaxHeight = 200;

        private static readonly Random rnd = new ThreadSafeRandom();

        [ThreadStatic] private static Node[] _preds;

        [ThreadStatic] private static Node[] _succs;

        private int _count;
        private Node _leftSentinel;
        private Node _rightSentinel;
        private uint _randomSeed;

        private readonly Func<T, int> @getKey;

        public ConcurrentSkipList() : this(value => value.GetHashCode()) {}
        public ConcurrentSkipList(IEqualityComparer<T> comparer) : this(value => comparer.GetHashCode(value)) {}

        public ConcurrentSkipList(Func<T, int> @hasher) {
            @hasher.ShouldNotBeNull("@hasher");
            @getKey = @hasher;
            Init();
        }

        private void Init() {
            if(_succs == null)
                _succs = new Node[MaxHeight];
            if(_preds == null)
                _preds = new Node[MaxHeight];

            _leftSentinel = new Node(int.MinValue, default(T), MaxHeight);
            _rightSentinel = new Node(int.MaxValue, default(T), MaxHeight);

            for(var i = 0; i < MaxHeight; i++)
                _leftSentinel.Nexts[i] = _rightSentinel;

            _randomSeed = ((uint)Math.Abs(rnd.Next())) | 0x0100;
        }

        public bool TryAdd(T item) {
            item.ShouldNotBeDefault("item");

            if(IsDebugEnabled)
                log.Debug("요소 추가를 시도합니다... item=[{0}]", item);

            CleanArrays();
            var topLayer = GetRandomLevel(ref _randomSeed);
            var v = @getKey.Invoke(item);

            while(true) {
                var found = FindNode(v, _preds, _succs);

                if(found != -1) {
                    var nodeFound = _succs[found];

                    if(nodeFound.Marked == false) {
                        var sw = new System.Threading.SpinWait();
                        while(nodeFound.FullyLinked == false) {
                            sw.SpinOnce();
                        }
                        return false;
                    }
                    continue;
                }
                var highestLocked = -1;
                try {
                    var valid = LockNodes(topLayer, ref highestLocked,
                                          (layer, pred, succ) => !pred.Marked && !succ.Marked && pred.Nexts[layer] == succ);
                    if(!valid)
                        continue;

                    var newNode = new Node(v, item, topLayer);
                    for(var layer = 0; layer <= topLayer; layer++) {
                        newNode.Nexts[layer] = _succs[layer];
                        _preds[layer].Nexts[layer] = newNode;
                    }
                    newNode.FullyLinked = true;
                }
                finally {
                    Unlock(_preds, highestLocked);
                }
                Interlocked.Increment(ref _count);
                return true;
            }
        }

        bool IProducerConsumerCollection<T>.TryTake(out T item) {
            throw new NotSupportedException();
        }

        public T[] ToArray() {
            var countSnapshot = _count;
            var result = new T[countSnapshot];

            CopyTo(result, 0);
            return result;
        }

        public void CopyTo(T[] array, int arrayIndex) {
            var i = arrayIndex;
            foreach(var item in GetAllItems())
                array[i++] = item;
        }

        void ICollection.CopyTo(Array array, int index) {
            var result = array as T[];

            if(result == null)
                return;

            CopyTo(result, index);
        }

        public bool Remove(T item) {
            item.ShouldNotBeDefault("item");

            if(IsDebugEnabled)
                log.Debug("요소[{0}]를 삭제합니다...", item);

            CleanArrays();

            Node toDelete = null;
            var isMarked = false;
            var topLayer = -1;
            var v = @getKey(item);

            while(true) {
                var found = FindNode(v, _preds, _succs);

                if(isMarked || (found != -1 && CanDelete(_succs[found], found))) {
                    // If not marked then logically delete the node
                    if(!isMarked) {
                        toDelete = _succs[found];
                        topLayer = toDelete.TopLayer;

                        var lockTaken = false;
                        toDelete.SpinLock.Enter(ref lockTaken);
                        // Now that we have the lock, check if the node hasn't already been marked
                        if(toDelete.Marked) {
                            toDelete.SpinLock.Exit(true);
                            return false;
                        }
                        toDelete.Marked = true;
                        isMarked = true;
                    }

                    var highestLocked = -1;
                    try {
                        var valid = LockNodes(topLayer, ref highestLocked,
                                              (layer, pred, succ) => !pred.Marked && pred.Nexts[layer] == succ);
                        if(!valid)
                            continue;

                        for(int layer = topLayer; layer >= 0; layer--) {
                            _preds[layer].Nexts[layer] = toDelete.Nexts[layer];
                        }
                        toDelete.SpinLock.Exit(true);
                    }
                    finally {
                        Unlock(_preds, highestLocked);
                    }
                    Interlocked.Decrement(ref _count);
                    return true;
                }

                return false;
            }
        }

        public bool Contains(T item) {
            item.ShouldNotBeNull("item");
            return ContainsFromHash(@getKey(item));
        }

        public int Count {
            get { return _count; }
        }

        int ICollection.Count {
            get { return Count; }
        }

        public object SyncRoot {
            get { return this; }
        }

        public bool IsSynchronized {
            get { return true; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public IEnumerator<T> GetEnumerator() {
            return GetAllItems().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private IEnumerable<T> GetAllItems() {
            Node curr = _leftSentinel;

            while((curr = curr.Nexts[0]) != _rightSentinel) {
                var sw = new System.Threading.SpinWait();

                while(curr.FullyLinked == false) {
                    sw.SpinOnce();
                }
                yield return curr.Value;
            }
        }

        internal bool ContainsFromHash(int hash) {
            CleanArrays();
            var found = FindNode(hash, _preds, _succs);

            return found != -1 &&
                   _succs[found].FullyLinked &&
                   _succs[found].Marked == false;
        }

        internal bool GetFromHash(int hash, out T value) {
            if(IsDebugEnabled)
                log.Debug("GetFromHash... hash=[{0}]", hash);

            value = default(T);
            CleanArrays();

            var found = FindNode(hash, _preds, _succs);

            if(found == -1)
                return false;

            bool lockTaken = false;
            try {
                _succs[found].SpinLock.Enter(ref lockTaken);
                var node = _succs[found];

                if(node.FullyLinked && node.Marked == false) {
                    value = node.Value;
                    return true;
                }
            }
            finally {
                _succs[found].SpinLock.Exit(true);
            }

            return false;
        }

        private static void Unlock(IList<Node> preds, int highestLocked) {
            for(var i = 0; i <= highestLocked; i++) {
                preds[i].SpinLock.Exit(true);
            }
        }

        private static bool LockNodes(int topLayer, ref int highestLocked, Func<int, Node, Node, bool> validityTest) {
            Node prevPred = null;
            var valid = true;

            for(var layer = 0; valid && (layer <= topLayer); layer++) {
                var pred = _preds[layer];
                var succ = _succs[layer];

                if(pred != prevPred) {
                    var lockTaken = false;

                    // NET_4_0에서만 가능, Silverlight에서는 Enter()만 사용하면 됨
                    pred.SpinLock.Enter(ref lockTaken);
                    highestLocked = layer;
                    prevPred = pred;
                }

                valid = validityTest(layer, pred, succ);
            }

            return valid;
        }

        private int FindNode(int v, IList<Node> preds, IList<Node> succs) {
            Guard.Assert(() => preds.Count == MaxHeight && succs.Count == MaxHeight,
                         "preds 와 succs가 충분히 큰 길이를 가지지 않았습니다. preds.Length=[{0}], succs.Length=[{1}]",
                         preds.Count, succs.Count);

            var found = -1;
            var pred = _leftSentinel;

            for(var layer = MaxHeight - 1; layer >= 0; layer--) {
                var curr = pred.Nexts[layer];

                while(v > curr.Key) {
                    pred = curr;
                    curr = curr.Nexts[layer];
                }

                if(found == -1 && v == curr.Key)
                    found = layer;

                preds[layer] = pred;
                succs[layer] = curr;
            }

            return found;
        }

        private static bool CanDelete(Node candidate, int found) {
            return candidate.FullyLinked &&
                   candidate.TopLayer == found &&
                   candidate.Marked == false;
        }

        private static int GetRandomLevel(ref uint randomSeed) {
            var x = randomSeed;
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
            randomSeed = x;

            if((x & 0x80000001) != 0)
                return 0;

            var level = 1;

            while(((x >>= 1) & 1) != 0)
                ++level;

            return level;
        }

        private static void CleanArrays() {
            if(IsDebugEnabled)
                log.Debug("내부 배열을 초기화합니다...");

            if(_succs == null)
                _succs = new Node[MaxHeight];
            if(_preds == null)
                _preds = new Node[MaxHeight];

            Array.Clear(_preds, 0, _preds.Length);
            Array.Clear(_succs, 0, _succs.Length);
        }
    }
}