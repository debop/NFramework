using System;
using System.Web;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web.HttpHandlers;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.DataServices.WebHost.HttpHandlers {
    /// <summary>
    /// DataService 를 수행하는 HttpHandler 입니다.
    /// </summary>
    public class DataService : AbstractHttpHandler {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// HttpHandler의 작업의 메인 메소드입니다. 재정의 하여 원하는 기능을 수행하되, 제일 첫번째에 부모 클래스의 메소들를 호출해주어야 합니다.
        /// </summary>
        /// <param name="context"></param>
        protected override void DoProcessRequest(HttpContext context) {
            context.ShouldNotBeNull("context");

            if(IsDebugEnabled)
                log.Debug(@"Execution for request from HttpContext is starting...");

            try {
                var productName = context.Request[HttpParams.Product].AsText();

                if(IsDebugEnabled)
                    log.Debug("ProductName=[{0}]에 대한 요청 처리를 접수했습니다...", productName);

                var adapter = DataServiceTool.ResolveDataServiceAdapter(productName);

                // Silverlight 때문에 요청/응답을 Base64 문자열로 받습니다.
                var responseText = adapter.Execute(context.Request.InputStream.ToText());
                // var responseBytes = adapter.Execute(context.Request.InputStream.ToBytes());

                context.WriteResponse(responseText);

                if(IsDebugEnabled)
                    log.Debug("ProductName=[{0}]에 대한 요청 처리를 완료하였습니다!!!", productName);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.Error("요청을 처리하는 동안 예외가 발생했습니다.", ex);
            }
        }
    }
}