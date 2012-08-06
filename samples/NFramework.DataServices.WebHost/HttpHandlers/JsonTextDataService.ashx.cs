using System;
using System.Web;
using NSoft.NFramework.DataServices.Messages;
using NSoft.NFramework.Json;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.DataServices.WebHost.HttpHandlers {
    /// <summary>
    /// JsonTextDataService 는 Javascript, jQuery 등에서 JSON 포맷으로 요청을 보낼 때 처리하고, JSON 포맷 TEXT 로 반환하는 Handler 입니다.
    /// </summary>
    /// <example>
    /// <code>
    /// http://localhost:41110/JsonTextDataService.axd?Product=Northwind&Method=Customer,GetAll&ResponseFormat=ResultSet
    /// </code>
    /// </example>
    public class JsonTextDataService : DataService {
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
                log.Debug(@"JSON TEXT 형식의 요청에 대해 작업 후, JSON TEXT 문자열로 반환합니다.");

            try {
                var productName = context.Request[HttpParams.Product].AsText();
                var method = context.Request[HttpParams.Method].AsText();
                var responseFormat = context.Request[HttpParams.ResponseFormat].AsEnum(ResponseFormatKind.ResultSet);

                if(IsDebugEnabled)
                    log.Debug("ProductName=[{0}], Method=[{1}], ResponseFormat=[{2}] 에 대한 요청 처리를 접수했습니다...",
                              productName, method, responseFormat);

                var requestMessage = new RequestMessage();
                requestMessage.AddItem(method, responseFormat);

                var dataService = DataServiceTool.ResolveDataService(productName);
                var responseMessage = dataService.Execute(requestMessage);

                var responseText = JsonTool.SerializeAsText(responseMessage);
                var responseBytes = Compressor.Compress(responseText.ToBytes());

                context.WriteResponse(responseBytes);

                //var response = context.Response;
                //response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
                //response.OutputStream.Flush();

                if(IsDebugEnabled)
                    log.Debug("ProductName=[{0}], Method=[{1}]에 대한 요청 처리를 완료하였습니다!!!", productName, method);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.Error("요청을 처리하는 동안 예외가 발생했습니다.", ex);
            }
        }
    }
}