using System;
using System.Collections;
using System.Collections.Generic;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Collections {
    /// <summary>
    /// SkipList 알고리듬을 구현한 것이다.
    /// SortedList와 BinarySearchTree 의 장점만을 조합해서 만든 것이다.
    /// </summary>
    /// <remarks>
    /// 기본적으로는 LinkedList이지만 정렬된 각 노드들이 Next 노드를 여러개를 가지고 있어서, 검색에서 빠른 결과를 가져온다.
    /// 단 구성하는 값은 중복을 허용하지 않는다.
    /// </remarks>
    /// <typeparam name="T">요소 타입</typeparam>
    [Serializable]
    public class SkipList<T> : ICollection<T>, ICollection {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        #region << Field >>

        private SkipListNode<T> _head;

        /// <summary>
        /// SkipListNode의 Height를 결정하기 위한 확률 요소
        /// </summary>
        [CLSCompliant(false)] protected readonly double _probability = 0.5;

        private int _count;
        private readonly Random _rnd;
        private readonly IComparer<T> _comparer;

        #endregion

        #region << Constructor >>

        /// <summary>
        /// 기본생성자
        /// </summary>
        public SkipList()
            : this(-1, null) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="randomSeed">내부 난수 시드 값</param>
        public SkipList(int randomSeed)
            : this(randomSeed, null) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="comparer">요소 비교자</param>
        public SkipList(IComparer<T> comparer)
            : this(-1, comparer) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="randomSeed">내부 난수 시드 값</param>
        /// <param name="comparer">요소 비교자</param>
        public SkipList(int randomSeed, IComparer<T> comparer) {
            _head = new SkipListNode<T>(1);
            _count++;
            _rnd = (randomSeed < 0) ? new Random() : new Random(randomSeed);
            _comparer = (comparer != null) ? comparer : Comparer<T>.Default;
        }

        #endregion

        #region << Property >>

        /// <summary>
        /// 검색을 위해 부가적으로 링크를 가지고 있는데 Height 가 높을 수록 검색 속도는 빨라지나 조작에는 성능이 느려진다.
        /// </summary>
        public int Height {
            get { return _head.Height; }
        }

        #endregion

        #region << Method >>

        /// <summary>
        /// 새 노드의 Height를 결정한다. (주사위로)
        /// 반환 값은 1 과 maxHeight 사이 값이다.
        /// </summary>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        protected virtual int ChooseRandomHeight(int maxHeight) {
            int height = 1;
            while(_rnd.NextDouble() < _probability && height < maxHeight)
                height++;

            return height;
        }

        /// <summary>
        /// SkipList에서 추가/삭제시에 Update해야 할 기존 노드들을 찾아서 빌드한다.
        /// </summary>
        /// <param name="value">추가나 삭제할 값</param>
        /// <returns></returns>
        protected SkipListNodeCollection<T> BuildUpdateTable(T value) {
            var updates = new SkipListNodeCollection<T>(_head.Height);
            var current = _head;

            for(int i = _head.Height - 1; i >= 0; i--) {
                while(current[i] != null && _comparer.Compare(current[i].Value, value) < 0) {
                    current = current[i];
                }
                updates[i] = current;
            }

            return updates;
        }

        /// <summary>
        /// 인스턴스를 본제한다.
        /// </summary>
        /// <returns></returns>
        public virtual SkipList<T> Clone() {
            var cloned = new SkipList<T>(-1, _comparer);

            foreach(T value in this)
                cloned.Add(value);

            return cloned;
        }

        /// <summary>
        /// 인스턴스 정보를 문자열로 만든다.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            // return RwCollection.AsString<T>(this);
            return this.CollectionToString();
        }

        /// <summary>
        /// 각 노드들의 Link 관계까지 자세히 표현한 정보를 반환한다.
        /// </summary>
        public virtual string ToStringDetail() {
            //string delimiter = string.Empty;
            //StringBuilder sb = new StringBuilder();

            //SkipListNode<T> current = _head[0];

            //while (current != null)
            //{
            //    sb.Append(current.Value.AsString());
            //    sb.Append(" [ H=").Append(current.Height);

            //    for (int i = current.Height - 1; i >= 0; i--)
            //    {
            //        sb.Append(" | ");
            //        sb.Append((current[i] == null) ? "null" : current[i].Value.AsString());
            //    }
            //    sb.Append(" ]; ");

            //    current = current[0];
            //}

            //return sb.AsString();
            return this.CollectionToString();
        }

        #endregion

        #region ICollection<T> Members

        /// <summary>
        /// 지정된 값을 가진 요소를 생성해서 리스트에 추가한다.
        /// </summary>
        /// <param name="value"></param>
        public void Add(T value) {
            // 추가에 의해 변경되어야 할 기존 노드들을 구한다.
            var updates = BuildUpdateTable(value);
            var current = updates[0];

            // 중복인지 검사해서 중복이면 추가 안함!!!
            if(current[0] != null && _comparer.Compare(current[0].Value, value) == 0)
                return;

            var newNode = new SkipListNode<T>(value, ChooseRandomHeight(_head.Height + 1));
            _count++;

            // 새로운 노드의 Height가 헤드보다 크다면, 헤드 노드의 크기를 키우고, 그 곳을 새로운 노드를 가르키게 한다.
            if(newNode.Height > _head.Height) {
                _head.IncHeight();
                _head[_head.Height - 1] = newNode;
            }

            // 새로운 노드의 자리를 정리한다.
            for(int i = 0; i < newNode.Height; i++) {
                if(i < updates.Count) {
                    newNode[i] = updates[i][i];
                    updates[i][i] = newNode;
                }
            }
        }

        /// <summary>
        /// 지정된 값들을 리스트에 추가한다.
        /// </summary>
        /// <param name="values"></param>
        public void AddRange(params T[] values) {
            if(values.Length == 0)
                return;

            foreach(T value in values)
                Add(value);
        }

        /// <summary>
        /// 리스트의 모든 요소를 삭제한다.
        /// </summary>
        public void Clear() {
            _head = null;
            _head = new SkipListNode<T>(1);
            _count = 0;
        }

        /// <summary>
        /// 리스트에 지정된 값을 가진 요소가 있는지 검사한다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <returns>존재 여부</returns>
        public bool Contains(T value) {
            var current = _head;

            //먼곳을 가르키는 Node부터 비교한다.
            for(int i = _head.Height - 1; i >= 0; i--) {
                while(current[i] != null) {
                    int result = _comparer.Compare(current[i].Value, value);

                    if(result == 0)
                        return true;
                    else if(result < 0)
                        current = current[i]; // 현재 Height가 가르키는 놈에서 찾음
                    else
                        break; // 현재 노드의 height를 낮추어서 검색
                }
            }

            return false;
        }

        /// <summary>
        /// 지정된 배열에 리스트의 값들을 복사한다.
        /// </summary>
        /// <param name="array">대상 배열</param>
        public void CopyTo(T[] array) {
            CopyTo(array, array.GetLowerBound(0));
        }

        /// <summary>
        /// 지정된 배열에 리스트의 값들을 복사한다.
        /// </summary>
        /// <param name="array">대상 배열</param>
        /// <param name="arrayIndex">복사 시작 위치</param>
        public void CopyTo(T[] array, int arrayIndex) {
            array.ShouldNotBeNull("array");
            arrayIndex.ShouldBePositiveOrZero("arrayIndex");

            if(array.Length - arrayIndex > _count)
                throw new ArgumentOutOfRangeException(
                    "arrayIndex larger than capacity, or array length is small than capacity");

            var current = _head[0];

            foreach(T value in this)
                array[arrayIndex++] = value;
        }

        /// <summary>
        /// 리스트가 가진 요소의 수
        /// </summary>
        /// <value></value>
        public int Count {
            get { return _count; }
        }

        /// <summary>
        /// 읽기전용 여부
        /// </summary>
        public bool IsReadOnly {
            get { return false; }
        }

        /// <summary>
        /// 지정된 값을 가진 요소를 리스트에서 삭제한다.
        /// </summary>
        /// <param name="value">요소 값</param>
        /// <returns>제거 여부</returns>
        public bool Remove(T value) {
            var updates = BuildUpdateTable(value);
            var current = updates[0][0];

            // 삭제할 노드가 맞다면
            if(current != null && _comparer.Compare(current.Value, value) == 0) {
                _count--;

                // 삭제될 노드의 기존 정보를 변경한다.
                for(int i = 0; i < _head.Height; i++) {
                    if(updates[i][i] != current)
                        break;
                    else
                        updates[i][i] = current[i];
                }

                // 가능하다면 리스트의 Height를 낮춘다.
                if(_head[_head.Height - 1] == null)
                    _head.DecHeight();

                current = null;
                return true;
            }
            else
                return false;
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// 반복자
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() {
            var current = _head[0];

            while(current != null) {
                yield return current.Value;
                current = current[0];
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index) {
            array.ShouldNotBeNull("array");
            index.ShouldBePositiveOrZero("index");

            if(array.Length - index < _count)
                throw new ArgumentOutOfRangeException("array",
                                                      @"array size is smaller than binary tree size or index is greater than proper offset.");

            var values = new T[_count];
            CopyTo(values, 0);

            Array.Copy(values, 0, array, index, array.Length - index);
        }

        int ICollection.Count {
            get { return _count; }
        }

        bool ICollection.IsSynchronized {
            get { return false; }
        }

        object ICollection.SyncRoot {
            get { return null; }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// Node of SkipList
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SkipListNode<T> : Node<T> {
        private readonly int _height = 1;

        #region << Constructor >>

        /// <summary>
        /// default constructor
        /// </summary>
        public SkipListNode() {}

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="height">count of searching node.</param>
        public SkipListNode(int height) {
            height.ShouldBePositive("height");
            _height = height;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="value">node value.</param>
        /// <param name="height">count of searching node.</param>
        public SkipListNode(T value, int height)
            : base(value) {
            height.ShouldBePositive("height");
            _height = height;
        }

        #endregion

        #region << Property >>

        /// <summary>
        /// 현재 노드와 연관된 노드들의 컬렉션입니다.
        /// </summary>
        public new SkipListNodeCollection<T> Nodes {
            get {
                if(base.Nodes == null)
                    base.Nodes = new SkipListNodeCollection<T>(_height);

                return (SkipListNodeCollection<T>)base.Nodes;
            }
            set {
                if(value != null)
                    base.Nodes = value;
            }
        }

        /// <summary>
        /// 현재 노드와 연관된 노드들의 갯수 (컬렉션의 크기입니다.)
        /// </summary>
        /// <value></value>
        public int Height {
            get { return Nodes.Count; }
        }

        /// <summary>
        /// 현재 노드와 관련된 노드를 반환한다.
        /// </summary>
        public SkipListNode<T> this[int index] {
            get { return Nodes[index]; }
            set { Nodes[index] = value; }
        }

        #endregion

        #region << Method >>

        /// <summary>
        /// 현재 노드와 연관된 노드를 추가하기 위해 버퍼의 크기를 1씩 증가시킵니다.
        /// </summary>
        internal void IncHeight() {
            Nodes.IncHeight();
        }

        /// <summary>
        /// 현재 노드와 연관된 노드를 추가하기 위해 버퍼의 크기를 1씩 감소시킵니다.
        /// </summary>
        internal void DecHeight() {
            Nodes.DecHeight();
        }

        #endregion

        /// <summary>
        /// 노드가 가진 값을 반환한다.
        /// </summary>
        /// <returns>ItemValue의 정보, null 이면 "null"을 반환한다.</returns>
        public override string ToString() {
            return string.Format("{0}, Height=[{1}]", base.ToString(), _height);
        }
    }

    /// <summary>
    /// Collection of <see cref="SkipListNode{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SkipListNodeCollection<T> : NodeCollection<T> {
        #region << Constructor >>

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="height">collection buffer size</param>
        public SkipListNodeCollection(int height)
            : base(height) {}

        #endregion

        #region << Property >>

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns><see cref="SkipListNode{T}"/> instance.</returns>
        public new SkipListNode<T> this[int index] {
            get { return base[index] as SkipListNode<T>; }
            set { base[index] = value; }
        }

        #endregion

        #region << Method >>

        /// <summary>
        /// Increse collection height
        /// </summary>
        internal void IncHeight() {
            Add(default(SkipListNode<T>));
        }

        /// <summary>
        /// Decrease collection height
        /// </summary>
        internal void DecHeight() {
            RemoveAt(Count - 1);
        }

        #endregion
    }
}