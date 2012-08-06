using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// Local Resource Assembly로부터 정보를 얻습니다. AssemblyName에 값을 넣지 않습니다.
    /// </summary>
    public class LocalResourceProvider : ResourceProviderBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        protected LocalResourceProvider() {}

        /// <summary>
        /// Initialize a new instance of LocalResourceProvider with default assembly name and resourceName.
        /// </summary>
        /// <param name="resourceName"></param>
        public LocalResourceProvider(string resourceName) : base(StringResourceTool.DEFAULT_ASSEMBLY_NAME, resourceName) {}

        /// <summary>
        /// Initialize a new instance of LocalResourceProvider with assembly name and resourceName.
        /// </summary>
        /// <param name="assemblyName">assembly name</param>
        /// <param name="resourceName">resource name</param>
        protected LocalResourceProvider(string assemblyName, string resourceName) : base(assemblyName, resourceName) {}

        /// <summary>
        /// 동기화 객체
        /// </summary>
        [CLSCompliant(false)] protected static readonly object _syncLock = new object();

        private ResourceManager _resourceManager;

        /// <summary>
        /// Extern Resource Manager
        /// </summary>
        public ResourceManager ResourceManager {
            get {
                if(_resourceManager == null)
                    CreateResourceManager();

                return _resourceManager;
            }
            set {
                lock(_syncLock)
                    _resourceManager = value;
            }
        }

        ///<summary>
        /// 소스에서 리소스 값을 읽기 위한 개체를 가져옵니다.
        ///</summary>
        ///<returns>
        /// 현재 리소스 공급자와 관련된 <see cref="T:System.Resources.IResourceReader" />입니다.
        ///</returns>
        /// <remarks>
        /// LocalResourceProvider 는 <see cref="IResourceReader"/>를 지원하지 않는다.
        /// </remarks>
        public override IResourceReader ResourceReader {
            get {
                if(log.IsInfoEnabled)
                    log.Info(@"ExternalResourceProvider는 ResourceReader를 지원하지 않습니다. null 을 반환합니다.");

                return null;
            }
        }

        /// <summary>
        /// Create <see cref="ResourceManager"/> with <see cref="ResourceProviderBase.AssemblyName"/> and <see cref="ResourceProviderBase.ResourceName"/>
        /// </summary>
        protected virtual void CreateResourceManager() {
            if(_resourceManager != null)
                return;

            lock(_syncLock) {
                if(_resourceManager == null) {
                    if(IsDebugEnabled)
                        log.Debug(@"Create new resource manager. resource file=[{0}.{1}]", AssemblyName, ResourceName);

                    try {
                        var asm = AssemblyName.IsWhiteSpace()
                                      ? Assembly.GetEntryAssembly()
                                      : Assembly.Load(AssemblyName);

                        if(IsDebugEnabled)
                            log.Debug(@"Load assembly: " + asm.FullName);

                        _resourceManager = new ResourceManager(string.Format("{0}.{1}", AssemblyName, ResourceName), asm);
                    }
                    catch(Exception ex) {
                        if(log.IsErrorEnabled) {
                            log.Error(@"ResourceManager를 생성하는데 실패했습니다. AssemblyName=[{0}], ResourceName=[{1}]", AssemblyName,
                                      ResourceName);
                            log.Error(ex);
                        }

                        throw;
                    }
                }
            }
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
        /// </param>
        public override object GetObject(string resourceKey, CultureInfo culture) {
            if(IsDebugEnabled)
                log.Debug(@"리소스 값을 조회합니다... resourceKey=[{0}], culture=[{1}]", resourceKey, culture);

            return ResourceManager.GetObject(resourceKey, culture.GetOrCurrentUICulture());
        }
    }
}