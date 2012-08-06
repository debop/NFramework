using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Powers {
    /// <summary>
    /// Each &lt;trendset&gt; element defines a new trend-set line on the chart. You can have any number of trend-sets on the chart, each displaying a different indicator.
    /// </summary>
    public class TrendSetElement : CollectionElement<TrendSetElement.SetElement> {
        public TrendSetElement() : base("trendset") {}

        /// <summary>
        /// Name of the trend-set e.g., "Simple Moving Average", "50 Day Average" etc. This name will be displayed in the legend of the chart.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Whether to include the name of this trend set in legend?
        /// </summary>
        public bool? IncludeInLegend { get; set; }

        private DashLineAttribute _lineAttr;

        public DashLineAttribute LineAttr {
            get { return _lineAttr ?? (_lineAttr = new DashLineAttribute()); }
            set { _lineAttr = value; }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Name.IsNotWhiteSpace())
                writer.WriteAttributeString("name", Name);
            if(IncludeInLegend.HasValue)
                writer.WriteAttributeString("IncludeInLegend", IncludeInLegend.GetHashCode().ToString());

            if(_lineAttr != null)
                _lineAttr.GenerateXmlAttributes(writer);
        }

        public class SetElement : ChartElementBase {
            public SetElement() : base("set") {}

            public double? X { get; set; }
            public double? Value { get; set; }

            public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
                base.GenerateXmlAttributes(writer);

                if(X.HasValue)
                    writer.WriteAttributeString("x", X.ToString());
                if(Value.HasValue)
                    writer.WriteAttributeString("value", Value.ToString());
            }
        }

        public SetElement AddSet(double x, double value) {
            var set = new SetElement
                      {
                          X = x,
                          Value = value
                      };

            Add(set);
            return set;
        }
    }
}