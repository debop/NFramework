using System;
using System.Drawing;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// Data Plotting 과 관련된 설정 정보를 나타냅니다.
    /// </summary>
    [Serializable]
    public class DataPlotAttribute : ChartAttributeBase {
        public bool? UseRoundEdges { get; set; }

        public bool? ShowPlotBorder { get; set; }
        public Color? PlotBorderColor { get; set; }
        public int? PlotBorderThickness { get; set; }
        public int? PlotBorderAlpha { get; set; }

        public bool? PlotBorderDashed { get; set; }
        public int? PlotBorderDashLen { get; set; }
        public int? PlotBorderDashGap { get; set; }

        public Color? PlotFillColor { get; set; }
        public int? PlotFillAngle { get; set; }
        public int? PlotFillRatio { get; set; }
        public int? PlotFillAlpha { get; set; }
        public Color? PlotGradientColor { get; set; }

        public bool? ShowShadow { get; set; }
        public bool? OverlapColumns { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML 속성으로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(UseRoundEdges.HasValue)
                writer.WriteAttributeString("useRoundEdges", UseRoundEdges.Value.GetHashCode().ToString());

            if(ShowPlotBorder.HasValue)
                writer.WriteAttributeString("showPlotBorder", ShowPlotBorder.Value.GetHashCode().ToString());
            if(PlotBorderColor.HasValue)
                writer.WriteAttributeString("plotBorderColor", PlotBorderColor.Value.ToHexString());
            if(PlotBorderThickness.HasValue)
                writer.WriteAttributeString("plotBorderThickness", PlotBorderThickness.Value.ToString());
            if(PlotBorderAlpha.HasValue)
                writer.WriteAttributeString("plotBorderAlpha", PlotBorderAlpha.Value.ToString());

            if(PlotBorderDashed.HasValue)
                writer.WriteAttributeString("PlotBorderDashed", PlotBorderDashed.Value.GetHashCode().ToString());
            if(PlotBorderDashLen.HasValue)
                writer.WriteAttributeString("PlotBorderDashLen", PlotBorderDashLen.Value.ToString());
            if(PlotBorderDashGap.HasValue)
                writer.WriteAttributeString("PlotBorderDashGap", PlotBorderDashGap.Value.ToString());

            if(PlotFillColor.HasValue)
                writer.WriteAttributeString("PlotFillColor", PlotFillColor.Value.ToHexString());
            if(PlotFillAngle.HasValue)
                writer.WriteAttributeString("PlotFillAngle", PlotFillAngle.Value.ToString());
            if(PlotFillRatio.HasValue)
                writer.WriteAttributeString("PlotFillRatio", PlotFillRatio.Value.ToString());
            if(PlotFillAlpha.HasValue)
                writer.WriteAttributeString("plotFillAlpha", PlotFillAlpha.Value.ToString());
            if(PlotGradientColor.HasValue)
                writer.WriteAttributeString("PlotGradientColor", PlotGradientColor.Value.ToHexString());

            if(ShowShadow.HasValue)
                writer.WriteAttributeString("ShowShadow", ShowShadow.Value.GetHashCode().ToString());
            if(OverlapColumns.HasValue)
                writer.WriteAttributeString("overlapColumns", OverlapColumns.Value.GetHashCode().ToString());
        }
    }
}