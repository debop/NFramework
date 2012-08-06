using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NSoft.NFramework.Parallelism.Tools;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// <see cref="Task{TResult}"/> 를 이용하여, 비동기적으로 캐시 값을 구하는 클래스입니다.
    /// 기본적으로 캐시에는 값을 계산하는 작업 (<see cref="Task{TValue}"/>)이 Lazy{Task{TValue}} 변수로 정의되어, 
    /// 지연한 작업 생성 및, 비동기적인 값 계산을 하게 됩니다.
    /// </summary>
    /// <typeparam name="TKey">캐시 키</typeparam>
    /// <typeparam name="TValue">캐시 값</typeparam>
    [DebuggerDisplay("Count={Count}")]
    [Serializable]
    public class AsyncCache<TKey, TValue> : ICollection<KeyValuePair<TKey, Task<TValue>>> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly ConcurrentDictionary<TKey, Lazy<Task<TValue>>> _map = new ConcurrentDictionary<TKey, Lazy<Task<TValue>>>();
        private readonly Func<TKey, Task<TValue>> _valueFactory;
        private readonly object _syncLock = new object();

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="valueFactory">값을 생성하는 Task를 반환하는 델리게이트입니다.</param>
        public AsyncCache(Func<TKey, Task<TValue>> valueFactory) {
            valueFactory.ShouldNotBeNull("valueFactory");
            _valueFactory = valueFactory;
        }

        /// <summary>
        /// 지정한 키에 해당하는 값을 가져옵니다. 없으면, 값 생성 delegate에 의해 생성된 값을 캐시에 저장하고, 반환합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual Task<TValue> GetValue(TKey key) {
            key.ShouldNotBeNull("key");

            if(IsDebugEnabled)
                log.Debug("비동기 캐시에서 값을 얻습니다. key=[{0}]", key);

            lock(_syncLock) {
                Func<TKey, Lazy<Task<TValue>>> @lazyFactory = k => new Lazy<Task<TValue>>(() => _valueFactory(k));
                return _map.GetOrAdd(key, @lazyFactory).Value;
            }
        }

        /// <summary>
        /// 캐시에 값을 지정하여 저장합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(TKey key, TValue value) {
            SetValue(key, Task.Factory.FromResult(value));
        }

        /// <summary>
        /// 캐시에 지정한 값을 저장합니다.
        /// </summary>
        public virtual void SetValue(TKey key, Task<TValue> value) {
            key.ShouldNotBeNull("key");

            if(IsDebugEnabled)
                log.Debug("비동기 캐시에 정보를 저장합니다... key=[{0}]", key);

            _map[key] = LazyTool.CreateLazy(value);
        }

        /// <summary>
        /// 해당 키의 값
        /// </summary>
        public TValue this[TKey key] {
            get { return GetValue(key).Result; }
            set {
                value.ShouldNotBeNull("value");
                SetValue(key, value);
            }
        }

        /// <summary>
        /// 모든 요소를 제거합니다.
        /// </summary>
        public void Clear() {
            lock(_syncLock)
                _map.Clear();
        }

        /// <summary>
        /// 캐시에 저장된 요소의 수
        /// </summary>
        public int Count {
            get { return _map.Count; }
        }

        /// <summary>
        /// 캐시가 비었는지 나타냅니다.
        /// </summary>
        public bool IsEmpty {
            get { return (_map.Keys.Any() == false); }
        }

        #region << IEnumerable >>

        public IEnumerator<KeyValuePair<TKey, Task<TValue>>> GetEnumerator() {
            return
                _map
                    .Select(p => new KeyValuePair<TKey, Task<TValue>>(p.Key, p.Value.Value))
                    .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

        #region << ICollection<KeyValuePair<TKey,Task<TValue>>> >>

        void ICollection<KeyValuePair<TKey, Task<TValue>>>.Add(KeyValuePair<TKey, Task<TValue>> item) {
            SetValue(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, Task<TValue>>>.Contains(KeyValuePair<TKey, Task<TValue>> item) {
            return _map.ContainsKey(item.Key);
        }

        void ICollection<KeyValuePair<TKey, Task<TValue>>>.CopyTo(KeyValuePair<TKey, Task<TValue>>[] array, int arrayIndex) {
            _map
                .Select(p => new KeyValuePair<TKey, Task<TValue>>(p.Key, p.Value.Value))
                .ToList()
                .CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, Task<TValue>>>.Remove(KeyValuePair<TKey, Task<TValue>> item) {
            Lazy<Task<TValue>> value;
            return _map.TryRemove(item.Key, out value);
        }

        bool ICollection<KeyValuePair<TKey, Task<TValue>>>.IsReadOnly {
            get { return false; }
        }

        #endregion
    }
}