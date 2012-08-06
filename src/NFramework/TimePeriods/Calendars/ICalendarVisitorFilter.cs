using System;
using System.Collections.Generic;

namespace NSoft.NFramework.TimePeriods.Calendars {
    /// <summary>
    /// Calendar 탐색 시의 필터 정보 (예외 기간, 포함 일자 정보를 가진다)
    /// </summary>
    public interface ICalendarVisitorFilter {
        /// <summary>
        /// 탐색 시 제외할 기간들
        /// </summary>
        ITimePeriodCollection ExcludePeriods { get; }

        /// <summary>
        /// 포함 년도
        /// </summary>
        IList<int> Years { get; }

        /// <summary>
        /// 포함 월
        /// </summary>
        IList<int> Months { get; }

        /// <summary>
        /// 포함 일
        /// </summary>
        IList<int> Days { get; }

        /// <summary>
        /// 포함 요일(DayOfWeek) (예: 월, 수, 금)
        /// </summary>
        IList<DayOfWeek> WeekDays { get; }

        /// <summary>
        /// 포함 시(Hour)
        /// </summary>
        IList<int> Hours { get; }

        /// <summary>
        /// 포함 분(Minute)
        /// </summary>
        IList<int> Minutes { get; }

        /// <summary>
        /// 주중 (월~금) 을 Working Day로 추가합니다.
        /// </summary>
        void AddWorkingWeekDays();

        /// <summary>
        /// 주말 (토,일) 을 Working Day로 추가합니다.
        /// </summary>
        void AddWeekendWeekDays();

        /// <summary>
        /// 지정한 요일들을 탐색 필터에 포함시킨다.
        /// </summary>
        /// <param name="dayOfWeeks"></param>
        void AddWeekDays(params DayOfWeek[] dayOfWeeks);

        /// <summary>
        /// 탐색 필터 및 예외 필터에 등록된 모든 내용을 삭제합니다.
        /// </summary>
        void Clear();
    }
}