using System;
using System.Globalization;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 현재 Thread 하에서 TimePeriod 관련하여 제공할 정보를 제공합니다.
    /// </summary>
    public static class PeriodContext {
        private const string CurrentTimeCalendarKey = @"NSoft.NFramework.TimePeriod.PeriodContext.CurrentTimeCalendar.Key";

        /// <summary>
        /// 현 Thread Context 하에서 설정된 TimeCalendar 관련 설정 정보
        /// </summary>
        public static class Current {
            /// <summary>
            /// 현재 Thread Context하에서 사용할 TimeCalendar입니다.
            /// </summary>
            public static ITimeCalendar TimeCalendar {
                get {
                    return
                        (ITimeCalendar)
                        (Local.Data[CurrentTimeCalendarKey] ??
                         (Local.Data[CurrentTimeCalendarKey] = NSoft.NFramework.TimePeriods.TimeCalendar.New()));
                }
                set { Local.Data[CurrentTimeCalendarKey] = value; }
            }

            /// <summary>
            /// 현 Thread 하에서 TimePeriod에서 사용할 기본 Culture
            /// </summary>
            public static CultureInfo Culture {
                get { return TimeCalendar.Culture; }
            }

            /// <summary>
            /// 현 Thread 하에서 TimePeriod 연산에 사용할 <see cref="WeekOfYearRuleKind"/>
            /// </summary>
            public static WeekOfYearRuleKind WeekOfYearRule {
                get { return TimeCalendar.WeekOfYearRule; }
            }

            /// <summary>
            /// 현 Thread 하에서 한 해의 첫 주를 결정하는 규칙
            /// </summary>
            public static CalendarWeekRule CalendarWeekRule {
                get { return TimeCalendar.CalendarWeekRule; }
            }

            /// <summary>
            /// 현 Thread 하에서 한 주의 첫번째 요일 정보
            /// </summary>
            public static DayOfWeek FirstDayOfWeek {
                get { return TimeCalendar.FirstDayOfWeek; }
            }
        }
    }
}