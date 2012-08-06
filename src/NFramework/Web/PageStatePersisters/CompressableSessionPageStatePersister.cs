using System;
using System.Web.SessionState;
using System.Web.UI;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Web.PageStatePersisters {
    /// <summary>
    /// 웹 페이지의 상태정보(ViewState)정보를 Session에 압축 저장하고, 필요 시 로드합니다. 
    /// 
    /// 참고: http://www.eggheadcafe.com/tutorials/aspnet/b48875f6-525b-4267-a3a8-64dd33bfc2fe/keep-viewstate-out-of-pag.aspx
    /// 참고: http://www.codeproject.com/KB/aspnet/ViewStateTricks.aspx
    /// </summary>
    [Serializable]
    public class CompressableSessionPageStatePersister : AbstractServerPageStatePersister {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public CompressableSessionPageStatePersister(Page page) : base(page) {}

        /// <summary>
        /// ViewState 저장소로부터 저장된 ViewState 정보를 가져옵니다.
        /// </summary>
        protected override void LoadFromRepository() {
            if(IsDebugEnabled)
                log.Debug("세션에 저장된 상태정보를 Repository에서 로드합니다... StateValue=[{0}]", StateValue);

            ThrowIfSessionStateModeIsOff(Page.Session.Mode);

            // 저장 조회용 키값이 없으므로, 저장하지 않았다는 뜻
            if(StateValue.IsWhiteSpace())
                return;

            var cacheKey = GetCacheKey();
            var stateEntity = Page.Session[cacheKey] as IPageStateEntity;

            if(stateEntity == null)
                return;

            object pageState;

            if(PageStateTool.TryParseStateEntity(stateEntity, Compressor, out pageState)) {
                if(pageState != null && pageState is Pair) {
                    ViewState = ((Pair)pageState).First;
                    ControlState = ((Pair)pageState).Second;

                    if(IsDebugEnabled)
                        log.Debug("세션에 저장된 상태정보를 로드하여, Page의 ViewState, ControlState에 설정했습니다!!! StateValue=[{0}]", StateValue);
                }
            }
            else {
                if(IsDebugEnabled)
                    log.Debug("세션에 해당하는 키의 상태정보가 저장되어 있지 않습니다!!!, StaetKeyValue=[{0}]", StateValue);
            }
        }

        /// <summary>
        /// Page의 ViewState 정보를 특정 저장소에 저장하고, 저장 토큰 값을 <see cref="AbstractServerPageStatePersister.StateValue"/>에 저장합니다.
        /// </summary>
        protected override void SaveToRepository() {
            if(IsDebugEnabled)
                log.Debug("상태정보를 Session에 저장하려고 합니다...");

            ThrowIfSessionStateModeIsOff(Page.Session.Mode);

            if(StateValue.IsWhiteSpace())
                StateValue = CreateStateValue();

            var pair = new Pair(ViewState, ControlState);
            var stateEntity = PageStateTool.CreateStateEntity(StateValue, pair, Compressor, CompressThreshold);

            Page.Session[GetCacheKey()] = stateEntity;

            if(IsDebugEnabled)
                log.Debug("세션에 상태정보를 저장했습니다!!! StateValue=[{0}]", StateValue);
        }

        /// <summary>
        /// Session을 사용하지 않도록 설정했다면 예외를 발생시킨다.
        /// </summary>
        /// <param name="mode"></param>
        private static void ThrowIfSessionStateModeIsOff(SessionStateMode mode) {
            Guard.Assert(mode != SessionStateMode.Off, "프로그램 또는 Page가 Session을 사용하지 않기 때문에 ViewState정보를 Session에 저장하지 못했습니다.");
        }
    }
}