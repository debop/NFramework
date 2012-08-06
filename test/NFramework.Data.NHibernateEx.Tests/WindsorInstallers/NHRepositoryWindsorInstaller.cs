using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NSoft.NFramework.Data.NHibernateEx;

namespace NSoft.NFramework.Data.WindsorInstallers {
    /// <summary>
    /// <see cref="NHRepository{T}"/>를 Windsor Container에 등록하는 Installer 입니다.
    /// </summary>
    public class NHRepositoryWindsorInstaller : IWindsorInstaller {
        public void Install(IWindsorContainer container, IConfigurationStore store) {
            container.Register(
                Component
                    .For(typeof(INHRepository<>))
                    .ImplementedBy(typeof(NHRepository<>)),
                AllTypes
                    .FromAssemblyContaining<IUnitOfWorkFactory>()
                    .BasedOn<IUnitOfWorkFactory>()
                    .WithServiceAllInterfaces()
                //.WithService.DefaultInterface());
                );
        }
    }
}