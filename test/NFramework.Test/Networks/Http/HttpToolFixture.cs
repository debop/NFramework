using NSoft.NFramework.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Networks {
    [TestFixture]
    public class HttpToolFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly string[] TargetUrls = new string[]
                                                      {
                                                          @"http://debop.egloos.com",
                                                          @"http://www.hani.co.kr",
                                                          @"http://cafe.daum.net"
                                                      };

        [Test]
        public void GetString() {
            foreach(var targetUrl in TargetUrls) {
                var content = HttpTool.GetString(targetUrl);

                Assert.IsNotNull(content);
                Assert.IsNotEmpty(content);

                if(IsDebugEnabled)
                    log.Debug(content.EllipsisChar(1024));
            }
        }

        [Test]
        public void PostString() {
            foreach(var targetUrl in TargetUrls) {
                string content = HttpTool.PostString(targetUrl, string.Empty);

                Assert.IsNotNull(content);
                Assert.IsNotEmpty(content);

                if(IsDebugEnabled)
                    log.Debug(content.EllipsisChar(1024));
            }
        }
    }
}