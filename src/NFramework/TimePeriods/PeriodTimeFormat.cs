using System;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 반복 작업의 주기를 설정하기 위한 방법으로 사용하는 형식을 나타낸다.
    /// </summary>
    /// <remarks>
    /// 반복 주기를 나타내는 문자열 형식은 다음과 같다. 첫번째 열부터 분, 시간, 일, 월, 요일 을 나타낸다.<br/>
    /// <br/>
    /// 분 (minute)     : 0 ~ 59<br/>
    /// 시 (hour)       : 0 ~ 23<br/>
    /// 일 (day of month) : 1 ~ 31<br/>
    /// 월 (month)        : 1 ~ 12<br/>
    /// 요일 (day of week): 0 ~ 7 (일요일이 0)<br/>
    /// </remarks>
    /// <example>
    ///	  // 5 * * * *      : 매시 5분에
    ///   // 0,30 * * * *   : 매시 0분, 30분에
    ///   // 0 21 * * * *   : 매일 21시 00분에
    ///   // 0 15,21 * * *  : 매일 15시, 21시에
    ///   // 31 15  * * *   : 매일 15시 31분에
    ///   // 7 4 * * 6      : 토요일 4시 7분에
    ///   // 15 21 4 7 *    : 7월 4일 21시 15분에
    /// 
    ///   // 이것은 아직 하지 않았다.
    ///   // 현재 단위별이 아닌 격주, 격월 등은 'b' 를 접두사로 두면 된다.
    ///	  // 0 3 * * b6     : 격주 토요일 3시 0분 마다
    /// 
    ///	// 
    ///	// 매일 0시와 15시에 반복수행하도록 설정, 현재시각에 지정된 작업을 새로 수행해야 할지 검사한다. (True 를 반환하면 새로운 작업 수행, 아니면 필요없다.)
    ///   Console.WriteLine(PeriodTimeFormat.IsExpired("0 0,15 * * *",
    ///                                                new DateTime(2007, 3, 22, 16, 0, 0),
    ///                                                new DateTime(2007, 3, 22, 22, 0, 0));
    /// </example>
    public static class PeriodTimeFormat {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private const int PERIOD_ITEM_COUNT = 5;
        private const char PERIOD_ITEM_DELIMITER = ' ';
        private const char PERIOD_ITEM_OR = ',';
        private const char PERIOD_ITEM_ANY = '*';

        /// <summary>
        /// 현재 시각이 지정된 주기를 경과했는지 확인합니다.
        /// </summary>
        /// <param name="periodFormat">반복 스케쥴 설정을 나타내는 문자열</param>
        /// <param name="getTime">마지막에 반복한 스케쥴의 시간 (이전 실행 시간)</param>
        /// <param name="currTime">현재 시각</param>
        /// <returns>지정된 시간이 경과했으면 True, 아직 경과시간이 안되었으면 False</returns>
        public static bool IsExpired(string periodFormat, DateTime getTime, DateTime? currTime) {
            periodFormat.ShouldNotBeWhiteSpace("periodFormat");

            var now = currTime ?? DateTime.Now;

            if(IsDebugEnabled)
                log.Debug("현재 시각이 지정된 주기에 대해 초과되었는지 확인합니다. periodFormat=[{0}], getTime=[{1}], currTime=[{2}]",
                          periodFormat, getTime, now);

            var fmtArray = periodFormat.Split(StringSplitOptions.RemoveEmptyEntries, PERIOD_ITEM_DELIMITER);

            Guard.Assert(fmtArray != null && fmtArray.Length == PERIOD_ITEM_COUNT,
                         "반복 주기를 나타내는 문자열의 형식이 잘못되었습니다. periodFormat=[{0}]", periodFormat);

            return ValidateMinute(fmtArray[0], getTime, now) &&
                   ValidateHour(fmtArray[1], getTime, now) &&
                   ValidateDayOfMonth(fmtArray[2], getTime, now) &&
                   ValidateMonth(fmtArray[3], getTime, now) &&
                   ValidateDayOfWeek(fmtArray[4], getTime, now);
        }

        /// <summary>
        /// 지정된 반복 스케쥴의 분(Minute) 단위에 대한 검사를 수행한다.
        /// </summary>
        /// <param name="formatItem">반복 스케쥴 설정을 나타내는 문자열</param>
        /// <param name="getTime">마지막에 반복한 스케쥴의 시간 (이전 실행 시간)</param>
        /// <param name="currTime">현재 시각</param>
        /// <returns>지정된 시간이 경과했으면 True, 아직 경과시간이 안되었으면 False</returns>
        private static bool ValidateMinute(string formatItem, DateTime getTime, DateTime currTime) {
            if(IsDebugEnabled)
                log.Debug("지정된 반복 스케쥴의 분(Minute) 단위에 대한 검사를 수행한다... formatItem=[{0}], getTime=[{1}], currTime=[{2}]",
                          formatItem, getTime, currTime);

            if(IsUndeterminded(formatItem))
                return true;

            var items = formatItem.Split(StringSplitOptions.RemoveEmptyEntries, PERIOD_ITEM_OR);

            foreach(var item in items) {
                int minute = item.AsInt(0);
                var ts = currTime.Subtract(getTime);

                if(IsDebugEnabled)
                    log.Debug("minute=[{0}], ts=[{1}]", minute, ts);

                // 결과 조건
                // 1. 분단위 유효성 검사에서 현재 시간과 기존수행시간의 차이가 60분을 넘어갔으므로 어떤 경우라도 Expired 가 된 것이다. 
                // 2. 분단위로 이전 실행시간의 분과 현재시각의 분 사이에 있으면 유효
                // 3. 이전 실행시각의 시와 현재시각의 시가 다르고 현재 시간의 분이 아직 지나지 않았으므로 유효
                // 4. 이전 실행시각의 시와 현재시각의 시가 다르고 이전 실행시각의 분이 지정시각의 분보다 이전이면 유효 (한번 무효화 됐으므로)
                //
                if((ts.TotalMinutes >= 60) ||
                   (getTime.Minute < minute && currTime.Minute >= minute) ||
                   (getTime.Hour != currTime.Hour && currTime.Minute >= minute) ||
                   (getTime.Hour != currTime.Hour && getTime.Minute < minute)) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 지정된 반복 스케쥴의 시간(Hour) 단위에 대한 검사를 수행한다.
        /// </summary>
        /// <param name="formatItem">반복 스케쥴 설정을 나타내는 문자열</param>
        /// <param name="getTime">마지막에 반복한 스케쥴의 시간 (이전 실행 시간)</param>
        /// <param name="currTime">현재 시각</param>
        /// <returns>지정된 시간이 경과했으면 True, 아직 경과시간이 안되었으면 False</returns>
        private static bool ValidateHour(string formatItem, DateTime getTime, DateTime currTime) {
            if(IsDebugEnabled)
                log.Debug("지정된 반복 스케쥴의 시간(Hour) 단위에 대한 검사를 수행한다... formatItem=[{0}], getTime=[{1}], currTime=[{2}]",
                          formatItem, getTime, currTime);

            if(IsUndeterminded(formatItem))
                return true;

            var items = formatItem.Split(StringSplitOptions.RemoveEmptyEntries, PERIOD_ITEM_OR);

            foreach(var item in items) {
                var hour = item.AsInt(0);
                var ts = currTime.Subtract(getTime);

                if(IsDebugEnabled)
                    log.Debug("hour=[{0}], ts=[{1}]", hour, ts);

                // 결과 조건
                // 1. Hour 단위 유효성 검사에서 현재 시간과 기존 수행시간의 차이가 24시를 넘어갔으므로 어떤 경우라도 Expired 가 된 것이다. 
                // 2. Hour 단위로 이전 실행시간의 Hour와 현재시각의 Hour 사이에 있으면 유효
                // 3. 이전 실행시각의 Day와 현재시각의 Day가 다르고 현재 시간의 Hour가 아직 지나지 않았으므로 유효
                // 4. 이전 실행시각의 Day와 현재시각의 Day가 다르고 이전 실행시각의 Hour가 지정시각의 Hour보다 이전이면 유효 (한번 무효화 됐으므로)
                //
                if((ts.TotalHours >= 24) ||
                   (ts.TotalHours < 1.0) ||
                   (getTime.Hour < hour && currTime.Hour >= hour) ||
                   (getTime.Day != currTime.Day && currTime.Hour >= hour) ||
                   (getTime.Day != currTime.Day && getTime.Hour < hour))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 지정된 반복 스케쥴의 일(Day) 단위에 대한 검사를 수행한다.
        /// </summary>
        /// <param name="formatItem">반복 스케쥴 설정을 나타내는 문자열</param>
        /// <param name="getTime">마지막에 반복한 스케쥴의 시간 (이전 실행 시간)</param>
        /// <param name="currTime">현재 시각</param>
        /// <returns>지정된 시간이 경과했으면 True, 아직 경과시간이 안되었으면 False</returns>
        private static bool ValidateDayOfMonth(string formatItem, DateTime getTime, DateTime currTime) {
            if(IsDebugEnabled)
                log.Debug("지정된 반복 스케쥴의 일(Day) 단위에 대한 검사를 수행한다... " +
                          @"formatItem=[{0}], getTime=[{1}], currTime=[{2}]",
                          formatItem, getTime, currTime);

            if(IsUndeterminded(formatItem))
                return true;

            var items = formatItem.Split(StringSplitOptions.RemoveEmptyEntries, PERIOD_ITEM_OR);

            foreach(var item in items) {
                var day = item.AsInt(1);
                var ts = currTime.Subtract(getTime);

                if((ts.TotalDays >= 30) ||
                   (ts.TotalDays < 1.0) ||
                   (getTime.Day < day && currTime.Day >= day) ||
                   (getTime.Month != currTime.Month && currTime.Day >= day) ||
                   (getTime.Month != currTime.Month && getTime.Day < day))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 지정된 반복 스케쥴의 월(Month) 단위에 대한 검사를 수행한다.
        /// </summary>
        /// <param name="formatItem">반복 스케쥴 설정을 나타내는 문자열</param>
        /// <param name="getTime">마지막에 반복한 스케쥴의 시간 (이전 실행 시간)</param>
        /// <param name="currTime">현재 시각</param>
        /// <returns>지정된 시간이 경과했으면 True, 아직 경과시간이 안되었으면 False</returns>
        private static bool ValidateMonth(string formatItem, DateTime getTime, DateTime currTime) {
            if(IsDebugEnabled)
                log.Debug("지정된 반복 스케쥴의 월(Month) 단위에 대한 검사를 수행한다... " +
                          @"formatItem=[{0}], getTime=[{1}], currTime=[{2}]",
                          formatItem, getTime, currTime);

            if(IsUndeterminded(formatItem))
                return true;

            var items = formatItem.Split(StringSplitOptions.RemoveEmptyEntries, PERIOD_ITEM_OR);

            foreach(var item in items) {
                var month = item.AsInt(1);
                var ts = currTime.Subtract(getTime);

                if((ts.TotalDays >= 365) ||
                   (ts.TotalDays < 1.0) ||
                   (getTime.Month < month && currTime.Month >= month) ||
                   (getTime.Year != currTime.Year && currTime.Month >= month) ||
                   (getTime.Year != currTime.Year && getTime.Month < month))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 지정된 반복 스케쥴의 주(Week) 단위에 대한 검사를 수행한다.
        /// </summary>
        /// <param name="formatItem">반복 스케쥴 설정을 나타내는 문자열</param>
        /// <param name="getTime">마지막에 반복한 스케쥴의 시간 (이전 실행 시간)</param>
        /// <param name="currTime">현재 시각</param>
        /// <returns>지정된 시간이 경과했으면 True, 아직 경과시간이 안되었으면 False</returns>
        private static bool ValidateDayOfWeek(string formatItem, DateTime getTime, DateTime currTime) {
            if(IsDebugEnabled)
                log.Debug("지정된 반복 스케쥴의 주(Week) 단위에 대한 검사를 수행한다... " +
                          @"formatItem=[{0}], getTime=[{1}], currTime=[{2}]",
                          formatItem, getTime, currTime);

            if(IsUndeterminded(formatItem))
                return true;

            var items = formatItem.Split(StringSplitOptions.RemoveEmptyEntries, PERIOD_ITEM_OR);

            foreach(string item in items) {
                var weekday = item.AsEnum(DayOfWeek.Sunday); // ConvertTool.ConvertEnum(item, DayOfWeek.Sunday);

                var ts = currTime.Subtract(getTime);

                if(ts.TotalDays >= 7)
                    return true;

                var dt = currTime;

                for(var i = 0; getTime < dt; i++) {
                    dt = dt.AddDays(-1);
                    if(dt.DayOfWeek == weekday)
                        return true;
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// 반복 스케쥴 설정에서 '*' 인 경우인지를 검사한다.
        /// </summary>
        /// <param name="formatItem"></param>
        /// <returns>formatItem 값이 '*' 이면 True, 아니면 False</returns>
        private static bool IsUndeterminded(string formatItem) {
            return formatItem.IndexOf(PERIOD_ITEM_ANY) >= 0;
        }
    }
}