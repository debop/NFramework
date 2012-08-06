using System;
using System.Linq;
using System.Web.UI;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.Serializations.Serializers;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Web.PageStatePersisters {
    /// <summary>
    /// ViewState를 다른 저장소에 저장하고, Load해서, 네트웍 속도를 증가시키도록 합니다.
    /// </summary>
    [Serializable]
    public abstract class AbstractServerPageStatePersister : PageStatePersister, IServerPageStatePersister {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 어떤 종류의 Persister인지를 Client에 알려주기 위해 Hidden Field로 알려줍니다.
        /// </summary>
        public const string PersisterKey = @"__NFramework_StatePersister";

        /// <summary>
        /// State 값이 저장된 저장소의 토큰 값을 나타내는 Hidden Field Name입니다.
        /// </summary>
        public const string StateKey = @"__NFRAMEWORK_VIEWSTATE";

        private ICompressor _compressor;

        protected AbstractServerPageStatePersister(Page page)
            : base(page) {
            CompressThreshold = 40960; // bytes
            Expiration = TimeSpan.FromDays(1);

            if(IsDebugEnabled)
                log.Debug("Page 상태정보를 관리하기 위해 [{0}] 인스턴스를 생성했습니다. CompressThreshold=[{1}], Expiration=[{2}] (minutes)",
                          GetType().FullName, CompressThreshold, Expiration);
        }

        private string _persisterName;

        /// <summary>
        /// Persister Type Name 
        /// </summary>
        public string PersisterName {
            get { return _persisterName ?? (_persisterName = GetType().FullName); }
        }

        /// <summary>
        /// ViewState 키 값
        /// </summary>
        public string StateValue { get; set; }

        /// <summary>
        /// 최소 압축 크기
        /// </summary>
        public int CompressThreshold { get; set; }

        /// <summary>
        /// ViewState 유효 기간 (분단위)
        /// </summary>
        public TimeSpan Expiration { get; set; }

        /// <summary>
        /// ViewState를 압축 저장하기 위한 Compressor. Compressor가 지정되지 않았으면, 기본적으로 <see cref="SharpGZipCompressor"/>을 사용합니다.
        /// </summary>
        public virtual ICompressor Compressor {
            get { return _compressor ?? (_compressor = SingletonTool<SharpBZip2Compressor>.Instance); }
            set { _compressor = value; }
        }

        /// <summary>
        /// Binary Serializer
        /// </summary>
        public ISerializer Serializer {
            get { return BinarySerializer.Instance; }
        }

        /// <summary>
        /// 저장된 Page의 ViewState 정보를 Load합니다. Load 후에는 저장소에서 삭제하셔야 합니다. ViewState는 매 요청시마다 다른 토큰으로 저장되기 때문입니다.
        /// </summary>
        public override void Load() {
            if(IsDebugEnabled)
                log.Debug("Page의 ViewState 정보를 저장소에서 로드합니다... ");

            try {
                StateValue = RetrieveHiddenFieldFromScriptManager();

                if(StateValue.IsWhiteSpace())
                    StateValue = RetrieveHiddenFieldFromPage();

                if(StateValue.IsWhiteSpace()) {
                    if(IsDebugEnabled)
                        log.Debug("저장 토큰 키가 없으므로, 상태정보를 로드 작업을 중단합니다!!!");

                    return;
                }

                // ViewState 정보를 저장소에서 로드하여 Page의 ViewState에 설정합니다.
                LoadFromRepository();
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("ViewState를 로드하는데 실패했습니다!!! StateValue=[{0}]", StateValue);
                    log.Error(ex);
                }
                throw;
            }
        }

        /// <summary>
        /// Page의 ViewState 정보를 원하는 장소(Cache, 파일, DB 등)에 저장합니다.
        /// </summary>
        public override void Save() {
            if(IsDebugEnabled)
                log.Debug("Page의 ViewState 정보를 저장소에 저장합니다...");

            try {
                SaveToRepository();

                if(IsDebugEnabled)
                    log.Debug("상태 정보를 나중에 로드하기 위한 토큰 키 값을 Hidden Field로 등록합니다... StateValue=[{0}]", StateValue);

                // StateValue 에 값이 없다는 뜻은 저장소에 저장할 상태값이 없어 저장하지 않았으므로, 토큰으로 받은 값이 없다는 뜻이다.
                //
                if(StateValue.IsNotWhiteSpace()) {
                    var registered = RegisterHiddenFieldToScriptManager();

                    if(registered == false)
                        registered = RegisterHiddenFieldToPage();

                    Guard.Assert(registered, "ViewState 정보의 토큰 키를 Hidden Field로 등록하는데 실패했습니다.");
                }
                else {
                    if(IsDebugEnabled)
                        log.Debug("상태 토큰 키가 없으므로, 저장하지 못했습니다.");
                }
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("ViewState를 저장소에 저장하는데 실패했습니다.");
                    log.Error(ex);
                }

                throw;
            }
        }

        /// <summary>
        /// ViewState 저장소로부터 저장된 ViewState 정보를 가져옵니다.
        /// </summary>
        protected abstract void LoadFromRepository();

        /// <summary>
        /// Page의 ViewState 정보를 특정 저장소에 저장하고, 저장 토큰 값을 <see cref="StateValue"/>에 저장합니다.
        /// </summary>
        protected abstract void SaveToRepository();

        /// <summary>
        /// Persister만을 위한 캐시 키를 빌드합니다. (캐시 시스템에서 다른 용도의 캐시 정보와 구분하기 위해 PersisterName을 Prefix로 사용합니다)
        /// </summary>
        /// <returns></returns>
        protected virtual string GetCacheKey() {
            return string.Concat(PersisterName, "@", StateValue);
        }

        /// <summary>
        /// 새로운 StateValue를 생성합니다.
        /// </summary>
        /// <returns></returns>
        protected virtual string CreateStateValue() {
            return Guid.NewGuid().ToString();
        }

        private string RetrieveHiddenFieldFromScriptManager() {
            if(IsDebugEnabled)
                log.Debug("ASP.NET Ajax ScriptManager에서 [{0}]에 해당하는 Hidden Field 값을 조회합니다...", StateKey);

            try {
                var hiddenField =
                    ScriptManager.GetCurrent(Page).GetRegisteredHiddenFields().FirstOrDefault(field => field.Name == StateKey);

                if(hiddenField != null) {
                    if(IsDebugEnabled)
                        log.Debug("ASP.NET Ajax ScriptManager에서 [{0}]에 해당하는 Hidden Field 값 [{1}]을 얻었습니다.", StateKey,
                                  hiddenField.InitialValue);

                    return hiddenField.InitialValue;
                }
                if(IsDebugEnabled)
                    log.Debug("ASP.NET Ajax ScriptManager에서 [{0}]에 해당하는 Hidden Field 값이 없습니다.", StateKey);

                return string.Empty;
            }
            catch {
                return string.Empty;
            }
        }

        private string RetrieveHiddenFieldFromPage() {
            if(IsDebugEnabled)
                log.Debug("Page.Request에서 [{0}] 에 해당하는 Hidden Field 값을 로드합니다...", StateKey);

            try {
                var stateValue = Page.Request[StateKey].AsText();

                if(IsDebugEnabled)
                    log.Debug("Page.Request에서 [{0}] 에 해당하는 Hidden Field 값 [{1}]을 얻었습니다.", StateKey, stateValue);

                return stateValue;
            }
            catch {
                if(IsDebugEnabled)
                    log.Debug("Page.Request에서 [{0}] 에 해당하는 Hidden Field 값이 없습니다.", StateKey);

                return string.Empty;
            }
        }

        private bool RegisterHiddenFieldToScriptManager() {
            if(IsDebugEnabled)
                log.Debug("ViewState HiddenField를 ASP.NET 기본 Ajax ScriptManager에 등록합니다...");

            try {
                ScriptManager.RegisterHiddenField(Page, PersisterKey, PersisterName);
                ScriptManager.RegisterHiddenField(Page, StateKey, StateValue);

                if(IsDebugEnabled)
                    log.Debug("ViewState HiddenField를 ASP.NET 기본 Ajax ScriptManager에 등록했습니다.");

                return true;
            }
            catch {
                return false;
            }
        }

        private bool RegisterHiddenFieldToPage() {
            if(IsDebugEnabled)
                log.Debug("ViewState HiddenField를 Page의 ClientScript에 등록합니다...");

            try {
                Page.ClientScript.RegisterHiddenField(PersisterKey, PersisterName);
                Page.ClientScript.RegisterHiddenField(StateKey, StateValue);

                if(IsDebugEnabled)
                    log.Debug("ViewState HiddenField를 Page의 ClientScript에 등록했습니다.");

                return true;
            }
            catch {
                return false;
            }
        }
    }
}