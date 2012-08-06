using System.Collections.Generic;
using System.Linq;
using System.Net;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Networks {
    /// <summary>
    /// Network Utility Class
    /// </summary>
    public static class NetTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 컴퓨터의 이름을 가지고 IP Adress를 찾아 반환한다.
        /// </summary>
        /// <param name="hostName">Computer Name (DNS Name)이 포함된 문자열</param>
        /// <returns>IPAddress의 문자열</returns>
        public static string GetIPAddressAsString(string hostName) {
            hostName.ShouldNotBeWhiteSpace("hostName");

            IPAddress[] addressList = GetIPAddress(hostName);

            return (addressList.Length > 0) ? addressList.ElementAt(addressList.Length - 1).ToString() : string.Empty;
        }

        /// <summary>
        /// 컴퓨터 이름을 통해 IP 주소를 검색해서 문자열 배열로 반환한다. (로컬 컴퓨터는 <see cref="Dns.GetHostName()"/>을 이용하면됩니다.)
        /// </summary>
        /// <param name="hostName">컴퓨터 명</param>
        /// <returns>컴퓨터의 Ip Address 들의 문자열, 없으면 빈 문자열을 반환한다.</returns>
        public static IList<string> GetIPAddressAsStringList(string hostName) {
            hostName.ShouldNotBeWhiteSpace("hostName");

            if(IsDebugEnabled)
                log.Debug("Get IP Address as string list. hostName=[{0}]", hostName);

            IPAddress[] ips = GetIPAddress(hostName);

            var result = ips.Select(ip => ip.ToString()).ToList();

            if(IsDebugEnabled)
                log.Debug("Host [{0}] has IP Addresses [{1}]", hostName, result.CollectionToString());

            return result;
        }

        /// <summary>
        /// hostName에 해당하는 IPAddress의 배열을 얻는다. (로컬 컴퓨터는 <see cref="Dns.GetHostName()"/>을 이용하면됩니다.)
        /// </summary>
        /// <param name="hostName"></param>
        /// <returns></returns>
        /// <remarks>
        /// <c>System.Net.Dns.GetHostEntry</c>를 직접이용해도 된다.
        /// </remarks>
        public static IPAddress[] GetIPAddress(string hostName) {
            hostName.ShouldNotBeWhiteSpace("hostName");

            if(IsDebugEnabled)
                log.Debug("Get IP Address list. hostName=[{0}]", hostName);

            return Dns.GetHostEntry(hostName).AddressList;
        }
    }
}