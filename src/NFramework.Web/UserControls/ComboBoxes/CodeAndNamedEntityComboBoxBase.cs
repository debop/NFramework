using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NHibernate.Criterion;
using Telerik.Web.UI;

namespace NSoft.NFramework.Web.UserControls
{
    /// <summary>
    /// <see cref="ICodeEntity"/> 을 상속받은 Entity 를 ComboBox에 표현하는 Control입니다.
    /// </summary>
    /// <typeparam name="T">엔티티 형식</typeparam>
    public abstract class CodeAndNamedEntityComboBoxBase<T> : EntityComboBoxBase<T> where T : class, INamedEntity, ICodeEntity
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 지정된 엔티티(<paramref name="entity"/>)를 선택된 아이템이 되도록 합니다.
        /// </summary>
        /// <param name="entity">선택할 엔티티</param>
        protected override void DoSelectedEntityBind(T entity)
        {
            EntityComboBox.Text = entity.Name;
            EntityComboBox.SelectedValue = entity.Code;
        }

        /// <summary>
        /// ComboBox Item(<paramref name="item"/>)의 DataBound 시에 특정 속성을 바인팅합니다.
        /// </summary>
        /// <param name="item">RadComboBoxItem</param>
        protected override void DoComboBoxItemDataBound(RadComboBoxItem item)
        {
            var entity = item.DataItem as T;

            if(entity != null)
            {
                item.Text = entity.Name;
                item.Value = entity.Code;
            }
        }

        // NOTE: Control 내에서 Domain Service 관련 작업을 수행하는 것은 좋지 않다.
        // TODO: Extensions 메소드로 빼라

        /// <summary>
        /// 선택된 코드(<param name="value"/>)값에 해당하는 Entity를 로드합니다.
        /// </summary>
        /// <param name="value">코드 값</param>
        /// <returns></returns>
        protected override T LoadEntity(object value)
        {
            var code = value.AsText();
            return Repository<T>.FindOne(item => item.Code == code);
        }

        /// <summary>
        /// 지정된 회사의 모든 사용자 정보를 Paging 처리하여 로드합니다
        /// 실제 Paging List로 로드하려고 할 때 호출하세요.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        protected override IPagingList<T> LoadEntityPagingList(object value, int pageIndex, int pageSize)
        {
            var query = QueryOver.Of<T>().AddInsensitiveLike(item => item.Name, value.AsText(), MatchMode.Anywhere);

            return Repository<T>.GetPage(pageIndex,
                                         pageSize,
                                         query,
                                         new NHOrder<T>(item => item.Name),
                                         new NHOrder<T>(item => item.Code));
        }
    }
}