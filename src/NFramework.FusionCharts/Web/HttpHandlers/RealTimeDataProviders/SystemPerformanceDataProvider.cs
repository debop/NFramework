using System;
using NSoft.NFramework.Diagnostics;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Web {
    /// <summary>
    /// System Performace 정보를 RealTime Chart에 제공해주는 Provider입니다.
    /// </summary>
    public class SystemPerformanceDataProvider : RealTimeDataProviderBase {
        #region Overrides of FusionChartRealTimeDataProviderBase

        /// <summary>
        /// 시스템 정보를 가공하여, Fusion Chart RealTime Data를 제공합니다.
        /// </summary>
        protected override string PopulateResponseData() {
            var label = DateTime.Now.ToUniversalSortableDateTimeString().TrimEnd('Z');
            var processorTime = PerformanceCounterTool.Processor.PercentOfProcessorTime.NextValue();
            var availableMemory = PerformanceCounterTool.Memory.AvailableMbytes.NextValue();
            var value = processorTime + DATASET_SEPARATOR + availableMemory;

            return string.Format(RealTimeResponseFormat, label, value);
        }

        #endregion

        /// <summary>
        /// 성능측정을 원하는 항목을 파싱합니다.
        /// </summary>
        private void ParseRequestParameters() {}

        /// <summary>
        /// 시스템의 성능 정보를 측정합니다.
        /// </summary>
        private void RetrieveSystemPerformanceData() {}
    }
}