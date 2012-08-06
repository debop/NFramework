using System;
using System.Diagnostics;
using System.Web.Mvc;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Web.Mvc.Filters {

    /// <summary>
    /// NLog로 로그를 쓰는 ActionFilter 입니다.
    /// </summary>
    public class NLogLoggingFilter : FilterAttribute, IActionFilter, IResultFilter {
        
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly Stopwatch _stopwatch = new Stopwatch();

        /// <summary>
        /// Called before an action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnActionExecuting(ActionExecutingContext filterContext) {
            if(IsDebugEnabled)
                log.Debug("Action 수행을 시작합니다... controller=[{0}], action=[{1}], parameters=[{2}]",
                          filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                          filterContext.ActionDescriptor.ActionName,
                          filterContext.ActionParameters.DictionaryToString());
            _stopwatch.Reset();
            _stopwatch.Start();
        }

        /// <summary>
        /// Called after the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnActionExecuted(ActionExecutedContext filterContext) {
            _stopwatch.Stop();

            if(IsDebugEnabled)
                log.Debug("Action 수행을 완료했습니다. controller=[{0}], action=[{1}], 실행시간=[{2}]",
                          filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                          filterContext.ActionDescriptor.ActionName,
                          _stopwatch.Elapsed);
        }

        /// <summary>
        /// Called before an action result executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnResultExecuting(ResultExecutingContext filterContext) {
            if(IsDebugEnabled)
                log.Debug("Result 수행을 시작합니다...");
            _stopwatch.Reset();
            _stopwatch.Start();
        }

        /// <summary>
        /// Called after an action result executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnResultExecuted(ResultExecutedContext filterContext) {
            _stopwatch.Stop();

            if(IsDebugEnabled)
                log.Debug("Result 수행을 완료했습니다. 실행시간=[{0}]", _stopwatch.Elapsed);
        }
    }
}