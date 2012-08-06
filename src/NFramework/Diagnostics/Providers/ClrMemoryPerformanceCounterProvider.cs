using System;
using System.Diagnostics;

namespace NSoft.NFramework.Diagnostics.Providers {
    /// <summary>
    /// .NET CLR Memory Category의 <see cref="PerformanceCounter"/>를 제공합니다.
    /// </summary>
    [Serializable]
    public class ClrMemoryPerformanceCounterProvider : InstancePerformanceCounterProviderBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public ClrMemoryPerformanceCounterProvider() : base(".NET CLR Memory", "_Global_") {}

        private PerformanceCounter _bytesInAllHeaps;

        /// <summary>
        /// # Bytes in all Heaps
        /// </summary>
        public virtual PerformanceCounter BytesInAllHeaps {
            get { return _bytesInAllHeaps ?? (_bytesInAllHeaps = CreatePerformanceCounter("# Bytes in all Heaps")); }
        }

        private PerformanceCounter _gcHandles;

        /// <summary>
        /// # GC Handles
        /// </summary>
        public virtual PerformanceCounter GCHandles {
            get { return _gcHandles ?? (_gcHandles = CreatePerformanceCounter("# GC Handles")); }
        }

        private PerformanceCounter _gen0Collections;

        /// <summary>
        /// # Gen 0 Collections
        /// </summary>
        public virtual PerformanceCounter Gen0Collections {
            get { return _gen0Collections ?? (_gen0Collections = CreatePerformanceCounter("# Gen 0 Collections")); }
        }

        private PerformanceCounter _gen1Collections;

        /// <summary>
        /// # Gen 1 Collections
        /// </summary>
        public virtual PerformanceCounter Gen1Collections {
            get { return _gen1Collections ?? (_gen1Collections = CreatePerformanceCounter("# Gen 1 Collections")); }
        }

        private PerformanceCounter _gen2Collections;

        /// <summary>
        /// # Gen 2 Collections
        /// </summary>
        public virtual PerformanceCounter Gen2Collections {
            get { return _gen2Collections ?? (_gen2Collections = CreatePerformanceCounter("# Gen 2 Collections")); }
        }

        private PerformanceCounter _totalCommittedBytes;

        /// <summary>
        /// # Total committed Bytes
        /// </summary>
        public virtual PerformanceCounter TotalCommittedBytes {
            get { return _totalCommittedBytes ?? (_totalCommittedBytes = CreatePerformanceCounter("# Total committed Bytes")); }
        }

        private PerformanceCounter _totalReservedBytes;

        /// <summary>
        /// # Total reserved Bytes
        /// </summary>
        public virtual PerformanceCounter TotalReservedBytes {
            get { return _totalReservedBytes ?? (_totalReservedBytes = CreatePerformanceCounter("# Total reserved Bytes")); }
        }

        private PerformanceCounter _timeInGCPercent;

        /// <summary>
        /// % Time in GC
        /// </summary>
        public virtual PerformanceCounter TimeInGCPercent {
            get { return _timeInGCPercent ?? (_timeInGCPercent = CreatePerformanceCounter("% Time in GC")); }
        }

        private PerformanceCounter _allocatedBytesPerSec;

        /// <summary>
        /// Allocated Bytes/sec
        /// </summary>
        public virtual PerformanceCounter AllocatedBytesPerSec {
            get { return _allocatedBytesPerSec ?? (_allocatedBytesPerSec = CreatePerformanceCounter("Allocated Bytes/sec")); }
        }

        private PerformanceCounter _largeObjectHeapSize;

        /// <summary>
        /// Large Object Heap size
        /// </summary>
        public virtual PerformanceCounter LargeObjectHeapSize {
            get { return _largeObjectHeapSize ?? (_largeObjectHeapSize = CreatePerformanceCounter("Large Object Heap size")); }
        }
    }
}