using System;
using System.Diagnostics;

namespace NSoft.NFramework.Diagnostics.Providers {
    /// <summary>
    /// 시스템 관련 성능 측정을 위한 <see cref="PerformanceCounter"/>를 제공합니다.
    /// </summary>
    [Serializable]
    public class SystemPerformanceCounterProvider : PerformanceCounterProviderBase {
        /// <summary>
        /// Constructor
        /// </summary>
        public SystemPerformanceCounterProvider() : base("System") {}

        private PerformanceCounter _contextSwitchesPerSec;

        /// <summary>
        /// Context Switches/sec
        /// </summary>
        public PerformanceCounter ContextSwitchesPerSec {
            get { return _contextSwitchesPerSec ?? (_contextSwitchesPerSec = CreatePerformanceCounter("Context Switches/sec")); }
        }

        private PerformanceCounter _processes;

        /// <summary>
        /// Processes
        /// </summary>
        public PerformanceCounter Processes {
            get { return _processes ?? (_processes = CreatePerformanceCounter("Processes")); }
        }

        private PerformanceCounter _processorQueueLength;

        /// <summary>
        /// Processor Queue Length
        /// </summary>
        public PerformanceCounter ProcessorQueueLength {
            get { return _processorQueueLength ?? (_processorQueueLength = CreatePerformanceCounter("Processor Queue Length")); }
        }

        private PerformanceCounter _systemCallsPerSec;

        /// <summary>
        /// System Calls/sec
        /// </summary>
        public PerformanceCounter SystemCallsPerSec {
            get { return _systemCallsPerSec ?? (_systemCallsPerSec = CreatePerformanceCounter("System Calls/sec")); }
        }

        private PerformanceCounter _threads;

        /// <summary>
        /// Threads
        /// </summary>
        public PerformanceCounter Threads {
            get { return _threads ?? (_threads = CreatePerformanceCounter("Threads")); }
        }
    }
}