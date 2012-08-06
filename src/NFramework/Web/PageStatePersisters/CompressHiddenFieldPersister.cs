using System;
using System.Web.UI;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Web.PageStatePersisters {
    /// <summary>
    /// Page의 상태정보를 <see cref="ICompressor"/>으로 압축하여 Client Hidden Field로 저장하여, Client에 전송하고, Postback시에는 압축 복원합니다. 
    /// 통신량을 줄여, 속도를 향상시킵니다.
    /// 
    /// 참고: http://www.codeproject.com/KB/viewstate/ViewStateTricks.aspx
    /// </summary>
    [Serializable]
    public class CompressHiddenFieldPersister : AbstractServerPageStatePersister {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public CompressHiddenFieldPersister(Page page) : base(page) {}

        protected override void LoadFromRepository() {
            if(IsDebugEnabled)
                log.Debug("저장된 Page 상태정보를 저장소에서 로드합니다.. StateValue=[{0}]", StateValue);

            if(StateValue.IsWhiteSpace())
                return;

            var stateEntity = Serializer.Deserialize(StateValue.Base64Decode()) as IPageStateEntity;

            object pageState;

            if(PageStateTool.TryParseStateEntity(stateEntity, Compressor, out pageState)) {
                if(pageState is Pair) {
                    ViewState = ((Pair)pageState).First;
                    ControlState = ((Pair)pageState).Second;

                    if(IsDebugEnabled)
                        log.Debug("압축된 Hidden Field의 값을 읽어와서 Page의 상태정보로 설정했습니다!!!");
                }
            }
        }

        protected override void SaveToRepository() {
            if(IsDebugEnabled)
                log.Debug("Page 상태정보를 압축하여 Hidden Field에 저장합니다...");

            var pair = new Pair(ViewState, ControlState);
            var stateEntity = PageStateTool.CreateStateEntity(string.Empty, pair, Compressor, CompressThreshold);

            StateValue = Serializer.Serialize(stateEntity).Base64Encode();

            if(IsDebugEnabled)
                log.Debug("Page 상태정보를 압축하여 Hidden Field에 저장했습니다!!!");
        }
    }
}