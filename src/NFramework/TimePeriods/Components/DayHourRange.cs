using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 특정 요일의 시간을 표현합니다. (예: 금요일 1시~5시)
    /// </summary>
    public class DayHourRange : HourRangeInDay {
        private readonly DayOfWeek _dayOfWeek;

        public DayHourRange(DayOfWeek dayOfWeek, int hour)
            : base(hour) {
            _dayOfWeek = dayOfWeek;
        }

        public DayHourRange(DayOfWeek dayOfWeek, int startHour, int endHour)
            : base(startHour, endHour) {
            _dayOfWeek = dayOfWeek;
        }

        public DayHourRange(DayOfWeek dayOfWeek, TimeValue start, TimeValue end) : base(start, end) {
            _dayOfWeek = dayOfWeek;
        }

        public DayOfWeek Day {
            get { return _dayOfWeek; }
        }

        public override int GetHashCode() {
            return HashTool.Compute(base.GetHashCode(), _dayOfWeek);
        }

        public override string ToString() {
            return Day + ": " + base.ToString();
        }
    }
}