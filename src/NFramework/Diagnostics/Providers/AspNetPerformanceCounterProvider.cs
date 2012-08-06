using System;
using System.Diagnostics;

namespace NSoft.NFramework.Diagnostics.Providers {
    /// <summary>
    /// ASP.NET 성능 측정 제공자
    /// </summary>
    [Serializable]
    public class AspNetPerformanceCounterProvider : PerformanceCounterProviderBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public AspNetPerformanceCounterProvider() : base("ASP.NET") {}

        private PerformanceCounter _applicationRestarts;

        /// <summary>
        /// Application Restarts
        /// </summary>
        public virtual PerformanceCounter ApplicationRestarts {
            get { return _applicationRestarts ?? (_applicationRestarts = CreatePerformanceCounter("Application Restarts")); }
        }

        private PerformanceCounter _applicationsRunning;

        /// <summary>
        /// Applications Running
        /// </summary>
        public virtual PerformanceCounter ApplicationsRunning {
            get { return _applicationsRunning ?? (_applicationsRunning = CreatePerformanceCounter("Applications Running")); }
        }

        private PerformanceCounter _requestExecutionTime;

        /// <summary>
        /// Request Execution Time
        /// </summary>
        public virtual PerformanceCounter RequestExecutionTime {
            get { return _requestExecutionTime ?? (_requestExecutionTime = CreatePerformanceCounter("Request Execution Time")); }
        }

        private PerformanceCounter _requestWaitTime;

        /// <summary>
        /// Request Wait Time
        /// </summary>
        public virtual PerformanceCounter RequestWaitTime {
            get { return _requestWaitTime ?? (_requestWaitTime = CreatePerformanceCounter("Request Wait Time")); }
        }

        private PerformanceCounter _requestsCurrent;

        /// <summary>
        /// Requests Current
        /// </summary>
        public virtual PerformanceCounter RequestsCurrent {
            get { return _requestsCurrent ?? (_requestsCurrent = CreatePerformanceCounter("Requests Current")); }
        }

        private PerformanceCounter _requestsDisconnected;

        /// <summary>
        /// Requests Disconnected
        /// </summary>
        public virtual PerformanceCounter RequestsDisconnected {
            get { return _requestsDisconnected ?? (_requestsDisconnected = CreatePerformanceCounter("Requests Disconnected")); }
        }

        private PerformanceCounter _requestsQueued;

        /// <summary>
        /// Requests Queued
        /// </summary>
        public virtual PerformanceCounter RequestsQueued {
            get { return _requestsQueued ?? (_requestsQueued = CreatePerformanceCounter("Requests Queued")); }
        }

        private PerformanceCounter _requestsRejected;

        /// <summary>
        /// Requests Rejected
        /// </summary>
        public virtual PerformanceCounter RequestsRejected {
            get { return _requestsRejected ?? (_requestsRejected = CreatePerformanceCounter("Requests Rejected")); }
        }

        private PerformanceCounter _workerProcessRestarts;

        /// <summary>
        /// Worker Process Restarts
        /// </summary>
        public virtual PerformanceCounter WorkerProcessRestarts {
            get { return _workerProcessRestarts ?? (_workerProcessRestarts = CreatePerformanceCounter("Worker Process Restarts")); }
        }

        private PerformanceCounter _workerProcessesRunning;

        /// <summary>
        /// Worker Processes Running
        /// </summary>
        public virtual PerformanceCounter WorkerProcessesRunning {
            get { return _workerProcessesRunning ?? (_workerProcessesRunning = CreatePerformanceCounter("Worker Processes Running")); }
        }
    }
}