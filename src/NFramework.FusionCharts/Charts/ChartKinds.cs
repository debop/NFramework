namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// Fusion Charts 의 종류를 나타내는 Enum 값들을 가집니다. 실제 swf 파일과 매칭됩니다.
    /// </summary>
    public static class ChartKinds {
        public static readonly SingleSeries Unknown = SingleSeries.Unknown;

        /// <summary>
        /// 변량이 한 종류인 Chart
        /// </summary>
        public enum SingleSeries {
            Unknown,
            Column2D,
            Column3D,
            Line2D,
            Area2D,
            Bar2D,
            Pie2D,
            Pie3D,
            Doughnut2D,
            Doughnut3D
        }

        /// <summary>
        /// 변량이 복수개인 Chart
        /// </summary>
        public enum MultiSeries {
            MSColumn2D,
            MSColumn3D,
            MSLine,
            MSBar2D,
            MSBar3D,
            MSArea
        }

        /// <summary>
        /// Stack 형태의 Chart
        /// </summary>
        public enum Stacked {
            StackedColumn2D,
            StackedColumn3D,
            StackedBar2D,
            StackedBar3D,
            StackedArea2D
        }

        /// <summary>
        /// 복합 chart
        /// </summary>
        public enum Combination {
            /// <summary>
            /// Multi-series 2D Single Y Combination Chart (Column + Line + Area)
            /// </summary>
            MSCombi2D,

            /// <summary>
            /// Multi-series 2D Dual Y Combination chart (Column + Line + Area)
            /// </summary>
            MSCombiDY2D,

            /// <summary>
            /// Multi-series Column 3D + Multi-series Line - Single Y Axis
            /// </summary>
            MSColumnLine3D,

            /// <summary>
            /// Multi-series Column 3D + Multi-series Line - Dual Y Axis
            /// </summary>
            MSColumn3DLineDY,

            /// <summary>
            /// Multi-series Stacked Column 2D Chart
            /// </summary>
            MSStackedColumn2D,

            /// <summary>
            /// Multi-series Stacked Column 2D Chart
            /// </summary>
            MSStackedColumn2DLineDY,

            /// <summary>
            /// Stacked Column3D + Line Dual Y Axis
            /// </summary>
            StackedColumn3DLineDY
        }

        /// <summary>
        /// (X,Y) 지점에 점을 찍는 Plot 형태의 chart
        /// </summary>
        public enum XYPlot {
            Scatter,
            Bubble
        }

        /// <summary>
        /// Scroll 이 가능한 Chart
        /// </summary>
        public enum Scroll {
            /// <summary>
            /// Scroll Column 2D
            /// </summary>
            ScrollColumn2D,

            /// <summary>
            /// Scroll Line 2D
            /// </summary>
            ScrollLine2D,

            /// <summary>
            /// Scroll Area 2D
            /// </summary>
            ScrollArea2D,

            /// <summary>
            /// Scroll stacked column 2D
            /// </summary>
            ScrollStackedColumn2D,

            /// <summary>
            /// Scroll combination 2D (Single Y)
            /// </summary>
            ScrollCombi2D,

            /// <summary>
            /// Scroll combination 2D (Dual Y)
            /// </summary>
            ScrollCombiDY2D,

            /// <summary>
            /// Single series grid component
            /// </summary>
            SSGrid
        }
    }
}