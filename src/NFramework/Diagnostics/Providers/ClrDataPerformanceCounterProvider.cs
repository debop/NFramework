using System;
using System.Diagnostics;

namespace NSoft.NFramework.Diagnostics.Providers {
    /// <summary>
    /// .NET CLR Data 관련 Performance Counter를 제공합니다.
    /// </summary>
    [Obsolete("InstanceName이 있어야 한다고 하는데... Category 에서 보면 Instance Name을 찾을 수 없는데...")]
    [Serializable]
    public class ClrDataPerformanceCounterProvider : InstancePerformanceCounterProviderBase {
        /// <summary>
        /// Contructor
        /// </summary>
        public ClrDataPerformanceCounterProvider() : base(".NET CLR Data", string.Empty) {}

        private PerformanceCounter _currentCountOfConnectionPools;

        /// <summary>
        /// Current # connection pools
        /// </summary>
        public PerformanceCounter CurrentCountOfConnectionPools {
            get {
                return _currentCountOfConnectionPools ??
                       (_currentCountOfConnectionPools = CreatePerformanceCounter("SqlClient: Current # connection pools"));
            }
        }

        private PerformanceCounter _currentCountOfPooledAndNonPooledConnecxtions;

        /// <summary>
        /// Current # ppoled and nonpooled connections
        /// </summary>
        public PerformanceCounter CurrentCountOfPooledAndNonPooledConnecxtions {
            get {
                return _currentCountOfPooledAndNonPooledConnecxtions ??
                       (_currentCountOfPooledAndNonPooledConnecxtions =
                        CreatePerformanceCounter("SqlClient: Current # ppoled and nonpooled connections"));
            }
        }

        private PerformanceCounter _currentCountOfPooledConnections;

        /// <summary>
        /// SqlClient: Current # pooled connections
        /// </summary>
        public PerformanceCounter CurrentCountOfPooledConnections {
            get {
                return _currentCountOfPooledConnections ??
                       (_currentCountOfPooledConnections = CreatePerformanceCounter("SqlClient: Current # pooled connections"));
            }
        }

        private PerformanceCounter _peekCountOfPooledConnections;

        /// <summary>
        /// SqlClient: Peek # pooled connections
        /// </summary>
        public PerformanceCounter PeekCountOfPooledConnections {
            get {
                return _peekCountOfPooledConnections ??
                       (_peekCountOfPooledConnections = CreatePerformanceCounter("SqlClient: Peek # pooled connections"));
            }
        }

        private PerformanceCounter _totalCountOfFailedCommands;

        /// <summary>
        /// SqlClient: Total # failed commands
        /// </summary>
        public PerformanceCounter TotalCountOfFailedCommands {
            get {
                return _totalCountOfFailedCommands ??
                       (_totalCountOfFailedCommands = CreatePerformanceCounter("SqlClient: Total # failed commands"));
            }
        }

        private PerformanceCounter _totalCountOfFailedConnects;

        /// <summary>
        /// SqlClient: Total # failed connects
        /// </summary>
        public PerformanceCounter TotalCountOfFailedConnects {
            get {
                return _totalCountOfFailedConnects ??
                       (_totalCountOfFailedConnects = CreatePerformanceCounter("SqlClient: Total # failed connects"));
            }
        }
    }
}