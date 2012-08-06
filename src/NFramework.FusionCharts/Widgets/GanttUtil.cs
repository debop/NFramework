using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.TimePeriods;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Gantt Chart 를 생성하기 위한 Utility class입니다.
    /// </summary>
    public static class GanttUtil {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 기간 단위에 따라 Category를 생성합니다.
        /// </summary>
        /// <param name="categoryCollectionList">생성된 <see cref="CategoriesElement"/> 정보가 담길 객체</param>
        /// <param name="timePeriod">Gantt에 표현할 전체 기간 (프로젝트 전체 기간)</param>
        /// <param name="periodFlags">Gantt Chart X축에 나타낼 기간 단위 정보</param>
        public static void GenerateCategories(IList<CategoriesElement> categoryCollectionList,
                                              ITimePeriod timePeriod,
                                              PeriodFlags periodFlags) {
            categoryCollectionList.ShouldNotBeNull("categoryCollectionList");
            timePeriod.ShouldNotBeNull("periodRange");
            Guard.Assert(timePeriod.HasPeriod, "Gantt에 나타낼 전체 기간은 시작과 끝이 있어야합니다.");

            if(IsDebugEnabled)
                log.Debug("Gantt의 기간 부분을 생성합니다. timePeriod=[{0}], periodFlags=[{1}]", timePeriod, periodFlags);

            categoryCollectionList.Clear();

            if((periodFlags & PeriodFlags.Year) > 0)
                categoryCollectionList.Add(CreateCategories(timePeriod.ForEachYears(),
                                                            range => range.Start.Year.ToString()));

            if((periodFlags & PeriodFlags.HalfYear) > 0)
                categoryCollectionList.Add(CreateCategories(timePeriod.ForEachYears(),
                                                            range => (range.End.HalfyearOf() == HalfyearKind.First) ? "1st" : "2nd"));

            if((periodFlags & PeriodFlags.Quarter) > 0)
                categoryCollectionList.Add(CreateCategories(timePeriod.ForEachQuarters(),
                                                            range => "Q" + range.End.QuarterOf().GetHashCode().ToString()));

            if((periodFlags & PeriodFlags.Month) > 0)
                categoryCollectionList.Add(CreateCategories(timePeriod.ForEachMonths(),
                                                            range => range.End.GetMonthName()));

            if((periodFlags & PeriodFlags.Week) > 0)
                categoryCollectionList.Add(CreateCategories(timePeriod.ForEachWeeks(),
                                                            range => "W" + range.End.GetYearAndWeek().Week.Value.ToString()));

            if((periodFlags & PeriodFlags.Day) > 0)
                categoryCollectionList.Add(CreateCategoriesAsParallel(timePeriod.ForEachDays(), range => range.End.Day.ToString()));

            if((periodFlags & PeriodFlags.Hour) > 0)
                categoryCollectionList.Add(CreateCategoriesAsParallel(timePeriod.ForEachHours(),
                                                                      range => "H" + range.End.Hour.ToString()));
        }

        /// <summary>
        /// 지정한 기간의 컬렉션에 대해 라벨 발급 함수를 통해 새로운 <see cref="CategoryElement"/> 들을 생성하여, 컬렉션으로 반환한다.
        /// </summary>
        /// <param name="periods"></param>
        /// <param name="labelFunc"></param>
        /// <returns></returns>
        public static CategoriesElement CreateCategories(IEnumerable<ITimePeriod> periods, Func<ITimePeriod, string> labelFunc) {
            var categories = new CategoriesElement();

            foreach(var period in periods) {
                var category = new CategoryElement
                               {
                                   Start = period.Start,
                                   End = period.End
                               };

                if(labelFunc != null)
                    category.ItemAttr.Label = labelFunc(period);

                categories.Add(category);
            }

            return categories;
        }

        /// <summary>
        /// 지정한 기간의 컬렉션에 대해 라벨 발급 함수를 통해 새로운 <see cref="CategoryElement"/> 들을 생성하여, 컬렉션으로 반환한다.
        /// </summary>
        /// <param name="periods"></param>
        /// <param name="labelFunc"></param>
        /// <returns></returns>
        public static CategoriesElement CreateCategoriesAsParallel(IEnumerable<ITimePeriod> periods, Func<ITimePeriod, string> labelFunc) {
            var categories = new CategoriesElement();

            periods
                .AsParallel()
                .AsOrdered()
                .Select(period => {
                            var category = new CategoryElement
                                           {
                                               Start = period.Start,
                                               End = period.End
                                           };

                            if(labelFunc != null)
                                category.ItemAttr.Label = labelFunc(period);

                            return category;
                        })
                .RunEach(category => categories.Add(category));

            return categories;
        }
    }
}