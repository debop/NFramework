using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Collections {
    /// <summary>
    /// 키-Set(값) 의 형식을 가진다. 즉 키는 하나인데, 값은 여러 개의 중복 없는 값을 가진다.
    /// </summary>
    /// <typeparam name="TKey">키 형식</typeparam>
    /// <typeparam name="TValue">값 형식</typeparam>
    [Serializable]
    public class MultiSet<TKey, TValue> : IDictionary<TKey, HashSet<TValue>> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly object _syncLock = new object();
        private readonly IDictionary<TKey, HashSet<TValue>> _dictionary;

        /// <summary>
        /// Inner Dictionary
        /// </summary>
        protected virtual IDictionary<TKey, HashSet<TValue>> InnerSet {
            get { return _dictionary; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MultiSet() {
            _dictionary = new Dictionary<TKey, HashSet<TValue>>(EqualityComparer<TKey>.Default);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="capacity">initial capacity</param>
        /// <param name="comparer">value comparer</param>
        public MultiSet(int capacity, IEqualityComparer<TKey> comparer) {
            _dictionary = new Dictionary<TKey, HashSet<TValue>>(Math.Max(capacity, CollectionTool.INITIAL_CAPACITY), comparer);
        }

        /// <summary>
        /// 키의 갯수
        /// </summary>
        public int KeyCount {
            get { return InnerSet.Keys.Count; }
        }

        /// <summary>
        /// 맵에 있는 모든 값의 갯수
        /// </summary>
        public int ValueCount {
            get {
                lock(_syncLock)
                    return InnerSet.Keys.Sum(key => InnerSet[key].Count);
            }
        }

        /// <summary>
        /// Add item
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public virtual void Add(TKey key, TValue value) {
            this[key].Add(value);
        }

        /// <summary>
        /// 지정된 value를 가지고 있는지 검사합니다.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool ContainsValue(TValue value) {
            return Keys.Any(key => InnerSet[key].Contains(value));
        }

        /// <summary>
        /// MultiSet에서 가지고 있는 모든 값을 하나의 HashSet으로 만들어서 반환합니다.
        /// </summary>
        /// <returns></returns>
        public virtual HashSet<TValue> GetAllValues() {
            var allValues = new HashSet<TValue>();

            foreach(var value in InnerSet.Keys.SelectMany(key => InnerSet[key])) {
                allValues.Add(value);
            }

            return allValues;
        }

        /// <summary>
        /// 지정된 키를 가지는지 검사한다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key) {
            return InnerSet.ContainsKey(key);
        }

        /// <summary>
        /// 지정된 키에 값들을 추가합니다. (중복불가)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        public virtual void Add(TKey key, IEnumerable<TValue> values) {
            if(values != null)
                foreach(var value in values)
                    this[key].Add(value);
        }

        void IDictionary<TKey, HashSet<TValue>>.Add(TKey key, HashSet<TValue> value) {
            Add(key, value);
        }

        /// <summary>
        /// 지정된 키를 제거합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key) {
            lock(_syncLock)
                if(InnerSet.ContainsKey(key))
                    return InnerSet.Remove(key);

            return false;
        }

        /// <summary>
        /// 지정된 키에 해당하는 HashSet 을 구해봅니다. 있으면 True를 반환하고, 없으면 False를 반환합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out HashSet<TValue> value) {
            lock(_syncLock) {
                var result = InnerSet.ContainsKey(key);
                value = (result) ? this[key] : null;
                return result;
            }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public HashSet<TValue> this[TKey key] {
            get {
                lock(_syncLock) {
                    if(InnerSet.ContainsKey(key) == false)
                        InnerSet.Add(key, new HashSet<TValue>());

                    return InnerSet[key];
                }
            }
            set {
                lock(_syncLock) {
                    Remove(key);
                    InnerSet.Add(key, value ?? new HashSet<TValue>());
                }
            }
        }

        /// <summary>
        /// Key Collection
        /// </summary>
        public ICollection<TKey> Keys {
            get { return InnerSet.Keys; }
        }

        /// <summary>
        /// Collection of HashSet{TValue}
        /// </summary>
        public ICollection<HashSet<TValue>> Values {
            get { return InnerSet.Values; }
        }

        /// <summary>
        /// Add new item
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<TKey, HashSet<TValue>> item) {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. 
        ///                 </exception>
        public void Clear() {
            lock(_syncLock)
                InnerSet.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        ///                 </param>
        public bool Contains(KeyValuePair<TKey, HashSet<TValue>> item) {
            if(InnerSet.ContainsKey(item.Key))
                return this[item.Key].IsProperSupersetOf(item.Value);

            return false;
        }

        /// <summary>
        /// 요소를 지정된 array로 복사한다.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<TKey, HashSet<TValue>>[] array, int arrayIndex) {
            array.ShouldNotBeNull("array");

            if(_dictionary.Count + arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException("arrayIndex");

            foreach(var pair in InnerSet)
                array[arrayIndex++] = new KeyValuePair<TKey, HashSet<TValue>>(pair.Key, pair.Value);
        }

        /// <summary>
        /// 지정된 요소가 있으면 제거합니다.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<TKey, HashSet<TValue>> item) {
            if(ContainsKey(item.Key)) {
                return (this[item.Key].RemoveWhere(value => item.Value.Contains(value))) > 0;
            }
            return false;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count {
            get { return InnerSet.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly {
            get { return false; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator() {
            return InnerSet.GetEnumerator();
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