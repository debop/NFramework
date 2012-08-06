using System;
using System.Linq;
using System.Web;
using System.Web.Caching;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web.HttpHandlers;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.FusionCharts.Web {
    /// <summary>
    /// FusionChart	SWF File 을 리소스로부터 읽어서 제공하는 Handler입니다. Parameter로는 file=line 이런 식으로 줘야 합니다.
    /// </summary>
    /// <example>
    /// <code>
    /// // web.config 의 system.web/httpHandlers 에 등록한다.
    /// &lt;add verb="GET" path="NSoft.NFramework.FusionCharts.ResourceFile.axd" type="NSoft.NFramework.FusionCharts.Web.ResourceFileHandler, NSoft.NFramework.FusionCharts" validate="false" /&gt;
    /// 
    /// // Line.swf 파일을 리소스에서 읽어서, 다운로드한다.
    /// FusionResourceFile.axd?File=Line.swf
    /// </code>
    /// </example>
    /// <seealso cref="HandlerSettings.ResourceFileHandler"/>
    public class ResourceFileHandler : AbstractHttpAsyncHandler {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string PARAM_FILE = @"File";

        /// <summary>
        /// HttpContext의 요청정보를 바탕으로 HttpHandler의 실제 작업을 처리하는 메소드입니다.
        /// </summary>
        /// <param name="context"></param>
        protected override void DoProcessRequest(HttpContext context) {
            base.DoProcessRequest(context);

            string filename = string.Empty;

            try {
                var request = context.Request;
                filename = request[PARAM_FILE].AsText();

                Guard.Assert(filename.IsNotWhiteSpace(), @"Request의 Parameter로 File=filename 형식으로 다운 받고자하는 파일명을 지정하셔야 합니다.");

                WriteChartFile(context, filename);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException(@"Fusion Resource File을 전송하는데 실패했습니다!!! File=" + filename, ex);

                throw;
            }
        }

        /// <summary>
        /// 지정한 HttpContext에 리소스 파일을 씁니다.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="filename"></param>
        protected void WriteChartFile(HttpContext context, string filename) {
            if(IsDebugEnabled)
                log.Debug(@"리소스에서 해당 파일 정보를 읽어 반환합니다... filename=" + filename);

            filename.ShouldNotBeWhiteSpace("filename");


            var buffer = GetResourceFileContent(context, filename);
            Guard.Assert(buffer != null && buffer.Length > 0, "지정된 파일이 리소스에 없습니다. filename=" + filename);

            SetRepsonseHeaders(context, filename);

            // file이 swf 처럼 이미 압축이 되어 있다면, 추가로 압축을 할 필요 없다.
            //
            var needCompress = CanCompression &&
                               WebTool.NoCompressionFileExtensions.Any(filename.Contains) == false &&
                               buffer.Length > CompressionThreshold;

            if(needCompress) {
                if(IsDebugEnabled)
                    log.Debug("리소스 파일[{0}] 를 압축하여, 반환합니다. 압축방법[{1}]", filename, CompressionKind);

                // 압축했을 때에, 압축 방법을 응답헤더에 꼭 붙여줘야 Client에서 알아먹겠죠?
                context.Response.AppendHeader("Content-Encoding", CompressionKind.ToString().ToLower());
                buffer = Compressor.Compress(buffer);
            }

            context.Response.AppendHeader(@"Content-Length", buffer.Length.ToString());
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.Flush();

            if(IsDebugEnabled)
                log.Debug(@"리소스 파일 내용을 Client에 전송했습니다!!! filename=" + filename);
        }

        /// <summary>
        /// 지정한 파일명에 해당하는 리소스를 로드하여 Stream으로 반환한다.
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="filename">리소스 파일명</param>
        /// <returns>리소스 파일 정보</returns>
        private static byte[] GetResourceFileContent(HttpContext context, string filename) {
            var cacheKey = GetCacheKey(filename);

            byte[] fileData = null;

            try {
                if(IsDebugEnabled)
                    log.Debug(@"캐시에서 파일 정보를 꺼냅니다. cacheKey=" + cacheKey);

                fileData = context.Cache.Get(cacheKey) as byte[];

                if(fileData == null || fileData.Length == 0) {
                    if(IsDebugEnabled)
                        log.Debug(@"캐시에 파일 정보가 없으므로, 리소스로부터 파일 정보를 읽습니다. filename=" + filename);

                    // Resource File의 Namespace를 조작해주는 하지만 Column3D.swf 나 MSColumn3D.swf 를 구분하지 못한다.
                    // 파일별로 제대로 구분하기 위해 파일명 앞에 "." 을 붙인다. (어차피 Namespace 때문에 "." 이 들어가므로 
                    //
                    var resourceStream = typeof(ChartBase).Assembly.GetEmbeddedResourceFile("." + filename.ToLower());
                    fileData = resourceStream.ToBytes();

                    if(fileData != null)
                        SaveToCache(context, cacheKey, fileData, TimeSpan.FromHours(12));
                }
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException(@"Resource File을 리소스로 부터 로드하는데 실패했습니다. filename=" + filename, ex);
            }

            return fileData ?? new byte[0];
        }

        /// <summary>
        /// 서버 메모리 캐시에 파일 정보를 저장합니다
        /// </summary>
        private static void SaveToCache(HttpContext context, string key, object value, TimeSpan duration) {
            if(IsDebugEnabled)
                log.Debug(@"Cache에 파일 데이터를 삽입합니다. key={0}, duration={1}", key, duration);

            context.Cache.Insert(key, value, null, Cache.NoAbsoluteExpiration, duration);
        }

        private static string GetCacheKey(string filename) {
            return @"NSoft.NFramework.FusionCharts.ResourceFileHandler." + filename;
        }

        /// <summary>
        /// Header에 ContentType, Cache 설정을 수행합니다.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="filename"></param>
        private static void SetRepsonseHeaders(HttpContext context, string filename) {
            context.Response.Clear();
            context.Response.ContentType = WebTool.GetMime(filename);

            context.Response.Cache.SetExpires(DateTime.Now.ToUniversalTime().AddYears(1));
            context.Response.Cache.SetMaxAge(new TimeSpan(365, 0, 0, 0));

            // NOTE : HTTPS 에서 동작하기 위해
            context.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
        }
    }
}