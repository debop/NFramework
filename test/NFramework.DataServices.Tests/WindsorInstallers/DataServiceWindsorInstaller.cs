using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.DataServices.Adapters;

namespace NSoft.NFramework.DataServices.WebHost.WindsorInstallers {
    public class DataServiceWindsorInstaller : IWindsorInstaller {
        public void Install(IWindsorContainer container, IConfigurationStore store) {
            container.Register(
                Component
                    .For<IDataService>()
                    .ImplementedBy<AsyncDataServiceImpl>()
                    .Named("DataService.Northwind")
                    .DependsOn(Dependency.OnComponent("AdoRepository", "AdoRepository.Northwind"),
                               Dependency.OnComponent("NameMapper", typeof(TrimNameMapper))),
                Component
                    .For<IDataService>()
                    .ImplementedBy<AsyncDataServiceImpl>()
                    .Named("DataService.Pubs")
                    .DependsOn(Dependency.OnComponent("AdoRepository", "AdoRepository.Pubs"),
                               Dependency.OnComponent("NameMapper", typeof(TrimNameMapper))),
                Component
                    .For<IDataServiceAdapter>()
                    .ImplementedBy<DataServiceAdapter>()
                    .Named("DataServiceAdapter.Northwind")
                    .DependsOn(Dependency.OnComponent("DataService", "DataService.Northwind"))
                    .DependsOn(Dependency.OnComponent("RequestSerializer", "MessageSerializer.Northwind"))
                    .DependsOn(Dependency.OnComponent("ResponseSerializer", "MessageSerializer.Northwind")),
                Component
                    .For<IDataServiceAdapter>()
                    .ImplementedBy<DataServiceAdapter>()
                    .Named("DataServiceAdapter.Pubs")
                    .DependsOn(Dependency.OnComponent("DataService", "DataService.Pubs"))
                    .DependsOn(Dependency.OnComponent("RequestSerializer", "MessageSerializer.Pubs"))
                    .DependsOn(Dependency.OnComponent("ResponseSerializer", "MessageSerializer.Pubs"))
                );
        }
    }
}