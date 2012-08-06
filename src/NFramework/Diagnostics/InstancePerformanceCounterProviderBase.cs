using System;
using System.Diagnostics;

namespace NSoft.NFramework.Diagnostics {
    /// <summary>
    /// 인스턴스 성능 카운터 제공자의 기본 클래스입니다.
    /// </summary>
    [Serializable]
    public abstract class InstancePerformanceCounterProviderBase : PerformanceCounterProviderBase,
                                                                   IInstancePerformanceCounterProvider {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="categoryName"></param>
        protected InstancePerformanceCounterProviderBase(string categoryName) : this(categoryName, "_Global_") {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="instanceName"></param>
        protected InstancePerformanceCounterProviderBase(string categoryName, string instanceName)
            : base(categoryName) {
            InstanceName = instanceName;
        }

        /// <summary>
        /// 인스턴스 명
        /// </summary>
        public string InstanceName { get; private set; }

        /// <summary>
        /// 지정된 이름의 Counter를 생성합니다.
        /// </summary>
        /// <param name="counterName"></param>
        /// <returns></returns>
        protected override PerformanceCounter CreatePerformanceCounter(string counterName) {
            if(IsDebugEnabled)
                log.Debug("PerformanceCounter를 생성합니다... CategoryName=[{0}], CounterName=[{1}], InstanceName=[{2}]",
                          CategoryName, counterName, InstanceName);

            return new PerformanceCounter(CategoryName, counterName, InstanceName);
        }
                                                                   }
}