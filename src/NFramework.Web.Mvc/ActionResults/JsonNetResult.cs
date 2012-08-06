using System;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace NSoft.NFramework.Web.Mvc
{
    /// <summary>
    /// Newtonsoft.Json.dll 라이브러리를 이용하여 Action 결과를 JSON Serialization/Deserialization 을 수행합니다.
    /// ASP.NET MVC 3 기본 직렬화 방식인 JavaScriptSerializer 보다 4배 이상 빠르다.
    /// </summary>
    public class JsonNetResult : ActionResult
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public JsonNetResult() : this((JsonSerializerSettings)null) {}

        public JsonNetResult(JsonSerializerSettings serializersettings)
        {
            SerializerSettings = serializersettings ?? new JsonSerializerSettings();

            ContentType = MvcTool.JsonContentType;
            Formatting = Formatting.None;
        }

        /// <summary>
        /// HttpResponse의 ContentEncoding 설정 (기본은 web.config의 Response ContentEncoding을 사용한다)
        /// </summary>
        public Encoding ContentEncoding { get; set; }

        /// <summary>
        /// Response ContentType (기본은 "application/json")
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 실제 ActionResult 결과 정보
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// Json 직렬화 설정
        /// </summary>
        public JsonSerializerSettings SerializerSettings { get; set; }

        /// <summary>
        /// Json 직렬화 결과 Text 포맷 종류
        /// </summary>
        public Formatting Formatting { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if(context == null)
                throw new ArgumentNullException("context");

            if(IsDebugEnabled)
                log.Debug(
                    "Action 처리결과를 Newtonsoft.Json의 JsonSerializer를 이용하여, 직렬화하여 HttpResponse에 씁니다... Result=[{0}]",
                    Result);

            if(Result == null)
                return;

            var response = context.HttpContext.Response;

            if(ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            response.ContentType = string.IsNullOrWhiteSpace(ContentType) ? MvcTool.JsonContentType : ContentType;

            using(var writer = new JsonTextWriter(response.Output) { Formatting = this.Formatting })
            {
                var serializer = JsonSerializer.Create(SerializerSettings);
                serializer.Serialize(writer, Result);

                writer.Flush();
            }
        }
    }
}