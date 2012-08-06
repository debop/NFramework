using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web;
using NSoft.NFramework.Web.Pages;
using NSoft.NFramework.Web.Tools;
using NSoft.NFramework.Web.UserControls;
using RealWeb.Portal.Controls;
using Telerik.Web.UI;

namespace RCL.Web.Pages
{
    /// <summary>
    /// 엔티티 목록을 Grid로 표현할 때 기본이 되는 Page입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EntityGridPageBase<T> : DefaultPageBase, IEntityGridPage<T> where T : class
    {
        #region << logger  >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        protected static readonly GridItemType[] GridItemTypeForEditing
            = new[]
              {
                  GridItemType.Item,
                  GridItemType.AlternatingItem,
                  GridItemType.SelectedItem
              };

        /// <summary>
        /// Grid의 실제 레코드를 나타내는 Item인지 판단한다.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected static bool CanEditableGridItem(GridItem item)
        {
            return GridItemTypeForEditing.Contains(item.ItemType);
        }

        private static readonly Type _concreteType = typeof(T);

        /// <summary>
        /// 엔티티의 형식
        /// </summary>
        public static Type ConcreteType
        {
            get { return _concreteType; }
        }

        /// <summary>
        /// Entity Grid의 정렬 항목
        /// </summary>
        public virtual string GridSortExpr
        {
            get { return ViewState["GridSortExpr"].AsText(); }
            set { ViewState["GridSortExpr"] = value.AsText(); }
        }

        /// <summary>
        /// Entity Grid의 정렬 방식 (Ascending|Descending)
        /// </summary>
        public virtual bool GridSortAsc
        {
            get { return ViewState["GridSortAsc"].AsBool(true); }
            set { ViewState["GridSortAsc"] = value; }
        }

        /// <summary>
        /// Entity Grid의 Page Index (0부터 시작)
        /// </summary>
        public virtual int PageIndex
        {
            get { return ViewState["PageIndex"].AsInt(0); }
            set { ViewState["PageIndex"] = Math.Max(0, value); }
        }

        /// <summary>
        /// Entity Grid의 Page Size (한 페이지당 엔티티 갯수, 기본은 <see cref="AppSettings.DefaultPageSize"/>)
        /// </summary>
        public virtual int PageSize
        {
            get { return ViewState["PageSize"].AsInt(AppSettings.DefaultPageSize); }
            set { ViewState["PageSize"] = Math.Max(3, value); }
        }

        /// <summary>
        /// 검색 항목
        /// </summary>
        public virtual string SearchKey
        {
            get { return ViewState["SearchKey"].AsText(); }
            set { ViewState["SearchKey"] = value; }
        }

        /// <summary>
        /// 검색 항목의 값 (여기에 값이 있으면, 검색으로 엔티티를 로드해야 함을 나타냅니다)
        /// </summary>
        public virtual object SearchValue
        {
            get { return ViewState["SearchValue"]; }
            set { ViewState["SearchValue"] = value; }
        }

        /// <summary>
        /// Grid에 표시할 엔티티 컬렉션
        /// </summary>
        public virtual IPagingList<T> Entities { get; set; }

        /// <summary>
        /// 엔티티를 표현할 Grid 
        /// </summary>
        public abstract RadGrid EntityGrid { get; }

        /// <summary>
        /// 엔티티를 표현할 Grid의 Pager
        /// </summary>
        public abstract RealPager GridPager { get; }

        /// <summary>
        /// 엔티티를 표현할 Grid의 Legend
        /// </summary>
        public virtual RealLegend GridLegend
        {
            get { return null; }
        }

        /// <summary>
        /// 편집용 컨트롤
        /// </summary>
        public virtual IEntityEditUserControl<T> EntityEditControl { get; protected set; }

        /// <summary>
        /// 초기 화면 표시
        /// </summary>
        protected virtual void DisplayDefault()
        {
            ResetPageIndex(0);
            ResetGridSort();
            LoadAndBindEntity();
        }

        /// <summary>
        /// PageIndex를 지정된 값으로 설정합니다.
        /// </summary>
        /// <param name="pageIndex">설정할 PageIndex 값</param>
        protected virtual void ResetPageIndex(int? pageIndex)
        {
            PageIndex = pageIndex.AsInt(0);

            if(GridPager != null)
                GridPager.ChangePageIndexInSilence(PageIndex);
        }

        /// <summary>
        /// Grid 정렬 방식 초기화
        /// </summary>
        protected virtual void ResetGridSort()
        {
            GridSortExpr = string.Empty;
            GridSortAsc = true;
        }

        /// <summary>
        ///  엔티티를 로드하고, UI Control에 바인딩 합니다. 
        /// </summary>
        protected void LoadAndBindEntity()
        {
            LoadAndBindEntity(null);
        }

        /// <summary>
        ///  엔티티를 로드하고, UI Control에 바인딩 합니다. 
        /// </summary>
        /// <param name="exceptionAction">예외발생 시 수행할 Action</param>
        protected virtual void LoadAndBindEntity(Action<Exception> exceptionAction)
        {
            if(IsDebugEnabled)
                log.Debug(@"엔티티[{0}] 목록을 로드하여, Grid에 바인딩 시킵니다...", ConcreteType.Name);
            try
            {
                LoadEntity();
                BindEntity();
            }
            catch(Exception ex)
            {
                var errorMsg = string.Format(@"엔티티[{0}]를 로드하고, Binding 시에 예외가 발생했습니다.", ConcreteType.Name);

                if(log.IsErrorEnabled)
                    log.ErrorException(errorMsg, ex);

                if(exceptionAction != null)
                    exceptionAction(ex);
                else
                    WebAppTool.MessageBox(errorMsg, this);
            }
        }

        /// <summary>
        /// 엔티티 목록을 로드하여 <see cref="Entities"/>에 할당합니다.
        /// </summary>
        protected abstract void LoadEntity();

        /// <summary>
        /// 로딩된 엔티티를 UI Control에 바인딩합니다.
        /// </summary>
        protected virtual void BindEntity()
        {
            if(IsDebugEnabled)
                log.Debug(@"EntityGrid에 로딩된 Entity[{0}] 컬렉션을 Binding합니다...", ConcreteType.Name);

            EntityGrid.DataSource = Entities;
            EntityGrid.DataBind();

            if(GridPager != null)
            {
                GridPager.PageCount = Entities.TotalPageCount;
                GridPager.Visible = Entities.TotalPageCount > 1;
            }

            if(GridLegend != null)
            {
                GridLegend.Count = Entities.TotalItemCount;
            }

            ResetPageIndex(Math.Max(0, Math.Min(Entities.PageIndex, Entities.TotalPageCount - 1)));
        }

        /// <summary>
        /// Page 초기화 시 수행할 작업들
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            DoRegisterEntityGridEventHandlers();
        }

