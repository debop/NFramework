namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// 문자열 리소스 제공자의 기본 인터페이스입니다.
    /// </summary>
    public interface IStringResourceProvider : System.Web.Compilation.IResourceProvider {
        /// <summary>
        /// Assembly name
        /// </summary>
        string AssemblyName { get; set; }

        /// <summary>
        /// Resource name
        /// </summary>
        string ResourceName { get; set; }
    }
}