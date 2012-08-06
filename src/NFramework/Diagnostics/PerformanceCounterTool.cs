using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NSoft.NFramework.Diagnostics.Providers;

namespace NSoft.NFramework.Diagnostics {
    /// <summary>
    /// Windows System 성능 측정을 위한 Utility Class 입니다.
    /// </summary>
    public static class PerformanceCounterTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private static readonly object _syncLock = new object();

        // public const string INSTANCENAME_GLOBAL = "_Global_";

        private static IList<PerformanceCounterCategory> _categories;

        /// <summary>
        /// 시스템에서 제공하는 성능 측정을 위한 Category 들
        /// </summary>
        public static IList<PerformanceCounterCategory> Categories {
            get { return _categories ?? (_categories = PerformanceCounterCategory.GetCategories().ToList()); }
        }

        private static SystemPerformanceCounterProvider _system;

        /// <summary>
        /// 시스템 성능 측정기
        /// </summary>
        public static SystemPerformanceCounterProvider System {
            get { return _system ?? (_system = new SystemPerformanceCounterProvider()); }
        }

        private static MemoryPerformanceCounterProvider _memory;

        /// <summary>
        /// 메모리 성능 측정기
        /// </summary>
        public static MemoryPerformanceCounterProvider Memory {
            get { return _memory ?? (_memory = new MemoryPerformanceCounterProvider()); }
        }

        private static ProcessorPerformanceCounterProvider _processor;

        /// <summary>
        /// 프로세스 성능 측정기
        /// </summary>
        public static ProcessorPerformanceCounterProvider Processor {
            get { return _processor ?? (_processor = new ProcessorPerformanceCounterProvider()); }
        }

        /// <summary>
        /// 모든 개별 프로세스 성능 측정기 얻기
        /// </summary>
        /// <returns></returns>
        public static ICollection<ProcessorPerformanceCounterProvider> GetProcessors() {
            var providers = new List<ProcessorPerformanceCounterProvider>();

            for(int i = 0; i < global::System.Environment.ProcessorCount; i++)
                providers.Add(GetProcessor(i.ToString()));

            return providers;
        }

        private static readonly ConcurrentDictionary<string, ProcessorPerformanceCounterProvider> _instanceProcessors =
            new ConcurrentDictionary<string, ProcessorPerformanceCounterProvider>();

        /// <summary>
        /// 지정한 인스턴스 이름의 Processor의 PerformanceCounterProvider를 반환합니다.
        /// </summary>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public static ProcessorPerformanceCounterProvider GetProcessor(string instanceName) {
            return _instanceProcessors.GetOrAdd(instanceName, name => new ProcessorPerformanceCounterProvider(name));

            //lock(_syncLock)
            //{
            //    var provider = _instanceProcessors.GetValueOrDefault(instanceName, null);

            //    if(provider == null)
            //        lock(_instanceProcessors)
            //        {
            //            provider = new ProcessorPerformanceCounterProvider(instanceName);
            //            _instanceProcessors.AddValue(instanceName, provider);
            //        }

            //    return provider;
            //}
        }

        //private static ClrDataPerformanceCounterProvider _clrData;
        //public static ClrDataPerformanceCounterProvider ClrData
        //{
        //    get { return _clrData ?? (_clrData = new ClrDataPerformanceCounterProvider()); }
        //}

        private static ClrLockAndThreadPerformanceCounterProvider _clrLockAndThreads;

        /// <summary>
        /// .NET CLR LocksAndThreads 범주의 PerformanceCounter를 제공합니다.
        /// </summary>
        public static ClrLockAndThreadPerformanceCounterProvider ClrLockAndThreads {
            get { return _clrLockAndThreads ?? (_clrLockAndThreads = new ClrLockAndThreadPerformanceCounterProvider()); }
        }

        private static ClrMemoryPerformanceCounterProvider _clrMemory;

        /// <summary>
        /// .NET CLR Memory Category의 <see cref="PerformanceCounter"/>를 제공합니다.
        /// </summary>
        public static ClrMemoryPerformanceCounterProvider ClrMemory {
            get { return _clrMemory ?? (_clrMemory = new ClrMemoryPerformanceCounterProvider()); }
        }

        private static AspNetPerformanceCounterProvider _aspNet;

        /// <summary>
        /// ASP.NET 성능 측정 제공자
        /// </summary>
        public static AspNetPerformanceCounterProvider AspNet {
            get { return _aspNet ?? (_aspNet = new AspNetPerformanceCounterProvider()); }
        }

        private static AspNetApplicationsPerformanceCounterProvider _aspNetApplications;

        /// <summary>
        /// ASP.NET Applications 에 대한 <see cref="PerformanceCounter"/>를 제공합니다.
        /// </summary>
        public static AspNetApplicationsPerformanceCounterProvider AspNetApplications {
            get { return _aspNetApplications ?? (_aspNetApplications = new AspNetApplicationsPerformanceCounterProvider()); }
        }
    }
}