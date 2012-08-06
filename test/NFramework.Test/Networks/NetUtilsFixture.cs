using System;
using System.Net;
using NUnit.Framework;

namespace NSoft.NFramework.Networks {
    [TestFixture]
    public class NetUtilsFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        [Test]
        public void GetIpAddress() {
            // return Id (Administrator)
            Console.WriteLine("UserName=" + Environment.UserName);

            // return ComputerName (debop-lenovo)
            Console.WriteLine("UserDomainName=" + Environment.UserDomainName);

            var ipAddress = NetTool.GetIPAddressAsString(Dns.GetHostName());
            Console.WriteLine("IPAddress=" + ipAddress);

            var hostName = Dns.GetHostName();
            Console.WriteLine("HostName=" + hostName);
            foreach(var ip in Dns.GetHostAddresses(hostName))
                Console.WriteLine("IPAddress=" + ip);
        }
    }
}