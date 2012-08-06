using System;
using System.Collections.Generic;
using NSoft.NFramework.Tools;
using Telerik.Web.UI;

namespace NSoft.NFramework.Web.UserControls
{
    /// <summary>
    /// Entity 를 ComboBox에 표현하는 Control입니다.
    /// </summary>
    [Serializable]
    public abstract class EntityComboBoxBase<T> : UserControlBase where T : class
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public event EventHandler SelectedValueChanged = delegate { };

        private int _pageSize = AppSettings.DefaultPageSize;

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = Math.Max(3, value); }
        }

        public virtual RadComboBox EntityComboBox { get; set; }

        public virtual IPagingList<T> Entities { get; set; }

        private string _selectedValue = string.Empty;

        public virtual string SelectedValue
        {
            get { return EntityComboBox.SelectedValue.AsText(_selectedValue.AsText()); }
            set
            {
                if(_selectedValue != value)
                {
                    _selectedValue = value;
                    OnSelectedValueChanged(_selectedValue);
                }
            }
        }

        private T _selectedEntity;

        public virtual T SelectedEntity
        {
            get
            {
                if(_selectedEntity == null && SelectedValue.IsNotWhiteSpace())
                    _selectedEntity = LoadEntity(SelectedValue);

                return _selectedEntity;
            }
            //set
            //{
            //    // Nothing to do.
            //}
        }

        protected virtual void OnSelectedValueChanged(string selectedValue)
        {
            if(selectedValue.IsWhiteSpace())
            {
                SelectedValueChanged(this, EventArgs.Empty);
                return;
            }

            _selectedEntity = LoadEntity(selectedValue);

            //ComboBox 가 EnableLoadOnDemand 기능이 true 인 경우
            if(EntityComboBox.EnableLoadOnDemand)
                BindEntities(EntityComboBox, new T[] {_selectedEntity});
            else
                BindEntities(EntityComboBox, LoadEntityPagingList(string.Empty, 0, 0));

            if(_selectedEntity == null)
                return;

            try
            {
                DoSelectedEntityBind(_selectedEntity);

                EntityComboBox.SelectedIndex = -1;
                foreach(RadComboBoxItem item in EntityComboBox.Items)
                {
                    if(item.Value == selectedValue)
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                if(log.IsWarnEnabled)
                    log.WarnException(@"ComboBox SelectedValued 변경 오류", ex);
            }

            SelectedValueChanged(this, EventArgs.Empty);
        }

        protected virtual void DoComboBoxItemsRequested(RadComboBoxItemsRequestedEventArgs e)
        {
            EntityComboBox.Items.Clear();

            int itemOffset = e.NumberOfItems;
            int pageIndex = (itemOffset < PageSize) ? 0 : itemOffset / PageSize;

            var entities = LoadEntityPagingList(e.Text, pageIndex, PageSize);

            var endOffset = Math.Min(itemOffset + PageSize, entities.TotalItemCount - 1);
            e.EndOfItems = (endOffset == entities.TotalItemCount - 1);

            if(IsDebugEnabled)
                log.Debug(@"{0} 선택... itemOffset={1}, endOffset={2}, productParts.TotalItemCount={3}",
                          typeof(T).Name, itemOffset, endOffset, entities.TotalItemCount);

            BindEntities(EntityComboBox, entities);
            e.Message = GetStatusMessage(endOffset, entities.TotalItemCount);
        }

        /// <summary>
        /// 지정된 엔티티를 선택된 아이템이 되도록 합니다.
        /// </summary>
        /// <param name="entity"></param>
        protected abstract void DoSelectedEntityBind(T entity);

        /// <summary>
        /// ComboBox Item의 DataBound 시에 특정 속성을 바인팅합니다.
        /// </summary>
        protected abstract void DoComboBoxItemDataBound(RadComboBoxItem item);

        /// <summary>
        /// 선택된 값에 해당하는 Entity를 로드합니다.
        /// </summary>
        protected abstract T LoadEntity(object value);

        /// <summary>
        /// 실제 Paging List로 로드하려고 할 때 호출하세요.
        /// </summary>
        /// <returns></returns>
        protected abstract IPagingList<T> LoadEntityPagingList(object value, int pageIndex, int pageSize);

        /// <summary>
        /// 로드된 엔티티들을 컨트롤에 바인딩 시킵니다.
        /// </summary>
        protected static void BindEntities(ControlItemContainer container, IEnumerable<T> entities)
        {
            if(IsDebugEnabled)
                log.Debug(@"엔티티를 ItemContainer에 바인딩합니다...");

            container.DataSource = entities;
            container.DataBind();
        }
    }
}