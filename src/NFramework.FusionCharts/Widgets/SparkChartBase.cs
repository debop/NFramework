using System.Drawing;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Spark charts are "data-intense, design-simple, word-sized graphics" charts for embedding in a context of words, numbers, images. 
    /// Whereas the typical chart is designed to show as much data as possible, and is set off from the flow of text, spark charts are intended to be succinct, memorable, and located where they are discussed. Their use inline usually means that they are about the same height as the surrounding text. Also, spark charts can be intensively used in space-efficient executive dashboards to show a lot of KPIs in a single view. 
    /// </summary>
    public abstract class SparkChartBase : WidgetBase {
        /// <summary>
        /// This attribute lets you set whether the y-axis lower limit would be 0 (in case of all positive values on chart) or should the y-axis lower limit adapt itself to a different figure based on values provided to the chart.
        /// </summary>
        public bool? SetAdaptiveYMin { get; set; }

        /// <summary>
        /// This attribute helps you explicitly set the lower limit of the chart. If you don't specify this value, it is automatically calculated by FusionCharts based on the data provided by you. 
        /// </summary>
        public int? YAxisMinValue { get; set; }

        /// <summary>
        /// This attribute helps you explicitly set the upper limit of the chart. If you don't specify this value, it is automatically calculated by FusionCharts based on the data provided by you. 
        /// </summary>
        public int? YAxisMaxValue { get; set; }

        private LineAttribute _lineAttr;

        public LineAttribute LineAttr {
            get { return _lineAttr ?? (_lineAttr = new LineAttribute("line")); }
            set { _lineAttr = value; }
        }

        public int? AnchorSides { get; set; }
        public int? AnchorRadius { get; set; }
        public Color? AnchorColor { get; set; }
        public int? AnchorAlpha { get; set; }

        public Color? OpenColor { get; set; }
        public Color? CloseColor { get; set; }
        public Color? HighColor { get; set; }
        public Color? LowColor { get; set; }

        public bool? ShowOpenAnchor { get; set; }
        public bool? ShowCloseAnchor { get; set; }
        public bool? ShowHighAnchor { get; set; }
        public bool? ShowLowAnchor { get; set; }

        public bool? ShowOpenValue { get; set; }
        public bool? ShowCloseValue { get; set; }
        public bool? ShowHighLowValue { get; set; }

        public int? PeriodLength { get; set; }
        public Color? PeriodColor { get; set; }
        public int? PeriodAlpha { get; set; }

        public void AddValue(double value) {
            Dataset.Add(new SetElement { Value = value });
        }

        private CollectionElement<SetElement> _dataset;

        public CollectionElement<SetElement> Dataset {
            get { return _dataset ?? (_dataset = new CollectionElement<SetElement>("dataset")); }
            set { _dataset = value; }
        }

        public class SetElement : ChartElementBase {
            public SetElement()
                : base("set") {}

            public double? Value { get; set; }

            public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
                base.GenerateXmlAttributes(writer);

                if(Value.HasValue)
                    writer.WriteAttributeString("value", Value.ToString());
            }
        }

        private CollectionElement<DoubleLineElement> _trendlines;

        public CollectionElement<DoubleLineElement> TrendLines {
            get { return _trendlines ?? (_trendlines = new CollectionElement<DoubleLineElement>("trendlines")); }
            set { _trendlines = value; }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(SetAdaptiveYMin.HasValue)
                writer.WriteAttributeString("SetAdaptiveYMin", SetAdaptiveYMin.GetHashCode().ToString());
            if(YAxisMinValue.HasValue)
                writer.WriteAttributeString("YAxisMinValue", YAxisMinValue.ToString());
            if(YAxisMaxValue.HasValue)
                writer.WriteAttributeString("YAxisMaxValue", YAxisMaxValue.ToString());

            if(_lineAttr != null)
                _lineAttr.GenerateXmlAttributes(writer);
            if(AnchorSides.HasValue)
                writer.WriteAttributeString("AnchorSides", AnchorSides.ToString());
            if(AnchorRadius.HasValue)
                writer.WriteAttributeString("AnchorRadius", AnchorRadius.ToString());
            if(AnchorColor.HasValue)
                writer.WriteAttributeString("AnchorColor", AnchorColor.Value.ToHexString());
            if(AnchorAlpha.HasValue)
                writer.WriteAttributeString("AnchorAlpha", AnchorAlpha.ToString());

            if(OpenColor.HasValue)
                writer.WriteAttributeString("OpenColor", OpenColor.Value.ToHexString());
            if(CloseColor.HasValue)
                writer.WriteAttributeString("CloseColor", CloseColor.Value.ToHexString());
            if(HighColor.HasValue)
                writer.WriteAttributeString("HighColor", HighColor.Value.ToHexString());
            if(LowColor.HasValue)
                writer.WriteAttributeString("LowColor", LowColor.Value.ToHexString());

            if(ShowOpenAnchor.HasValue)
                writer.WriteAttributeString("ShowOpenAnchor", ShowOpenAnchor.GetHashCode().ToString());
            if(ShowCloseAnchor.HasValue)
                writer.WriteAttributeString("ShowCloseAnchor", ShowCloseAnchor.GetHashCode().ToString());
            if(ShowHighAnchor.HasValue)
                writer.WriteAttributeString("ShowHighAnchor", ShowHighAnchor.GetHashCode().ToString());
            if(ShowLowAnchor.HasValue)
                writer.WriteAttributeString("ShowLowAnchor", ShowLowAnchor.GetHashCode().ToString());

            if(ShowOpenValue.HasValue)
                writer.WriteAttributeString("ShowOpenValue", ShowOpenValue.GetHashCode().ToString());
            if(ShowCloseValue.HasValue)
                writer.WriteAttributeString("ShowCloseValue", ShowCloseValue.GetHashCode().ToString());
            if(ShowHighLowValue.HasValue)
                writer.WriteAttributeString("ShowHighLowValue", ShowHighLowValue.GetHashCode().ToString());

            if(PeriodLength.HasValue)
                writer.WriteAttributeString("PeriodLength", PeriodLength.ToString());
            if(PeriodColor.HasValue)
                writer.WriteAttributeString("PeriodColor", PeriodColor.Value.ToHexString());
            if(PeriodAlpha.HasValue)
                writer.WriteAttributeString("PeriodAlpha", PeriodAlpha.ToString());
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(System.Xml.XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_dataset != null)
                _dataset.WriteXmlElement(writer);

            if(_trendlines != null)
                _trendlines.WriteXmlElement(writer);
        }
    }
}