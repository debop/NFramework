using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using NSoft.NFramework.IO;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.Web.HttpHandlers {
    /// <summary>
    /// Static file을 제공할 때, 압축이 가능하면 압축을 사용하고, 캐시로 저장해두어, 사용자의 정적 파일 요청을 빠르게 처리하는 HttpHandler입니다.
    /// </summary>							  
    [Serializable]
    public class StaticFileHttpHandler : AbstractHttpAsyncHandler {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 캐시 키의 접두사로 사용되어, 구분할 수 있도록 합니다.
        /// </summary>
        private static readonly string HandlerTypeName = typeof(StaticFileHttpHandler).FullName;

        private string _virtualPath;
        private string _contentType;
        private string _cacheKey;
        private bool _isCompressed;

        private TimeSpan _cacheDuration = TimeSpan.FromDays(7);

        /// <summary>
        /// 캐시 Duration입니다.
        /// </summary>
        public virtual TimeSpan CacheDuration {
            get { return _cacheDuration; }
            set { _cacheDuration = value; }
        }

        /// <summary>
        /// HttpHandler의 작업의 메인 메소드입니다. 재정의 하여 원하는 기능을 수행하되, 제일 첫번째에 부모 클래스의 메소들를 호출해주어야 합니다.
        /// </summary>
        /// <param name="context"></param>
        protected override void DoProcessRequest(HttpContext context) {
            base.DoProcessRequest(context);

            if(IsDebugEnabled)
                log.Debug("정적 파일 (Static File) 요청 처리를 시작합니다...");

            ParseRequestParams(context);

            // file not found!!!
            var physicalPath = context.Server.MapPath(WebTool.GetScriptPath(_virtualPath));
            if(physicalPath.FileExists() == false) {
                ResponseFileNotFound(context, _virtualPath);
                // context.Response.End();
                return;
            }

            // if requested file already exist in cache, send file from cache
            //
            if(WriteFromCache(context, _cacheKey, CacheDuration, CompressionKind) == false) {
                _contentType = WebTool.GetMime(physicalPath);

                var readTask = ReadFile(context, physicalPath);

                _isCompressed = CanCompression;

                var fileExt = physicalPath.ExtractFileExt();

                // 이미지 파일이거나, 압축이 필요없는 파일은 압축하지 않습니다.
                if(_contentType.Contains("image") || WebTool.NoCompressionFileExtensions.Any(fileExt.Contains)) {
                    if(IsDebugEnabled)
                        log.Debug("이미지 파일등 이미 압축된 상태의 파일은 압축하지 않습니다.... physicalFile=[{0}]", physicalPath);

                    _isCompressed = false;
                }

                byte[] readBytes = readTask.Result;

                if(_isCompressed && readBytes.Length < CompressionThreshold) {
                    if(IsDebugEnabled)
                        log.Debug("전송할 데이타의 크기가 기본 Threadhold [{0}] bytes 보다 작아 압축을 하지 않습니다. Content-Length=[{1}] bytes",
                                  CompressionThreshold, readBytes.Length);
                    _isCompressed = false;
                }

                // 결론적으로 압축이 필요하다면 압축을 합니다.
                //
                if(_isCompressed)
                    readBytes = Compressor.Compress(readBytes);

                var cacheItem = new CacheItem(readBytes, _isCompressed, _contentType);

                SaveToCache(context, _cacheKey, cacheItem, CacheDuration);
                WriteBytes(context, cacheItem, CacheDuration, CompressionKind);
            }

            // context.Response.End();

            if(IsDebugEnabled)
                log.Debug("정적 파일 (Static File) 요청 처리를 완료했습니다!!! file=[{0}]", _virtualPath);
        }

        /// <summary>
        /// Parameter 정보를 파싱해서, 원하는 결과를 만듭니다.
        /// </summary>
        /// <param name="context"></param>
        private void ParseRequestParams(HttpContext context) {
            var request = context.Request;

            if(IsDebugEnabled)
                log.Debug("Static File 요청 정보를 파싱합니다. RawUrl=[{0}]", request.RawUrl);

            _virtualPath = request["F"].AsText();
            var version = request["V"].AsText();

            if(_virtualPath.IsWhiteSpace())
                _virtualPath = request.FilePath;

            _cacheKey = GetCacheKey(_virtualPath, version);

            if(IsDebugEnabled)
                log.Debug("요청한 Static File의 VirtualPath=[{0}], CacheKey=[{1}]", _virtualPath, _cacheKey);
        }

        /// <summary>
        /// 서버 메모리 캐시에 파일 정보를 저장합니다
        /// </summary>
        private static void SaveToCache(HttpContext context, string key, CacheItem cacheItem, TimeSpan duration) {
            if(IsDebugEnabled)
                log.Debug("Cache에 파일 데이터를 삽입합니다. key=[{0}], duration=[{1}]", key, duration);

            context.Cache.Insert(key, cacheItem, null, Cache.NoAbsoluteExpiration, duration);
        }

        /// <summary>
        /// 캐시에 저장된 정보가 있다면, 그 정보를 Client에게 보냅니다.
        /// </summary>
        private static bool WriteFromCache(HttpContext context, string cacheKey, TimeSpan cacheDuration,
                                           WebCompressionKind compressionKind) {
            var cacheItem = context.Cache[cacheKey] as CacheItem;

            if(cacheItem == null || cacheItem.Data == null || cacheItem.Data.Length == 0)
                return false;

            if(IsDebugEnabled)
                log.Debug("캐시에 정적 파일 데이타가 저장되어 있습니다. 캐시 정보를 이용하여 응답합니다. cackeKey=[{0}]", cacheKey);

            WriteBytes(context, cacheItem, cacheDuration, compressionKind);

            return true;
        }

        /// <summary>
        /// 요청 파일의 정보를 응답 스트림에 씁니다.
        /// </summary>
        private static void WriteBytes(HttpContext context, CacheItem item, TimeSpan cacheDuration, WebCompressionKind compressionKind) {
            if(item == null || item.Data == null || item.Data.Length == 0)
                return;

            if(IsDebugEnabled)
                log.Debug("요청 파일을 응답스트림에 씁니다... isCompressed=[{0}], contentType=[{1}], cacheDuration=[{2}]",
                          item.IsCompressed, item.ContentType, cacheDuration);

            HttpResponse response = context.Response;

            if(response.IsClientConnected == false)
                return;

            response.Clear();
            SetResponseHeaders(response, item, cacheDuration, compressionKind);
            response.OutputStream.Write(item.Data, 0, item.Data.Length);
            response.Flush();
        }

        private static void SetResponseHeaders(HttpResponse response, CacheItem item, TimeSpan cacheDuration,
                                               WebCompressionKind compressionKind) {
            response.ShouldNotBeNull("response");

            response.AppendHeader("Content-Length", item.Data.Length.ToString());
            response.ContentType = item.ContentType;

            if(item.IsCompressed)
                response.AppendHeader("Content-Encoding", compressionKind.ToString().ToLower());

            var cache = response.Cache;

            // NOTE : HTTPS 에서도 동작하기 위해
            cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            cache.SetCacheability(HttpCacheability.Public);

            // Cache Expiration 설정
            cache.SetExpires(DateTime.Now.ToUniversalTime().Add(cacheDuration));
            cache.SetMaxAge(cacheDuration);
            cache.SetAllowResponseInBrowserHistory(true);
        }

        /// <summary>
        /// 지정된 파일을 비동기적으로 읽습니다.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="physicalPath">파일의 물리적 경로</param>
        /// <returns></returns>
        private static Task<byte[]> ReadFile(HttpContext context, string physicalPath) {
            if(IsDebugEnabled)
                log.Debug("Static File을 비동기 방식으로 읽습니다. physicalPath=[{0}]", physicalPath);

            if(File.Exists(physicalPath))
                return FileAsync.ReadAllBytes(physicalPath);

            if(log.IsWarnEnabled)
                log.Warn("파일 정보가 없습니다. 길이가 0인 byte 배열을 반환합니다. physicalPath=[{0}]", physicalPath);

            return Task.Factory.FromResult(new byte[0]);
        }

        /// <summary>
        /// 캐시 키를 빌드합니다.
        /// </summary>
        /// <param name="virtualPath">파일 경로</param>
        /// <param name="version">파일 버전</param>
        /// <returns></returns>
        private static string GetCacheKey(string virtualPath, string version) {
            return string.Format("{0}@{1}@{2}#{3}", HandlerTypeName, HostingEnvironment.ApplicationVirtualPath, virtualPath, version);
        }

        /// <summary>
        /// 요청파일이 없을 때, 파일을 찾을 수 없음 (404 에러) 를 반환합니다.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="virtualPath"></param>
        private static void ResponseFileNotFound(HttpContext context, string virtualPath) {
            context.Response.StatusCode = 404;
            context.Response.Status = "404 Bad Requests";
            context.Response.StatusDescription = "File not found to be requested. file=" + virtualPath;
        }

        [Serializable]
        public class CacheItem {
            public CacheItem(byte[] data, bool isCompressed = true, string contentType = FileTool.DEFAULT_MIME_TYPE) {
                Data = data;
                IsCompressed = isCompressed;
                ContentType = contentType;
            }

            public byte[] Data { get; private set; }
            public bool IsCompressed { get; private set; }
            public string ContentType { get; private set; }
        }
    }
}