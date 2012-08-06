using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.Adapters;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Web.PageStatePersisters;

namespace NSoft.NFramework.Web.PageAdapters {
    /// <summary>
    /// Castle.Windsor IoC를 이용하여, PageStatePersister를 인스턴싱합니다. IoC 환경설정에서 Persister마다 다양한 설정을 수행할 수 있으므로, 유연성에 좋습니다.
    /// 단 꼭 component의 lifestyle을 transient로 해야 합니다.
    /// 
    /// 참고: http://www.eggheadcafe.com/tutorials/aspnet/b48875f6-525b-4267-a3a8-64dd33bfc2fe/keep-viewstate-out-of-pag.aspx
    /// 참고: http://www.codeproject.com/KB/viewstate/ViewStateTricks.aspx
    /// </summary>
    [Serializable]
    public class IoCPageStateAdapter : PageAdapter {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 컨트롤 상태와 뷰 상태를 유지하기 위해 웹 페이지에서 사용하는 개체를 반환합니다.
        /// </summary>
        /// <returns>
        /// <see cref="T:System.Web.UI.Page"/>의 결합된 컨트롤 상태와 뷰 상태를 만들고 추출하는 것을 지원하는 <see cref="T:System.Web.UI.PageStatePersister"/>에서 파생된 개체입니다.
        /// </returns>
        public override PageStatePersister GetStatePersister() {
            return ResolvePageStatePersister(Page);
        }

        internal static PageStatePersister ResolvePageStatePersister(Page page) {
            page.ShouldNotBeNull("page");

            if(IsDebugEnabled)
                log.Debug(
                    "Castle.Windsor의 Container로부터 PageStatePersister를 Resolve합니다. 단 PageStatePersister의 Lifestyle은 항상 Trasient 이어야 합니다!!!");

            IServerPageStatePersister persister;

            try {
                var arguments = new Hashtable { { "page", page } };
                persister = IoC.Resolve<IServerPageStatePersister>(arguments);

                persister.ShouldNotBeNull("persister");

                if(IsDebugEnabled)
                    log.Debug("Castle.Windsor Container로부터 PageStatePersister를 Resolve 했습니다. PageStatePersister=[{0}]",
                              persister.GetType().FullName);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("PageStatePersister를 Resolve하는데 실패했습니다. 환경설정이 제대로 되었는지 확인하시기 바랍니다. CachePageStatePersister를 기본으로 제공합니다.");
                    log.Warn(ex);
                }

                persister = new SysCachePageStatePersister(page);
            }

            return (PageStatePersister)persister;
        }
    }
}