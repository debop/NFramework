using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NSoft.NFramework.Data.QueryProviders;

namespace NSoft.NFramework.Data.WindsorInstallers {
    public class AdoRepositoryWindsorInstaller : IWindsorInstaller {
        public void Install(IWindsorContainer container, IConfigurationStore store) {
            container.Register(
                Component
                    .For<IIniQueryProvider>()
                    .ImplementedBy<IniAdoQueryProvider>()
                    .Named("IniQueryStringProvider.Northwind")
                    .DependsOn(Dependency.OnAppSettingsValue("queryFilePath", "QueryStringFile.Northwind")),
                //.Parameters(Parameter
                //                .ForKey("queryFilePath")
                //                .Eq(ConfigTool.GetAppSettings("QueryStringFile.Northwind", "QueryFiles/Northwind.ado.mssql.ini"))),

                Component
                    .For<IAdoRepository>()
                    .ImplementedBy<AdoRepositoryImpl>()
                    .Named("AdoRepository.Northwind")
                    .DependsOn(Dependency.OnValue("dbName", "Northwind"),
                               Dependency.OnComponent("queryStringProvider", "IniQueryStringProvider.Northwind"))

                //.Parameters(Parameter
                //                .ForKey("dbName")
                //                .Eq("Northwind"))
                //.ServiceOverrides(ServiceOverride
                //                    .ForKey("queryStringProvider")
                //                    .Eq("IniQueryStringProvider.Northwind"))
                );
        }
    }
}