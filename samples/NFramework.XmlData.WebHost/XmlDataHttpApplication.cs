using System.Threading.Tasks;
using NSoft.NFramework.Data;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Web.HttpApplications;

namespace NSoft.NFramework.XmlData.WebHost {
    public class XmlDataHttpApplication : WindsorAsyncHttpApplication {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        protected override void ApplicationStartAfter(System.Web.HttpContext context) {
            base.ApplicationStartAfter(context);

            Task.Factory.StartNew(() => IoC.Resolve<IAdoRepository>());
        }
    }
}