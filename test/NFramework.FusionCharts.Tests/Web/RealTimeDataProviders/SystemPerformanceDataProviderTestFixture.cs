using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.FusionCharts.Web.RealTimeDataProviders {
    [TestFixture]
    public class SystemPerformanceDataProviderTestFixture : ChartTestFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private const string SupportedCategoryNames = @"System, Processor, Memory, .NET CLR Memory, ASP.NET Applications";

        private static bool IsSupportedCategory(string categoryName) {
            if(categoryName.IsWhiteSpace())
                return false;

            if(SupportedCategoryNames.Contains(categoryName))
                return true;

            return categoryName.StartsWith(".NET");
        }

        /// <summary>
        /// 시스템에서 제공하는 PerformanceCounter의 정보를 제공합니다.
        /// </summary>
        [Test]
        public void GetPerformanceCounterAndCounters() {
            foreach(var category in PerformanceCounterCategory.GetCategories().OrderBy(c => c.CategoryName)) {
                try {
                    // 관심있는 것만...
                    if(IsSupportedCategory(category.CategoryName) == false)
                        continue;

                    Console.WriteLine("CategoryName={0}", category.CategoryName);

                    var instanceNames = category.GetInstanceNames().OrderBy(name => name.ToString()).ToArray();

                    if(instanceNames.Length > 0) {
                        int cpuNumber;
                        // instance가 일반 application이 아니라 1. 시스템 전체에 대한 것 2. CPU와 관련된 것만 본다
                        foreach(var instanceName in instanceNames.Where(n => n.StartsWith("_") || int.TryParse(n, out cpuNumber))) {
                            Console.WriteLine("\tInstanceName=" + instanceName);
                            category.GetCounters(instanceName)
                                .OrderBy(c => c.CounterName)
                                .RunEach(counter => Console.WriteLine("\t\tCounter Name={0}", counter.CounterName));
                        }
                    }
                    else {
                        category.GetCounters()
                            .OrderBy(c => c.CounterName)
                            .RunEach(counter => Console.WriteLine("\t\tCounter Name={0}", counter.CounterName));
                    }
                }
                catch(Exception ex) {
                    Console.WriteLine("------------------------------------------------");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("------------------------------------------------");
                }
            }
        }

        [Test]
        public void GetPerformanceCountValues() {
            var counter = GetCounter("Processor", "% Processor Time", "_Total");

            for(var i = 0; i < 100; i++) {
                Console.WriteLine("% Processor Time = {0} %", counter.NextValue());
                Thread.Sleep(50);
            }

            counter = GetCounter("Memory", "Available MBytes");
            Console.WriteLine("Memory Available MBytes = {0} Mb", counter.NextValue());
        }

        private static PerformanceCounter GetCounter(string categoryName, string counterName) {
            try {
                return new PerformanceCounter(categoryName, counterName);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled)
                    log.WarnException("지정한 Performance Counter를 찾지 못했습니다.", ex);
            }
            return null;
        }

        private static PerformanceCounter GetCounter(string categoryName, string counterName, string instanceName) {
            try {
                return new PerformanceCounter(categoryName, counterName, instanceName);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled)
                    log.WarnException("지정한 Performance Counter를 찾지 못했습니다.", ex);
            }
            return null;
        }
    }
}