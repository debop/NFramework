using NSoft.NFramework.Web.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Web {
    [TestFixture]
    public class MimetypeToolFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        [Test]
        public void GetMimeTypeTest() {
            Assert.AreEqual("text/html", "string.html".GetMime());
            Assert.AreEqual("text/xml", "data.xml".GetMime());
            Assert.AreEqual("image/jpeg", "image.jpg".GetMime());
            Assert.AreEqual("application/x-shockwave-flash", "line.swf".GetMime());

            Assert.AreEqual("application/octet-stream", "file.abcdefg".GetMime());
        }
    }
}