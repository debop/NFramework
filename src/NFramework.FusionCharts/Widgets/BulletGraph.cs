using System.Drawing;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Bullet Graph
    /// </summary>
    public class BulletGraph : GaugeBase {
        /// <summary>
        /// Whether to plot the value as bar or as dot? 
        /// </summary>
        public bool? PlotAsDot { get; set; }

        /// <summary>
        /// Height percent of the color range that the plot fill bar should occupy? (0 ~ 100)
        /// </summary>
        public int? PlotFillPercent { get; set; }

        /// <summary>
        ///	Height percent of the color range that the plot fill bar should occupy? (0 ~ 100)
        /// </summary>
        public int? TargetFillPercent { get; set; }

        /// <summary>
        /// Fill color for the plot bar. 
        /// </summary>
        public Color? PlotFillColor { get; set; }

        /// <summary>
        /// Fill alpha for the plot bar.
        /// </summary>
        public int? PlotFillAlpha { get; set; }

        private BorderAttribute _plotBorder;

        public BorderAttribute PlotBorder {
            get { return _plotBorder ?? (_plotBorder = new BorderAttribute("plot")); }
            set { _plotBorder = value; }
        }

        public Color? TargetColor { get; set; }
        public int? TargetThickness { get; set; }

        private ColorRangeAttribute _colorRangeAttr;

        public ColorRangeAttribute ColorRangeAttr {
            get { return _colorRangeAttr ?? (_colorRangeAttr = new ColorRangeAttribute()); }
            set { _colorRangeAttr = value; }
        }

        public double? Value { get; set; }
        public double? Target { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(PlotAsDot.HasValue)
                writer.WriteAttributeString("PlotAsDot", PlotAsDot.GetHashCode().ToString());
            if(PlotFillPercent.HasValue)
                writer.WriteAttributeString("PlotFillPercent", PlotFillPercent.ToString());
            if(TargetFillPercent.HasValue)
                writer.WriteAttributeString("TargetFillPercent", TargetFillPercent.ToString());
            if(PlotFillColor.HasValue)
                writer.WriteAttributeString("PlotFillColor", PlotFillColor.Value.ToHexString());
            if(PlotFillAlpha.HasValue)
                writer.WriteAttributeString("PlotFillAlpha", PlotFillAlpha.ToString());

            if(_plotBorder != null)
                _plotBorder.GenerateXmlAttributes(writer);

            if(TargetColor.HasValue)
                writer.WriteAttributeString("TargetColor", TargetColor.Value.ToHexString());
            if(TargetThickness.HasValue)
                writer.WriteAttributeString("TargetThickness", TargetThickness.ToString());

            if(_colorRangeAttr != null)
                _colorRangeAttr.GenerateXmlAttributes(writer);
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(Value.HasValue)
                ChartUtil.WriteElementText(writer, "value", Value.ToString());
            if(Target.HasValue)
                ChartUtil.WriteElementText(writer, "target", Target.ToString());
        }

        /// <summary>
        /// 색상 범위를 나타내는 특성 클래스 입니다.
        /// </summary>
        public class ColorRangeAttribute : ChartAttributeBase {
            public ColorRangeAttribute() : this("colorRange") {}

            public ColorRangeAttribute(string prefix) {
                Prefix = (prefix ?? string.Empty).Trim();
            }

            protected string Prefix { get; private set; }

            public string FillMix { get; set; }
            public int? FillRatio { get; set; }
            private BorderAttribute _border;

            public BorderAttribute Border {
                get { return _border ?? (_border = new BorderAttribute(Prefix)); }
                set { _border = value; }
            }

            /// <summary>
            /// Chart 설정 또는 변량에 대해 XML 속성으로 생성합니다.
            /// </summary>
            /// <param name="writer">xml writer</param>
            public override void GenerateXmlAttributes(XmlWriter writer) {
                base.GenerateXmlAttributes(writer);

                if(FillMix.IsNotWhiteSpace())
                    writer.WriteAttributeString(Prefix + "FillMix", FillMix);
                if(FillRatio.HasValue)
                    writer.WriteAttributeString(Prefix + "FillRatio", FillRatio.ToString());

                if(_border != null)
                    _border.GenerateXmlAttributes(writer);
            }
        }
    }
}