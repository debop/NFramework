using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 년도 종류 (년의 종류에 따라 시작일이 달라집니다) (예: 회계년도의 시작월은 2월이고, 한국의 학기는 3월에 시작입니다)
    /// </summary>
    [Serializable]
    public enum YearKind {
        /// <summary>
        /// 설치된 OS의 문화권에 해당하는 Year
        /// </summary>
        SystemYear,

        /// <summary>
        /// 현 ThreadContext에 지정된 문화권의 Calendar의 Year
        /// </summary>
        CalendarYear,

        /// <summary>
        /// 회계 년 (2월 시작)
        /// </summary>
        FiscalYear,

        /// <summary>
        /// 교육년 (3월 시작, 9월 시작)
        /// </summary>
        SchoolYear,

        /// <summary>
        /// 사용자 정의 년
        /// </summary>
        CustomYear
    }
}