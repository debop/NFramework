using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Collections.Concurrent {
    /// <summary>
    /// Thread-Safe 한 Multi-Set 자료구조입니다. (Key-Set{TValue}) 형태입니다.
    /// </summary>
    /// <typeparam name="TKey">키의 수형</typeparam>
    /// <typeparam name="TValue">값의 수형</typeparam>
    /// <seealso cref="MultiMap{TKey, TValue}"/>
    [Serializable]
    public class ConcurrentMultiSet<TKey, TValue> : IDictionary<TKey, ISet<TValue>> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private ConcurrentDictionary<TKey, ISet<TValue>> _map = new ConcurrentDictionary<TKey, ISet<TValue>>();
        private readonly object _syncLock = new object();

        /// <summary>
        /// 생성자
        /// </summary>
        public ConcurrentMultiSet() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="collection"></param>
        public ConcurrentMultiSet(IEnumerable<KeyValuePair<TKey, IEnumerable<TValue>>> collection) {
            collection.ShouldNotBeNull("collection");

            foreach(var pair in collection)
                if(pair.Value != null)
                    Add(pair.Key, pair.Value.ToArray<TValue>());
        }

        public ConcurrentMultiSet(IEqualityComparer<TKey> keyComparer) {
            keyComparer.ShouldNotBeNull("keyComparer");
            InnerMap = new ConcurrentDictionary<TKey, ISet<TValue>>(keyComparer);
        }

        protected ConcurrentDictionary<TKey, ISet<TValue>> InnerMap {
            get { return _map ?? (_map = new ConcurrentDictionary<TKey, ISet<TValue>>()); }
            set { _map = value; }
        }

        /// <summary>
        /// 키의 갯수
        /// </summary>
        public int KeyCount {
            get { return InnerMap.Keys.Count; }
        }

        /// <summary>
        /// 전체 Value의 갯수 (하나의 키에 여러 개의 Value가 매핑되어 있으므로 실제 Value는 Key의 갯수 이상이다.) 단 여기서 다른 Key에 존재하는 중복된 값도 Counting 한다.
        /// </summary>
        /// <remarks>전체 Value 갯수를 계산하는데 시간이 걸리므로, 한번 호출해서 변수에 할당하여 사용하세요.</remarks>
        public int ValueCount {
            get { return InnerMap.Keys.Sum(key => InnerMap[key].Count); }
        }

        /// <summary>
        /// 요소를 추가합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value) {
            this[key].Add(value);
        }

        /// <summary>
        /// 컬렉션을 반복하는 열거자를 반환합니다.
        /// </summary>
        /// <returns>
        /// 컬렉션을 반복하는 데 사용할 수 있는 <see cref="T:System.Collections.Generic.IEnumerator`1"/>입니다.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<KeyValuePair<TKey, ISet<TValue>>> GetEnumerator() {
            return InnerMap.GetEnumerator();
        }

        /// <summary>
        /// 컬렉션을 반복하는 열거자를 반환합니다.
        /// </summary>
        /// <returns>
        /// 컬렉션을 반복하는 데 사용할 수 있는 <see cref="T:System.Collections.IEnumerator"/> 개체입니다.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        /// <see cref="T:System.Collections.Generic.ICollection`1"/>에 항목을 추가합니다.
        /// </summary>
        /// <param name="item"><see cref="T:System.Collections.Generic.ICollection`1"/>에 추가할 개체입니다.</param><exception cref="T:System.NotSupportedException"><see cref="T:System.Collections.Generic.ICollection`1"/>가 읽기 전용인 경우</exception>
        public void Add(KeyValuePair<TKey, ISet<TValue>> item) {
            lock(_syncLock)
                foreach(var v in item.Value)
                    this[item.Key].Add(v);
        }

        /// <summary>
        /// <see cref="T:System.Collections.Generic.ICollection`1"/>에서 항목을 모두 제거합니다.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException"><see cref="T:System.Collections.Generic.ICollection`1"/>가 읽기 전용인 경우 </exception>
        public void Clear() {
            InnerMap.Clear();
        }

        /// <summary>
        /// <see cref="T:System.Collections.Generic.ICollection`1"/>에 특정 값이 들어 있는지 여부를 확인합니다.
        /// </summary>
        /// <returns>
        /// <paramref name="item"/>이 <see cref="T:System.Collections.Generic.ICollection`1"/>에 있으면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        /// <param name="item"><see cref="T:System.Collections.Generic.ICollection`1"/>에서 찾을 개체입니다.</param>
        public bool Contains(KeyValuePair<TKey, ISet<TValue>> item) {
            if(item.Value.IsEmptySequence())
                return false;

            if(ContainsKey(item.Key) == false)
                return false;

            lock(_syncLock) {
                var set = this[item.Key];
                return item.Value.All(v => set.Contains(v));
            }
        }

        /// <summary>
        /// <paramref name="value"/>를 값으로 가지고 있는지 검사합니다.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ContainsValue(TValue value) {
            lock(_syncLock)
                return InnerMap.SelectMany(item => item.Value).Any(v => Equals(v, value));
        }

        /// <summary>
        /// 특정 <see cref="T:System.Array"/> 인덱스부터 시작하여 <see cref="T:System.Collections.Generic.ICollection`1"/>의 요소를 <see cref="T:System.Array"/>에 복사합니다.
        /// </summary>
        /// <param name="array"><see cref="T:System.Collections.Generic.ICollection`1"/>에서 복사한 요소의 대상인 일차원 <see cref="T:System.Array"/>입니다. <see cref="T:System.Array"/>의 인덱스는 0부터 시작해야 합니다.</param>
        /// <param name="arrayIndex"><paramref name="array"/>에서 복사가 시작되는 인덱스(0부터 시작)입니다.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/>가 null인 경우</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/>가 0보다 작은 경우</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="array"/>가 다차원인 경우-또는-소스 <see cref="T:System.Collections.Generic.ICollection`1"/>의 요소 수가 
        /// <paramref name="arrayIndex"/>에서 대상 <paramref name="array"/> 끝까지 사용 가능한 공간보다 큰 경우-또는-<paramref name="T"/> 형식을 대상 <paramref name="array"/>의 형식으로 자동 캐스팅할 수 없는 경우
        /// </exception>
        public void CopyTo(KeyValuePair<TKey, ISet<TValue>>[] array, int arrayIndex) {
            lock(_syncLock)
                InnerMap.ToArray().CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// <see cref="T:System.Collections.Generic.ICollection`1"/>에서 맨 처음 발견되는 특정 개체를 제거합니다.
        /// </summary>
        /// <returns>
        /// <paramref name="item"/>이 <see cref="T:System.Collections.Generic.ICollection`1"/>에서 성공적으로 제거되었으면 true이고, 그렇지 않으면 false입니다.이 메서드는 <paramref name="item"/>이 원래 <see cref="T:System.Collections.Generic.ICollection`1"/>에 없는 경우에도 false를 반환합니다.
        /// </returns>
        /// <param name="item"><see cref="T:System.Collections.Generic.ICollection`1"/>에서 제거할 개체입니다.</param>
        /// <exception cref="T:System.NotSupportedException"><see cref="T:System.Collections.Generic.ICollection`1"/>가 읽기 전용인 경우</exception>
        public bool Remove(KeyValuePair<TKey, ISet<TValue>> item) {
            if(item.Value.IsEmptySequence())
                return false;
            if(ContainsKey(item.Key) == false)
                return false;

            var removed = false;
            lock(_syncLock) {
                var set = this[item.Key];

                foreach(var v in item.Value)
                    if(set.Contains(v))
                        removed = set.Remove(v);
            }
            return removed;
        }

        /// <summary>
        /// 요소의 수 (Key 의 수)
        /// </summary>
        /// <returns>
        /// <see cref="T:System.Collections.Generic.ICollection`1"/>에 포함된 요소 수입니다.
        /// </returns>
        public int Count {
            get { return InnerMap.Count; }
        }

        /// <summary>
        /// <see cref="T:System.Collections.Generic.ICollection`1"/>이 읽기 전용인지 여부를 나타내는 값을 가져옵니다.
        /// </summary>
        /// <returns>
        /// <see cref="T:System.Collections.Generic.ICollection`1"/>이 읽기 전용이면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        public bool IsReadOnly {
            get { return false; }
        }

        /// <summary>
        /// <see cref="T:System.Collections.Generic.IDictionary`2"/>에 지정된 키가 있는 요소가 포함되어 있는지 여부를 확인합니다.
        /// </summary>
        /// <returns>
        /// <see cref="T:System.Collections.Generic.IDictionary`2"/>에 해당 키가 있는 요소가 포함되어 있으면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        /// <param name="key"><see cref="T:System.Collections.Generic.IDictionary`2"/>에서 찾을 수 있는 키입니다.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/>가 null인 경우</exception>
        public bool ContainsKey(TKey key) {
            return InnerMap.ContainsKey(key);
        }

        /// <summary>
        /// 제공된 키와 값이 있는 요소를 <see cref="T:System.Collections.Generic.IDictionary`2"/>에 추가합니다.
        /// </summary>
        /// <param name="key">추가할 요소의 키로 사용할 개체입니다.</param><param name="values">추가할 요소의 값으로 사용할 개체입니다.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/>가 null인 경우</exception>
        /// <exception cref="T:System.ArgumentException">같은 키를 가지는 요소가 이미 <see cref="T:System.Collections.Generic.IDictionary`2"/>에 있는 경우</exception><exception cref="T:System.NotSupportedException"><see cref="T:System.Collections.Generic.IDictionary`2"/>가 읽기 전용인 경우</exception>
        public void Add(TKey key, params TValue[] values) {
            if(values == null || values.IsEmptySequence())
                return;

            var set = this[key];

            lock(_syncLock)
                foreach(var v in values)
                    set.Add(v);
        }

        /// <summary>
        /// 제공된 키와 값이 있는 요소를 <see cref="T:System.Collections.Generic.IDictionary`2"/>에 추가합니다.
        /// </summary>
        /// <param name="key">추가할 요소의 키로 사용할 개체입니다.</param><param name="value">추가할 요소의 값으로 사용할 개체입니다.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/>가 null인 경우</exception><exception cref="T:System.ArgumentException">같은 키를 가지는 요소가 이미 <see cref="T:System.Collections.Generic.IDictionary`2"/>에 있는 경우</exception><exception cref="T:System.NotSupportedException"><see cref="T:System.Collections.Generic.IDictionary`2"/>가 읽기 전용인 경우</exception>
        void IDictionary<TKey, ISet<TValue>>.Add(TKey key, ISet<TValue> value) {
            if(value != null)
                Add(key, value.ToArray());
        }

        /// <summary>
        /// <see cref="T:System.Collections.Generic.IDictionary`2"/>에서 지정한 키를 가지는 요소를 제거합니다.
        /// </summary>
        /// <returns>
        /// 요소가 성공적으로 제거되었으면 true이고, 그렇지 않으면 false입니다.이 메서드는 <paramref name="key"/>가 원래 <see cref="T:System.Collections.Generic.IDictionary`2"/>에 없는 경우에도 false를 반환합니다.
        /// </returns>
        /// <param name="key">제거할 요소의 키입니다.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/>가 null인 경우</exception><exception cref="T:System.NotSupportedException"><see cref="T:System.Collections.Generic.IDictionary`2"/>가 읽기 전용인 경우</exception>
        public bool Remove(TKey key) {
            ISet<TValue> set;
            return InnerMap.TryRemove(key, out set);
        }

        /// <summary>
        /// 지정된 키와 연결된 값을 가져옵니다.
        /// </summary>
        /// <returns>
        /// <see cref="T:System.Collections.Generic.IDictionary`2"/>를 구현하는 개체에 지정한 키가 있는 요소가 포함되어 있으면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        /// <param name="key">가져올 값이 있는 키입니다.</param><param name="value">이 메서드가 반환될 때 지정된 키가 있으면 해당 키와 연결된 값이고, 그렇지 않으면 <paramref name="value"/> 매개 변수의 형식에 대한 기본값입니다.이 매개 변수는 초기화되지 않은 상태로 전달됩니다.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/>가 null인 경우</exception>
        public bool TryGetValue(TKey key, out ISet<TValue> value) {
            return InnerMap.TryGetValue(key, out value);
        }

        /// <summary>
        /// 지정된 키가 있는 요소를 가져오거나 설정합니다.
        /// </summary>
        /// <returns>
        /// 지정한 키가 있는 요소입니다.
        /// </returns>
        /// <param name="key">가져오거나 설정할 요소의 키입니다.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/>가 null인 경우</exception><exception cref="T:System.Collections.Generic.KeyNotFoundException">속성이 검색되었지만 <paramref name="key"/>가 없는 경우</exception><exception cref="T:System.NotSupportedException">속성이 설정되어 있으며 <see cref="T:System.Collections.Generic.IDictionary`2"/>가 읽기 전용인 경우</exception>
        public ISet<TValue> this[TKey key] {
            get { return InnerMap.GetOrAdd(key, k => new HashSet<TValue>()); }
            set {
                if(value != null)
                    Add(key, value.ToArray<TValue>());
                else
                    this[key].Clear();
            }
        }

        /// <summary>
        /// <see cref="T:System.Collections.Generic.IDictionary`2"/>의 키를 포함하는 <see cref="T:System.Collections.Generic.ICollection`1"/>을 가져옵니다.
        /// </summary>
        /// <returns>
        /// <see cref="T:System.Collections.Generic.IDictionary`2"/>를 구현하는 개체의 키를 포함하는 <see cref="T:System.Collections.Generic.ICollection`1"/>입니다.
        /// </returns>
        public ICollection<TKey> Keys {
            get { return InnerMap.Keys; }
        }

        /// <summary>
        /// <see cref="T:System.Collections.Generic.IDictionary`2"/>의 값을 포함하는 <see cref="T:System.Collections.Generic.ICollection`1"/>을 가져옵니다.
        /// </summary>
        /// <returns>
        /// <see cref="T:System.Collections.Generic.IDictionary`2"/>를 구현하는 개체의 값을 포함하는 <see cref="T:System.Collections.Generic.ICollection`1"/>입니다.
        /// </returns>
        public ICollection<ISet<TValue>> Values {
            get { return InnerMap.Values; }
        }

        public bool IsEmpty {
            get { return InnerMap.IsEmpty; }
        }

        public bool IsValueEmpty {
            get { return InnerMap.SelectMany(item => item.Value).Any() == false; }
        }

        public ISet<TValue> GetOrAdd(TKey key, Func<TKey, ISet<TValue>> valuesFactory) {
            return InnerMap.GetOrAdd(key, valuesFactory);
        }

        public ISet<TValue> GetOrAdd(TKey key, ISet<TValue> values) {
            return InnerMap.GetOrAdd(key, k => values);
        }

        public virtual string AsString() {
            return string.Concat("{",
                                 Keys.Select(key => string.Format("[{0}, {1}]", key, this[key].CollectionToString())).AsJoinedText(","),
                                 "}");
        }
    }
}