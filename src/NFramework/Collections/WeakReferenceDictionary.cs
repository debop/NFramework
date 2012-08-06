using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Collections {
    /// <summary>
    /// 개체를 저장해 두지만, Gabage Collector가 삭제할 수 있도록 개체의 
    /// <see cref="WeakReference"/>를 저장한다.
    /// </summary>
    /// <remarks>
    /// WeakReferenceDictionary의 Instance 는 Thread-safe하다.<br/>
    /// 
    /// 개체의 <see cref="System.WeakReference"/>를 저장하는 것이므로 
    /// Gabage Collector에 의해 삭제되면 null값을 가지게 되므로
    /// 반환받은 값은 항상 null 값을 검사해야 한다.
    /// </remarks>
    /// <typeparam name="TKey">Key 타입</typeparam>
    /// <typeparam name="TValue">Value 타입</typeparam>
    [Serializable]
    public class WeakReferenceDictionary<TKey, TValue> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        #region Fields

        /// <summary>
        /// 내부 저장소 (키와 Value의 WeakReference를 저장한다.)
        /// </summary>
        private readonly Dictionary<TKey, WeakReference> _repository = new Dictionary<TKey, WeakReference>();

        /// <summary>
        /// thread-safe 한 함수들로 만들기 위해
        /// </summary>
        [CLSCompliant(false)] protected readonly object _syncLock = new object();

        #endregion

        #region Properties

        /// <summary>
        /// 저장소
        /// </summary>
        protected virtual Dictionary<TKey, WeakReference> Repository {
            get { return _repository; }
        }

        /// <summary>
        /// 캐시에 저장된 key 값을 가진 개체
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key] {
            get { return GetValue(key); }
            set { Add(key, value); }
        }

        /// <summary>
        /// 캐시에 저장된 전체 개체의 수 (상태에 상관없이)
        /// </summary>
        /// <value></value>
        public int Count {
            get { return _repository.Count; }
        }

        /// <summary>
        /// 캐시에 저장된 개체중 살아있는 개체의 수
        /// </summary>
        /// <value></value>
        public int ActiveCount {
            get {
                lock(_syncLock)
                    return _repository.Values.Count(wr => wr.IsAlive);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 캐시에 저장된 key 값을 가진 객체를 반환한다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue GetValue(TKey key) {
            key.ShouldNotBeNull("key");

            lock(_syncLock) {
                if(_repository.ContainsKey(key)) {
                    var wr = _repository[key];

                    if(wr.IsAlive)
                        return (TValue)wr.Target;
                }
            }

            // generic에서 casting시에 직접하지 못하고 임시변수를 통해서 해야만 한다.
            //
            object result = null;
            return (TValue)result;
        }

        /// <summary>
        /// 캐시에 객체를 저장한다.
        /// </summary>
        /// <param name="key">객체의 유일 키값</param>
        /// <param name="value">객체</param>
        /// <returns>저장 여부</returns>
        /// <exception cref="ArgumentNullException">key나 value가 null이면 예외를 발생한다.</exception>
        public bool Add(TKey key, TValue value) {
            key.ShouldNotBeNull("key");
            value.ShouldNotBeNull("value");

            lock(_syncLock) {
                var wr = new WeakReference(value);
                _repository.AddValue(key, wr);
            }

            return true;
        }

        /// <summary>
        /// 캐시 저장소에 해당 key를 가진 개체가 존재하는지 검사
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key) {
            key.ShouldNotBeNull("key");

            lock(_syncLock)
                return _repository.ContainsKey(key);
        }

        /// <summary>
        /// 캐시에 저장된 객체가 Gabage Collector에 의해 리소스가 해제되지 않았음을 검사한다.
        /// 즉 아직 메모리에 살아있는지 검사한다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsAlive(TKey key) {
            key.ShouldNotBeNull("key");

            lock(_syncLock) {
                if(_repository.ContainsKey(key)) {
                    var wr = _repository[key];
                    return wr.IsAlive;
                }
            }
            return false;
        }

        /// <summary>
        /// 캐시에 키값을 가지는 개체의 GabageCollector 의 Generation 값을 구한다.
        /// </summary>
        /// <remarks>
        /// Generation 값이 -1 이면 메모리에서 해제 된 것이고, 
        /// 숫자가 커질 수록 사용하지 않고, 오래된 개체이다.
        /// </remarks>
        /// <param name="key">객체의 키값</param>
        /// <returns>Generation Number, -1이면 메모리에서 해제된 것이다.</returns>
        public int GetGeneration(TKey key) {
            key.ShouldNotBeNull("key");
#if !SILVERLIGHT
            lock(_syncLock) {
                if(IsAlive(key)) {
                    return GC.GetGeneration(_repository[key]);
                }
            }
#endif
            return -1;
        }

        #endregion
    }
}