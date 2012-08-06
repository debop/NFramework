using System;
using System.Reflection;

namespace NSoft.NFramework.ResourceProviders {
    /// <summary>
    /// 어셈블리의 리소스의 정보를 제공합니다.
    /// </summary>
    [Serializable]
    public class ResourceProvider : ResourceProviderBase {
        public ResourceProvider(Assembly resourceAssembly) : base(resourceAssembly) {}
    }
}