namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// FusionWidgets 의 Chart 들
    /// </summary>
    public static class WidgetKinds {
        public static readonly RealTimes Unknown = RealTimes.Unknown;

        /// <summary>
        /// Real-time data-streaming charts
        /// </summary>
        public enum RealTimes {
            Unknown,
            RealTimeArea,
            RealTimeColumn,
            RealTimeStackedArea,
            RealTimeStackedColumn,
            RealTimeLineDY
        }

        public enum Gagues {
            /// <summary>
            /// Angular
            /// </summary>
            AngularGague,

            /// <summary>
            /// Bulb
            /// </summary>
            Bulb,

            /// <summary>
            /// Cylinder
            /// </summary>
            Cylinder,

            /// <summary>
            /// Horizontal LED
            /// </summary>
            HLED,

            /// <summary>
            /// Horizontal Linear
            /// </summary>
            HLinearGuage,

            /// <summary>
            /// Thermometer
            /// </summary>
            Thermometer,

            /// <summary>
            /// Vertical LED
            /// </summary>
            VLED
        }

        public enum SparkCharts {
            /// <summary>
            /// Spark line
            /// </summary>
            SparkLine,

            /// <summary>
            /// Spark column
            /// </summary>
            SparkColumn,

            /// <summary>
            /// Spark Win/Loss
            /// </summary>
            SparkWinLoss
        }

        public enum BulletGraphs {
            /// <summary>
            /// Horizontal bullet graph
            /// </summary>
            HBullet,

            /// <summary>
            /// Vertical bullet graph
            /// </summary>
            VBullet,
            Funnel,
            Pyramid,
            Gantt,
            DrawingPad
        }
    }
}