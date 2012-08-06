using System.Drawing;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// XY Plot Chart의 DataSet Element
    /// </summary>
    public class XYPlotDataSetElement : DataSetElement {
        public bool? ShowPlotBorder { get; set; }
        public Color? PlotBorderColor { get; set; }
        public int? PlotBorderThickness { get; set; }
        public int? PlotBorderAlpha { get; set; }

        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(ShowPlotBorder.HasValue)
                writer.WriteAttributeString("showPlotBorder", ShowPlotBorder.GetHashCode().ToString());
            if(PlotBorderColor.HasValue)
                writer.WriteAttributeString("plotBorderColor", PlotBorderColor.Value.ToHexString());
            if(PlotBorderThickness.HasValue)
                writer.WriteAttributeString("plotBorderThickness", PlotBorderThickness.ToString());
            if(PlotBorderAlpha.HasValue)
                writer.WriteAttributeString("plotBorderAlpha", PlotBorderAlpha.ToString());
        }
    }
}