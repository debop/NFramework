using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.TimePeriods {
    public static partial class TimeTool {
        /// <summary>
        /// 기간을 년(Year) 단위로 열거자를 제공한다.
        /// </summary>
        public static IEnumerable<ITimePeriod> ForEachYears(this ITimePeriod period) {
            if(IsDebugEnabled)
                log.Debug("기간[{0}]에 대해 HalfYear 단위로 열거합니다...", period);

            if(period.IsAnytime)
                yield break;

            AssertHasPeriod(period);

            if(period.Start.Year == period.End.Year) {
                yield return new TimeRange(period.Start, period.End);
                yield break;
            }

            yield return new TimeRange(period.Start, period.Start.EndTimeOfYear());

            var current = period.Start.StartTimeOfYear().AddYears(1);

            while(current.Year < period.End.Year) {
                yield return GetYearRange(current);
                current = current.AddYears(1);
            }

            if(current < period.End)
                yield return new TimeRange(current.StartTimeOfYear(), period.End);
        }

        /// <summary>
        /// 기간을 반기별로 열거자를 제공한다.
        /// </summary>
        public static IEnumerable<ITimePeriod> ForEachHalfYears(this ITimePeriod period) {
            if(IsDebugEnabled)
                log.Debug("기간[{0}]에 대해 HalfYear 단위로 열거합니다...", period);

            if(period.IsAnytime)
                yield break;

            AssertHasPeriod(period);

            if(period.Start.Year == period.End.Year && period.Start.HalfyearOf() == period.End.HalfyearOf()) {
                yield return new TimeRange(period.Start, period.End);
                yield break;
            }

            var current = period.Start.EndTimeOfHalfyear();
            yield return new TimeRange(period.Start, current);

            var endHashCode = period.End.Year * 10 + period.End.HalfyearOf();
            current = current.AddDays(1);
            while(current.Year * 10 + current.HalfyearOf() < endHashCode) {
                yield return GetHalfyearRange(current);
                //yield return new TimeRange(current, current.EndOfHalfyear());
                current = current.AddMonths(TimeSpec.MonthsPerHalfyear);
            }

            if(current < period.End)
                yield return new TimeRange(current.StartTimeOfHalfyear(), period.End);
        }

        /// <summary>
        /// 기간을 분기단위로 열거한다.
        /// </summary>
        public static IEnumerable<ITimePeriod> ForEachQuarters(this ITimePeriod period) {
            if(IsDebugEnabled)
                log.Debug("기간[{0}]에 대해 Quarter 단위로 열거합니다...", period);

            if(period.IsAnytime)
                yield break;

            AssertHasPeriod(period);

            if(period.Start.Year == period.End.Year && period.Start.GetQuarter() == period.End.GetQuarter()) {
                yield return new TimeRange(period.Start, period.End);
                yield break;
            }

            var current = EndTimeOfQuarter(period.Start.Year, period.Start.QuarterOf());
            yield return new TimeRange(period.Start, current);

            var endHashCode = period.End.Year * 10 + period.End.GetQuarter().GetHashCode();

            current = current.AddDays(1);
            while(current.Year * 10 + current.GetQuarter().GetHashCode() < endHashCode) {
                yield return GetQuarterRange(current);
                //yield return new TimeRange(current.Quarter().StartOfQuarter(current.Year),
                //                           current.Quarter().EndOfQuarter(current.Year));

                current = current.AddMonths(TimeSpec.MonthsPerQuarter);
            }

            if(current < period.End)
                yield return new TimeRange(StartTimeOfQuarter(current.Year, current.QuarterOf()), period.End);
        }

        /// <summary>
        /// 기간을 월 단위로 열거자를 제공한다.
        /// </summary>
        public static IEnumerable<ITimePeriod> ForEachMonths(this ITimePeriod period) {
            if(IsDebugEnabled)
                log.Debug("기간[{0}]에 대해 Month 단위로 열거합니다...", period);

            if(period.IsAnytime)
                yield break;

            AssertHasPeriod(period);

            var monthEnd = period.End.EndTimeOfMonth();

            if(monthEnd == period.Start.EndTimeOfMonth())
                yield return new TimeRange(period.Start, period.End);

            else if(period.Start.EndTimeOfMonth() < monthEnd)
                yield return new TimeRange(period.Start, period.Start.EndTimeOfMonth());

            var current = period.Start.AddMonths(1);
            while(current.EndTimeOfMonth() < period.End) {
                yield return GetMonthRange(current);
                current = current.AddMonths(1);
            }

            if(current.StartTimeOfMonth() <= period.End)
                yield return new TimeRange(current.StartTimeOfMonth(), period.End);
        }

        /// <summary>
        /// 기간을 주 단위로 열거자를 제공한다.
        /// </summary>
        public static IEnumerable<ITimePeriod> ForEachWeeks(this ITimePeriod period) {
            if(IsDebugEnabled)
                log.Debug("기간[{0}]에 대해 Week 단위로 열거합니다...", period);

            if(period.IsAnytime)
                yield break;

            AssertHasPeriod(period);

            var current = period.Start;
            var weekEnd = current.EndTimeOfWeek();
            yield return new TimeRange(period.Start, weekEnd);

            current = weekEnd.Date.AddDays(1);
            while(current.EndTimeOfWeek() < period.End) {
                yield return GetWeekRange(current);
                current = current.AddDays(TimeSpec.DaysPerWeek);
            }

            if(current < period.End)
                yield return new TimeRange(current.StartTimeOfWeek(), period.End);
        }

        /// <summary>
        /// 기간을 날짜 단위로 열거합니다.
        /// </summary>
        public static IEnumerable<ITimePeriod> ForEachDays(this ITimePeriod period) {
            if(IsDebugEnabled)
                log.Debug("기간[{0}]에 대해 Day 단위로 열거합니다...", period);

            if(period.IsAnytime)
                yield break;

            AssertHasPeriod(period);

            // if(period.Start.TimeOfDay > TimeSpan.Zero)
            yield return new TimeRange(period.Start, period.Start.Date.AddDays(1));

            var endDate = period.End;
            var current = period.Start.Date.AddDays(1);
            while(current < endDate) {
                yield return GetDayRange(current);
                current = current.AddDays(1);
            }

            if(endDate.TimeOfDay > TimeSpan.Zero)
                yield return new TimeRange(endDate.Date, endDate);
        }

        /// <summary>
        /// 지정된 기간을 시간 단위로 열거합니다.
        /// </summary>
        public static IEnumerable<ITimePeriod> ForEachHours(this ITimePeriod period) {
            if(IsDebugEnabled)
                log.Debug("기간[{0}]에 대해 Hour 단위로 열거합니다...", period);

            if(period.IsAnytime)
                yield break;

            AssertHasPeriod(period);

            var current = period.Start.TrimToHour(period.Start.Hour + 1);
            yield return new TimeRange(period.Start, current);

            var endDate = period.End;

            while(current < endDate) {
                yield return GetHourRange(current); //GetPeriodOf(current, PeriodKind.Hour);
                current = current.AddHours(1);
            }

            if(endDate.AddHours(-endDate.Hour).TimeOfDay > TimeSpan.Zero)
                yield return new TimeRange(current.AddHours(-1), endDate);
        }

        /// <summary>
        /// 지정된 기간을 분(Minute) 단위로 열거합니다.
        /// </summary>
        public static IEnumerable<ITimePeriod> ForEachMinutes(this ITimePeriod period) {
            if(IsDebugEnabled)
                log.Debug("기간[{0}]에 대해 분(Minute) 단위로 열거합니다...", period);

            if(period.IsAnytime)
                yield break;

            AssertHasPeriod(period);

            var current = period.Start.TrimToMinute(period.Start.Minute + 1);
            yield return new TimeRange(period.Start, current);

            var endDate = period.End;

            while(current < endDate) {
                yield return GetMinuteRange(current);
                current = current.AddMinutes(1);
            }

            if(endDate.AddMinutes(-endDate.Minute).TimeOfDay > TimeSpan.Zero)
                yield return new TimeRange(current.AddMinutes(-1), endDate);
        }

        /// <summary>
        /// <paramref name="period"/>를 <paramref name="periodKind"/> 단위로 열거합니다.
        /// </summary>
        /// <param name="period">전체 기간</param>
        /// <param name="periodKind">열거할 기간의 단위</param>
        /// <returns>열거할 기간 단위를 가지는 기간의 컬렉션</returns>
        public static IEnumerable<ITimePeriod> ForEachPeriods(this ITimePeriod period, PeriodKind periodKind) {
            switch(periodKind) {
                case PeriodKind.Year:
                    return period.ForEachYears();

                case PeriodKind.Halfyear:
                    return period.ForEachHalfYears();

                case PeriodKind.Quarter:
                    return period.ForEachQuarters();

                case PeriodKind.Month:
                    return period.ForEachMonths();

                case PeriodKind.Week:
                    return period.ForEachWeeks();

                case PeriodKind.Day:
                    return period.ForEachDays();

                case PeriodKind.Hour:
                    return period.ForEachHours();

                case PeriodKind.Minute:
                    return period.ForEachMinutes();

                default:
                    throw new InvalidOperationException("지원하지 않는 PeriodKind입니다. PeriodKind=" + periodKind);
            }
        }

        /// <summary>
        /// 지정된 기간(<paramref name="period"/>)을 <paramref name="periodKind"/> 단위로 열거하면서, <paramref name="runner"/>을 실행합니다.
        /// </summary>
        /// <typeparam name="T">실행한 결과 값의 수형</typeparam>
        /// <param name="period">전체 기간</param>
        /// <param name="periodKind">열거할 기간의 단위</param>
        /// <param name="runner">각 단위 기간별 실행할 델리게이트</param>
        /// <returns>각 단위 기간별 실행 결과</returns>
        /// <example>
        /// <code>
        ///     var calendar = CultureInfo.CurrentCulture.Calendar;
        ///		var results = RunEach(new YearRange(DateTime.Now), PeriodKind.Day, (day)=>calendar.GetDaysOfYear(day.Start));
        /// </code>
        /// </example>
        public static IEnumerable<T> RunPeriod<T>(this ITimePeriod period, PeriodKind periodKind, Func<ITimePeriod, T> runner) {
            period.ShouldNotBeNull("period");
            runner.ShouldNotBeNull("runner");
            Guard.Assert(period.HasPeriod, "period는 기간을 가져야 합니다. period=" + period);

            if(IsDebugEnabled)
                log.Debug("기간[{0}] 을 [{1}] 단위로 열거하고, 함수를 실행합니다.", period, periodKind);

            return ForEachPeriods(period, periodKind).Select(p => runner(p));
        }

        /// <summary>
        /// 지정된 기간(<paramref name="period"/>)을 <paramref name="periodKind"/> 단위로 열거하면서, 병렬로 <paramref name="runner"/>을 실행합니다.
        /// </summary>
        /// <typeparam name="T">실행한 결과 값의 수형</typeparam>
        /// <param name="period">전체 기간</param>
        /// <param name="periodKind">열거할 기간의 단위</param>
        /// <param name="runner">각 단위 기간별 실행할 델리게이트</param>
        /// <returns>각 단위 기간별 실행 결과</returns>
        /// <example>
        /// <code>
        ///     var calendar = CultureInfo.CurrentCulture.Calendar;
        ///		var results = RunEachAsParallel(new YearRange(DateTime.Now), PeriodKind.Day, (day)=>calendar.GetDaysOfYear(day.Start));
        /// </code>
        /// </example>
        public static IEnumerable<T> RunPeriodAsParallel<T>(this ITimePeriod period, PeriodKind periodKind, Func<ITimePeriod, T> runner) {
            period.ShouldNotBeNull("period");
            runner.ShouldNotBeNull("runner");
            AssertHasPeriod(period);

            if(IsDebugEnabled)
                log.Debug("기간[{0}] 을 [{1}] 단위로 열거하고, 병렬로 함수를 실행합니다.", period, periodKind);

            return
                ForEachPeriods(period, periodKind)
#if !SILVERLIGHT
                    .AsParallel()
                    .AsOrdered()
#endif
                    .Select(p => runner(p));
        }

        /// <summary>
        /// 지정된 기간(<paramref name="period"/>)을 <paramref name="periodKind"/> 단위로 열거하면서, <paramref name="runner"/>을 비동기 방식으로 실행합니다.
        /// </summary>
        /// <typeparam name="T">실행한 결과 값의 수형</typeparam>
        /// <param name="period">전체 기간</param>
        /// <param name="periodKind">열거할 기간의 단위</param>
        /// <param name="runner">각 단위 기간별 실행할 델리게이트</param>
        /// <returns>각 단위 기간별 실행 결과</returns>
        /// <example>
        /// <code>
        ///     var calendar = CultureInfo.CurrentCulture.Calendar;
        ///		var results = RunEachAsync(new YearRange(DateTime.Now), PeriodKind.Day, (day)=>calendar.GetDaysOfYear(day.Start));
        /// </code>
        /// </example>
        /// <seealso cref="EnumerableTool.RunEachAsync{T,TResult}"/>
        public static IEnumerable<T> RunPeriodAsync<T>(this ITimePeriod period, PeriodKind periodKind, Func<ITimePeriod, T> runner) {
            period.ShouldNotBeNull("period");
            runner.ShouldNotBeNull("runner");
            AssertHasPeriod(period);

            if(IsDebugEnabled)
                log.Debug("기간[{0}] 을 [{1}] 단위로 열거하고, 비동기 방식으로 함수를 실행합니다.", period, periodKind);


            return ForEachPeriods(period, periodKind).RunEachAsync(runner);
        }

        private static void AssertHasPeriod(ITimePeriod period) {
            Guard.Assert(period.HasPeriod, "한정된 기간이 없으므로, 열거자를 생성하지 못합니다. period=[{0}]", period);
        }
    }
}