        /// <summary>
        /// Page Load 시 수행할 작업
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if(IsPostBack == false)
                DisplayDefault();
        }

        /// <summary>
        /// Grid, Pager, EditContorl의 표준 이벤트에 대해 이벤트 핸들러를 자동으로 등록합니다.
        /// </summary>
        protected virtual void DoRegisterEntityGridEventHandlers()
        {
            if(IsDebugEnabled)
                log.Debug(@"EntityGrid의 Event에 EventHandler들을 등록합니다. ItemCommand, ItemDataBound, SortCommand 이벤트를 등록합니다.");

            EntityGrid.ItemCommand += EntityGrid_ItemCommand;
            EntityGrid.ItemDataBound += EntityGrid_ItemDataBound;
            EntityGrid.SortCommand += EntityGrid_SortCommand;

            if(IsDebugEnabled)
                log.Debug(@"GridPager의 PageIndexChanged Event에 EventHandler들을 등록합니다.");

            if(GridPager != null)
                GridPager.PageIndexChanged += GridPager_PageIndexChanged;

            if(EntityEditControl != null && EntityEditControl is EntityEditUserControlBase<T>)
            {
                if(IsDebugEnabled)
                    log.Debug(@"EntityEditControl의 Saved Event에 EventHandler들을 등록합니다.");

                ((EntityEditUserControlBase<T>)EntityEditControl).Saved += EntityEditControl_Saved;
            }
        }

        /// <summary>
        /// EntityGrid.ItemCommand 이벤트에 대한 핸들러입니다.
        /// </summary>
        private void EntityGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            DoGridItemCommand(e);
        }

        /// <summary>
        /// EntityGrid.ItemDataBound 이벤트에 대한 핸들러입니다.
        /// </summary>
        private void EntityGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            DoGridItemDataBound(e.Item);
        }

        /// <summary>
        /// EntityGrid.SortCommand 이벤트에 대한 핸들러입니다.
        /// </summary>
        private void EntityGrid_SortCommand(object sender, GridSortCommandEventArgs e)
        {
            DoGridSortCommand(e.SortExpression, e.NewSortOrder);
        }

        /// <summary>
        /// GridPager.PageIndexChanged 이벤트에 대한 핸들러입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridPager_PageIndexChanged(object sender, RealPagingEventArgs e)
        {
            DoPageIndexChange(e.NewPageIndex);
        }

        /// <summary>
        /// 엔티티 편집 컨트롤이 저장작업을 완료했다는 이벤트에 대한 핸들러입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntityEditControl_Saved(object sender, EventArgs e)
        {
            DoEntitySaved();
        }

        /// <summary>
        /// Grid Item에서 Command 가 발생했을 경우
        /// </summary>
        /// <param name="e"></param>
        protected virtual void DoGridItemCommand(GridCommandEventArgs e)
        {
            if(IsDebugEnabled)
                log.Debug(@"EntityGrid의 ItemCommand 이벤트가 발생했습니다. e.CommandName=" + e.CommandName);
        }

        /// <summary>
        /// Grid Item이 Binding될 때마다 발생하는 이벤트 핸들러
        /// </summary>
        /// <param name="item"></param>
        protected virtual void DoGridItemDataBound(GridItem item) { }

        /// <summary>
        /// Grid를 정렬합니다.
        /// </summary>
        protected void DoGridSortCommand(string sortExpression, GridSortOrder gridSortOrder)
        {
            DoGridSortCommand(sortExpression, gridSortOrder != GridSortOrder.Descending);
        }

        /// <summary>
        /// Grid를 정렬합니다.
        /// </summary>
        protected virtual void DoGridSortCommand(string sortExpression, bool? sortAscending)
        {
            GridSortExpr = sortExpression;
            GridSortAsc = sortAscending.GetValueOrDefault(true);

            if(IsDebugEnabled)
                log.Debug(@"EntityGrid의 정렬을 수행합니다. GridSortExpr={0}, GridSortAsc={1}",
                          GridSortExpr, GridSortAsc);

            LoadAndBindEntity(ex => WebAppTool.MessageBox(@"정렬을 수행하는 중에 예외가 발생했습니다.", this));
        }

        /// <summary>
        /// Grid 의 Page 변경 시 작업
        /// </summary>
        /// <param name="pageIndex"></param>
        protected virtual void DoPageIndexChange(int pageIndex)
        {
            if(IsDebugEnabled)
                log.Debug(@"목록 페이지가 변경되었습니다!!! pageIndex=" + pageIndex);

            PageIndex = pageIndex;
            LoadAndBindEntity();
        }

        /// <summary>
        /// 검색 작업 시
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected virtual void DoSearchEntity(string key, object value)
        {
            if(IsDebugEnabled)
                log.Debug(@"엔티티를 조회하려고 합니다. key={0}, value={1}", key, value);

            SearchKey = key;
            SearchValue = value.AsText();

            ResetPageIndex(0);
            LoadAndBindEntity(ex => WebAppTool.MessageBox(@"검색 시 예외가 발생했습니다. message=" + ex.Message, this));
        }

        /// <summary>
        /// 엔티티 추가 생성 시 작업
        /// </summary>
        protected virtual void DoAddEntity()
        {
            EntityEditControl.ShouldNotBeNull("EntityEditControl");

            if(IsDebugEnabled)
                log.Debug(@"새로운 엔티티를 추가하려고 합니다..");

            DoEditEntity(null);
        }

        /// <summary>
        /// 지정된 엔티티를 수정하도록 편집 Control을 화면에 띄웁니다. entity가 null인 경우 새로 생성하는 것입니다.
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void DoEditEntity(T entity)
        {
            EntityEditControl.ShouldNotBeNull("EntityEditControl");

            if(IsDebugEnabled)
                log.Debug(@"엔티티 추가 또는 편집을 하기 위해 EditControl을 보여줍니다... entity=" + entity);

            EntityEditControl.Show(entity);
        }

        /// <summary>
        /// 엔티티가 추가 또는 갱신되어 실제 저장에 성공했을 때, 목록을 Refresh하기 위한 작업을 수행합니다.
        /// </summary>
        protected virtual void DoEntitySaved()
        {
            if(GridPager != null)
                PageIndex = GridPager.PageIndex;

            if(IsDebugEnabled)
                log.Debug(@"저장이벤트가 발생하였습니다.PageIndex={0}", PageIndex);

            LoadAndBindEntity(null);
        }

        /// <summary>
        /// 그리드의 Item 중 선택된 항목의 dataKey ("Id" 또는 "Code") 값을 추출해서, 해당 엔티티를 삭제합니다.
        /// </summary>
        protected virtual void DoRemoveEntity(string dataKey)
        {
            var idsToRemove = new List<object>();

            foreach(GridDataItem item in EntityGrid.SelectedItems)
            {
                if(item.ItemIndex >= 0)
                {
                    var id = item.OwnerTableView.DataKeyValues[item.ItemIndex][dataKey].AsText();
                    if(id.IsNotWhiteSpace())
                        idsToRemove.Add(id);
                }
            }

            if(idsToRemove.Count == 0)
            {
                WebAppTool.MessageBox(@"삭제할 엔티티를 선택하세요.", this);
                return;
            }

            RemoveEntitiesById(idsToRemove);
            LoadAndBindEntity(null);
        }

        /// <summary>
        /// 지정한 엔티티 ID를 가진 엔티티들을 삭제합니다.
        /// </summary>
        /// <param name="entityIds">삭제할 엔티티의 Identifier의 컬렉션</param>
        protected virtual void RemoveEntitiesById(ICollection entityIds)
        {
            // override 하세요
        }
    }
}