using System.Threading;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.Extensions.APM {
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class DnsAsyncTestCase : ParallelismFixtureBase {
        public const int ThreadCount = 5;

        private static readonly string[] HostNames = new string[]
                                                     {
                                                         "www.realweb21.com",
                                                         "www.hani.co.kr",
                                                         "www.naver.com",
                                                         "www.daum.net",
                                                         "www.microsoft.com"
                                                     };

        [Test]
        public void Can_GetHostAddressesAsync() {
            TestTool
                .RunTasks(ThreadCount,
                          () => {
                              foreach(var hostName in HostNames) {
                                  using(var task = DnsAsync.GetHostAddresses(hostName)) {
                                      // 비동기 호출 후, 다른 작업을 하는 부분을 나타낸다.
                                      Thread.Sleep(10);

                                      var ipAddrs = task.Result;

                                      Assert.IsNotNull(ipAddrs);
                                      CollectionAssert.AllItemsAreNotNull(ipAddrs);

                                      if(IsDebugEnabled)
                                          foreach(var ipAddr in ipAddrs)
                                              log.Debug("HostName={0}, IPAddress={1}", hostName, ipAddr);
                                  }
                              }
                          });
        }

        [Test]
        public void Can_GetHostEntryAsync() {
            TestTool
                .RunTasks(ThreadCount,
                          () => {
                              foreach(var hostName in HostNames) {
                                  using(var task = DnsAsync.GetHostEntry(hostName)) {
                                      // 비동기 호출 후, 다른 작업을 하는 부분을 나타낸다.
                                      Thread.Sleep(10);

                                      var hostEntry = task.Result;
                                      Assert.IsNotNull(hostEntry);

                                      if(IsDebugEnabled) {
                                          hostEntry.AddressList.RunEach(
                                              addr => log.Debug("HostName={0}, IPAddress={1}", hostEntry.HostName, addr));
                                          hostEntry.Aliases.RunEach(
                                              alias => log.Debug("HostName={0}, Alias={1}", hostEntry.HostName, alias));
                                      }
                                  }
                              }
                          });
        }
    }
}