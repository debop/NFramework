using System;
using System.IO;
using System.Json;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace NSoft.NFramework.Web.Mvc.WebApi {
    /// <summary>
    ///     WebApi 에서 기본으로 사용하는 Json Formatter 를 JavaScriptSerializer 을 이용하도록 하는 Formatter 입니다. 실제 사용을 위해서는 참고 URL에서 등록 방법을 참고하세요. 참고 : http://www.west-wind.com/weblog/posts/2012/Mar/09/Using-an-alternate-JSON-Serializer-in-ASPNET-Web-API?utm_source=feedburner&utm_medium=feed&utm_campaign=Feed%3A+RickStrahl+%28Rick+Strahl%27s+WebLog%29
    /// </summary>
    /// <seealso cref="JsonNetFormatter" />
    public class JavaScriptSerializerFormatter : MediaTypeFormatter {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        protected override bool CanWriteType(Type type) {
            if(IsNotWriteType(type))
                return false;

            return true;
        }


        protected override bool CanReadType(Type type) {
            return type != typeof(IKeyValueModel);
        }


        protected override Task<object> OnReadFromStreamAsync(Type type,
                                                              System.IO.Stream stream,
                                                              System.Net.Http.Headers.HttpContentHeaders contentHeaders,
                                                              FormatterContext formatterContext) {
            return
                Task.Factory.StartNew(() => {
                                          var serializer = new JavaScriptSerializer();

                                          using(var reader = new StreamReader(stream))
                                              return serializer.Deserialize(reader.ReadToEnd(), type);
                                      });
        }


        protected override Task OnWriteToStreamAsync(Type type, object value,
                                                     System.IO.Stream stream,
                                                     System.Net.Http.Headers.HttpContentHeaders contentHeaders,
                                                     FormatterContext formatterContext,
                                                     System.Net.TransportContext transportContext) {
            return
                Task.Factory.StartNew(() => {
                                          var serializer = new JavaScriptSerializer();
                                          var sb = new StringBuilder();

                                          serializer.Serialize(value, sb);

                                          var buffer = Encoding.UTF8.GetBytes(sb.ToString());
                                          stream.Write(buffer, 0, buffer.Length);
                                          stream.Flush();
                                      });
        }


        private static bool IsNotWriteType(Type type) {
            // don't serialize JsonValue structure use default for that
            return type == typeof(JsonValue) ||
                   type == typeof(JsonObject) ||
                   type == typeof(JsonArray);
        }
    }
}