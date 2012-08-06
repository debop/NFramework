using System;

namespace NSoft.NFramework.Web.UserControls
{
    /// <summary>
    /// 엔티티 편집용 UserControl의 기본 클래스입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class EntityEditUserControlBase<T> : UserControlBase, IEntityEditUserControl<T> where T : class
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsTraceEnabled = log.IsTraceEnabled;
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 편집할 엔티티 정보
        /// </summary>
        public virtual T EditEntity { get; protected set; }

        /// <summary>
        /// 엔티티에 대한 편집 모드 (Create/Read/Update/Delete)
        /// </summary>
        public EntityEditMode EditMode
        {
            get { return ViewState["EditMode"].AsEnum(EntityEditMode.Read); }
            set { ViewState["EditMode"] = value; }
        }

        public bool IsEntityUpdateMode
        {
            get { return EditMode == EntityEditMode.Update; }
        }

        public bool IsEntityCreateMode
        {
            get { return EditMode == EntityEditMode.Create; }
        }

        /// <summary>
        /// 엔티티 편집용 컨트롤을 화면에 표시합니다.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="parent"></param>
        public virtual void Show(T entity, T parent = null)
        {
            EditMode = (entity != null) ? EntityEditMode.Update : EntityEditMode.Create;
            EditEntity = entity;
            DoBindControls();

            if(IsDebugEnabled)
                log.Debug("엔티티를 추가/수정하기 위해 편집창을 열었습니다... EditMode={0}, entity={1}", EditMode, EditEntity);
        }

        /// <summary>
        /// Entity 를 저장합니다.
        /// </summary>
        /// <param name="savedAction">저장완료후 Action</param>
        public virtual void Save(Action<T> savedAction)
        {
            DoSaveOrUpdateEntity();

            if(savedAction != null)
                savedAction(EditEntity);

            OnSaved(EditEntity);
        }

        /// <summary>
        /// 엔티티 편집 완료 후 저장 시 발생하는 이벤트입니다.
        /// </summary>
        public event EventHandler Saved = delegate { };

        /// <summary>
        /// 저장 이벤트 발생
        /// </summary>
        /// <param name="entity">저장된 객체</param>
        protected virtual void OnSaved(T entity)
        {
            Saved(this, EventArgs.Empty);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if(IsPostBack == false)
                DoInitControls();
        }

        /// <summary>
        /// Control 초기화를 수행합니다. 
        /// </summary>
        protected virtual void DoInitControls()
        {
            if(IsTraceEnabled)
                log.Trace("OnInitControls()...");
        }

        /// <summary>
        /// 엔티티 정보를 UI Control에 바인딩합니다.
        /// </summary>
        protected virtual void DoBindControls()
        {
            if(IsTraceEnabled)
                log.Trace("DoBindControls()...");
        }

        /// <summary>
        /// 엔티티 정보를 새로 추가하거나 갱신합니다.
        /// </summary>
        protected virtual void DoSaveOrUpdateEntity()
        {
            if(IsTraceEnabled)
                log.Trace("DoSaveOrUpdateEntity()...");
        }
    }
}