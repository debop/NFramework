using Castle.Windsor;
using Castle.Windsor.Installer;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.WindsorInstallers {
    [TestFixture]
    public class WindsorInstallerFixture {
        [Test]
        public void AdoRepositoryWindsorInstallerTest() {
            var container = new WindsorContainer();
            container.Install(FromAssembly.This());

            var repository = container.Resolve<IAdoRepository>();
            repository.Should().Not.Be.Null();

            repository = container.Resolve<IAdoRepository>("AdoRepository.Northwind");
            repository.Should().Not.Be.Null();
            repository.DbName.Should().Be("Northwind");
            repository.QueryProvider.Should().Not.Be.Null();
            repository.QueryProvider.QueryFilePath.Contains("Northwind.ado.mssql.ini").Should().Be.True();

            repository.QueryProvider.GetQueries().Count.Should().Be.GreaterThan(0);
        }
    }
}