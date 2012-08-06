using NSoft.NFramework.Data.NHibernateEx.Domain;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx.UserTypes {
    [TestFixture]
    public class CompanyFixture : NHRepositoryTestFixtureBase {
        protected override void OnTestFixtureSetUp() {
            base.OnTestFixtureSetUp();

            UnitOfWork.CurrentSession.SaveOrUpdate(CreateCompany("리얼웹", "RW"));
            UnitOfWork.CurrentSession.SaveOrUpdate(CreateCompany("마이크로소트프", "Microsoft"));

            UnitOfWork.Current.TransactionalFlush();
        }

        [Test]
        public void LoadCompany() {
            var companies = Repository<Company>.FindAll();
            Assert.IsTrue(companies.Count > 0);
        }

        [Test]
        public void ConvertToTest() {
            var company = Repository<Company>.FindFirst();
            Assert.IsNotNull(company);

            var copied = company.MapDataObject<Company>();

            Assert.AreNotEqual(company.Id, copied.Id);
            Assert.AreNotEqual(company.IsSaved, copied.IsSaved);
            Assert.AreNotEqual(company.IsTransient, copied.IsTransient);

            Assert.AreEqual(company.Name, copied.Name);
            Assert.AreEqual(company.Code, copied.Code);
        }

        private static Company CreateCompany(string name, string code) {
            return new Company { Name = name, Code = code };
        }
    }

    [TestFixture]
    public class CompanyFixture_SQLServer : CompanyFixture {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.MsSql2005;
        }
    }

    [TestFixture]
    public class CompanyFixture_Oracle : CompanyFixture {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.DevartOracle;
        }
    }
}