using System;
using System.Diagnostics;

namespace NSoft.NFramework.Diagnostics.Providers {
    /// <summary>
    /// ASP.NET Applications 에 대한 <see cref="PerformanceCounter"/>를 제공합니다.
    /// </summary>
    /*
			Anonymous Requests
			Anonymous Requests/Sec
			Application Lifetime Events
			Application Lifetime Events/Sec
			Audit Failure Events Raised
			Audit Success Events Raised
			Cache % Machine Memory Limit Used
			Cache % Machine Memory Limit Used Base
			Cache % Process Memory Limit Used
			Cache % Process Memory Limit Used Base
			Cache API Entries
			Cache API Hit Ratio
			Cache API Hit Ratio Base
			Cache API Hits
			Cache API Misses
			Cache API Trims
			Cache API Turnover Rate
			Cache Total Entries
			Cache Total Hit Ratio
			Cache Total Hit Ratio Base
			Cache Total Hits
			Cache Total Misses
			Cache Total Trims
			Cache Total Turnover Rate
			Compilations Total
			Debugging Requests
			Error Events Raised
			Error Events Raised/Sec
			Errors During Compilation
			Errors During Execution
			Errors During Preprocessing
			Errors Total
			Errors Total/Sec
			Errors Unhandled During Execution
			Errors Unhandled During Execution/Sec
			Events Raised
			Events Raised/Sec
			Forms Authentication Failure
			Forms Authentication Success
			Infrastructure Error Events Raised
			Infrastructure Error Events Raised/Sec
			Membership Authentication Failure
			Membership Authentication Success
			Output Cache Entries
			Output Cache Hit Ratio
			Output Cache Hit Ratio Base
			Output Cache Hits
			Output Cache Misses
			Output Cache Trims
			Output Cache Turnover Rate
			Pipeline Instance Count
			Request Bytes In Total
			Request Bytes Out Total
			Request Error Events Raised
			Request Error Events Raised/Sec
			Request Events Raised
			Request Events Raised/Sec
			Request Execution Time
			Request Wait Time
			Requests Disconnected
			Requests Executing
			Requests Failed
			Requests In Application Queue
			Requests Not Authorized
			Requests Not Found
			Requests Rejected
			Requests Succeeded
			Requests Timed Out
			Requests Total
			Requests/Sec
			Session SQL Server connections total
			Session State Server connections total
			Sessions Abandoned
			Sessions Active
			Sessions Timed Out
			Sessions Total
			Transactions Aborted
			Transactions Committed
			Transactions Pending
			Transactions Total
			Transactions/Sec
			Viewstate MAC Validation Failure
	 */
    [Serializable]
    public class AspNetApplicationsPerformanceCounterProvider : InstancePerformanceCounterProviderBase {
        /// <summary>
        /// Constructor
        /// </summary>
        public AspNetApplicationsPerformanceCounterProvider() : base("ASP.NET Applications", "__Total__") {}

        private PerformanceCounter _applicationLifetimeEvents;

        /// <summary>
        /// Application Lifetime Events
        /// </summary>
        public virtual PerformanceCounter ApplicationLifetimeEvents {
            get {
                return _applicationLifetimeEvents ??
                       (_applicationLifetimeEvents = CreatePerformanceCounter("Application Lifetime Events"));
            }
        }

        private PerformanceCounter _applicationLifetimeEventsPerSec;

        /// <summary>
        /// Application Lifetime Events
        /// </summary>
        public virtual PerformanceCounter ApplicationLifetimeEventsPerSec {
            get {
                return _applicationLifetimeEventsPerSec ??
                       (_applicationLifetimeEventsPerSec = CreatePerformanceCounter("Application Lifetime Events/Sec"));
            }
        }

        private PerformanceCounter _auditFailureEventsRaised;

        /// <summary>
        /// Audit Failure Events Raised
        /// </summary>
        public virtual PerformanceCounter AuditFailureEventsRaised {
            get {
                return _auditFailureEventsRaised ??
                       (_auditFailureEventsRaised = CreatePerformanceCounter("Audit Failure Events Raised"));
            }
        }

        private PerformanceCounter _auditSuccessEventsRaised;

        /// <summary>
        /// Audit Success Events Raised
        /// </summary>
        public virtual PerformanceCounter AuditSuccessEventsRaised {
            get {
                return _auditSuccessEventsRaised ??
                       (_auditSuccessEventsRaised = CreatePerformanceCounter("Audit Success Events Raised"));
            }
        }

        private PerformanceCounter _cachePercentOfMachineMemoryLimitUsed;

