using System;
using System.Diagnostics;
using System.Web;
using NSoft.NFramework.Diagnostics;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Web.HttpHandlers {
    /// <summary>
    /// 시스템의 성능 측정기(<see cref="PerformanceCounter"/>의 값을 읽어 제공한다.
    /// </summary>
    /// <remarks>
    /// CategoryName, CounterName [, InstanceName] 을 제공해야 합니다.
    /// </remarks>
    /// <example>
    /// <code>
    /// // 
    /// PerformanceCounterHandler.ashx?
    /// </code>
    /// </example>
    /// <seealso cref="PerformanceCounterTool"/>
    /// <seealso cref="IPerformanceCounterProvider"/>
    [Serializable]
    public abstract class PerformanceCounterHandlerBase : AbstractHttpHandler {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        protected override void DoProcessRequest(HttpContext context) {
            CurrentContext = context;
            Request = context.Request;
            Response = context.Response;

            if(IsDebugEnabled)
                log.Debug("요청정보를 받았습니다... Request.RawUrl=[{0}]", Request.RawUrl);

            try {
                string categoryName;
                string counterName;
                string instanceName;

                ParseRequestParameters(out categoryName, out counterName, out instanceName);
                RetrivePerformanceCount(categoryName, counterName, instanceName);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("요청처리 시에 예외가 발생했습니다. Request=[{0}]", Request.RawUrl);
                    log.Error(ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Current HttpContext Instance
        /// </summary>
        protected HttpContext CurrentContext { get; set; }

        /// <summary>
        /// Current HttpRequest Instance
        /// </summary>
        protected HttpRequest Request { get; set; }

        /// <summary>
        /// Current HttpResponse Instance
        /// </summary>
        protected HttpResponse Response { get; set; }

        /// <summary>
        /// 얻고자하는 성능 측정 값을 얻는다.
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="counterName"></param>
        /// <param name="instanceName"></param>
        protected virtual void ParseRequestParameters(out string categoryName, out string counterName, out string instanceName) {
            categoryName = Request["CategoryName"].AsText(PerformanceCounterTool.Processor.CategoryName);
            counterName = Request["CounterName"].AsText(PerformanceCounterTool.Processor.PercentOfProcessorTime.CounterName);
            instanceName = Request["InstanceName"].AsText(PerformanceCounterTool.Processor.InstanceName);

            if(IsDebugEnabled)
                log.Debug("Parse request parameters... CategoryName=[{0}], CounterName=[{1}], InstanceName=[{2}]",
                          categoryName, counterName, instanceName);
        }

        /// <summary>
        /// 지정된 범주의 PerformanceCount를 얻어서 HttpResponse 객체에 씁니다.
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="counterName"></param>
        protected void RetrivePerformanceCount(string categoryName, string counterName) {
            RetrivePerformanceCount(categoryName, counterName, null);
        }

        /// <summary>
        /// 지정된 범주의 PerformanceCount를 얻어서 HttpResponse 객체에 씁니다.
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="counterName"></param>
        /// <param name="instanceName"></param>
        protected virtual void RetrivePerformanceCount(string categoryName, string counterName, string instanceName) {
            var counter = (instanceName.IsWhiteSpace())
                              ? new PerformanceCounter(categoryName, counterName)
                              : new PerformanceCounter(categoryName, counterName, instanceName);
            var nextValue = counter.NextValue();

            if(IsDebugEnabled)
                log.Debug(
                    "Retrieve PerformanceCounter Value. categoryName=[{0}], counterName=[{1}], instanceName=[{2}], nextValue=[{3}]",
                    categoryName, counterName, instanceName, nextValue);

            Response.Write(nextValue);
        }
    }
}