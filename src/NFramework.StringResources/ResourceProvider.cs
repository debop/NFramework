using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Web.Compilation;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// 사용자가 일반적으로 사용하는 클래스이다.
    /// Castle.Windsor component에는 IResourceProviderFactory를 정의하면된다.
    /// </summary>
    public static class ResourceProvider {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Provider Cache
        /// </summary>
        private static readonly IDictionary<string, IResourceProvider> _providers = new Dictionary<string, IResourceProvider>();

        private static IResourceProviderFactory _providerFactory;
        private static readonly object _syncLock = new object();

        /// <summary>
        /// <see cref="IResourceProvider"/>를 생성해주는 <see cref="IResourceProviderFactory"/>를 반환한다. 
        /// 이 설정정보는 Castle Windsor component로 정의되어져 있다.
        /// </summary>
        /// <remark>
        /// 리소스에 있는 Windsor.Provider.config 를 참고하세요
        /// </remark>
        /// <example>
        ///		<!-- ResourceProviderFactory를 지정합니다. -->
        ///     <component id="ResourceProviderFactory"
        ///                service="NSoft.NFramework.StringResources.IResourceProviderFactory, NSoft.NFramework.StringResources"                  
        ///                type="NSoft.NFramework.StringResources.DefaultResourceProviderFactory, NSoft.NFramework.StringResources">
        ///         <parameters>
        ///             <culture>#{culture}</culture>
        ///				<GlobalResourceProviderId>ExternalResourceProvider</GlobalResourceProviderId>
        ///				<?if DEBUG?>
        ///				<LocalResourceProviderId>FileResourceProvider</LocalResourceProviderId>
        ///				<?else?>
        ///				<LocalResourceProviderId>NHResourceProvider</LocalResourceProviderId>
        ///				<?end?>
        ///			</parameters>
        ///		</component>
        /// </example>
        public static IResourceProviderFactory ProviderFactory {
            get {
                if(_providerFactory == null)
                    lock(_syncLock)
                        if(_providerFactory == null) {
                            var factory = IoC.TryResolve<IResourceProviderFactory>(() => new DefaultResourceProviderFactory(), true);
                            Thread.MemoryBarrier();
                            _providerFactory = factory;
                        }

                return _providerFactory;
            }
            set {
                lock(_syncLock)
                    _providerFactory = value;
            }
        }

        /// <summary>
        /// 지정된 Class Key에 해당하는 Provider를 제공한다. 
        /// classKey의 형식은 [AssemblyName|]ResourceName 또는 [FileName|]SectionName 으로 표현된다. 
        /// </summary>
        /// <param name="classKey"></param>
        /// <returns></returns>
        internal static IResourceProvider GetProvider(string classKey) {
            classKey.ShouldNotBeWhiteSpace("classKey");

            if(IsDebugEnabled)
                log.Debug(@"ResourceProvider 인스턴스를 빌드합니다. classKey=[{0}]", classKey);

            IResourceProvider provider = null;

            lock(_syncLock) {
                if(_providers.TryGetValue(classKey, out provider) == false) {
                    provider = ProviderFactory.CreateGlobalResourceProvider(classKey);

                    if(provider != null)
                        _providers.Add(classKey, provider);
                }
            }

            provider.ShouldNotBeNull("provider");

            if(log.IsInfoEnabled)
                log.Info("새로운 Global resource provider를 생성했습니다. classKey=[{0}], provider=[{1}], CurrentUICulture=[{2}]",
                         classKey, provider.GetType().FullName, CultureInfo.CurrentUICulture);

            return provider;
        }

        /// <summary>
        /// 지정된 Class Key에 해당하는 Provider를 제공한다. 
        /// classKey의 형식은 [AssemblyName|]ResourceName 또는 [FileName|]SectionName 으로 표현된다. 
        /// </summary>
        public static object GetObject(string classKey, string resourceKey, CultureInfo culture = null) {
            return GetProvider(classKey).GetObject(resourceKey, culture.GetOrCurrentUICulture());
        }

        /// <summary>
        /// 지정된 Class Key에 해당하는 Provider를 제공한다. 
        /// classKey의 형식은 [AssemblyName|]ResourceName 또는 [FileName|]SectionName 으로 표현된다. 
        /// </summary>
        public static string GetString(string classKey, string resourceKey, CultureInfo culture = null) {
            return GetProvider(classKey).GetObject(resourceKey, culture.GetOrCurrentUICulture()) as string;
        }
    }
}