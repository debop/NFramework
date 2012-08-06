using System.Globalization;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx.UserTypes {
    [TestFixture]
    public class NetCultureInfoFixture : NHRepositoryTestFixtureBase {
        protected override void OnTestFixtureSetUp() {
            base.OnTestFixtureSetUp();

            foreach(CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
                if(culture.Name.IsNotWhiteSpace())
                    Repository<Locale>.SaveOrUpdate(new Locale(culture));

            UnitOfWork.Current.TransactionalFlush();
        }

        protected override void OnTestFixtureTearDown() {
            Repository<Locale>.DeleteAll();

            base.OnTestFixtureTearDown();
        }

        [Test]
        public void Load() {
            var locales = Repository<Locale>.FindAll();

            Assert.IsNotNull(locales);
            Assert.Greater(locales.Count, 0);

            foreach(Locale locale in locales) {
                if(locale.Id != CultureInfo.InvariantCulture)
                    Assert.IsNotEmpty(locale.Name, "locale.Name is empty. locale=" + locale);
            }
        }

        [Test]
        public void Load_Criterion() {
            var locale = Repository<Locale>.Get(CultureInfo.CurrentCulture);
            Assert.IsNotNull(locale);
            Assert.AreEqual(CultureInfo.CurrentCulture, locale.Id);
        }
    }

    [TestFixture]
    public class NetCultureInfoFixture_SQLServer : NetCultureInfoFixture {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.MsSql2005;
        }
    }

    [TestFixture]
    public class NetCultureInfoFixture_Oracle : NetCultureInfoFixture {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.DevartOracle;
        }
    }

    [TestFixture]
    public class NetCultureInfoFixture_PostgreSql : NetCultureInfoFixture {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.PostgreSql;
        }
    }
}