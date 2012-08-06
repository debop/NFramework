using System;
using System.Web;

namespace NSoft.NFramework.Web.HttpHandlers {
    /// <summary>
    /// 비동기 처리가 가능한 HttpHandler의 기본 클래스입니다.	 
    /// <see cref="AbstractHttpHandler.DoProcessRequest"/>메소드를 override 해서 원하시는 작업을 정의하시면 됩니다.
    /// 참고 : http://madskristensen.net/post/How-to-use-the-IHttpAsyncHandler-in-ASPNET.aspx
    /// </summary>
    /// <remarks>
    /// <see cref="AbstractHttpHandler.DoProcessRequest"/> 메소드 내에서 Response.End() 메소드를 호출하지 마십시요. 비동기 방식에서는 예외가 발생합니다.
    /// </remarks>
    [Serializable]
    public abstract class AbstractHttpAsyncHandler : AbstractHttpHandler, IHttpAsyncHandler {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 비동기 호출방식에서는 HttpHandler 인스턴스의 재사용은 문제를 일으킬 수 있습니다. 문제가 발생하면, False로 변환해보시기 바랍니다.
        /// </summary>
        public override bool IsReusable {
            get { return true; }
        }

        /// <summary>
        /// HTTP 처리기에 대한 비동기 호출을 시작합니다.
        /// </summary>
        /// <returns>
        /// 프로세스 상태에 대한 정보가 들어 있는 <see cref="T:System.IAsyncResult"/>입니다.
        /// </returns>
        /// <param name="context">
        /// Request, Response, Session, Server 등과 같이 HTTP 요청을 처리하는 데 사용되는 내장 서버 개체에 대한 참조를 제공하는 <see cref="T:System.Web.HttpContext"/> 개체입니다.
        /// </param>
        /// <param name="cb">비동기 메서드 호출이 완료될 때 호출할 <see cref="T:System.AsyncCallback"/>입니다.<paramref name="cb"/>이 null이면 대리자가 호출되지 않습니다.</param>
        /// <param name="extraData">요청을 처리하는 데 필요한 모든 추가 데이터입니다.</param>
        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData) {
            if(IsDebugEnabled)
                log.Debug("비동기 방식의 HttpHandler의 처리를 시작합니다... context=[{0}], cb=[{1}], extraData=[{2}]", context, cb, extraData);

            _asyncProcess = new AsyncProcessDelegate(DoProcessRequest);

            return _asyncProcess.BeginInvoke(context, cb, extraData);
        }

        /// <summary>
        /// 처리가 끝날 때 비동기 프로세스 End 메서드를 제공합니다.
        /// </summary>
        /// <param name="result">프로세스 상태에 대한 정보가 들어 있는 <see cref="T:System.IAsyncResult"/>입니다.</param>
        public void EndProcessRequest(IAsyncResult result) {
            try {
                if(_asyncProcess != null)
                    _asyncProcess.EndInvoke(result);

                if(IsDebugEnabled)
                    log.Debug("비동기 방식의 HttpHandler의 처리를 완료했습니다!!!");
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled)
                    log.WarnException("HttpAsyncHandler의 비동기 처리 작업 완료 중에 예외가 발생했습니다. Response.End()를 호출하시면 안됩니다. 무시합니다.", ex);
            }
        }

        /// <summary>
        /// 비동기 함수 호출을 위한 델리게이트
        /// </summary>
        /// <param name="context"></param>
        private delegate void AsyncProcessDelegate(HttpContext context);

        private AsyncProcessDelegate _asyncProcess;
    }
}