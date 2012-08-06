namespace NSoft.NFramework {
    /// <summary>
    /// Paging 관련 정보를 가진다.
    /// </summary>
    public interface IPagingInfo {
        /// <summary>
        /// 현재 Page 번호 (0 부터 시작)
        /// </summary>
        int PageIndex { get; }

        /// <summary>
        /// Page 당 Item의 수 (예: 한 페이지에 10개의 Item이면 PageSize는 10이다.)
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// 전체 Page 수
        /// </summary>
        int TotalPageCount { get; }

        /// <summary>
        /// 전체 Item의 갯수
        /// </summary>
        int TotalItemCount { get; }
    }
}