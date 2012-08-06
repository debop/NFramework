using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Data.Persisters;

namespace NSoft.NFramework.Data.WindsorInstallers {
    /// <summary>
    /// Ado 관련 Component 들을 등록합니다.
    /// </summary>
    public class AdoWindsorInstaller : IWindsorInstaller {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        public void Install(IWindsorContainer container, IConfigurationStore store) {
            if(log.IsInfoEnabled) {
                log.Info("INameMapper를 IoC 컨테이너에 등록합니다...");
                log.Info("IReaderPersister<> 를 IoC 컨테이너에 등록합니다...");
                log.Info("IRowPersister<> 를 IoC 컨테이너에 등록합니다...");
            }

            container.Register(
                AllTypes
                    .FromThisAssembly()
                    .BasedOn<INameMapper>()
                    .WithServiceAllInterfaces()
                    .LifestyleSingleton(),
                AllTypes
                    .FromThisAssembly()
                    .BasedOn(typeof(IReaderPersister<>))
                    .WithServiceAllInterfaces()
                    .LifestyleSingleton(),
                AllTypes
                    .FromThisAssembly()
                    .BasedOn(typeof(IRowPersister<>))
                    .WithServiceAllInterfaces()
                    .LifestyleSingleton()
                );
        }
    }
}