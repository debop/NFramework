using System.Resources;

namespace NSoft.NFramework.ResourceProviders {
    /// <summary>
    /// 리소스 정보를 제공하는 Provider입니다.
    /// </summary>
    public interface IResourceProvider {
        ResourceManager ResourceManager { get; }

        /// <summary>
        /// 리소스 문자열을 로드합니다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetString(string name);

        /// <summary>
        /// 리소스 문자열을 로드합니다.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        string GetString(string name, params object[] values);

        /// <summary>
        /// 리소스 키에 해당하는 정보를 로드합니다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        object GetObject(string name);

        /// <summary>
        /// 리소스 키에 해당하는 정보를 로드합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T GetObject<T>(string name);
    }
}