        /// <summary>
        /// Cache % Machine Memory Limit Used
        /// </summary>
        public virtual PerformanceCounter CachePercentOfMachineMemoryLimitUsed {
            get {
                return _cachePercentOfMachineMemoryLimitUsed ??
                       (_cachePercentOfMachineMemoryLimitUsed = CreatePerformanceCounter("Cache % Machine Memory Limit Used"));
            }
        }

        private PerformanceCounter _cachePercentOfProcessMemoryLimitUsed;

        /// <summary>
        /// Cache % Process Memory Limit Used
        /// </summary>
        public virtual PerformanceCounter CachePercentOfProcessMemoryLimitUsed {
            get {
                return _cachePercentOfProcessMemoryLimitUsed ??
                       (_cachePercentOfProcessMemoryLimitUsed = CreatePerformanceCounter("Cache % Process Memory Limit Used"));
            }
        }

        private PerformanceCounter _compilationsTotal;

        /// <summary>
        /// Compilations Total
        /// </summary>
        public virtual PerformanceCounter CompilationsTotal {
            get { return _compilationsTotal ?? (_compilationsTotal = CreatePerformanceCounter("Compilations Total")); }
        }

        private PerformanceCounter _debuggingRequests;

        /// <summary>
        /// Debugging Requests
        /// </summary>
        public virtual PerformanceCounter DebuggingRequests {
            get { return _debuggingRequests ?? (_debuggingRequests = CreatePerformanceCounter("Debugging Requests")); }
        }

        private PerformanceCounter _errorEventRaised;

        /// <summary>
        /// Error Events Raised
        /// </summary>
        public virtual PerformanceCounter ErrorEventRaised {
            get { return _errorEventRaised ?? (_errorEventRaised = CreatePerformanceCounter("Error Events Raised")); }
        }

        private PerformanceCounter _errorEventRaisedPerSec;

        /// <summary>
        /// Error Events Raised/Sec
        /// </summary>
        public virtual PerformanceCounter ErrorEventRaisedPerSec {
            get { return _errorEventRaisedPerSec ?? (_errorEventRaisedPerSec = CreatePerformanceCounter("Error Events Raised/Sec")); }
        }

        private PerformanceCounter _errorsDuringCompilation;

        /// <summary>
        /// Errors During Compilation
        /// </summary>
        public virtual PerformanceCounter ErrorsDuringCompilation {
            get {
                return _errorsDuringCompilation ??
                       (_errorsDuringCompilation = CreatePerformanceCounter("Errors During Compilation"));
            }
        }

        private PerformanceCounter _errorsDuringExecution;

        /// <summary>
        /// Errors During Execution
        /// </summary>
        public virtual PerformanceCounter ErrorsDuringExecution {
            get { return _errorsDuringExecution ?? (_errorsDuringExecution = CreatePerformanceCounter("Errors During Execution")); }
        }

        private PerformanceCounter _errorsDuringPreProcessing;

        /// <summary>
        /// Errors During Preprocessing
        /// </summary>
        public virtual PerformanceCounter ErrorsDuringPreProcessing {
            get {
                return _errorsDuringPreProcessing ??
                       (_errorsDuringPreProcessing = CreatePerformanceCounter("Errors During Preprocessing"));
            }
        }

        private PerformanceCounter _errorsTotal;

        /// <summary>
        /// Errors Total
        /// </summary>
        public virtual PerformanceCounter ErrorsTotal {
            get { return _errorsTotal ?? (_errorsTotal = CreatePerformanceCounter("Errors Total")); }
        }

        private PerformanceCounter _errorsTotalPerSec;

        /// <summary>
        /// Errors Total
        /// </summary>
        public virtual PerformanceCounter ErrorsTotalPerSec {
            get { return _errorsTotalPerSec ?? (_errorsTotalPerSec = CreatePerformanceCounter("Errors Total/Sec")); }
        }

        private PerformanceCounter _errorsUnhandledDuringExecution;

        /// <summary>
        /// Errors Unhanded During Execution
        /// </summary>
        public virtual PerformanceCounter ErrorsUnhandledDuringExecution {
            get {
                return _errorsUnhandledDuringExecution ??
                       (_errorsUnhandledDuringExecution = CreatePerformanceCounter("Errors Unhandled During Execution"));
            }
        }

        private PerformanceCounter _errorsUnhandledDuringExecutionPerSec;

        /// <summary>
        /// Errors Unhanded During Execution/Sec
        /// </summary>
        public virtual PerformanceCounter ErrorsUnhandledDuringExecutionPerSec {
            get {
                return _errorsUnhandledDuringExecutionPerSec ??
                       (_errorsUnhandledDuringExecutionPerSec = CreatePerformanceCounter("Errors Unhandled During Execution/Sec"));
            }
        }

