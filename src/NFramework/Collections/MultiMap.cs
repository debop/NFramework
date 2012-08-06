using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Collections {
    /// <summary>
    /// 일반적인 Key - Value 의 1:1 쌍이 아닌 1:N 관계인 Key-ValueCollection 관계를 갖는다. (실제로는 IDictionary{TKey, IList{TValue}} 와 같다.)
    /// </summary>
    /// <remarks>
    /// 일반적인 Dictionary의 경우에는 키-값이 1:1의 쌍으로 구성됩니다. 
    /// <see cref="MultiMap{TKey,TValue}"/>은 1:N의 관계를 가질 수 있도록 해줍니다.
    /// </remarks>
    /// <typeparam name="TKey">키 타입</typeparam>
    /// <typeparam name="TValue">값 타입</typeparam>
    [Serializable]
    public class MultiMap<TKey, TValue> : IDictionary<TKey, IList<TValue>> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly object _syncLock = new object();
        private readonly IDictionary<TKey, IList<TValue>> _dictionary;

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public MultiMap() : this(CollectionTool.INITIAL_CAPACITY) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="capacity">초기 용량</param>
        public MultiMap(int capacity) {
            capacity = Math.Max(capacity, CollectionTool.INITIAL_CAPACITY);
            _dictionary = new Dictionary<TKey, IList<TValue>>(capacity);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="comparer">키 비교자</param>
        public MultiMap(IEqualityComparer<TKey> comparer) {
            _dictionary = new Dictionary<TKey, IList<TValue>>(comparer ?? EqualityComparer<TKey>.Default);
        }

        /// <summary>
        /// 키의 갯수
        /// </summary>
        public int KeyCount {
            get { return _dictionary.Keys.Count; }
        }

        /// <summary>
        /// 맵에 있는 모든 값의 갯수
        /// </summary>
        public int ValueCount {
            get {
                //lock(_syncLock)
                return _dictionary.Keys.SelectMany(k => _dictionary[k]).Count();
            }
        }

        public bool IsEmpty {
            get { return !_dictionary.Keys.Any(); }
        }

        public bool IsValueEmpty {
            get { return !_dictionary.Values.SelectMany(v => v).Any(); }
        }

        /// <summary>
        /// 새로운 요소를 추가한다.
        /// </summary>
        /// <param name="key">키</param>
        /// <param name="value">값</param>
        public virtual void Add(TKey key, TValue value) {
            this[key].Add(value);
        }

        /// <summary>
        /// 요소 하나를 추가한다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected virtual void AddSingleMap(TKey key, TValue value) {
            this[key].Add(value);
        }

        /// <summary>
        /// 지정된 값을 가진 요소가 있는지 여부
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool ContainsValue(TValue value) {
            return Keys.Any(key => _dictionary[key].Contains(value));
        }

        /// <summary>
        /// 인스턴스 정보를 문자열로 표현
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return this.DictionaryToString();
        }

        /// <summary>
        /// 요소를 추가한다.
        /// </summary>
        /// <param name="key">키값</param>
        /// <param name="values">요소 값들</param>
        public void Add(TKey key, IEnumerable<TValue> values) {
            if(values == null)
                return;

            if(IsDebugEnabled)
                log.Debug("새로운 요소를 추가합니다. key=[{0}], values=[{1}]", key, values.CollectionToString());

            lock(_syncLock) {
                if(this[key] is List<TValue>) {
                    ((List<TValue>)this[key]).AddRange(values);
                }
                else {
                    var map = this[key];
                    foreach(var v in values)
                        map.Add(v);
                }
            }
        }

        /// <summary>
        /// 요소를 추가한다.
        /// </summary>
        /// <param name="key">키값</param>
        /// <param name="values">요소 값들</param>
        void IDictionary<TKey, IList<TValue>>.Add(TKey key, IList<TValue> values) {
            if(values == null || values.Count == 0)
                return;

            Add(key, values);
        }

        /// <summary>
        /// 지정된 키를 가진 요소의 존재여부
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key) {
            return _dictionary.ContainsKey(key);
        }

        /// <summary>
        /// 키 컬렉션
        /// </summary>
        public ICollection<TKey> Keys {
            get { return _dictionary.Keys; }
        }

        /// <summary>
        /// 지정된 키를 가진 요소를 삭제한다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key) {
            if(IsDebugEnabled)
                log.Debug("키[{0}]를 삭제합니다...", key);

            if(_dictionary.ContainsKey(key))
                lock(_syncLock)
                    return _dictionary.Remove(key);

            return false;
        }

        /// <summary>
        /// Multimap value collection. value type is IList{TValue}
        /// </summary>
        public ICollection<IList<TValue>> Values {
            get { return _dictionary.Values; }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="key">키</param>
        /// <returns>값 컬렉션</returns>
        public IList<TValue> this[TKey key] {
            get {
                if(_dictionary.ContainsKey(key) == false)
                    lock(_syncLock)
                        _dictionary.Add(key, new List<TValue>());

                return _dictionary[key];
            }
            set {
                lock(_syncLock) {
                    Remove(key);
                    _dictionary[key] = value ?? new List<TValue>();
                }
            }
        }

        /// <summary>
        /// Try get value from internal dictionary, if not exists, return false
        /// </summary>
        /// <param name="key">key to retrieve</param>
        /// <param name="value">value matched by key</param>
        /// <returns>indicate exists the key</returns>
        public bool TryGetValue(TKey key, out IList<TValue> value) {
            lock(_syncLock) {
                var result = _dictionary.ContainsKey(key);
                value = (result) ? this[key] : null;
                return result;
            }
        }

        /// <summary>
        /// 요소를 추가한다.
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<TKey, IList<TValue>> item) {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// 모든 요소를 제거한다.
        /// </summary>
        public void Clear() {
            _dictionary.Clear();
        }

        /// <summary>
        /// 지정된 요소의 값들이 모두 존재하는지 검사 (하나라도 존재하지 않으면 False를 반환한다.
        /// </summary>
        /// <param name="item">검사할 요소</param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<TKey, IList<TValue>> item) {
            item.ShouldNotBeNull("item");

            if(_dictionary.ContainsKey(item.Key)) {
                return item.Value.All(v => this[item.Key].Contains(v));
            }

            return false;
        }

        /// <summary>
        /// 요소들을 지정된 배열에 복사한다.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<TKey, IList<TValue>>[] array, int arrayIndex) {
            array.ShouldNotBeNull("array");
            arrayIndex.ShouldBeGreaterOrEqual(0, "arrayIndex");
            // arrayIndex.ShouldBePositiveOrZero("arrayIndex");

            if(_dictionary.Count + arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException("arrayIndex");

            foreach(KeyValuePair<TKey, IList<TValue>> pair in _dictionary)
                array[arrayIndex++] = new KeyValuePair<TKey, IList<TValue>>(pair.Key, pair.Value);
        }

        /// <summary>
        /// 요소 수
        /// </summary>
        public int Count {
            get { return _dictionary.Count; }
        }

        /// <summary>
        /// 읽기 전용 여부
        /// </summary>
        public virtual bool IsReadOnly {
            get { return false; }
        }

        /// <summary>
        /// 지정된 요소 제거
        /// </summary>
        /// <param name="item"></param>
        /// <returns>제거 여부</returns>
        public bool Remove(KeyValuePair<TKey, IList<TValue>> item) {
            lock(_syncLock) {
                if(ContainsKey(item.Key)) {
                    foreach(var value in item.Value)
                        if(this[item.Key].Contains(value))
                            this[item.Key].Remove(value);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Return enumerator
        /// </summary>
        /// <returns>enumerator of <see cref="KeyValuePair{TKey,TValue}"/></returns>
        public IEnumerator<KeyValuePair<TKey, IList<TValue>>> GetEnumerator() {
            return _dictionary.GetEnumerator();
        }

        /// <summary>
        /// 인스턴스를 복제한다.
        /// </summary>
        /// <returns></returns>
        public MultiMap<TKey, TValue> Clone() {
            var clonedMap = new MultiMap<TKey, TValue>();

            lock(_syncLock)
                foreach(var key in Keys)
                    clonedMap.Add(key, this[key]);

            return clonedMap;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public virtual string AsString() {
            return string.Concat("{",
                                 Keys
                                     .Select(key => string.Format("[{0}, {1}]", key, this[key].CollectionToString()))
                                     .AsJoinedText(","),
                                 "}");
        }
    }
}