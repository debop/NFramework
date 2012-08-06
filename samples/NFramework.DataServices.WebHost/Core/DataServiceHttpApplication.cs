using System.Web;
using System.Web.Hosting;
using Castle.Windsor;
using Castle.Windsor.Installer;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Web.HttpApplications;

namespace NSoft.NFramework.DataServices.WebHost {
    public class DataServiceHttpApplication : WindsorAsyncHttpApplication {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        protected override void ApplicationStartAfter(HttpContext context) {
            base.ApplicationStartAfter(context);

            // Application 시작 시에 비동기 방식으로 수행할 작업을 추가합니다.
            //
            if(IsDebugEnabled)
                log.Debug("DataServiceHttpApplication이 시작되었습니다. Application Path=[{0}]",
                          HostingEnvironment.ApplicationVirtualPath);
        }

        protected override IWindsorContainer SetUpContainer() {
            var container = new WindsorContainer();

            container.Install(FromAssembly.This(),
                              FromAssembly.Containing<ICompressor>(),
                              FromAssembly.Containing<INameMapper>());

            return container;
        }
    }
}