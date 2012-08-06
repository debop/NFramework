using System;
using NSoft.NFramework.TimePeriods;
using NSoft.NFramework.TimePeriods.TimeRanges;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.FusionCharts.Widgets {
    [TestFixture]
    public class GanttUtilTestFixture : ChartTestFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static readonly DateTime StartDate = new DateTime(2007, 2, 7);
        public static readonly ITimePeriod ProjectPeriod = StartDate.GetRelativeMonthPeriod(60);

        [Test]
        public void GenerateCategories_YearWeek() {
            var period = new CalendarTimeRange(new DateTime(2008, 7, 12), new DateTime(2011, 2, 12), TimeCalendar.NewEmptyOffset());
            var chart = new GanttChart();

            GanttUtil.GenerateCategories(chart.CategoriesList, period, PeriodFlags.YearWeek);
            chart.SetExportInServer();

            if(IsDebugEnabled)
                log.Debug("chart=" + chart.GetDataXml(true));
        }

        [Test]
        public void GenerateCategories_YearWeekDayHour() {
            using(new OperationTimer("Year,Week,Day,Hour")) {
                var chart = new GanttChart();
                GanttUtil.GenerateCategories(chart.CategoriesList, ProjectPeriod, PeriodFlags.YearWeekDayHour);

                if(IsDebugEnabled)
                    log.Debug("chart=" + chart.GetDataXml(true));
            }
        }

        [Test]
        public void GenerateCategories_YearQuarterMonthDayHour() {
            using(new OperationTimer("Year,Quarter,Month,Day,Hour")) {
                var chart = new GanttChart();
                GanttUtil.GenerateCategories(chart.CategoriesList, ProjectPeriod, PeriodFlags.YearQuarterMonthDayHour);


                if(IsDebugEnabled)
                    log.Debug("chart=" + chart.GetDataXml(true));
            }
        }

        [Test]
        public void GenerateCategories_Year_HalfYear_Quarter_Month_Day_Hour() {
            using(new OperationTimer("Year,HalfYear,Quarter,Month,Day,Hour")) {
                var chart = new GanttChart();
                GanttUtil.GenerateCategories(chart.CategoriesList, ProjectPeriod,
                                             PeriodFlags.YearQuarterMonthDayHour | PeriodFlags.HalfYear);

                if(IsDebugEnabled)
                    log.Debug("chart=" + chart.GetDataXml(true));
            }
        }
    }
}