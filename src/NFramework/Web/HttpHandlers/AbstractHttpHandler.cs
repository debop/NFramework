using System;
using System.Web;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.Web.HttpHandlers {
    /// <summary>
    /// HttpHandler의 가장 기본이 되는 클래스입니다.
    /// </summary>
    [Serializable]
    public abstract class AbstractHttpHandler : IHttpHandler {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly object _syncLock = new object();

        #region << IHttpHandler >>

        public void ProcessRequest(HttpContext context) {
            if(IsDebugEnabled)
                log.Debug("동기 방식의 HttpHandler의 처리를 시작합니다...");

            DoProcessRequest(context);

            if(IsDebugEnabled)
                log.Debug("동기 방식의 HttpHandler의 처리를 완료했습니다!!!");
        }

        public virtual bool IsReusable {
            get { return true; }
        }

        #endregion

        /// <summary>
        /// HttpHandler의 작업의 메인 메소드입니다. 재정의 하여 원하는 기능을 수행하되, 제일 첫번째에 부모 클래스의 메소들를 호출해주어야 합니다.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void DoProcessRequest(HttpContext context) {
            context.ShouldNotBeNull("context");
            _compressionKind = RetriveAvailableCompressionKind(context);

            if(IsDebugEnabled)
                log.Debug("Client가 수용할 수 있는 압축 방식=[{0}]", _compressionKind);
        }

        /// <summary>
        /// 최소 압축 크기
        /// </summary>
        public const int DefaultCompressionThreshold = 0x1000; // (NOTE: 4096 byte 이상이어야 압축을 합니다)

        private int _compressionThreshold = DefaultCompressionThreshold;
        private WebCompressionKind _compressionKind;
        private ICompressor _compressor;

        /// <summary>
        /// 압축하기 위한 최소 크기 (이 크기 이하의 컨텐츠는 굳이 압축하지 않는다)
        /// </summary>
        protected virtual int CompressionThreshold {
            get { return _compressionThreshold; }
            set { _compressionThreshold = Math.Max(value, DefaultCompressionThreshold); }
        }

        /// <summary>
        /// Client가 수용할 수 있는 압축 방법
        /// </summary>
        protected WebCompressionKind CompressionKind {
            get { return _compressionKind; }
        }

        /// <summary>
        /// 압축을 할 수 있는지?
        /// </summary>
        protected bool CanCompression {
            get { return (_compressionKind != WebCompressionKind.None); }
        }

        /// <summary>
        /// 압축기
        /// </summary>
        protected ICompressor Compressor {
            get {
                if(_compressor == null)
                    lock(_syncLock)
                        if(_compressor == null) {
                            var compressor = CreateCompressor(_compressionKind);
                            System.Threading.Thread.MemoryBarrier();
                            _compressor = compressor;
                        }

                return _compressor;
            }
        }

        private static WebCompressionKind RetriveAvailableCompressionKind(HttpContext context) {
            if(context.CanBrowserAcceptGzip())
                return WebCompressionKind.GZip;

            if(context.CanBrowserAcceptDeflate())
                return WebCompressionKind.Deflate;

            return WebCompressionKind.None;
        }

        private static ICompressor CreateCompressor(WebCompressionKind compressionKind) {
            switch(compressionKind) {
                case WebCompressionKind.GZip:
                    // NOTE: SharpGZipCompressor() 가 압축률이 더 좋다!!! (CPU 리소스는 더 먹는다) 단 암호화등과 같이 쓰면 예외가 발생한다.
                    // return new SharpGZipCompressor();
                    return new GZipCompressor();

                case WebCompressionKind.Deflate:
                    return new DeflateCompressor();

                default:
                    // NOTE: DummyCompressor는 압축도 하지 않고, 복사도 하지 않고, 인스턴스 메모리 참조값을 그대로 전달해준다.
                    return new DummyCompressor();
            }
        }
    }
}