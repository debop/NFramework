using System;
using System.IO;
using System.Threading;
using NSoft.NFramework.IO;
using NSoft.NFramework.Nini.Config;
using NSoft.NFramework.Nini.Ini;

namespace NSoft.NFramework.Data.QueryProviders {
    /// <summary>
    /// 쿼리문이 정의된 파일로부터 쿼리문을 제공하는 프로바이더의 기본 클래스입니다. 동적으로 파일의 변화에 따른 갱신을 수행합니다.
    /// NOTE: 파일을 취급하므로, Thread-Safe 하지 않습니다!!!
    /// </summary>
    public abstract class DynamicQueryProviderBase : InIQueryProviderBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 쿼리문 조회시, Section과 Key의 구분자 (",")
        /// </summary>
        public const string SectionDelimiter = ",";

        private readonly object _syncLock = new object();

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="queryPath">Query string 이 정의된 파일의 전체 경로</param>
        protected DynamicQueryProviderBase(string queryPath) : base(queryPath) {
            BuildFileSystemWatcher(QueryFilePath);
        }

        /// <summary>
        /// 쿼리정의 파일에 대한 변화를 감시하는 Watcher입니다.
        /// </summary>
        protected FileSystemWatcher Watcher { get; private set; }

        private IniConfigSource _querySource;

        /// <summary>
        /// 쿼리 문장을 제공하는 <see cref="Nini.Config.IConfigSource"/>의 인스턴스
        /// </summary>
        protected override IConfigSource QuerySource {
            get {
                if(_querySource == null)
                    lock(_syncLock)
                        if(_querySource == null) {
                            BuildQuerySource();
                        }

                return _querySource;
            }
        }

        /// <summary>
        /// 쿼리 파일 내용이 변경되었을 때 발생하는 이벤트입니다.
        /// </summary>
        public event EventHandler<FileSystemEventArgs> QueryFileChanged = delegate { };

        /// <summary>
        /// 쿼리 문자열을 제공하는 파일을 읽어서 <see cref="IniConfigSource"/> 인스턴스를 빌드합니다.
        /// </summary>
        protected virtual void BuildQuerySource() {
            if(IsDebugEnabled)
                log.Debug("Samba style의 Ini 파일형태의 쿼리정의 정보를 제공하는 QuerySource를 빌드합니다... queryFilePath=[{0}]", QueryFilePath);

            Guard.Assert(QueryFilePath.FileExists(), @"지정된 파일을 찾을 수 없습니다. QueryFilePath=[{0}]", QueryFilePath);

            var source = new IniConfigSource(new IniDocument(QueryFilePath, IniFileType.SambaStyle));
            Thread.MemoryBarrier();
            _querySource = source;
        }

        /// <summary>
        /// 쿼리 문자열을 제공하는 파일의 변화를 감지하기 위해 <see cref="FileSystemWatcher"/> 를 이용한 감시기능을 빌드합니다.
        /// </summary>
        /// <param name="queryFile"></param>
        protected void BuildFileSystemWatcher(string queryFile) {
            queryFile.ShouldNotBeWhiteSpace("queryFile");

            Watcher = new FileSystemWatcher(Path.GetDirectoryName(queryFile))
                      {
                          Filter = Path.GetFileName(queryFile),
                          EnableRaisingEvents = true
                      };

            Watcher.Changed += OnQueryFileChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnQueryFileChanged(object sender, FileSystemEventArgs e) {
            if(e.FullPath.Equals(QueryFilePath, StringComparison.InvariantCultureIgnoreCase) &&
               (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created)) {
                if(log.IsInfoEnabled)
                    log.Info("쿼리 파일 내용이 변경되어 새로 빌드합니다... QueryFilePath=[{0}]", QueryFilePath);

                BuildQuerySource();

                QueryFileChanged(this, e);
            }
        }
    }
}