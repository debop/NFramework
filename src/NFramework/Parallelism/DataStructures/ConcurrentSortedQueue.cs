using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// 멀티스레드에 안전하며, 키를 기준으로 정렬되는 큐입니다.
    /// </summary>
    /// <typeparam name="TKey">정렬을 위한 Key의 수형</typeparam>
    /// <typeparam name="TValue">요소의 수형</typeparam>
    [Serializable]
    public class ConcurrentSortedQueue<TKey, TValue> : IProducerConsumerCollection<KeyValuePair<TKey, TValue>>
        where TKey : IComparable<TKey> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private readonly object _syncLock = new object();
        private readonly SortedDictionary<TKey, TValue> _sortedDictionary = new SortedDictionary<TKey, TValue>();

        #region << Constructors >>

        public ConcurrentSortedQueue() {}

        public ConcurrentSortedQueue(IEnumerable<KeyValuePair<TKey, TValue>> collection) {
            collection.ShouldNotBeNull("collection");
            collection.RunEach(item => _sortedDictionary.Add(item.Key, item.Value));
        }

        #endregion

        /// <summary>
        /// 큐에 요소를 추가합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Enqueue(TKey key, TValue value) {
            Enqueue(new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// 큐에 요소를 추가합니다.
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(KeyValuePair<TKey, TValue> item) {
            lock(_syncLock)
                _sortedDictionary.Add(item.Key, item.Value);
        }

        /// <summary>
        /// 큐에서 요소 꺼내기를 시도합니다.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryDequeue(out KeyValuePair<TKey, TValue> item) {
            item = default(KeyValuePair<TKey, TValue>);

            lock(_syncLock) {
                if(_sortedDictionary.IsNotEmptySequence()) {
                    item = _sortedDictionary.First();
                    return _sortedDictionary.Remove(item.Key);
                }
            }
            return false;
        }

        /// <summary>
        /// 큐의 첫번째 요소를 Peek 해 봅니다.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryPeek(out KeyValuePair<TKey, TValue> item) {
            item = default(KeyValuePair<TKey, TValue>);

            lock(_syncLock) {
                if(_sortedDictionary.IsNotEmptySequence()) {
                    item = _sortedDictionary.First();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 큐를 비웁니다.
        /// </summary>
        public void Clear() {
            lock(_syncLock)
                _sortedDictionary.Clear();
        }

        /// <summary>
        /// 큐가 비었는지 알려줍니다.
        /// </summary>
        public bool IsEmpty {
            get { return (Count == 0); }
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// 컬렉션을 반복하는 열거자를 반환합니다.
        /// </summary>
        /// <returns>
        /// 컬렉션을 반복하는 데 사용할 수 있는 <see cref="T:System.Collections.Generic.IEnumerator`1"/>입니다.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            var array = ToArray();
            return array.AsEnumerable().GetEnumerator();
        }

        /// <summary>
        /// 컬렉션을 반복하는 열거자를 반환합니다.
        /// </summary>
        /// <returns>
        /// 컬렉션을 반복하는 데 사용할 수 있는 <see cref="T:System.Collections.IEnumerator"/> 개체입니다.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection

        /// <summary>
        /// 특정 <see cref="T:System.Array"/> 인덱스부터 시작하여 <see cref="T:System.Collections.ICollection"/>의 요소를 <see cref="T:System.Array"/>에 복사합니다.
        /// </summary>
        /// <param name="array"><see cref="T:System.Collections.ICollection"/>에서 복사한 요소의 대상인 일차원 <see cref="T:System.Array"/>입니다.<see cref="T:System.Array"/>의 인덱스는 0부터 시작해야 합니다.</param><param name="index"><paramref name="array"/>에서 복사가 시작되는 0부터 시작하는 인덱스입니다. </param><exception cref="T:System.ArgumentNullException"><paramref name="array"/>이 null입니다. </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/>가 0보다 작습니다. </exception><exception cref="T:System.ArgumentException"><paramref name="array"/>가 다차원인 경우- 또는 - 소스 <see cref="T:System.Collections.ICollection"/>의 요소 수가 <paramref name="index"/>에서 대상 <paramref name="array"/> 끝까지 사용 가능한 공간보다 큰 경우 </exception><exception cref="T:System.ArgumentException">소스 <see cref="T:System.Collections.ICollection"/> 형식을 대상 <paramref name="array"/> 형식으로 자동 캐스팅할 수 없는 경우 </exception>
        void ICollection.CopyTo(Array array, int index) {
            lock(_syncLock)
                ((ICollection)_sortedDictionary).CopyTo(array, index);
        }

        /// <summary>
        /// <see cref="T:System.Collections.ICollection"/>에 포함된 요소 수를 가져옵니다.
        /// </summary>
        /// <returns>
        /// <see cref="T:System.Collections.ICollection"/>에 포함된 요소 수입니다.
        /// </returns>
        public int Count {
            get {
                lock(_syncLock)
                    return _sortedDictionary.Count;
            }
        }

        /// <summary>
        /// <see cref="T:System.Collections.ICollection"/>에 대한 액세스를 동기화하는 데 사용할 수 있는 개체를 가져옵니다.
        /// </summary>
        /// <returns>
        /// <see cref="T:System.Collections.ICollection"/>에 대한 액세스를 동기화하는 데 사용할 수 있는 개체입니다.
        /// </returns>
        object ICollection.SyncRoot {
            get { return _syncLock; }
        }

        /// <summary>
        /// <see cref="T:System.Collections.ICollection"/>에 대한 액세스가 동기화되어 스레드로부터 안전하게 보호되는지 여부를 나타내는 값을 가져옵니다.
        /// </summary>
        /// <returns>
        /// <see cref="T:System.Collections.ICollection"/>에 대한 액세스가 동기화되어 스레드로부터 안전하게 보호되면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        bool ICollection.IsSynchronized {
            get { return true; }
        }

        #endregion

        #region Implementation of IProducerConsumerCollection<KeyValuePair<TKey,TValue>>

        /// <summary>
        /// 지정된 인덱스부터 시작하여 <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1"/>의 요소를 <see cref="T:System.Array"/>에 복사합니다.
        /// </summary>
        /// <param name="array"><see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1"/>에서 복사한 요소의 대상인 1차원 <see cref="T:System.Array"/>입니다. 배열의 인덱스는 0부터 시작해야 합니다.</param><param name="index"><paramref name="array"/>에서 복사가 시작되는 인덱스(0부터 시작)입니다.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/>는 null 참조(Visual Basic에서는 Nothing)입니다.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/>가 0보다 작은 경우</exception><exception cref="T:System.ArgumentException"><paramref name="index"/>가 <paramref name="array"/>의 길이와 같거나 큰 경우 -또는- 소스 <see cref="T:System.Collections.Concurrent.ConcurrentQueue`1"/>의 요소 수가 대상 <paramref name="array"/>의 <paramref name="index"/>부터 끝까지의 사용 가능한 공간보다 큰 경우.</exception>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index) {
            lock(_syncLock)
                _sortedDictionary.CopyTo(array, index);
        }

        /// <summary>
        /// <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1"/>에 개체를 추가하려고 시도합니다.
        /// </summary>
        /// <returns>
        /// 개체가 성공적으로 추가되었으면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        /// <param name="item"><see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1"/>에 추가할 개체입니다.</param><exception cref="T:System.ArgumentException"><paramref name="item"/>은 이 컬렉션에 올바르지 않습니다.</exception>
        bool IProducerConsumerCollection<KeyValuePair<TKey, TValue>>.TryAdd(KeyValuePair<TKey, TValue> item) {
            Enqueue(item);
            return true;
        }

        /// <summary>
        /// <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1"/>에서 개체를 제거하고 반환하려고 시도합니다.
        /// </summary>
        /// <returns>
        /// 개체가 성공적으로 제거되고 반환되었으면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        /// <param name="item">이 메서드가 반환될 경우 개체가 성공적으로 제거되고 반환되었으면 <paramref name="item"/>에는 제거된 개체가 들어 있습니다.제거할 수 있는 개체가 없으면 이 값은 지정되지 않습니다.</param>
        bool IProducerConsumerCollection<KeyValuePair<TKey, TValue>>.TryTake(out KeyValuePair<TKey, TValue> item) {
            return TryDequeue(out item);
        }

        /// <summary>
        /// <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1"/>에 포함된 요소를 새 배열에 복사합니다.
        /// </summary>
        /// <returns>
        /// <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1"/>에서 복사된 요소를 포함하는 새 배열입니다.
        /// </returns>
        public KeyValuePair<TKey, TValue>[] ToArray() {
            lock(_syncLock) {
                return _sortedDictionary.ToArray();
            }
        }

        #endregion
        }
}