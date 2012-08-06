using System.Globalization;
using System.IO;
using System.Resources;
using NSoft.NFramework.IO;

namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// Ini 파일에 저장된 String Resource 정보를 제공합니다.
    /// </summary>
    public class FileResourceProvider : ResourceProviderBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly object _syncLock = new object();

        /// <summary>
        /// 리소스 파일의 기본 타입 (ini)
        /// </summary>
        public const string DefaultFileType = @"ini";

        private FileResourceHandler _handler;

        /// <summary>
        /// Constructor
        /// </summary>
        protected FileResourceProvider() {
            FileType = DefaultFileType;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="resourceName"></param>
        public FileResourceProvider(string assemblyName, string resourceName) : base(assemblyName, resourceName) {
            if(IsDebugEnabled)
                log.Debug("FileResourceProvider 인스턴스 생성. assemblyName=[{0}], resourceName=[{1}]", assemblyName, resourceName);

            FileType = DefaultFileType;
        }

        /// <summary>
        /// Resource File들이 있는 Directory Name (절대, 상대 둘다 된다.)
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// File Extension name of Resource file. (value="ini" | "xml", default="ini")
        /// </summary>
        public string FileType { get; set; }

        internal FileResourceHandler Handler {
            get {
                if(_handler == null)
                    lock(_syncLock)
                        if(_handler == null) {
                            var handler = new FileResourceHandler(GetResourceFilePath(), ResourceName);
                            System.Threading.Thread.MemoryBarrier();
                            _handler = handler;
                        }

                return _handler;
            }
        }

        /// <summary>
        /// 파일 변경을 감시하는 Watcher입니다.
        /// </summary>
        protected FileSystemWatcher Watcher { get; set; }

        ///<summary>
        /// 소스에서 리소스 값을 읽기 위한 개체를 가져옵니다.
        ///</summary>
        ///<returns>
        /// 현재 리소스 공급자와 관련된 <see cref="IResourceReader"/>입니다.
        ///</returns>
        public override IResourceReader ResourceReader {
            get { return new FileResourceReader(_handler.GetResources(CultureInfo.CurrentUICulture)); }
        }

        ///<summary>
        /// 키 및 culture에 대한 리소스 개체를 반환합니다.
        ///</summary>
        ///<returns>
        ///<paramref name="resourceKey" /> 및 <paramref name="culture" />에 대한 리소스 값을 포함하는 <see cref="T:System.Object" />입니다.
        ///</returns>
        ///<param name="resourceKey">
        /// 특정 리소스를 식별하는 키입니다.
        /// </param>
        ///<param name="culture">
        /// 리소스의 지역화된 값을 식별하는 culture입니다.
        ///</param>
        /// <returns>resource value, if not exists, return null</returns>
        public override object GetObject(string resourceKey, CultureInfo culture) {
            return Handler.GetObject(resourceKey, culture);
        }

        /// <summary>
        /// 리소스 파일 경로
        /// </summary>
        /// <returns></returns>
        private string GetResourceFilePath() {
            return FileTool.GetPhysicalPath(Path.Combine(Directory, AssemblyName + "." + FileType));
        }
    }
}