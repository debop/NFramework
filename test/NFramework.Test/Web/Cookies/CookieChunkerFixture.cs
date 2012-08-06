using NUnit.Framework;

namespace NSoft.NFramework.Web.Cookies {
    [TestFixture]
    public class CookieChunkerFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly UserInfo UserSample = UserInfo.GetSample();

        [Test]
        public void ChunkCookie() {
            var cookies = CookieChunker<UserInfo>.Instance.Chunk("user", UserSample);

            Assert.Greater(cookies.Count, 0);

            if(IsDebugEnabled)
                foreach(string cookieName in cookies.AllKeys)
                    log.Debug("[Cookie] name={0}, value={1}", cookieName, cookies[cookieName].Value);

            var cookieUser = CookieChunker<UserInfo>.Instance.Unchunk(cookies, "user");

            Assert.IsNotNull(cookieUser);

            Assert.AreEqual(UserSample.Address, cookieUser.Address);
            Assert.AreEqual(UserSample.FavoriteMovies.Count, cookieUser.FavoriteMovies.Count);
        }
    }
}