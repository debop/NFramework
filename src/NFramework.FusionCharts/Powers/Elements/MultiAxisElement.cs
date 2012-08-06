using System.Drawing;
using NSoft.NFramework.FusionCharts.Charts;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Powers {
    /// <summary>
    /// Each axis element on the multi-axis line chart represents an axis. An axis can either be visible or imaginary. Imaginary axis are the ones that do not show up on the chart, but can have data sets that adhere to the same.
    /// Visible axis can be placed either to the left or right side of canvas.
    /// For each axis, you can define a lot of properties like:
    /// <list>
    ///		<item>It's lower and upper limits.</item>
    ///		<item>Cosmetic properties.</item>
    ///		<item>Number of divisional lines and their functional and cosmetic properties.</item>
    ///		<item>Line properties for the datasets that adhere to this particular axis</item>
    ///		<item>Number formatting properties</item>
    /// </list>
    /// </summary>
    public class MultiAxisElement : ChartElementBase {
        public MultiAxisElement() : base("axis") {}

        /// <summary>
        /// Whether this axis is a visible axis or imaginary axis?
        /// </summary>
        public bool? ShowAxis { get; set; }

        /// <summary>
        /// Whether to show the axis on left side of canvas or right side of canvas?
        /// </summary>
        public bool? AxisOnLeft { get; set; }

        /// <summary>
        /// Title of the axis.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Where to align the title of axis - possible values are left, right or bottom.
        /// </summary>
        public FusionTextAlign? TitlePos { get; set; }

        /// <summary>
        /// This attribute helps you explicitly set the upper limit of the axis. If you don't specify this value, it is automatically calculated by FusionCharts based on the data provided by you.
        /// </summary>
        public double? MaxValue { get; set; }

        /// <summary>
        /// This attribute helps you explicitly set the lower limit of the axis. If you don't specify this value, it is automatically calculated by FusionCharts based on the data provided by you.
        /// </summary>
        public double? MinValue { get; set; }

        /// <summary>
        /// This attribute lets you set whether the y-axis lower limit would be 0 (in case of all positive values on chart) or should the y-axis lower limit adapt itself to a different figure based on values provided to the axis.
        /// </summary>
        public bool? SetAdaptiveYMin { get; set; }

        /// <summary>
        /// Thickness of the line using which axis will be plotted.
        /// </summary>
        public int? AxisLineThickness { get; set; }

        /// <summary>
        /// Width of the tick marks that extend from axis.
        /// </summary>
        public int? TickWidth { get; set; }

        /// <summary>
        /// Color of the axis.
        /// </summary>
        public Color? Color { get; set; }

        /// <summary>
        /// Number of horizontal axis division lines that you want for this axis.
        /// </summary>
        public int? NumDivLines { get; set; }

        private DashLineAttribute _divLine;

        public DashLineAttribute DivLine {
            get { return _divLine ?? (_divLine = new DashLineAttribute("divLine")); }
            set { _divLine = value; }
        }

        /// <summary>
        /// FusionCharts automatically tries to adjust divisional lines and limit values based on the data provided. However, if you want to set your explicit lower and upper limit values and number of divisional lines, first set this attribute to false. That would disable automatic adjustment of divisional lines.
        /// </summary>
        public bool? AdjustDiv { get; set; }

        /// <summary>
        /// Each axis is divided into vertical sections using div (divisional) lines. Each div line assumes a value based on its position. Using this attribute you can set whether to show those div line (y-axis) values or not.
        /// </summary>
        public bool? ShowYAxisValues { get; set; }

        /// <summary>
        /// By default, all div lines show their values. However, you can opt to skip every x(th) div line value using this attribute.
        /// </summary>
        public int? YAxisValuesStep { get; set; }

        private ZeroPaneAttribute _zeroPane;

        public ZeroPaneAttribute ZeroPane {
            get { return _zeroPane ?? (_zeroPane = new ZeroPaneAttribute()); }
            set { _zeroPane = value; }
        }

        private NumberFormatAttribute _numberFormat;

        public NumberFormatAttribute NumberFormat {
            get { return _numberFormat ?? (_numberFormat = new NumberFormatAttribute()); }
            set { _numberFormat = value; }
        }

        private DashLineAttribute _line;

        public DashLineAttribute Line {
            get { return _line ?? (_line = new DashLineAttribute()); }
            set { _line = value; }
        }

        private DataSetElement _dataset;

        public DataSetElement DataSet {
            get { return _dataset ?? (_dataset = new DataSetElement()); }
            set { _dataset = value; }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(ShowAxis.HasValue)
                writer.WriteAttributeString("showAxis", ShowAxis.GetHashCode().ToString());
            if(AxisOnLeft.HasValue)
                writer.WriteAttributeString("AxisOnLeft", AxisOnLeft.GetHashCode().ToString());
            if(Title.IsNotWhiteSpace())
                writer.WriteAttributeString("Title", Title);
            if(TitlePos.HasValue)
                writer.WriteAttributeString("TitlePos", TitlePos.ToString());

            if(MaxValue.HasValue)
                writer.WriteAttributeString("MaxValue", MaxValue.ToString());
            if(MinValue.HasValue)
                writer.WriteAttributeString("MinValue", MinValue.ToString());

            if(SetAdaptiveYMin.HasValue)
                writer.WriteAttributeString("setAdaptiveYMin", SetAdaptiveYMin.GetHashCode().ToString());
            if(AxisLineThickness.HasValue)
                writer.WriteAttributeString("axisLineThickness", AxisLineThickness.ToString());
            if(TickWidth.HasValue)
                writer.WriteAttributeString("tickWidth", TickWidth.ToString());

            if(Color.HasValue)
                writer.WriteAttributeString("Color", Color.Value.ToHexString());
            if(NumDivLines.HasValue)
                writer.WriteAttributeString("NumDivLines", NumDivLines.ToString());

            if(AdjustDiv.HasValue)
                writer.WriteAttributeString("AdjustDiv", AdjustDiv.GetHashCode().ToString());

            if(ShowYAxisValues.HasValue)
                writer.WriteAttributeString("ShowYAxisValues", ShowYAxisValues.GetHashCode().ToString());
            if(YAxisValuesStep.HasValue)
                writer.WriteAttributeString("YAxisValuesStep", YAxisValuesStep.GetHashCode().ToString());

            if(_divLine != null)
                _divLine.GenerateXmlAttributes(writer);

            if(_zeroPane != null)
                _zeroPane.GenerateXmlAttributes(writer);
            if(_numberFormat != null)
                _numberFormat.GenerateXmlAttributes(writer);
            if(_line != null)
                _line.GenerateXmlAttributes(writer);
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(System.Xml.XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_dataset != null)
                _dataset.WriteXmlElement(writer);
        }
    }
}