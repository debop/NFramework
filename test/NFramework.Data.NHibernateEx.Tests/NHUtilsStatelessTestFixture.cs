using NHibernate;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx {
    // TODO : NHUtils 관련 테스트 코드 추가 해야 함!!!

    /// <summary>
    /// Stateless 작업은 SQLite Memory DB에서는 예외가 발생한다. 꼭 실제 물리적 DB가 있는 종류에서 테스트해야 한다.
    /// </summary>
    [TestFixture]
    public class NHUtilsStatelessTestFixture : NHRepositoryTestFixtureBase {
        // NOTE: Stateless 작업은 실제 DB 파일이 있어야 합니다. 
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.MsSql2005;
        }

        [Test]
        public void FindAll_With_StatelessSession() {
            var results = NHTool.FindAllStateless<Parent>();
            Assert.IsNotNull(results);
            Assert.IsTrue(results.Count > 0);
        }

        [Test]
        public void Exists_With_StatelessSession() {
            Assert.IsTrue(WhereNameEquals(parentsInDB[0].Name).ExistsStateless<Parent>());
            Assert.IsTrue(WhereNameEquals(parentsInDB[1].Name).ExistsStateless<Parent>());
            Assert.IsTrue(WhereNameEquals(parentsInDB[2].Name).ExistsStateless<Parent>());

            Assert.IsFalse(WhereNameEquals("NotExists").ExistsStateless<Parent>());
        }

        [Test]
        public void GetStateless_By_Id() {
            var id = parentsInDB[0].Id;

            var parentFromSession = Repository<Parent>.Get(id);
            var parentFromStateless = NHTool.GetStateless<Parent>(id, LockMode.None);

            Assert.IsNotNull(parentFromStateless);
            Assert.AreEqual(id, parentFromStateless.Id);

            Assert.AreEqual(parentFromSession, parentFromStateless);
        }
    }
}