        private PerformanceCounter _eventsRaised;

        /// <summary>
        /// Events Raised
        /// </summary>
        public virtual PerformanceCounter EventsRaised {
            get { return _eventsRaised ?? (_eventsRaised = CreatePerformanceCounter("Events Raised")); }
        }

        private PerformanceCounter _eventsRaisedPerSec;

        /// <summary>
        /// Events Raised/Sec
        /// </summary>
        public virtual PerformanceCounter EventsRaisedPerSec {
            get { return _eventsRaisedPerSec ?? (_eventsRaisedPerSec = CreatePerformanceCounter("Events Raised/Sec")); }
        }

        private PerformanceCounter _requestBytesInTotal;

        /// <summary>
        /// Request Bytes In Total
        /// </summary>
        public virtual PerformanceCounter RequestBytesInTotal {
            get { return _requestBytesInTotal ?? (_requestBytesInTotal = CreatePerformanceCounter("Request Bytes In Total")); }
        }

        private PerformanceCounter _requestBytesOutTotal;

        /// <summary>
        /// Request Bytes Out Total
        /// </summary>
        public virtual PerformanceCounter RequestBytesOutTotal {
            get { return _requestBytesOutTotal ?? (_requestBytesOutTotal = CreatePerformanceCounter("Request Bytes Out Total")); }
        }

        private PerformanceCounter _requestErrorEventsRaised;

        /// <summary>
        /// Request Error Events Raised
        /// </summary>
        public virtual PerformanceCounter RequestErrorEventsRaised {
            get {
                return _requestErrorEventsRaised ??
                       (_requestErrorEventsRaised = CreatePerformanceCounter("Request Error Events Raised"));
            }
        }

        private PerformanceCounter _requestErrorEventsRaisedPerSec;

