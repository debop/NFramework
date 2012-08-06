using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSoft.NFramework.LinqEx;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// 값을 제공하는 여러가지 방법을 준비하도록 하고, 최초 결과를 캐시에 사용하도록 합니다.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class FutureSpeculativeCache<TKey, TValue> : FutureCache<TKey, TValue> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly BlockingCollection<TKey> _queue;
        private readonly Func<TKey, IEnumerable<TKey>> _speculativeKeyFactory;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="valueFactory"></param>
        /// <param name="speculativeKeyFactory"></param>
        public FutureSpeculativeCache(Func<TKey, TValue> valueFactory, Func<TKey, IEnumerable<TKey>> speculativeKeyFactory)
            : base(valueFactory) {
            speculativeKeyFactory.ShouldNotBeNull("speculativeKeyFactory");

            _speculativeKeyFactory = speculativeKeyFactory;
            _queue = new BlockingCollection<TKey>();

            // 공급자-소비자 컬렉션을 이용하여 Background Thread에서, 공급되는 TKey에 해당하는 Value를 계산한다.
            //
            Task.Factory.StartNew(() => {
                                      // 공급자-소비자 컬렉션에서 공급자 키가 들어오면, 그 키와 관련된 키를 구해서, 값을 미리 구해 놓는다.
                                      //
                                      //foreach(var key in _queue.GetConsumingEnumerable())
                                      //  _speculativeKeyFactory(key)
                                      //      .Where(k => !ContainsKey(k))
                                      //      .RunEach(nextKey => GetValueInBackground(nextKey));

                                      Parallel.ForEach(_queue.GetConsumingEnumerable(),
                                                       key => _speculativeKeyFactory(key)
                                                                  .Where(k => !ContainsKey(k))
                                                                  .RunEach(nextKey => GetValueInBackground(nextKey)));
                                  });
        }

        // base 함수에 대해서 작업하기 위해
        //
        private TValue GetValueInBackground(TKey key) {
            return base.GetValue(key);
        }

        /// <summary>
        /// 지정된 키에 해당하는 값을 반환합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override TValue GetValue(TKey key) {
            if(IsDebugEnabled)
                log.Debug("Get Value... key=[{0}]", key);

            if(_queue.Contains(key) == false)
                _queue.Add(key);

            return base.GetValue(key);
        }

        /// <summary>
        /// 지정된 키의 항목의 값을 설정합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="task"></param>
        public override void SetValue(TKey key, Task<TValue> task) {
            if(IsDebugEnabled)
                log.Debug("Set value... key=[{0}]", key);

            base.SetValue(key, task);

            if(_queue.Contains(key) == false)
                _queue.Add(key);
        }
    }
}