using System;
using NUnit.Framework;

namespace NSoft.NFramework.Diagnostics {
    [TestFixture]
    public class PerformanceCounterUtilFixture : AbstractFixture {
        [Test]
        public void SystemPerformanceCounterProviderTest() {
            ValidatePerformanceCounterProvider(PerformanceCounterTool.System);
        }

        [Test]
        public void MemoryPerformanceCounterProviderTest() {
            ValidatePerformanceCounterProvider(PerformanceCounterTool.Memory);

            foreach(var provider in PerformanceCounterTool.GetProcessors())
                ValidatePerformanceCounterProvider(provider);
        }

        [Test]
        public void ProcessorPerformanceCounterProviderTest() {
            ValidatePerformanceCounterProvider(PerformanceCounterTool.Processor);
        }

        //[Test]
        //[ExpectedException(typeof(InvalidOperationException), Description = "InstanceName을 알아야 성능측정기를 만들 수 있습니다.")]
        //public void ClrDataPerformanceCounterProviderTest()
        //{
        //    ValidatePerformanceCounterProvider(PerformanceCounterTool.ClrData);
        //}

        [Test]
        public void ClrLockAndThreadsPerformanceCounterProviderTest() {
            ValidatePerformanceCounterProvider(PerformanceCounterTool.ClrLockAndThreads);
        }

        [Test]
        public void ClrMemoryPerformanceCounterProviderTest() {
            ValidatePerformanceCounterProvider(PerformanceCounterTool.ClrMemory);
        }

        [Test]
        public void AspNetPerformanceCounterProviderTest() {
            ValidatePerformanceCounterProvider(PerformanceCounterTool.AspNet);
        }

        [Test]
        public void AspNetApplicationsPerformanceCounterProviderTest() {
            ValidatePerformanceCounterProvider(PerformanceCounterTool.AspNetApplications);
        }

        private static void ValidatePerformanceCounterProvider(IPerformanceCounterProvider provider) {
            Assert.IsNotNull(provider);
            Assert.IsNotEmpty(provider.CategoryName);
            Console.WriteLine("Category name of Provider = " + provider.CategoryName);

            foreach(var perfCounter in provider.PerformanceCounters) {
                Assert.IsNotNull(perfCounter);
                Console.WriteLine("\tPerformanceCounter# Name={0}, NextValue={1}, Help={2}",
                                  perfCounter.CounterName,
                                  perfCounter.NextValue(),
                                  perfCounter.CounterHelp);
            }
        }
    }
}