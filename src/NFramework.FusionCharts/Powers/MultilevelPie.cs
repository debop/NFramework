using System.Drawing;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Powers {
    /// <summary>
    /// 조직도 등 Hierarchy를 가지는 정보를 Pie Chart 형식으로 표현할 때 사용한다.
    /// </summary>
    public class MultilevelPie : ChartBase {
        /// <summary>
        /// List of hex color codes separated by comma.
        /// While the palette attribute allows to select a palette theme that applies to chart background, canvas, font and tool-tips, it does not change the colors of data items (i.e., column, line, pie etc.). Using paletteColors attribute, you can specify your custom list of hex colors for the data items. The list of colors have to be separated by comma e.g., <chart paletteColors='FF0000,0372AB,FF5904...'>. The chart will cycle through the list of specified colors and then render the data plot accordingly. 
        /// To use the same set of colors throughout all your charts in a web application, you can store the list of palette colors in your application globally and then provide the same in each chart XML.
        /// </summary>
        public string PaletteColors { get; set; }

        /// <summary>
        /// Whether the pie border would show up.
        /// </summary>
        public bool? ShowPlotBorder { get; set; }

        /// <summary>
        /// Color for pie border
        /// </summary>
        public Color? PlotBorderColor { get; set; }

        /// <summary>
        /// Thickness for pie border
        /// </summary>
        public int? PlotBorderThickness { get; set; }

        /// <summary>
        /// Alpha for pie border
        /// </summary>
        public int? PlotBorderAlpha { get; set; }

        /// <summary>
        /// This attribute lets you set the fill alpha for plot.
        /// </summary>
        public int? PlotFillAlpha { get; set; }

        /// <summary>
        /// Fill color for all the pies
        /// </summary>
        public Color? PlotFillColor { get; set; }

        /// <summary>
        /// This attribute lets you explicitly set the outer radius of the chart. FusionCharts automatically calculates the best fit pie radius for the chart. This attribute is useful if you want to enforce one of your own values.
        /// </summary>
        public int? PieRadius { get; set; }

        public int? PieFillAlpha { get; set; }
        public int? PieBorderThickness { get; set; }
        public Color? HoverFillColor { get; set; }
        public Color? PieBorderColor { get; set; }
        public bool? UseHoverColor { get; set; }

        private MultilevelCategoryElement _category;

        public MultilevelCategoryElement Category {
            get { return _category ?? (_category = new MultilevelCategoryElement()); }
        }

        public virtual void SetRootCategory(MultilevelCategoryElement root) {
            _category = root;
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(PaletteColors.IsNotWhiteSpace())
                writer.WriteAttributeString("paletteColors", PaletteColors);
            if(ShowPlotBorder.HasValue)
                writer.WriteAttributeString("showPlotBorder", ShowPlotBorder.GetHashCode().ToString());
            if(PlotBorderColor.HasValue)
                writer.WriteAttributeString("plotBorderColor", PlotBorderColor.Value.ToHexString());
            if(PlotBorderThickness.HasValue)
                writer.WriteAttributeString("plotBorderThickness", PlotBorderThickness.ToString());
            if(PlotBorderAlpha.HasValue)
                writer.WriteAttributeString("plotBorderAlpha", PlotBorderAlpha.ToString());
            if(PlotFillAlpha.HasValue)
                writer.WriteAttributeString("plotFillAlpha", PlotFillAlpha.ToString());
            if(PlotFillColor.HasValue)
                writer.WriteAttributeString("plotFillColor", PlotFillColor.Value.ToHexString());
            if(PieRadius.HasValue)
                writer.WriteAttributeString("pieRadius", PieRadius.ToString());

            if(PieFillAlpha.HasValue)
                writer.WriteAttributeString("PieFillAlpha", PieFillAlpha.ToString());
            if(PieBorderThickness.HasValue)
                writer.WriteAttributeString("PieBorderThickness", PieBorderThickness.ToString());
            if(HoverFillColor.HasValue)
                writer.WriteAttributeString("HoverFillColor", HoverFillColor.Value.ToHexString());
            if(PieBorderColor.HasValue)
                writer.WriteAttributeString("PieBorderColor", PieBorderColor.Value.ToHexString());
            if(UseHoverColor.HasValue)
                writer.WriteAttributeString("useHoverColor", UseHoverColor.GetHashCode().ToString());
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(System.Xml.XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_category != null)
                _category.WriteXmlElement(writer);
        }
    }
}