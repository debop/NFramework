using Castle.Windsor;
using Castle.Windsor.Installer;
using NSoft.NFramework.Data.NHibernateEx.Domain.Mappings;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.NHibernateEx.WindsorInstallers {
    [TestFixture]
    public class WindsorInstallerFixture {
        [Test]
        public void NHRepositoryWindsorInstallerTest() {
            var container = new WindsorContainer();
            container.Install(FromAssembly.This());

            var repository = container.Resolve<INHRepository<User>>();
            repository.Should().Not.Be.Null();
        }
    }
}