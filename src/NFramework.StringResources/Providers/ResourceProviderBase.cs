using System.Globalization;

namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// Base class for Resource Providing.
    /// </summary>
    public abstract class ResourceProviderBase : IStringResourceProvider // System.Web.Compilation.IResourceProvider
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        protected ResourceProviderBase() {}

        /// <summary>
        /// Constructor
        /// </summary>
        protected ResourceProviderBase(string assemblyName, string resourceName) {
            if(IsDebugEnabled)
                log.Debug("ResourceProviderBase 인스턴스 생성. assemblyName=[{0}], resourceName=[{1}]", assemblyName, resourceName);

            AssemblyName = assemblyName;
            ResourceName = resourceName;
        }

        /// <summary>
        /// Assembly name
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// Resource name
        /// </summary>
        public string ResourceName { get; set; }

        ///<summary>
        /// 소스에서 리소스 값을 읽기 위한 개체를 가져옵니다.
        ///</summary>
        ///<returns>
        /// 현재 리소스 공급자와 관련된 <see cref="T:System.Resources.IResourceReader" />입니다.
        ///</returns>
        public abstract System.Resources.IResourceReader ResourceReader { get; }

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
        public abstract object GetObject(string resourceKey, CultureInfo culture);

        /// <summary>
        /// 현재 인스턴스를 문자열로 나타냅니다.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return string.Format("{0}# AssemblyName=[{1}], ResourceName=[{2}]", GetType().Name, AssemblyName, ResourceName);
        }
    }
}