using System;

namespace NSoft.NFramework.TimePeriods {
    public static partial class TimeTool {
        /// <summary>
        /// 1월
        /// </summary>
        public const int January = (int)MonthKind.January;

        /// <summary>
        /// 2월
        /// </summary>
        public const int Feburary = (int)MonthKind.Feburary;

        /// <summary>
        /// 3월
        /// </summary>
        public const int March = (int)MonthKind.March;

        /// <summary>
        /// 4월
        /// </summary>
        public const int April = (int)MonthKind.April;

        /// <summary>
        /// 5월
        /// </summary>
        public const int May = (int)MonthKind.May;

        /// <summary>
        /// 6월
        /// </summary>
        public const int June = (int)MonthKind.June;

        /// <summary>
        /// 7월
        /// </summary>
        public const int July = (int)MonthKind.July;

        /// <summary>
        /// 8월
        /// </summary>
        public const int August = (int)MonthKind.August;

        /// <summary>
        /// 9월
        /// </summary>
        public const int September = (int)MonthKind.September;

        /// <summary>
        /// 10월
        /// </summary>
        public const int October = (int)MonthKind.October;

        /// <summary>
        /// 11월
        /// </summary>
        public const int November = (int)MonthKind.November;

        /// <summary>
        /// 12월
        /// </summary>
        public const int December = (int)MonthKind.December;

        /// <summary>
        /// null 을 표현하는 문자열 (NULL) 입니다.
        /// </summary>
        public const string NullString = @"NULL";

        /// <summary>
        /// Unix 시스템에서 최소시각으로 정한 일자 (1970-01-01)
        /// </summary>
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
    }
}