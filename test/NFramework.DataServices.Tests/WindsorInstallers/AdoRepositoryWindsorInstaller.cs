using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NSoft.NFramework.Data;
using NSoft.NFramework.Data.QueryProviders;
using NSoft.NFramework.Data.SqlServer;

namespace NSoft.NFramework.DataServices.WebHost.WindsorInstallers {
    public class AdoRepositoryWindsorInstaller : IWindsorInstaller {
        public void Install(IWindsorContainer container, IConfigurationStore store) {
            container.Register(
                Component
                    .For<IAdoRepository>()
                    .ImplementedBy<SqlRepositoryImpl>()
                    .Named("AdoRepository.Northwind")
                    .DependsOn(Dependency.OnValue("dbName", "Northwind"))
                    .DependsOn(Dependency.OnComponent("queryStringProvider", "QueryProvider.Northwind")),
                Component
                    .For<IAdoRepository>()
                    .ImplementedBy<SqlRepositoryImpl>()
                    .Named("AdoRepository.Pubs")
                    .DependsOn(Dependency.OnValue("dbName", "Pubs"))
                    .DependsOn(Dependency.OnComponent("queryStringProvider", "QueryProvider.Pubs")),
                Component
                    .For<IIniQueryProvider>()
                    .ImplementedBy<IniAdoQueryProvider>()
                    .Named("QueryProvider.Northwind")
                    .DependsOn(Dependency.OnValue("queryFilePath", @"QueryFiles\Northwind.ado.mssql.ini")),
                Component
                    .For<IIniQueryProvider>()
                    .ImplementedBy<IniAdoQueryProvider>()
                    .Named("QueryProvider.Pubs")
                    .DependsOn(Dependency.OnValue("queryFilePath", @"QueryFiles\Pubs.ado.mssql.ini"))
                );
        }
    }
}