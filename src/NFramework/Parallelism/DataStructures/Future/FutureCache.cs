using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// Future 패턴으로 항목을 캐시하는 클래스입니다.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [DebuggerDisplay("Count={Count}")]
    [Serializable]
    public class FutureCache<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly ConcurrentDictionary<TKey, Task<TValue>> _map = new ConcurrentDictionary<TKey, Task<TValue>>();
        private readonly Func<TKey, TValue> _valueFactory;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="valueFactory">캐시할 항목의 값을 생성할 Factory</param>
        public FutureCache(Func<TKey, TValue> valueFactory) {
            valueFactory.ShouldNotBeNull("valueFactory");
            _valueFactory = valueFactory;
        }

        /// <summary>
        /// 캐시에서 키에 해당하는 값을 구합니다.
        /// </summary>
        /// <param name="key">항목 키</param>
        /// <returns>항목에 해당하는 값</returns>
        public virtual TValue GetValue(TKey key) {
            key.ShouldNotBeNull("key");

            if(IsDebugEnabled)
                log.Debug("Future Value를 캐시에서 구합니다... key=[{0}]", key);

            var valueTask = _map.GetOrAdd(key, k => Task.Factory.StartNew(() => _valueFactory(k), TaskCreationOptions.None));
            var value = valueTask.Result;

            if(IsDebugEnabled) {
                object val = value;

                if(val is string)
                    val = ((string)val).EllipsisChar(80);

                log.Debug("Future Value를 캐시에서 구했습니다. key=[{0}], value=[{1}]", key, val);
            }

            return value;
        }

        /// <summary>
        /// 캐시에 값을 직접 설정합니다. (valueFactory를 거치지 않고, 설정할 수 있도록 합니다)
        /// </summary>
        /// <param name="key">키</param>
        /// <param name="value">값</param>
        public void SetValue(TKey key, TValue value) {
            SetValue(key, Task.Factory.FromResult(value));
        }

        /// <summary>
        /// 캐시에 값을 도출하는 Task를 직접 설정합니다. (valueFactory를 거치지 않고, 설정할 수 있도록 합니다)
        /// </summary>
        /// <param name="key">키</param>
        /// <param name="task">값을 도출하는 Task</param>
        public virtual void SetValue(TKey key, Task<TValue> task) {
            key.ShouldNotBeNull("key");
            task.ShouldNotBeNull("task");

            if(IsDebugEnabled) {
                if(task.IsCompleted)
                    log.Debug("FutureCache에 값을 설정합니다. key=[{0}], value=[{1}]", key, task.Result);
                else
                    log.Debug("FutureCache에 아직 결과가 나오지 않은 Task를 설정합니다. key=[{0}], task=[{1}]", key, task.Id);
            }

            _map[key] = task;
            // _map.TryAdd(key, task);
        }

        /// <summary>
        /// 캐시에 해당 키가 존재하는지 확인합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key) {
            key.ShouldNotBeNull("key");
            return _map.ContainsKey(key);
        }

        /// <summary>
        /// 해당 키를 제거합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key) {
            key.ShouldNotBeNull("key");

            if(ContainsKey(key) == false)
                return false;

            Task<TValue> removedTask;
            return TryRemove(key, out removedTask);
        }

        /// <summary>
        /// <paramref name="key"/>에 해당하는 항목을 삭제하려고 합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valueTask"></param>
        /// <returns></returns>
        public bool TryRemove(TKey key, out Task<TValue> valueTask) {
            key.ShouldNotBeNull("key");
            return _map.TryRemove(key, out valueTask);
        }

        /// <summary>
        /// 캐시에 있는 모든 요소를 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<TKey, TValue>> GetAllItems() {
            return With.TryFunctionAsync(() => _map.Select(item => new KeyValuePair<TKey, TValue>(item.Key, item.Value.Result)));
        }

        /// <summary>
        /// 인덱서
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key] {
            get { return GetValue(key); }
            set { SetValue(key, value); }
        }

        /// <summary>
        /// 항목을 추가합니다.
        /// </summary>
        /// <param name="item">추가할 항목</param>
        public void Add(KeyValuePair<TKey, TValue> item) {
            SetValue(item.Key, item.Value);
        }

        /// <summary>
        /// 모든 항목을 제거합니다.
        /// </summary>
        public void Clear() {
            _map.Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) {
            return ContainsKey(item.Key) && Equals(GetValue(item.Key), item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            GetAllItems().ToList().CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) {
            Task<TValue> removedValue;
            return _map.TryRemove(item.Key, out removedValue);
        }

        /// <summary>
        /// 항목 수
        /// </summary>
        public int Count {
            get { return _map.Count; }
        }

        /// <summary>
        /// 읽기 전용인가?
        /// </summary>
        public virtual bool IsReadOnly {
            get { return false; }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return GetAllItems().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}