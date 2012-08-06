using System;
using System.Web;
using NSoft.NFramework.Web.HttpHandlers;

namespace NSoft.NFramework.FusionCharts.Web {
    /// <summary>
    /// Fusion Chart 의 RealTime Chart 에 실시간 Data를 제공하는 기본 HttpHandler
    /// </summary>
    public abstract class RealTimeDataProviderBase : AbstractHttpAsyncHandler {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// HttpContext의 요청정보를 바탕으로 HttpHandler의 실제 작업을 처리하는 메소드입니다.
        /// </summary>
        /// <param name="context"></param>
        protected override void DoProcessRequest(HttpContext context) {
            base.DoProcessRequest(context);

            try {
                CurrentContext = context;
                Request = CurrentContext.Request;
                Response = CurrentContext.Response;

                if(IsDebugEnabled)
                    log.Debug("RealTime Data를 전송을 시작합니다. Request=[{0}]", Request.RawUrl);

                Response.Write(PopulateResponseData());

                if(IsDebugEnabled)
                    log.Debug("RealTime Data를 전송했습니다. Request=[{0}]", Request.RawUrl);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("RealTime Data를 제공하기 위한 처리 중 예외가 발생했습니다.", ex);
                throw;
            }
        }

        protected virtual HttpContext CurrentContext { get; set; }
        protected virtual HttpRequest Request { get; set; }
        protected virtual HttpResponse Response { get; set; }

        public const string PARAM_LABEL = @"label";
        public const string PARAM_VALUE = @"value";
        public const string VALUE_SEPARATOR = @",";
        public const string DATASET_SEPARATOR = @"|";

        /// <summary>
        /// 하나의 값: label=xxx&value=123
        /// 두종류의 값   : label=xxx&value=123|435
        /// 한종류에 대해 3개의 Data를 배치로 보낼때: label=x1,x2,x3&value=111,222,333
        /// 두종류에 대해 여러개를 배치로 보낼때: label=x1,x2,x3|y1,y2,y3&value=111,222,333|100,200,300
        /// </summary>
        public static readonly string RealTimeResponseFormat = "&" + PARAM_LABEL + "={0}&" + PARAM_VALUE + "={1}";

        /// <summary>
        /// 실시간으로 데이타를 전송합니다.
        /// </summary>
        protected abstract string PopulateResponseData();
    }
}