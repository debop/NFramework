using System.Net;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.Tools {
    /// <summary>
    /// <see cref="Dns"/> 관련 메소드를 비동기 실행할 수 있도록 합니다.
    /// </summary>
    /// <remarks>
    /// 참고 사이트 :
    /// <list>
    ///		<item>http://msdn.microsoft.com/ko-kr/library/ms228963.aspx</item>
    ///		<item>http://msdn.microsoft.com/ko-kr/library/dd997423.aspx</item>
    /// </list>
    /// </remarks>
    public static class DnsAsync {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// <paramref name="hostNameOrAddress"/>에 해당하는 <see cref="IPAddress"/> 들을 비동기적으로 조회합니다.
        /// </summary>
        /// <param name="hostNameOrAddress"></param>
        /// <returns></returns>
        public static Task<IPAddress[]> GetHostAddresses(string hostNameOrAddress) {
            hostNameOrAddress.ShouldNotBeWhiteSpace("hostNameOrAddress");

            return
                Task<IPAddress[]>.Factory
                    .FromAsync(Dns.BeginGetHostAddresses,
                               Dns.EndGetHostAddresses,
                               hostNameOrAddress,
                               null);
        }

        /// <summary>
        /// 호스트 이름 또는 IP 주소로 <see cref="IPHostEntry"/>로 빌드합니다.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static Task<IPHostEntry> GetHostEntry(IPAddress address) {
            address.ShouldNotBeNull("address");

            return
                Task<IPHostEntry>.Factory
                    .FromAsync(Dns.BeginGetHostEntry,
                               Dns.EndGetHostEntry,
                               address,
                               null);
        }

        /// <summary>
        /// 호스트 이름 또는 IP 주소로 <see cref="IPHostEntry"/>로 빌드합니다.
        /// </summary>
        /// <param name="hostNameOrAddress"></param>
        /// <returns></returns>
        public static Task<IPHostEntry> GetHostEntry(string hostNameOrAddress) {
            hostNameOrAddress.ShouldNotBeWhiteSpace("hostNameOrAddress");

            return
                Task<IPHostEntry>.Factory
                    .FromAsync(Dns.BeginGetHostEntry,
                               Dns.EndGetHostEntry,
                               hostNameOrAddress,
                               null);
        }
    }
}