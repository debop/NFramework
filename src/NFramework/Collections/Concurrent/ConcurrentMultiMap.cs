using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Collections.Concurrent {
    /// <summary>
    /// Thread-Safe 한 Multi-Map 자료구조입니다. (TKey - ICollection{TValue}) 형태를 가집니다.
    /// </summary>
    /// <typeparam name="TKey">키의 수형</typeparam>
    /// <typeparam name="TValue">값의 수형</typeparam>
    /// <seealso cref="MultiMap{TKey, TValue}"/>
    [Serializable]
    public class ConcurrentMultiMap<TKey, TValue> : IDictionary<TKey, ICollection<TValue>> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private ConcurrentDictionary<TKey, ICollection<TValue>> _map = new ConcurrentDictionary<TKey, ICollection<TValue>>();
        private readonly object _syncLock = new object();

        /// <summary>
        /// 생성자
        /// </summary>
        public ConcurrentMultiMap() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="collection"></param>
        public ConcurrentMultiMap(IEnumerable<KeyValuePair<TKey, ICollection<TValue>>> collection) {
            collection.ShouldNotBeNull("collection");

            foreach(var pair in collection)
                if(pair.Value != null)
                    Add(pair.Key, pair.Value.ToArray<TValue>());
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="comparer"></param>
        public ConcurrentMultiMap(IEqualityComparer<TKey> comparer) {
            comparer.ShouldNotBeNull("comparer");
            InnerMap = new ConcurrentDictionary<TKey, ICollection<TValue>>(comparer);
        }

        /// <summary>
        /// 내부 저장소
        /// </summary>
        protected ConcurrentDictionary<TKey, ICollection<TValue>> InnerMap {
            get { return _map ?? (_map = new ConcurrentDictionary<TKey, ICollection<TValue>>()); }
            private set { _map = value; }
        }

        /// <summary>
        /// 컬렉션을 반복하는 열거자를 반환합니다.
        /// </summary>
        public IEnumerator<KeyValuePair<TKey, ICollection<TValue>>> GetEnumerator() {
            return InnerMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        /// 요소 키의 갯수
        /// </summary>
        public int KeyCount {
            get { return InnerMap.Keys.Count; }
        }

        /// <summary>
        /// 전체 Value의 갯수 (하나의 키에 여러개의 Value가 매핑되어 있으므로 실제 Value는 Key의 갯수보다 같거나 크다)
        /// </summary>
        /// <remarks>전체 Value 갯수를 계산하는데 시간이 걸리므로, 한번 호출해서 변수로 사용해야 한다.</remarks>
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
        /// 요소를 추가합니다.
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<TKey, ICollection<TValue>> item) {
            if(IsDebugEnabled)
                log.Debug("요소를 추가합니다. item=[{0}]", item);

            var bag = this[item.Key];

            lock(_syncLock) {
                if(bag is List<TValue>)
                    ((List<TValue>)bag).AddRange(item.Value);
                else {
                    foreach(var v in item.Value)
                        bag.Add(v);
                }
            }
        }

        /// <summary>
        /// 지정한 값을 가지고 있는지 확인합니다.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ContainsValue(TValue value) {
            lock(_syncLock)
                return InnerMap.SelectMany(item => item.Value).Any(v => Equals(v, value));
        }

        /// <summary>
        /// 요소를 모두 삭제합니다.
        /// </summary>
        public void Clear() {
            InnerMap.Clear();
        }

        /// <summary>
        /// 지정한 요소를 가지고 있는지 확인합니다.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<TKey, ICollection<TValue>> item) {
            if(item.Value.IsEmptySequence())
                return false;

            if(InnerMap.ContainsKey(item.Key) == false)
                return false;

            lock(_syncLock) {
                var bag = InnerMap[item.Key];
                return item.Value.All(v => bag.Contains(v));
            }
        }

        /// <summary>
        /// 요소를 지정한 배열에 복사합니다.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<TKey, ICollection<TValue>>[] array, int arrayIndex) {
            lock(_syncLock)
                InnerMap.ToArray().CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 지정한 요소가 있다면, 삭제합니다.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<TKey, ICollection<TValue>> item) {
            if(ContainsKey(item.Key) == false)
                return false;

            if(IsDebugEnabled)
                log.Debug("요소를 제거합니다. item Key=[{0}], Value=[{1}]", item.Key, item.Value.CollectionToString());

            var removed = false;

            lock(_syncLock) {
                var bag = InnerMap[item.Key];

                foreach(var v in item.Value)
                    if(bag.Contains(v))
                        removed = bag.Remove(v);
            }
            return removed;
        }

        /// <summary>
        /// 요소 수
        /// </summary>
        public int Count {
            get { return InnerMap.Count; }
        }

        /// <summary>
        /// 읽기 전용 ?
        /// </summary>
        public bool IsReadOnly {
            get { return false; }
        }

        /// <summary>
        /// 지정한 키를 가지고 있는가?
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key) {
            return InnerMap.ContainsKey(key);
        }

        /// <summary>
        /// 지정한 키에 값들을 추가합니다.
        /// </summary>
        /// <param name="key">키</param>
        /// <param name="values">값들</param>
        public virtual void Add(TKey key, params TValue[] values) {
            if(values.IsEmptySequence())
                return;

            var bag = this[key];
            lock(_syncLock) {
                if(bag is List<TValue>)
                    ((List<TValue>)bag).AddRange(values);
                else {
                    foreach(var v in values)
                        bag.Add(v);
                }
            }
        }

        /// <summary>
        /// 지정한 키에 값들을 추가합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        void IDictionary<TKey, ICollection<TValue>>.Add(TKey key, ICollection<TValue> values) {
            if(values != null)
                Add(key, values.ToArray<TValue>());
        }

        /// <summary>
        /// 해당 키를 삭제합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key) {
            ICollection<TValue> bag;
            return InnerMap.TryRemove(key, out bag);
        }

        /// <summary>
        /// 해당 키의 값을 조회합니다. 있으면 true를 반환하고, 없으면 false를 반환합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out ICollection<TValue> value) {
            return InnerMap.TryGetValue(key, out value);
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ICollection<TValue> this[TKey key] {
            get { return InnerMap.GetOrAdd(key, k => new List<TValue>()); }
            set {
                if(value != null)
                    Add(key, value.ToArray<TValue>());
                else
                    this[key].Clear();
            }
        }

        //ICollection<TValue> IDictionary<TKey, ICollection<TValue>>.this[TKey key]
        //{
        //    get
        //    {
        //        return this[key];
        //    }
        //    set
        //    {
        //        if(value != null)
        //            this[key] = new List<TValue>(value);
        //    }
        //}

        /// <summary>
        /// Key collection
        /// </summary>
        public ICollection<TKey> Keys {
            get { return InnerMap.Keys; }
        }

        /// <summary>
        /// Value collection
        /// </summary>
        public ICollection<ICollection<TValue>> Values {
            get { return InnerMap.Values; }
        }

        /// <summary>
        /// 저장소가 비었는지 여부
        /// </summary>
        public bool IsEmpty {
            get { return InnerMap.IsEmpty; }
        }

        /// <summary>
        /// 저장소에 값이 없는지 파악
        /// </summary>
        public bool IsValueEmpty {
            get { return InnerMap.SelectMany(item => item.Value).Any() == false; }
        }

        /// <summary>
        /// 해당 키에 값이 없다면 값을 넣고, 반환합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public ICollection<TValue> GetOrAdd(TKey key, Func<TKey, ICollection<TValue>> valueFactory) {
            valueFactory.ShouldNotBeNull("valueFactory");

            if(ContainsKey(key)) {
                var values = this[key];
                foreach(var value in valueFactory(key))
                    values.Add(value);

                return values;
            }

            return InnerMap.GetOrAdd(key, valueFactory);
        }

        /// <summary>
        /// 해당 키에 값이 없다면 값을 넣고, 반환합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICollection<TValue> GetOrAdd(TKey key, ICollection<TValue> value) {
            if(value == null)
                return this[key];

            return GetOrAdd(key, (k) => value);
        }

        /// <summary>
        /// 항목을 문자열로 표현합니다.
        /// </summary>
        /// <returns></returns>
        public virtual string AsString() {
            return string.Concat("{",
                                 Keys.Select(key => string.Format("[{0}, {1}]", key, this[key].CollectionToString())).AsJoinedText(","),
                                 "}");
        }
    }
}