using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NSoft.NFramework.Nini.Config;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.StringResources {
    internal class FileResourceHandler : IDisposable {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// constructors
        /// </summary>
        /// <param name="path">Resource File의 전체 경로 (예:c:\resources\glossary.ini)</param>
        /// <param name="section">Resource File 내부의 Ini Section이름 </param>
        public FileResourceHandler(string path, string section) {
            if(IsDebugEnabled)
                log.Debug("FileResourceHandler 인스턴스 생성. path=[{0}], section=[{1}]", path, section);

            DirectoryName = Path.GetDirectoryName(path);
            FileName = Path.GetFileNameWithoutExtension(path);
            Section = section;
            FileExtension = Path.GetExtension(DirectoryName).TrimStart('.');

            if(FileExtension.Length == 0)
                FileExtension = "ini";
        }

        /// <summary>
        /// Directory name of resource file.
        /// </summary>
        public string DirectoryName { get; private set; }

        /// <summary>
        /// File name of resource file.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Section name
        /// </summary>
        public string Section { get; private set; }

        /// <summary>
        /// File extension of resource file.
        /// </summary>
        public string FileExtension { get; private set; }

        ///<summary>
        /// 키 및 culture에 대한 리소스 개체를 반환합니다.
        ///</summary>
        ///<returns>
        ///<paramref name="resourceKey" /> 및 <paramref name="culture" />에 대한 리소스 값을 포함하는 <see cref="T:System.Object" />입니다.
        ///</returns>
        ///<param name="resourceKey">특정 리소스를 식별하는 키입니다.</param>
        ///<param name="culture">리소스의 지역화된 값을 식별하는 culture입니다.</param>
        /// <returns>resource value, if not exists, return null</returns>
        public virtual object GetObject(string resourceKey, CultureInfo culture) {
            return GetInvariant(resourceKey, culture.GetCulture());
        }

        /// <summary>
        /// 지정한 culture에 해당하는 resourceKey의 값을 찾는다.
        /// 해당 culture용 파일에 resourceKey가 정의되어 있지 않다면, culture의 부모 culture로 찾는다.
        /// 최후에는 기본 culture에서 값을 찾고, 그래도 없다면 null을 반환한다.
        /// </summary>
        ///<param name="resourceKey">특정 리소스를 식별하는 키입니다.</param>
        ///<param name="culture">리소스의 지역화된 값을 식별하는 culture입니다.</param>
        /// <returns>resource value, if not exists, return null</returns>
        protected virtual object GetInvariant(string resourceKey, CultureInfo culture) {
            if(culture.IsNullCulture())
                return null;

            object result = null;
            var resources = new ArrayList();

            var path = GetResourceFilePath(culture);

            if(File.Exists(path) == false) {
                if(log.IsWarnEnabled)
                    log.Warn("리소스 파일을 찾을 수 없습니다. path=[{0}]", path);
            }
            else {
                var cfg = GetConfigSource(path).Configs[Section];
                if(cfg != null && cfg.Contains(resourceKey)) {
                    resources.Add(cfg.Get(resourceKey));
                }
            }

            // 지정된 ResourceKey를 찾지 못했다면
            if(resources.Count == 0) {
                // 부모 Culture로부터 찾는다.
                result = GetInvariant(resourceKey, culture.Parent);
            }
            else if(resources.Count == 1)
                result = resources[0];
            else if(resources.Count > 1)
                throw new InvalidOperationException("리소스 키가 중복되었습니다. resourceKey=" + resourceKey);

            return result;
        }

        /// <summary>
        /// <see cref="FileResourceReader"/>를 위한 리소스에 대한 전체 정보
        /// </summary>
        ///<param name="culture">리소스의 지역화된 값을 식별하는 culture입니다.</param>
        /// <returns>리소스의 모든 정보 (Key=Value)</returns>
        public Hashtable GetResources(CultureInfo culture) {
            var resources = new Hashtable();

            try {
                culture = culture.GetCulture();
                var path = GetResourceFilePath(culture);

                var source = GetConfigSource(path);
                if(source != null) {
                    var cfg = source.Configs[Section];

                    if(cfg != null) {
                        foreach(var key in cfg.GetKeys())
                            resources.Add(key, cfg.Get(key));
                    }
                }
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled)
                    log.WarnException("리소스를 로드하는데 실패했습니다.", ex);
            }
            return resources;
        }

        /// <summary>
        ///지정한 Culture에 해당하는 Resource 파일을 찾는다. 파일을 찾을 때까지 부모 Culture의 파일 경로를 반환한다.
        /// (예: glossary.ini, glosssary.en.ini 만 존재할 때, 
        ///      en-US에 대해 찾으면 glossary.en.ini 을 반환
        ///      ko-KR에 대해 찾으면 glossary.ini 을 반환
        /// </summary>
        ///<param name="culture">리소스의 지역화된 값을 식별하는 culture입니다.</param>
        /// <returns>지역화 리소스 파일의 전체 경로</returns>
        protected virtual string GetResourceFilePath(CultureInfo culture) {
            var path = culture.Name.IsWhiteSpace()
                           ? Path.Combine(DirectoryName, string.Format("{0}.{1}", FileName, FileExtension))
                           : Path.Combine(DirectoryName, string.Format("{0}.{1}.{2}", FileName, culture.Name, FileExtension));

            if(File.Exists(path) == false)
                path = Path.Combine(DirectoryName, string.Format("{0}.{1}", FileName, FileExtension));

            if(!File.Exists(path))
                if(log.IsWarnEnabled)
                    log.Warn(@"리소스 파일을 찾을 수 없습니다!!! path=[{0}], culture=[{1}]", path, culture);

            return path;
        }

        private class ConfingSourceMap : Dictionary<string, IConfigSource> {}

        private static readonly ConfingSourceMap _configSourceMap = new ConfingSourceMap();

        /// <summary>
        /// 지정된 Resouce 파일에 대한 NIni 용 <see cref="IConfigSource"/> 인스턴스를 생성, 반환한다.
        /// 속도를 위해 한번 생성된 인스턴스는 캐시된다.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static IConfigSource GetConfigSource(string path) {
            IConfigSource configSource;

            lock(_configSourceMap) {
                if(_configSourceMap.TryGetValue(path, out configSource) == false) {
                    if(Path.GetExtension(path).ToLower().Contains("xml"))
                        configSource = new XmlConfigSource(path);
                    else
                        configSource = new IniConfigSource(path);

                    configSource.ExpandKeyValues();

                    _configSourceMap.Add(path, configSource);

                    if(IsDebugEnabled)
                        log.Debug("Nini.IConfigSource 인스턴스를 생성하여 캐시에 저장했습니다. path=[{0}]", path);
                }
            }

            return configSource;
        }

        #region << IDisposable >>

        public bool IsDisposed { get; protected set; }

        ~FileResourceHandler() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if(IsDisposed)
                return;

            if(disposing) {
                With.TryAction(() => {
                                   lock(_configSourceMap)
                                       _configSourceMap.Clear();
                               });

                if(IsDebugEnabled)
                    log.Debug(@"FileResourceHandler 인스턴스를 Dispose 했습니다.");
            }

            IsDisposed = true;
        }

        #endregion
    }
}