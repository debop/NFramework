using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using NSoft.NFramework.IO;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Web.HttpHandlers {
    // Referecen: http://omaralzabir.com/http_handler_to_combine_multiple_files__cache_and_deliver_compressed_output_for_faster_page_load/

    /// <summary>
    /// 여러 파일을 한꺼번에 묶어 압축하여 다운로드할 수 있도록 해주는 HttpHandler입니다. 
    /// 서버상에서는 요청된 파일들을 Cache에 저장하하므로, 다음 요청 시에는 빠른 결과를 제공합니다.
    /// 예: 
    ///		// 직접 파일을 지정하는 경우
    ///		MultiFileHttpHandler.axd?F=~/js/js1.js|~/js/js2.js&T=text/javascript&V=1
    ///		MultiFileHttpHandler.axd?F=~/css/css1.css|~/css/css2.css&T=text/css&V=1
    /// 
    ///		// appSettings에 파일들이 정의되어 있는 경우
    ///		MultiFileHttpHandler.axd?S=Set.Javascript&T=text/javascript&V=1
    ///		MultiFileHttpHandler.axd?S=Set.Css&T=text/css&V=1
    /// </summary>
    [Serializable]
    public class MultiFileHttpHandler : AbstractHttpAsyncHandler {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 파일 구분자
        /// </summary>
        public const char FileDelimiter = '|';

        private static readonly string HandlerTypeName = typeof(MultiFileHttpHandler).FullName;

        private string _cacheKey;
        private string _contentType;
        private bool _isCompressed;
        private readonly HashSet<string> _filenames = new HashSet<string>();

        private TimeSpan _cacheDuration = TimeSpan.FromDays(365);

        /// <summary>
        /// 캐시 Duration입니다.
        /// </summary>
        public virtual TimeSpan CacheDuration {
            get { return _cacheDuration; }
            set { _cacheDuration = value; }
        }

        /// <summary>
        /// HttpContext의 요청정보를 바탕으로 HttpHandler의 실제 작업을 처리하는 메소드입니다.
        /// </summary>
        /// <param name="context"></param>
        protected override void DoProcessRequest(HttpContext context) {
            base.DoProcessRequest(context);

            if(IsDebugEnabled)
                log.Debug("멀티 파일 요청 처리를 시작합니다...");

            ParseRequestParams(context);

            // 캐시에 있다면 그 값을 보낸다... 
            //
            if(WriteFromCache(context, _cacheKey, CacheDuration) == false) {
                var readBytes = ReadFiles(context);

                _isCompressed = CanCompression;

                // 전송할 데이타가 최소 압축 크기보다 커야 압축을 합니다.
                if(_isCompressed && readBytes.Length < CompressionThreshold) {
                    if(IsDebugEnabled)
                        log.Debug("전송할 데이타의 크기가 기본 Threadhold[{0}] bytes 보다 작아 압축을 하지 않습니다. Content-Length=[{1}] bytes",
                                  CompressionThreshold, readBytes.Length);
                    _isCompressed = false;
                }

                if(_isCompressed)
                    readBytes = Compressor.Compress(readBytes);

                var cacheItem = new CacheItem(readBytes, _isCompressed, _contentType);

                SaveToCache(context, _cacheKey, cacheItem, CacheDuration);

                if(context.Response.IsClientConnected)
                    WriteBytes(context, cacheItem, CacheDuration);
            }

            //context.Response.End();

            if(IsDebugEnabled)
                log.Debug("멀티 파일 요청 처리를 완료했습니다!!!");
        }

        /// <summary>
        /// Parameter 정보를 파싱해서, 원하는 결과를 만듭니다.
        /// </summary>
        /// <param name="context"></param>
        private void ParseRequestParams(HttpContext context) {
            var request = context.Request;

            // var _encoding = context.Response.ContentEncoding;

            var setName = request["S"].AsText();
            var fileName = request["F"].AsText();
            var version = request["V"].AsText("1.0");
            _contentType = request["T"].AsText(FileTool.DEFAULT_MIME_TYPE);
            _cacheDuration = TimeSpan.FromDays(request["Ex"].AsInt(30));

            // 유일한 캐시키가 만들어집니다^^
            _cacheKey = GetCacheKey(setName, fileName, version);

            if(IsDebugEnabled)
                log.Debug("요청정보. S=[{0}], F=[{1}], C=[{2}], V=[{3}], _cacheKey=[{4}]", setName, fileName, _contentType, version,
                          _cacheKey);


            // NOTE: Handler를 재사용하는 경우, 꼭 초기화를 시켜주어야 합니다.
            //
            _filenames.Clear();

            // 요청 파일 정보를 파싱해서 넣습니다.
            if(setName.IsNotWhiteSpace() && ConfigTool.HasAppSettings(setName)) {
                var setFiles = ConfigTool.GetAppSettings(setName, string.Empty);
                if(setFiles.IsNotWhiteSpace()) {
                    var filenames = setFiles.Split(new[] { FileDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                    filenames.RunEach(n => _filenames.Add(n));
                }
            }

            if(fileName.IsNotWhiteSpace()) {
                var filenames = fileName.Split(new[] { FileDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                filenames.RunEach(n => _filenames.Add(n));
            }

            if(IsDebugEnabled)
                log.Debug("요청파일=[{0}]", FileDelimiter.ToString().Join(_filenames.ToArray()));
        }

        private byte[] ReadFiles(HttpContext context) {
            if(_filenames == null)
                return new byte[0];

            if(IsDebugEnabled)
                log.Debug("요청한 파일 정보를 모두 로드합니다. files=[{0}]", _filenames.CollectionToString());

            var readTasks = new List<Task<byte[]>>();

            With.TryActionAsync(() => {
                                    readTasks.AddRange(_filenames.Select(filename => ReadFileTask(context, filename)));
                                    Task.WaitAll(readTasks.ToArray());
                                },
                                age => age.Handle(ex => {
                                                      if(log.IsErrorEnabled) {
                                                          log.Error("요청파일을 로드하는데 실패했습니다.");
                                                          log.Error(ex);
                                                      }

                                                      return true;
                                                  }));


            // 개행문자를 넣는다.
            var newlineBytes = context.Response.ContentEncoding.GetBytes(Environment.NewLine);

            byte[] multiBytes = new byte[3];

            using(var stream = new MemoryStream(0x1000)) {
                var isFirst = true;

                foreach(var task in readTasks.Where(t => t.IsCompleted)) {
                    var bytes = task.Result;
                    if(bytes == null || bytes.Length == 0)
                        continue;

                    // NOTE: 동아시아의 멀티바이트 언어인 경우 구별을 위해 붙는 PREFIX 를 첫번째 파일을 제외하고는 제거해야 합니다.
                    // 
                    var needRemoveMultibytesPrefix = (isFirst == false && bytes.Length >= multiBytes.Length &&
                                                      multiBytes.All(i => StringTool.MultiBytesPrefixBytes[i] == bytes[i]));

                    // 두번째 파일부터는 멀티바이트인지 검사해서 제거합니다.
                    if(needRemoveMultibytesPrefix)
                        stream.Write(bytes, 3, bytes.Length - 3);
                    else
                        stream.Write(bytes, 0, bytes.Length);

                    // 파일별로 구분하기 위해 NewLine을 넣는다.
                    stream.Write(newlineBytes, 0, newlineBytes.Length);
                    isFirst = false;
                }

                return stream.ToArray();
            }
        }

        /// <summary>
        /// 지정된 경로에서 리소스를 읽어오는 Task{byte[]}를 빌드합니다.
        /// </summary>
        private static Task<byte[]> ReadFileTask(HttpContext context, string virtualPath) {
            virtualPath.ShouldNotBeWhiteSpace("virtualPath");

            Task<byte[]> task = null;

            if(IsDebugEnabled)
                log.Debug("지정된 경로에서 리소스를 읽어오는 Task<byte[]>를 빌드합니다. virtualPath=[{0}]", virtualPath);

            // 외부경로의 리소스라면, 다운로드 받는다.
            if(IsWebResource(virtualPath)) {
                var webClient = new WebClient();

                task = webClient.DownloadDataTask(virtualPath);
                task.ContinueWith(_ => webClient.Dispose(), TaskContinuationOptions.ExecuteSynchronously);
            }
            else {
                var path = FileTool.GetPhysicalPath(virtualPath);

                task = path.FileExists()
                           ? FileAsync.ReadAllBytes(FileTool.GetPhysicalPath(virtualPath))
                           : Task.Factory.FromResult(new byte[0]);
            }

            return task;
        }

        /// <summary>
        /// 서버 메모리 캐시에 파일 정보를 저장합니다
        /// </summary>
        private static void SaveToCache(HttpContext context, string key, CacheItem cacheItem, TimeSpan duration) {
            if(IsDebugEnabled)
                log.Debug("웹 캐시에 파일 데이터를 보관합니다... key=[{0}], duration=[{1}]", key, duration);

            context.Cache.Insert(key, cacheItem, null, Cache.NoAbsoluteExpiration, duration);
        }

        /// <summary>
        /// 캐시에 저장된 정보가 있다면, 그 정보를 Client에게 보냅니다.
        /// </summary>
        private bool WriteFromCache(HttpContext context, string cacheKey, TimeSpan cacheDuration) {
            var cacheItem = context.Cache[cacheKey] as CacheItem;

            if(cacheItem == null || cacheItem.Data == null || cacheItem.Data.Length == 0)
                return false;

            if(IsDebugEnabled)
                log.Debug("웹 캐시에 데이타가 보관되어 있습니다. 캐시 정보를 이용하여 응답합니다. cackeKey=[{0}]", cacheKey);

            WriteBytes(context, cacheItem, cacheDuration);
            return true;
        }

        /// <summary>
        /// 요청 파일의 정보를 응답 스트림에 씁니다.
        /// </summary>
        private void WriteBytes(HttpContext context, CacheItem item, TimeSpan cacheDuration) {
            if(item == null || item.Data == null || item.Data.Length == 0)
                return;

            if(IsDebugEnabled)
                log.Debug("복수의 파일의 통합본 정보를 응답스트림에 씁니다... isCompressed=[{0}], contentType=[{1}], cacheDuration=[{2}]",
                          item.IsCompressed, item.ContentType, cacheDuration);

            HttpResponse response = context.Response;

            if(response.IsClientConnected == false)
                return;

            response.Clear();
            SetResponseHeaders(response, item, cacheDuration);
            response.OutputStream.Write(item.Data, 0, item.Data.Length);
            response.Flush();
        }

        private void SetResponseHeaders(HttpResponse response, CacheItem item, TimeSpan cacheDuration) {
            response.ShouldNotBeNull("response");

            response.AppendHeader("Content-Length", item.Data.Length.ToString());
            response.ContentType = item.ContentType;

            if(item.IsCompressed)
                response.AppendHeader("Content-Encoding", CompressionKind.ToString().ToLower());

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
        /// 지정한 경로가 웹 리소스를 나타내는지 확인합니다.
        /// </summary>
        /// <param name="uriString"></param>
        /// <returns></returns>
        private static bool IsWebResource(string uriString) {
            return uriString.StartsWith(Uri.UriSchemeHttp, StringComparison.InvariantCultureIgnoreCase) ||
                   uriString.StartsWith(Uri.UriSchemeHttps, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 캐시 키로 사용할 문자열을 만듭니다.
        /// </summary>
        private static string GetCacheKey(string setName, string fileName, string version) {
            return string.Format("{0}@{1}.{2}${3}#{4}", HandlerTypeName, HostingEnvironment.ApplicationVirtualPath, setName, fileName,
                                 version);
        }

        [Serializable]
        public class CacheItem {
            public CacheItem(byte[] data) : this(data, true, FileTool.DEFAULT_MIME_TYPE) {}
            public CacheItem(byte[] data, bool isCompressed) : this(data, isCompressed, FileTool.DEFAULT_MIME_TYPE) {}

            public CacheItem(byte[] data, bool isCompressed, string contentType) {
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