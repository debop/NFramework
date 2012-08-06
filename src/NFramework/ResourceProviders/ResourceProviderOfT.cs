using System;
using System.Reflection;
using System.Resources;

namespace NSoft.NFramework.ResourceProviders {
    /// <summary>
    /// 해당 클래스가 속한 Assembly에서 리소스 로더를 생성합니다.
    /// </summary>
    /// <typeparam name="TClass">리소스를 찾고자 하는 어셈블리의 해당 클래스</typeparam>
    [Serializable]
    public class ResourceProvider<TClass> : ResourceProviderBase {
        private static readonly object _syncLock = new object();
        private static IResourceProvider _resourceProvider;

        protected ResourceProvider(Assembly resourceAssembly) : base(resourceAssembly) {}

        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static IResourceProvider Instance {
            get {
                if(_resourceProvider == null)
                    lock(_syncLock)
                        if(_resourceProvider == null) {
                            var loader = new ResourceProvider<TClass>(typeof(TClass).Assembly);
                            System.Threading.Thread.MemoryBarrier();
                            _resourceProvider = loader;
                        }

                return _resourceProvider;
            }
        }

        /// <summary>
        /// 리소스 매니저
        /// </summary>
        public new static ResourceManager ResourceManager {
            get { return Instance.ResourceManager; }
        }

        /// <summary>
        /// <paramref name="name"/>에 해당하는 리소스 문자열을 반환합니다.
        /// </summary>
        public new string GetString(string name) {
            return Instance.GetString(name);
        }

        /// <summary>
        /// <paramref name="name"/>에 해당하는 리소스 문자열을 반환합니다.
        /// </summary>
        public new string GetString(string name, object[] values) {
            return Instance.GetString(name, values);
        }

        /// <summary>
        /// <paramref name="name"/>에 해당하는 리소스 객체를 반환합니다.
        /// </summary>
        public new object GetObject(string name) {
            return Instance.GetObject(name);
        }

        /// <summary>
        /// <paramref name="name"/>에 해당하는 리소스 객체를 반환합니다.
        /// </summary>
        public new T GetObject<T>(string name) {
            return Instance.GetObject<T>(name);
        }
    }
}