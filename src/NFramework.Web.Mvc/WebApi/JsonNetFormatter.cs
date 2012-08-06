using System;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NSoft.NFramework.Web.Mvc.WebApi
{
    /// <summary>
    /// WebApi 에서 기본으로 사용하는 Json Formatter 를 JSON.NET 을 이용하도록 하는 Formatter 입니다.
    /// 실제 사용을 위해서는 참고 URL에서 등록 방법을 참고하세요.
    /// 참고 : http://www.west-wind.com/weblog/posts/2012/Mar/09/Using-an-alternate-JSON-Serializer-in-ASPNET-Web-API?utm_source=feedburner&utm_medium=feed&utm_campaign=Feed%3A+RickStrahl+%28Rick+Strahl%27s+WebLog%29
    /// 참고 : http://blogs.msdn.com/b/henrikn/archive/2012/02/18/using-json-net-with-asp-net-web-api.aspx
    /// </summary>
    /// <seealso cref="JavaScriptSerializerFormatter"/>
    public class JsonNetFormatter : MediaTypeFormatter
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public JsonNetFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeWithQualityHeaderValue(MvcTool.JsonContentType));
        }

        protected override bool CanWriteType(Type type)
        {
            if(MvcTool.IsNotWriteType(type))
                return false;

            return true;
        }

        protected override bool CanReadType(Type type)
        {
            return type != typeof(IKeyValueModel);
        }

        protected override Task<object> OnReadFromStreamAsync(Type type,
                                                              Stream stream,
                                                              HttpContentHeaders contentHeaders,
                                                              FormatterContext formatterContext)
        {
            if(IsDebugEnabled)
                log.Debug("스트림으로부터 정보를 읽습니다...");

            return
                Task.Factory.StartNew(() =>
                                      {
                                          using(var reader = new StreamReader(stream))
                                          {
                                              var jsonText = reader.ReadToEnd();
                                              return JsonConvert.DeserializeObject(jsonText,
                                                                                   MvcTool.DefaultJsonSerializerSettings);
                                          }
                                      });
        }

        protected override Task OnWriteToStreamAsync(Type type,
                                                     object value,
                                                     Stream stream,
                                                     HttpContentHeaders contentHeaders,
                                                     FormatterContext formatterContext,
                                                     TransportContext transportContext)
        {
            if(IsDebugEnabled)
                log.Debug("객체정보를 스트림에 씁니다...");

            return
                Task.Factory.StartNew(() =>
                                      JsonConvert.SerializeObject(value,
                                                                  Formatting.Indented,
                                                                  MvcTool.DefaultJsonSerializerSettings));
        }
    }
}