using System;
using System.Diagnostics;

namespace NSoft.NFramework.Diagnostics.Providers {
    /// <summary>
    /// Processor Category에 대한 <see cref="PerformanceCounter"/> 를 제공합니다.
    /// </summary>
    /// <example>
    /// <code>
    /// CategoryName=Processor
    /// 	InstanceName=_Total
    /// 		Counter Name=% C1 Time
    /// 		Counter Name=% C2 Time
    /// 		Counter Name=% C3 Time
    /// 		Counter Name=% DPC Time
    /// 		Counter Name=% Idle Time
    /// 		Counter Name=% Interrupt Time
    /// 		Counter Name=% Privileged Time
    /// 		Counter Name=% Processor Time
    /// 		Counter Name=% User Time
    /// 		Counter Name=C1 Transitions/sec
    /// 		Counter Name=C2 Transitions/sec
    /// 		Counter Name=C3 Transitions/sec
    /// 		Counter Name=DPC Rate
    /// 		Counter Name=DPCs Queued/sec
    /// 		Counter Name=Interrupts/sec
    /// 	InstanceName=0
    /// 		Counter Name=% C1 Time
    /// 		Counter Name=% C2 Time
    /// 		Counter Name=% C3 Time
    /// 		Counter Name=% DPC Time
    /// 		Counter Name=% Idle Time
    /// 		Counter Name=% Interrupt Time
    /// 		Counter Name=% Privileged Time
    /// 		Counter Name=% Processor Time
    /// 		Counter Name=% User Time
    /// 		Counter Name=C1 Transitions/sec
    /// 		Counter Name=C2 Transitions/sec
    /// 		Counter Name=C3 Transitions/sec
    /// 		Counter Name=DPC Rate
    /// 		Counter Name=DPCs Queued/sec
    /// 		Counter Name=Interrupts/sec
    /// 	InstanceName=1
    /// 		Counter Name=% C1 Time
    /// 		Counter Name=% C2 Time
    /// 		Counter Name=% C3 Time
    /// 		Counter Name=% DPC Time
    /// 		Counter Name=% Idle Time
    /// 		Counter Name=% Interrupt Time
    /// 		Counter Name=% Privileged Time
    /// 		Counter Name=% Processor Time
    /// 		Counter Name=% User Time
    /// 		Counter Name=C1 Transitions/sec
    /// 		Counter Name=C2 Transitions/sec
    /// 		Counter Name=C3 Transitions/sec
    /// 		Counter Name=DPC Rate
    /// 		Counter Name=DPCs Queued/sec
    /// 		Counter Name=Interrupts/sec
    /// </code>
    /// </example>
    [Serializable]
    public class ProcessorPerformanceCounterProvider : InstancePerformanceCounterProviderBase {
        /// <summary>
        /// "_Total"에 대한 Processor Category에 대한 <see cref="PerformanceCounter"/>를 제공합니다.
        /// </summary>
        public ProcessorPerformanceCounterProvider() : base("Processor", "_Total") {}

        /// <summary>
        /// CPU 별로 Performance Counter를 얻을 때, "0", "1" ... CPU 숫자까지...
        /// </summary>
        /// <param name="instanceName"></param>
        public ProcessorPerformanceCounterProvider(string instanceName) : base("Processor", instanceName) {}

        private PerformanceCounter _percentOfC1Time;

        /// <summary>
        ///	% C1 Time
        /// </summary>
        public PerformanceCounter PercentOfC1Time {
            get { return _percentOfC1Time ?? (_percentOfC1Time = CreatePerformanceCounter("% C1 Time")); }
        }

        private PerformanceCounter _percentOfC2Time;

        /// <summary>
        ///	% C2 Time
        /// </summary>
        public PerformanceCounter PercentOfC2Time {
            get { return _percentOfC2Time ?? (_percentOfC2Time = CreatePerformanceCounter("% C2 Time")); }
        }

        private PerformanceCounter _percentOfC3Time;

        /// <summary>
        ///	% C3 Time
        /// </summary>
        public PerformanceCounter PercentOfC3Time {
            get { return _percentOfC3Time ?? (_percentOfC3Time = CreatePerformanceCounter("% C3 Time")); }
        }

        private PerformanceCounter _percentOfDPCTime;

        /// <summary>
        ///	% DPC Time
        /// </summary>
        public PerformanceCounter PercentOfDpcTime {
            get { return _percentOfDPCTime ?? (_percentOfDPCTime = CreatePerformanceCounter("% DPC Time")); }
        }

        private PerformanceCounter _percentOfIdleTime;

        /// <summary>
        ///	% Idle Time
        /// </summary>
        public PerformanceCounter PercentOfIdleTime {
            get { return _percentOfIdleTime ?? (_percentOfIdleTime = CreatePerformanceCounter("% Idle Time")); }
        }

        private PerformanceCounter _percentOfInterruptTime;

        /// <summary>
        ///	% Interrupt Time
        /// </summary>
        public PerformanceCounter PercentOfInterruptTime {
            get { return _percentOfInterruptTime ?? (_percentOfInterruptTime = CreatePerformanceCounter("% Interrupt Time")); }
        }

        private PerformanceCounter _percentOfPrivilegedTime;

        /// <summary>
        ///	% Privileged Time
        /// </summary>
        public PerformanceCounter PercentOfPrivilegedTime {
            get { return _percentOfPrivilegedTime ?? (_percentOfPrivilegedTime = CreatePerformanceCounter("% Privileged Time")); }
        }

        private PerformanceCounter _percentOfProcessorTime;

        /// <summary>
        ///	% Processor Time
        /// </summary>
        public PerformanceCounter PercentOfProcessorTime {
            get { return _percentOfProcessorTime ?? (_percentOfProcessorTime = CreatePerformanceCounter("% Processor Time")); }
        }

        private PerformanceCounter _percentOfUserTime;

        /// <summary>
        ///	% User Time
        /// </summary>
        public PerformanceCounter PercentOfUserTime {
            get { return _percentOfUserTime ?? (_percentOfUserTime = CreatePerformanceCounter("% User Time")); }
        }

        private PerformanceCounter _interruptsPerSec;

        /// <summary>
        ///	Interrupts/sec
        /// </summary>
        public PerformanceCounter InterruptsPerSec {
            get { return _interruptsPerSec ?? (_interruptsPerSec = CreatePerformanceCounter("Interrupts/sec")); }
        }
    }
}