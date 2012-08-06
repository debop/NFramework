using System.Collections;
using System.Collections.Generic;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// Base Factory for <see cref="System.Web.Compilation.IResourceProvider"/>
    /// </summary>
    public abstract class ResourceProviderFactoryBase : System.Web.Compilation.ResourceProviderFactory, IResourceProviderFactory {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Lookup table to store providers.
        /// </summary>
        public class ProviderLookupTable : Dictionary<string, IStringResourceProvider> {}

        private static readonly ProviderLookupTable _providers = new ProviderLookupTable();
        private static readonly object _sync = new object();

        /// <summary>
        /// Lookup table of Providers.
        /// </summary>
        public static ProviderLookupTable ProviderTable {
            get { return _providers; }
        }

        /// <summary>
        /// global resource provider name.
        /// </summary>
        public string GlobalResourceProviderName { get; set; }

        /// <summary>
        /// local resource provider name.
        /// </summary>
        public string LocalResourceProviderName { get; set; }

        /// <summary>
        /// Create Global Resource Provider
        /// </summary>
        /// <param name="classKey">[AssemblyName|]resourceFilename (or [Application|]Section / [Filename|]Section)</param>
        /// <returns></returns>
        public override System.Web.Compilation.IResourceProvider CreateGlobalResourceProvider(string classKey) {
            if(IsDebugEnabled)
                log.Debug(@"Global Resource Provider를 생성합니다... providerName=[{0}], classKey=[{1}]", GlobalResourceProviderName, classKey);

            return CreateResourceProvider(GlobalResourceProviderName, classKey);
        }

        /// <summary>
        /// Create Local Resource Provider
        /// </summary>
        /// <param name="virtualPath">[Application|]virtualPath</param>
        /// <returns></returns>
        public override System.Web.Compilation.IResourceProvider CreateLocalResourceProvider(string virtualPath) {
            if(IsDebugEnabled)
                log.Debug("Local Resource Provider 생성합니다... providerName=[{0}], virtualPath=[{1}]", LocalResourceProviderName,
                          virtualPath);

            return CreateResourceProvider(LocalResourceProviderName, virtualPath);
        }

        /// <summary>
        /// IoC 를 이용해 원하는 ResourceProvider component를 resolve한다.
        /// </summary>
        /// <param name="componentId"></param>
        /// <param name="classKey"></param>
        /// <returns></returns>
        internal static IStringResourceProvider CreateResourceProvider(string componentId, string classKey) {
            if(IsDebugEnabled)
                log.Debug("Castle.Windsor를 이용하여 IStringResourceProvider의 인스턴스를 생성합니다... component id=[{0}], classKey=[{1}]", componentId,
                          classKey);

            var arguments = new Hashtable();

            string assemblyName, resourceName;
            StringResourceTool.ParseClassKey(classKey, out assemblyName, out resourceName);

            var key = componentId + ":" + assemblyName + StringResourceTool.CLASS_KEY_DELIMITER + resourceName;

            if(IsDebugEnabled)
                log.Debug("Parsing classKey. store provider instance in hashtable with key=[{0}]", key);

            IStringResourceProvider provider;

            lock(_sync) {
                if(_providers.TryGetValue(key, out provider) == false) {
                    if(IsDebugEnabled)
                        log.Debug("캐시에 없으므로 새로운 ResourceProvider를 생성합니다.");

                    if(assemblyName.IsNotWhiteSpace() && assemblyName != StringResourceTool.DEFAULT_ASSEMBLY_NAME)
                        arguments.Add(StringResourceTool.CLASS_ASSEMBLY_NAME, assemblyName);

                    arguments.Add(StringResourceTool.CLASS_RESOURCE_NAME, resourceName);

                    // NOTE : ResourceProvider의 Lifestyle은 항상 Transient|Thread|PerWebRequest 여야 한다.

                    provider = IoC.Resolve<IStringResourceProvider>(componentId, arguments);

                    if(provider != null && provider is ResourceProviderBase) {
                        // Parameterized Key 를 파싱할 수 있는 Provider로 decoration한다. 
                        // FileResourceProvider는 자체적으로 제공하므로 필요없다.
                        if((provider is FileResourceProvider) == false)
                            provider = new ParameterizedResourceProvider((ResourceProviderBase)provider);

                        if(IsDebugEnabled)
                            log.Debug("생성된 IResourceProvider를 캐시에 저장합니다. key=[{0}]", key);

                        _providers.Add(key, provider);
                    }
                }

                if(IsDebugEnabled)
                    log.Debug("새로운 ResourceProvider를 생성했습니다!!! provider=[{0}]", provider);
            }

            return provider;
        }

        IStringResourceProvider IResourceProviderFactory.CreateGlobalResourceProvider(string classKey) {
            return (IStringResourceProvider)CreateGlobalResourceProvider(classKey);
        }

        IStringResourceProvider IResourceProviderFactory.CreateLocalResourceProvider(string virtualPath) {
            return (IStringResourceProvider)CreateLocalResourceProvider(virtualPath);
        }
    }
}