        /// <summary>
        /// Events Raised/Sec
        /// </summary>
        public virtual PerformanceCounter RequestErrorEventsRaisedPerSec {
            get {
                return _requestErrorEventsRaisedPerSec ??
                       (_requestErrorEventsRaisedPerSec = CreatePerformanceCounter("Request Error Events Raised/Sec"));
            }
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

        private PerformanceCounter _requestsDisconnected;

        /// <summary>
        /// Request Disconnected
        /// </summary>
        public virtual PerformanceCounter RequestsDisconnected {
            get { return _requestsDisconnected ?? (_requestsDisconnected = CreatePerformanceCounter("Requests Disconnected")); }
        }

        private PerformanceCounter _requestsExecuting;

        /// <summary>
        /// Request Executing
        /// </summary>
        public virtual PerformanceCounter RequestsExecuting {
            get { return _requestsExecuting ?? (_requestsExecuting = CreatePerformanceCounter("Requests Executing")); }
        }

        private PerformanceCounter _requestsFailed;

        /// <summary>
        /// Request Failed
        /// </summary>
        public virtual PerformanceCounter RequestsFailed {
            get { return _requestsFailed ?? (_requestsFailed = CreatePerformanceCounter("Requests Failed")); }
        }

        private PerformanceCounter _requestsInApplicationQueue;

        /// <summary>
        /// Requests In Application Queue
        /// </summary>
        public virtual PerformanceCounter RequestsInApplicationQueue {
            get {
                return _requestsInApplicationQueue ??
                       (_requestsInApplicationQueue = CreatePerformanceCounter("Requests In Application Queue"));
            }
        }

        private PerformanceCounter _requestsRejected;

        /// <summary>
        /// Requests Rejected
        /// </summary>
        public virtual PerformanceCounter RequestsRejected {
            get { return _requestsRejected ?? (_requestsRejected = CreatePerformanceCounter("Requests Rejected")); }
        }

        private PerformanceCounter _requestsSucceeded;

        /// <summary>
        /// Requests Succeeded
        /// </summary>
        public virtual PerformanceCounter RequestsSucceeded {
            get { return _requestsSucceeded ?? (_requestsSucceeded = CreatePerformanceCounter("Requests Succeeded")); }
        }

        private PerformanceCounter _requestsTimedOut;

        /// <summary>
        /// Requests Timed Out
        /// </summary>
        public virtual PerformanceCounter RequestsTimedOut {
            get { return _requestsTimedOut ?? (_requestsTimedOut = CreatePerformanceCounter("Requests Timed Out")); }
        }

        private PerformanceCounter _requestsTotal;

        /// <summary>
        /// Requests Total
        /// </summary>
        public virtual PerformanceCounter RequestsTotal {
            get { return _requestsTotal ?? (_requestsTotal = CreatePerformanceCounter("Requests Total")); }
        }

        private PerformanceCounter _requestsPerSec;

        /// <summary>
        /// Requests/Sec
        /// </summary>
        public virtual PerformanceCounter RequestsPerSec {
            get { return _requestsPerSec ?? (_requestsPerSec = CreatePerformanceCounter("Requests/Sec")); }
        }

        private PerformanceCounter _sessionSQLServerConnectionsTotal;

        /// <summary>
        /// Session SQL Server connections total
        /// </summary>
        public virtual PerformanceCounter SessionSQLServerConnectionsTotal {
            get {
                return _sessionSQLServerConnectionsTotal ??
                       (_sessionSQLServerConnectionsTotal = CreatePerformanceCounter("Session SQL Server connections total"));
            }
        }

        private PerformanceCounter _sessionStateServerConnectionsTotal;

        /// <summary>
        /// Session State Server connections total
        /// </summary>
        public virtual PerformanceCounter SessionStateServerConnectionsTotal {
            get {
                return _sessionStateServerConnectionsTotal ??
                       (_sessionStateServerConnectionsTotal = CreatePerformanceCounter("Session State Server connections total"));
            }
        }

        private PerformanceCounter _sessionsAbandoned;

        /// <summary>
        /// Sessions Abandoned
        /// </summary>
        public virtual PerformanceCounter SessionsAbandoned {
            get { return _sessionsAbandoned ?? (_sessionsAbandoned = CreatePerformanceCounter("Sessions Abandoned")); }
        }

        private PerformanceCounter _sessionsActive;

        /// <summary>
        /// Sessions Active
        /// </summary>
        public virtual PerformanceCounter SessionsActive {
            get { return _sessionsActive ?? (_sessionsActive = CreatePerformanceCounter("Sessions Active")); }
        }

        private PerformanceCounter _sessionsTimedOut;

        /// <summary>
        /// Sessions Timed Out
        /// </summary>
        public virtual PerformanceCounter SessionsTimedOut {
            get { return _sessionsTimedOut ?? (_sessionsTimedOut = CreatePerformanceCounter("Sessions Timed Out")); }
        }

        private PerformanceCounter _sessionsTotal;

        /// <summary>
        /// Sessions Total
        /// </summary>
        public virtual PerformanceCounter SessionsTotal {
            get { return _sessionsTotal ?? (_sessionsTotal = CreatePerformanceCounter("Sessions Total")); }
        }

        private PerformanceCounter _transactionsAborted;

        /// <summary>
        /// Transactions Aborted
        /// </summary>
        public virtual PerformanceCounter TransactionsAborted {
            get { return _transactionsAborted ?? (_transactionsAborted = CreatePerformanceCounter("Transactions Aborted")); }
        }

        private PerformanceCounter _transactionsCommitted;

        /// <summary>
        /// Transactions Committed
        /// </summary>
        public virtual PerformanceCounter TransactionsCommitted {
            get { return _transactionsCommitted ?? (_transactionsCommitted = CreatePerformanceCounter("Transactions Committed")); }
        }

        private PerformanceCounter _transactionsPending;

        /// <summary>
        /// Transactions Pending
        /// </summary>
        public virtual PerformanceCounter TransactionsPending {
            get { return _transactionsPending ?? (_transactionsPending = CreatePerformanceCounter("Transactions Pending")); }
        }

        private PerformanceCounter _transactionsTotal;

        /// <summary>
        /// Transactions Total
        /// </summary>
        public virtual PerformanceCounter TransactionsTotal {
            get { return _transactionsTotal ?? (_transactionsTotal = CreatePerformanceCounter("Transactions Total")); }
        }

        private PerformanceCounter _transactionsPerSec;

        /// <summary>
        /// Transactions/Sec
        /// </summary>
        public virtual PerformanceCounter TransactionsPerSec {
            get { return _transactionsPerSec ?? (_transactionsPerSec = CreatePerformanceCounter("Transactions/Sec")); }
        }

        private PerformanceCounter _viewstateMACValidationFailure;

        /// <summary>
        /// Viewstate MAC Validation Failure
        /// </summary>
        public virtual PerformanceCounter ViewstateMACValidationFailure {
            get {
                return _viewstateMACValidationFailure ??
                       (_viewstateMACValidationFailure = CreatePerformanceCounter("Viewstate MAC Validation Failure"));
            }
        }
    }
}