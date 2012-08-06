using Castle.Windsor;
using NSoft.NFramework.InversionOfControl;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.NHibernateEx.UnitOfWorks {
    [TestFixture]
    public class FluentNHUnitOfWorkFactoryFixture {
        [Test]
        public void CreateTest() {
            var factory = new FluentNHUnitOfWorkFactory(new string[] { "NSoft.NFramework.Data.NHibernateEx.Tests" });
            factory.Should().Not.Be.Null();
            factory.MappingAssemblies.Count.Should().Be.GreaterThan(0);
        }

        [Test]
        public void CreateByIoC() {
            IoC.Initialize(new WindsorContainer(@".\UnitOfWorks\IoC.FluentUnitOfWork.config"));

            var factory = IoC.Resolve<IUnitOfWorkFactory>() as FluentNHUnitOfWorkFactory;
            factory.Should().Not.Be.Null();
            factory.MappingAssemblies.Count.Should().Be.GreaterThan(0);

            factory.Convention.Should().Not.Be.Null();
        }
    }
}