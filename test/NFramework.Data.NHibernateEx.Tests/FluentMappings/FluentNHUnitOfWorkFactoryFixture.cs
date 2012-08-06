using FluentNHibernate.Cfg;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.NHibernateEx.FluentMappings {
    [TestFixture]
    public class FluentNHUnitOfWorkFactoryFixture {
        [Test]
        public void ConfigurationFromDefaultFile() {
            //! HBM.xml 을 FluentNHibnerate 의 FluentMappings 와 HbmMappings 양쪽에서 받아서 그렇게 됨.
            //! Fluent 사용 시에는 꼭 Fluent 에서 제공하지 않는 것만 HBM으로 줘야 함.
            Assert.Throws<FluentConfigurationException>(() => {
                                                            var fluentNHUnitOfWorkFactory = new FluentNHUnitOfWorkFactory();

                                                            fluentNHUnitOfWorkFactory.Configuration.Should().Not.Be.Null();
                                                            fluentNHUnitOfWorkFactory.SessionFactory.Should().Not.Be.Null();
                                                        });
        }

        [Test]
        public void ConfigurationFromFile() {
            var fluentNHUnitOfWorkFactory = new FluentNHUnitOfWorkFactory("hibernate.fluent.cfg.xml");

            fluentNHUnitOfWorkFactory.Configuration.Should().Not.Be.Null();
            fluentNHUnitOfWorkFactory.SessionFactory.Should().Not.Be.Null();
        }

        [Test]
        public void ConfigurationFromFileWithAssembly() {
            var fluentNHUnitOfWorkFactory =
                new FluentNHUnitOfWorkFactory(new[] { typeof(FluentProductMap).Assembly },
                                              "hibernate.fluent.cfg.xml");

            fluentNHUnitOfWorkFactory.Configuration.Should().Not.Be.Null();
            fluentNHUnitOfWorkFactory.SessionFactory.Should().Not.Be.Null();
        }
    }
}