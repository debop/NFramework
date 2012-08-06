using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx.WithTests {
    [TestFixture]
    public class WithCachingFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        [Test]
        public void CanEnterCachingMode() {
            // 기본적으로 Caching은 False이다.
            Assert.IsFalse(NHWith.Caching.Enabled);

            // Query Caching을 가능한 영역
            using(NHWith.QueryCache()) {
                Assert.IsTrue(NHWith.Caching.Enabled);
            }
            Assert.IsFalse(NHWith.Caching.Enabled);
        }

        [Test]
        public void CanEnterCachingModeRecursive() {
            // 기본적으로 Caching은 False이다.
            Assert.IsFalse(NHWith.Caching.Enabled);

            // Query Caching을 가능한 영역
            using(NHWith.QueryCache()) {
                Assert.IsTrue(NHWith.Caching.Enabled);

                using(NHWith.QueryCache()) {
                    Assert.IsTrue(NHWith.Caching.Enabled);

                    using(NHWith.QueryCache()) {
                        Assert.IsTrue(NHWith.Caching.Enabled);
                    }
                    Assert.IsTrue(NHWith.Caching.Enabled);
                }
                Assert.IsTrue(NHWith.Caching.Enabled);
            }
            Assert.IsFalse(NHWith.Caching.Enabled);
        }
    }
}