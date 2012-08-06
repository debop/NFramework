using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 날짜 검색 시, 해당 검색이 완료 경계까지 인가 그 다음에 해당하는 부분인가를 정의한다.
    /// </summary>
    [Serializable]
    public enum SeekBoundaryMode {
        /// <summary>
        /// DateTime 검색 시 검색한 값을 반환하도록 한다.
        /// </summary>
        Fill,

        /// <summary>
        /// DateTime 검색 시 검색한 다음 값을 반환하도록 한다.
        /// </summary>
        Next
    }
}