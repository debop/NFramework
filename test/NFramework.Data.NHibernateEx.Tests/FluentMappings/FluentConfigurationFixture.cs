using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.NHibernateEx.FluentMappings {
    [TestFixture]
    public class FluentConfigurationFixture {
        [Test]
        public void FluentlyConfigurationByCode() {
            var configuration =
                Fluently.Configure()
                    .Database(SQLiteConfiguration.Standard.InMemory())
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<FluentProductMap>())
                    .BuildConfiguration();

            configuration.Should().Not.Be.Null();

            var sessionFactory = configuration.BuildSessionFactory();
            sessionFactory.Should().Not.Be.Null();
        }

        [Test]
        public void FluentlyConfigurationFromFile() {
            var configuration = new NHibernate.Cfg.Configuration();
            configuration.Configure("hibernate.fluent.cfg.xml");

            var cfg =
                Fluently.Configure(configuration)
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<FluentProductMap>())
                    .BuildConfiguration();

            cfg.Should().Not.Be.Null();

            var sessionFactory = cfg.BuildSessionFactory();
            sessionFactory.Should().Not.Be.Null();
        }
    }
}