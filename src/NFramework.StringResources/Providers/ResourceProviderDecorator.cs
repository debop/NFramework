using System.Globalization;

namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// StringResourceProvider에 대한 Decorator
    /// </summary>
    public abstract class ResourceProviderDecorator : IStringResourceProvider {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="innerProvider">wrapped instance of ResourceProviderBase</param>
        protected ResourceProviderDecorator(ResourceProviderBase innerProvider) {
            innerProvider.ShouldNotBeNull("innerProvider");
            InnerProvider = innerProvider;
        }

        /// <summary>
        /// Wrapped instance of ResourceProviderBase
        /// </summary>
        public ResourceProviderBase InnerProvider { get; protected set; }

        /// <summary>
        /// Assembly name
        /// </summary>
        public virtual string AssemblyName {
            get { return InnerProvider.AssemblyName; }
            set { InnerProvider.AssemblyName = value; }
        }

        /// <summary>
        /// Resource name
        /// </summary>
        public virtual string ResourceName {
            get { return InnerProvider.ResourceName; }
            set { InnerProvider.ResourceName = value; }
        }

        ///<summary>
        /// 소스에서 리소스 값을 읽기 위한 개체를 가져옵니다.
        ///</summary>
        ///<returns>
        /// 현재 리소스 공급자와 관련된 <see cref="T:System.Resources.IResourceReader" />입니다.
        ///</returns>
        public virtual System.Resources.IResourceReader ResourceReader {
            get { return InnerProvider.ResourceReader; }
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
        public virtual object GetObject(string resourceKey, CultureInfo culture) {
            return InnerProvider.GetObject(resourceKey, culture);
        }
    }
}