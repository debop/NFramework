using System;
using System.Diagnostics;

namespace NSoft.NFramework.Diagnostics.Providers {
    /// <summary>
    ///  Memory 관련 <see cref="PerformanceCounter"/>를 제공합니다.
    /// </summary>
    [Serializable]
    public class MemoryPerformanceCounterProvider : PerformanceCounterProviderBase {
        /// <summary>
        /// Constructor
        /// </summary>
        public MemoryPerformanceCounterProvider() : base("Memory") {}

        private PerformanceCounter _committedBytesInUsePercent;

        /// <summary>
        /// 사용중인 메모리 (Bytes)
        /// </summary>
        public PerformanceCounter CommittedBytesInUsePercent {
            get {
                return _committedBytesInUsePercent ??
                       (_committedBytesInUsePercent = CreatePerformanceCounter("% Committed Bytes In Use"));
            }
        }

        private PerformanceCounter _availableBytes;

        /// <summary>
        /// 시스템의 남은 메모리 용량 (Byte 단위)
        /// </summary>
        public PerformanceCounter AvailableBytes {
            get { return _availableBytes ?? (_availableBytes = CreatePerformanceCounter("Available Bytes")); }
        }

        private PerformanceCounter _availableKbytes;

        /// <summary>
        /// 시스템의 남은 메모리 용량 (Kb 단위)
        /// </summary>
        public PerformanceCounter AvailableKbytes {
            get { return _availableKbytes ?? (_availableKbytes = CreatePerformanceCounter("Available Kbytes")); }
        }

        private PerformanceCounter _availableMbytes;

        /// <summary>
        /// 시스템의 남은 메모리 용량 (Mb 단위)
        /// </summary>
        public PerformanceCounter AvailableMbytes {
            get { return _availableMbytes ?? (_availableMbytes = CreatePerformanceCounter("Available Mbytes")); }
        }

        private PerformanceCounter _cacheBytes;

        /// <summary>
        /// Pages per second
        /// </summary>
        public PerformanceCounter CacheBytes {
            get { return _cacheBytes ?? (_cacheBytes = CreatePerformanceCounter("Cache Bytes")); }
        }

        private PerformanceCounter _pagesPerSec;

        /// <summary>
        /// Pages per second
        /// </summary>
        public PerformanceCounter PagesPerSec {
            get { return _pagesPerSec ?? (_pagesPerSec = CreatePerformanceCounter("Pages/sec")); }
        }
    }
}