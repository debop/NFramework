using System;
using System.Collections.Generic;
using NSoft.NFramework.LinqEx;

namespace NSoft.NFramework.TimePeriods.Calendars {
    /// <summary>
    /// Calendar 탐색 시의 필터 정보 (예외 기간, 포함 일자 정보를 가진다)
    /// </summary>
    [Serializable]
    public class CalendarVisitorFilter : ICalendarVisitorFilter {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private readonly ITimePeriodCollection _excludePeriods = new TimePeriodCollection();
        private readonly List<int> _years = new List<int>();
        private readonly List<int> _months = new List<int>();
        private readonly List<int> _days = new List<int>();
        private readonly List<DayOfWeek> _weekDays = new List<DayOfWeek>();
        private readonly List<int> _hours = new List<int>();
        private readonly List<int> _minutes = new List<int>();

        private readonly object _syncLock = new object();

        /// <summary>
        /// 탐색 시 제외할 기간들
        /// </summary>
        public ITimePeriodCollection ExcludePeriods {
            get { return _excludePeriods; }
        }

        /// <summary>
        /// 포함 년도
        /// </summary>
        public IList<int> Years {
            get { return _years; }
        }

        /// <summary>
        /// 포함 월
        /// </summary>
        public IList<int> Months {
            get { return _months; }
        }

        /// <summary>
        /// 포함 일
        /// </summary>
        public IList<int> Days {
            get { return _days; }
        }

        /// <summary>
        /// 포함 요일(DayOfWeek) (예: 월, 수, 금)
        /// </summary>
        public IList<DayOfWeek> WeekDays {
            get { return _weekDays; }
        }

        /// <summary>
        /// 포함 시(Hour)
        /// </summary>
        public IList<int> Hours {
            get { return _hours; }
        }

        /// <summary>
        /// 포함 분(Minute)
        /// </summary>
        public IList<int> Minutes {
            get { return _minutes; }
        }

        /// <summary>
        /// 주중 (월~금) 을 Working Day로 추가합니다.
        /// </summary>
        public virtual void AddWorkingWeekDays() {
            AddWeekDays(DayOfWeek.Monday,
                        DayOfWeek.Tuesday,
                        DayOfWeek.Wednesday,
                        DayOfWeek.Thursday,
                        DayOfWeek.Friday);
        }

        /// <summary>
        /// 주말 (토,일) 을 Working Day로 추가합니다.
        /// </summary>
        public virtual void AddWeekendWeekDays() {
            AddWeekDays(DayOfWeek.Saturday,
                        DayOfWeek.Sunday);
        }

        /// <summary>
        /// 지정한 요일들을 탐색 필터에 포함시킨다.
        /// </summary>
        /// <param name="dayOfWeeks"></param>
        public virtual void AddWeekDays(params DayOfWeek[] dayOfWeeks) {
            if(dayOfWeeks != null)
                dayOfWeeks.RunEach(dow => _weekDays.Add(dow));
        }

        /// <summary>
        /// 탐색 필터 및 예외 필터에 등록된 모든 내용을 삭제합니다.
        /// </summary>
        public virtual void Clear() {
            lock(_syncLock) {
                _years.Clear();
                _months.Clear();
                _days.Clear();
                _weekDays.Clear();
                _hours.Clear();
                _minutes.Clear();
            }
        }

        public override string ToString() {
            return this.AsString();
        }
    }
}