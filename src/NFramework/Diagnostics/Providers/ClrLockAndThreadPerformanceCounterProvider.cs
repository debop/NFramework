using System;
using System.Diagnostics;

namespace NSoft.NFramework.Diagnostics.Providers {
    /// <summary>
    /// .NET CLR LocksAndThreads 범주의 PerformanceCounter를 제공합니다.
    /// </summary>
    /// <example>
    /// <code>
    /// CategoryName=.NET CLR LocksAndThreads
    /// 	InstanceName=_Global_
    /// 		Counter Name=# of current logical Threads
    /// 		Counter Name=# of current physical Threads
    /// 		Counter Name=# of current recognized threads
    /// 		Counter Name=# of total recognized threads
    /// 		Counter Name=Contention Rate / sec
    /// 		Counter Name=Current Queue Length
    /// 		Counter Name=Queue Length / sec
    /// 		Counter Name=Queue Length Peak
    /// 		Counter Name=rate of recognized threads / sec
    /// 		Counter Name=Total # of Contentions
    /// </code>
    /// </example>
    [Serializable]
    public class ClrLockAndThreadPerformanceCounterProvider : InstancePerformanceCounterProviderBase {
        /// <summary>
        /// Constructor
        /// </summary>
        public ClrLockAndThreadPerformanceCounterProvider() : base(".NET CLR LocksAndThreads") {}

        //protected override void RegistPerformanceCounter(ICollection<PerformanceCounter> perfCounters)
        //{
        //    perfCounters.Add(CounterOfCurrentLogicalThreads);
        //    perfCounters.Add(CounterOfCurrentPhysicalThreads);
        //    perfCounters.Add(CountOfCurrentRecognizedThreads);
        //    perfCounters.Add(ContentionRatePerSec);
        //    perfCounters.Add(CurrentQueueLength);
        //    perfCounters.Add(QueueLengthPerSec);
        //    perfCounters.Add(QueueLengthPeak);
        //    perfCounters.Add(TotalCountOfContentions);
        //}

        private PerformanceCounter _countOfCurrentLogicalThreads;

        /// <summary>
        /// # of current logical Threads
        /// </summary>
        public virtual PerformanceCounter CounterOfCurrentLogicalThreads {
            get {
                return _countOfCurrentLogicalThreads ??
                       (_countOfCurrentLogicalThreads = CreatePerformanceCounter("# of current logical Threads"));
            }
        }

        private PerformanceCounter _countOfCurrentPhysicalThreads;

        /// <summary>
        /// # of current physical Threads
        /// </summary>
        public virtual PerformanceCounter CounterOfCurrentPhysicalThreads {
            get {
                return _countOfCurrentPhysicalThreads ??
                       (_countOfCurrentPhysicalThreads = CreatePerformanceCounter("# of current physical Threads"));
            }
        }

        private PerformanceCounter _countOfCurrentRecognizedThreads;

        /// <summary>
        /// # of current recognized Threads
        /// </summary>
        public virtual PerformanceCounter CountOfCurrentRecognizedThreads {
            get {
                return _countOfCurrentRecognizedThreads ??
                       (_countOfCurrentRecognizedThreads = CreatePerformanceCounter("# of current recognized Threads"));
            }
        }

        private PerformanceCounter _contentationRatePerSec;

        /// <summary>
        /// Contention Rate / sec
        /// </summary>
        public virtual PerformanceCounter ContentionRatePerSec {
            get { return _contentationRatePerSec ?? (_contentationRatePerSec = CreatePerformanceCounter("Contention Rate / sec")); }
        }

        private PerformanceCounter _currentQueueLength;

        /// <summary>
        /// Current Queue Length
        /// </summary>
        public virtual PerformanceCounter CurrentQueueLength {
            get { return _currentQueueLength ?? (_currentQueueLength = CreatePerformanceCounter("Current Queue Length")); }
        }

        private PerformanceCounter _queueLengthPerSec;

        /// <summary>
        /// Queue Length / sec
        /// </summary>
        public virtual PerformanceCounter QueueLengthPerSec {
            get { return _queueLengthPerSec ?? (_queueLengthPerSec = CreatePerformanceCounter("Queue Length / sec")); }
        }

        private PerformanceCounter _queueLengthPeak;

        /// <summary>
        /// Queue Length Peak
        /// </summary>
        public virtual PerformanceCounter QueueLengthPeak {
            get { return _queueLengthPeak ?? (_queueLengthPeak = CreatePerformanceCounter("Queue Length Peak")); }
        }

        private PerformanceCounter _totalCountOfContentions;

        /// <summary>
        /// Total # of Contentions
        /// </summary>
        public virtual PerformanceCounter TotalCountOfContentions {
            get { return _totalCountOfContentions ?? (_totalCountOfContentions = CreatePerformanceCounter("Total # of Contentions")); }
        }
    }
}