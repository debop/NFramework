using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Diagnostics {
    /// <summary>
    /// 성능 카운터 제공자의 기본 클래스입니다.
    /// </summary>
    [Serializable]
    public abstract class PerformanceCounterProviderBase : IPerformanceCounterProvider {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly object _syncObj = new object();

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="categoryName"></param>
        protected PerformanceCounterProviderBase(string categoryName) {
            categoryName.ShouldNotBeWhiteSpace("categoryName");

            CategoryName = categoryName;
        }

        /// <summary>
        /// PerformanceCounter가 속한 Category 명
        /// </summary>
        public virtual string CategoryName { get; private set; }

        private IList<PerformanceCounter> _performanceCounters;

        /// <summary>
        /// 제공되는 PerformanceCounter의 컬렉션입니다.
        /// </summary>
        public IList<PerformanceCounter> PerformanceCounters {
            get {
                if(_performanceCounters == null)
                    lock(_syncObj)
                        if(_performanceCounters == null) {
                            var counters = new List<PerformanceCounter>();
                            System.Threading.Thread.MemoryBarrier();
                            _performanceCounters = counters;

                            RegistPerformanceCounter(this);
                        }
                return _performanceCounters;
            }
        }

        /// <summary>
        /// 제공하는 PerformanceCounter 들의 CountName을 반환합니다.
        /// </summary>
        /// <returns></returns>
        public string[] GetCounterNames() {
            return PerformanceCounters.Select(c => c.CounterName).ToArray();
        }

        /// <summary>
        /// 지정된 이름의 Counter를 생성합니다.
        /// </summary>
        /// <param name="counterName"></param>
        /// <returns></returns>
        protected virtual PerformanceCounter CreatePerformanceCounter(string counterName) {
            if(IsDebugEnabled)
                log.Debug("PerformanceCounter를 생성합니다... CategoryName=[{0}], CounterName=[{1}]", CategoryName, counterName);

            return new PerformanceCounter(CategoryName, counterName);
        }

        /// <summary>
        /// 제공하는 PerformanceCounter를 등록합니다.
        /// </summary>
        /// <param name="provider">PerformanceCounter 제공자</param>
        private static void RegistPerformanceCounter(IPerformanceCounterProvider provider) {
            provider.ShouldNotBeNull("provider");

            if(IsDebugEnabled)
                log.Debug("해당 Provider의 PerformanceCounter 들을 생성합니다... CategoryName=[{0}]", provider.CategoryName);

            var accessor = DynamicAccessorFactory.CreateDynamicAccessor(provider.GetType(), true);

            foreach(var propName in accessor.GetPropertyNames()) {
                if(accessor.GetPropertyType(propName) == typeof(PerformanceCounter))
                    provider.PerformanceCounters.Add((PerformanceCounter)accessor.GetPropertyValue(provider, propName));
            }
        }
    }
}