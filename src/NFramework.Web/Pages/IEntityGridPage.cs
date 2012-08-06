using System;
using NSoft.NFramework.Web.UserControls;
using RealWeb.Portal.Controls;
using Telerik.Web.UI;

namespace NSoft.NFramework.Web.Pages
{
    /// <summary>
    /// 엔티티 목록을 Grid로 표현하는 웹 페이지의 기본 인터페이스입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEntityGridPage<T> where T : class
    {
        /// <summary>
        /// Entity Grid의 정렬 항목
        /// </summary>
        string GridSortExpr { get; set; }

        /// <summary>
        /// Entity Grid의 정렬 방식 (Ascending|Descending)
        /// </summary>
        bool GridSortAsc { get; set; }

        /// <summary>
        /// Entity Grid의 Page Index (0부터 시작)
        /// </summary>
        int PageIndex { get; set; }

        /// <summary>
        /// Entity Grid의 Page Size (한 페이지당 엔티티 갯수, 기본은 <see cref="AppSettings.DefaultPageSize"/>)
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// 검색 항목
        /// </summary>
        string SearchKey { get; set; }

        /// <summary>
        /// 검색 항목의 값 (여기에 값이 있으면, 검색으로 엔티티를 로드해야 함을 나타냅니다)
        /// </summary>
        object SearchValue { get; set; }

        /// <summary>
        /// Grid에 표시할 엔티티 컬렉션
        /// </summary>
        IPagingList<T> Entities { get; set; }

        /// <summary>
        /// 엔티티를 표현할 Grid 
        /// </summary>
        RadGrid EntityGrid { get; }

        /// <summary>
        /// 엔티티를 표현할 Grid의 Pager
        /// </summary>
        RealPager GridPager { get; }

        /// <summary>
        /// 엔티티를 표현할 Grid의 Legend
        /// </summary>
        RealLegend GridLegend { get; }

        /// <summary>
        /// 편집용 컨트롤
        /// </summary>
        IEntityEditUserControl<T> EntityEditControl { get; }
    }
}