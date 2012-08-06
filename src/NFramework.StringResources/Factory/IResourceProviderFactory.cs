namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// Resource Provider의 인스턴스를 생성하는 Factory
    /// </summary>
    public interface IResourceProviderFactory {
        /// <summary>
        /// Global Resource 제공자 타입명 (예: ExternalResourceProvider, FileResourceProvider, AdoResourceProvider 등)
        /// </summary>
        string GlobalResourceProviderName { get; set; }

        /// <summary>
        /// Local Resource 제공자 타입명 (예: ExternalResourceProvider, FileResourceProvider, AdoResourceProvider 등)
        /// </summary>
        string LocalResourceProviderName { get; set; }

        /// <summary>
        /// 지정한 ClassKey에 해당하는 Global Resource Provider를 생성합니다.
        /// </summary>
        /// <param name="classKey"></param>
        /// <returns></returns>
        IStringResourceProvider CreateGlobalResourceProvider(string classKey);

        /// <summary>
        /// 지정한 ClassKey에 해당하는 Local Resource Provider를 생성합니다.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        IStringResourceProvider CreateLocalResourceProvider(string virtualPath);
    }
}