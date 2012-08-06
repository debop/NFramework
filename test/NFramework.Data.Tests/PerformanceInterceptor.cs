using System.Diagnostics;
using Castle.DynamicProxy;

namespace NSoft.NFramework.Data {
    public class PerformanceInterceptor : StandardInterceptor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly Stopwatch _stopwatch = new Stopwatch();

        protected override void PreProceed(IInvocation invocation) {
            if(IsDebugEnabled)
                log.Debug(invocation.Method.Name + " is proceeding...");


            _stopwatch.Start();

            base.PreProceed(invocation);
        }

        protected override void PostProceed(IInvocation invocation) {
            base.PostProceed(invocation);

            _stopwatch.Stop();

            if(IsDebugEnabled)
                log.Debug("{0} is executed. elapsed time=[{1}] (msec)", (object)invocation.Method.Name,
                          (object)_stopwatch.ElapsedMilliseconds);

            // NOTE: Stopwatch 를 Reset을 하지 않으면, 측정값이 계속 누적되어 정확한 값을 계산하지 못합니다.
            _stopwatch.Reset();
        }
    }
}