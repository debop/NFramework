using System;
using System.Collections.Generic;

namespace NSoft.NFramework {
    /// <summary>
    /// Persistent Object를 Paging된 Control에 Binding하기 위해 사용되는 List
    /// </summary>
    /// <typeparam name="T">Type of persistent object</typeparam>
    [Serializable]
    public class PagingList<T> : List<T>, IPagingList<T> {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="items">collection of persistent object</param>
        /// <param name="pageIndex">index of page</param>
        /// <param name="pageSize">size of page</param>
        /// <param name="totalItemCount">total count of items</param>
        public PagingList(IEnumerable<T> items, int pageIndex, int pageSize, int totalItemCount) {
            pageIndex.ShouldBePositiveOrZero("pageIndex");

            if(items != null)
                AddRange(items);

            PageIndex = pageIndex;
            PageSize = (pageSize > 0) ? pageSize : totalItemCount;
            TotalItemCount = totalItemCount;
            TotalPageCount = (int)Math.Ceiling(totalItemCount / (double)PageSize);
        }

        /// <summary>
        /// Page Index (0 부터 시작)
        /// </summary>
        public virtual int PageIndex { get; private set; }

        /// <summary>
        /// Page 당 Item의 수 (예: 한 페이지에 10개의 Item이면 PageSize는 10이다.)
        /// </summary>
        public virtual int PageSize { get; private set; }

        /// <summary>
        /// 전체 Page 수
        /// </summary>
        public virtual int TotalPageCount { get; private set; }

        /// <summary>
        /// 전체 Item의 갯수
        /// </summary>
        public virtual int TotalItemCount { get; private set; }

        /// <summary>
        /// 현재 인스턴스를 나타내는 문자열을 반환합니다.
        /// </summary>
        public override string ToString() {
            return string.Format("PagingList<{0}># PageIndex=[{1}], PageSize=[{2}], TotalPageCount=[{3}], TotalItemCount=[{4}]",
                                 typeof(T).FullName, PageIndex, PageSize, TotalPageCount, TotalItemCount);
        }
    }
}