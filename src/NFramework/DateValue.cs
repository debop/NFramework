using System;

namespace NSoft.NFramework {
    /// <summary>
    /// <see cref="DateTime"/>의 시간 부분을 제외한 날짜 부분만을 표현합니다.
    /// </summary>
    [Serializable]
    public struct DateValue : IEquatable<DateValue>, IComparable<DateValue>, IComparable {
        /// <summary>
        /// 현재 날짜
        /// </summary>
        public static DateValue Now {
            get { return new DateValue(DateTime.Now); }
        }

        private readonly DateTime _date;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="moment"></param>
        public DateValue(DateTime moment)
            : this() {
            _date = moment.Date;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="year">년</param>
        /// <param name="month">월</param>
        /// <param name="day">일</param>
        public DateValue(int year, int month = 1, int day = 1)
            : this() {
            _date = new DateTime(year, month, day);
        }

        /// <summary>
        /// 년
        /// </summary>
        public int Year {
            get { return _date.Year; }
        }

        /// <summary>
        /// 월
        /// </summary>
        public int Month {
            get { return _date.Month; }
        }

        /// <summary>
        /// 일
        /// </summary>
        public int Day {
            get { return _date.Day; }
        }

        public DateTime GetDateTime(TimeValue time) {
            return _date.Add(time.Duration);
        }

        public DateTime GetDateTime(int hours = 0, int minutes = 0, int seconds = 0, int milliseconds = 0) {
            return _date.Add(new TimeSpan(0, hours, minutes, seconds, milliseconds));
        }

        public bool Equals(DateValue other) {
            return GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is DateValue) && Equals((DateValue)obj);
        }

        public override int GetHashCode() {
            return HashTool.Compute(_date);
        }

        public int CompareTo(object obj) {
            if(obj is DateValue)
                return CompareTo((DateValue)obj);

            throw new InvalidOperationException("Date 수형이 아닙니다.");
        }

        public int CompareTo(DateValue other) {
            return _date.CompareTo(other._date);
        }

        public override string ToString() {
            return "DateValue# " + _date;
        }
    }
}