using System.Web;
using NSoft.NFramework.Data.NHibernateEx;

namespace NSoft.NFramework.StringResources.WebHost {
    public class StringResourcesHttpApplication : UnitOfWorkHttpApplication {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Application_Start 시에 실행할 비동기 작업의 본체입니다.
        /// </summary>
        protected override void ApplicationStartAfter(HttpContext context) {
            base.ApplicationStartAfter(context);

            if(IsDebugEnabled)
                log.Debug("StringResource HttpApplication의 시작시에 수행할 FutureTask 작업을 시작합니다...");

            if(UnitOfWork.IsNotStarted)
                UnitOfWork.Start();

            // 리소스를 미리 읽어놓는다^^ - Prefetching
            try {
                var hello = NSoft.NFramework.StringResources.WebResourceExpressionBuilder.GetGlobalResourceObject("CommonTerms", "Hello");

                if(IsDebugEnabled)
                    log.Debug(@"Prefetch한 'Hello' 의 값은 [{0}] 입니다.", hello);
            }
            catch {
                // Nothing to do.
            }
        }
    }
}