using System.Drawing;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Guage Chart의 기본 클래스입니다.
    /// </summary>
    public abstract class GaugeBase : WidgetBase {
        /// <summary>
        /// hether to show each dial's value?
        /// </summary>
        public bool? ShowValue { get; set; }

        /// <summary>
        /// Whether to show dial value below the pivot or above it?
        /// </summary>
        public bool? ValueBelowPivot { get; set; }

        /// <summary>
        /// Whether the gauge (color range scale) labels would be visible or not? 
        /// </summary>
        public bool? ShowGagueLabels { get; set; }

        /// <summary>
        /// Gradient fill formula for the scale.
        /// </summary>
        /// <example>
        /// <code>
        /// &lt;chart ... gaugeFillMix='{light-10},{light-30},{light-20},{dark-5},{color},{light-30},{light-20},{dark-10}' gaugeFillRatio='' ..&gt;
        /// </code>
        /// </example>
        public string GaugeFillMix { get; set; }

        /// <summary>
        /// Ratio fill mix for the scale.
        /// Ratio of each color separated by comma
        /// </summary>
        public string GaugeFillRatio { get; set; }

        /// <summary>
        /// Gauge fill color
        /// </summary>
        public Color? GaugeFillColor { get; set; }

        private BorderAttribute _border;

        public BorderAttribute Border {
            get { return _border ?? (_border = new BorderAttribute("gauge")); }
            set { _border = value; }
        }

        private GaugeAxisAttribute _axis;

        public GaugeAxisAttribute Axis {
            get { return _axis ?? (_axis = new GaugeAxisAttribute()); }
            set { _axis = value; }
        }

        private CollectionElement<ColorElement> _colorRange;

        public CollectionElement<ColorElement> ColorRange {
            get { return _colorRange ?? (_colorRange = new CollectionElement<ColorElement>("colorRange")); }
            set { _colorRange = value; }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(ShowValue.HasValue)
                writer.WriteAttributeString("showValue", ShowValue.GetHashCode().ToString());
            if(ValueBelowPivot.HasValue)
                writer.WriteAttributeString("valueBelowPivot", ValueBelowPivot.GetHashCode().ToString());

            if(ShowGagueLabels.HasValue)
                writer.WriteAttributeString("ShowGagueLabels", ShowGagueLabels.GetHashCode().ToString());
            if(GaugeFillMix.IsNotWhiteSpace())
                writer.WriteAttributeString("GaugeFillMix", GaugeFillMix);
            if(GaugeFillRatio.IsNotWhiteSpace())
                writer.WriteAttributeString("GaugeFillRatio", GaugeFillRatio);

            if(GaugeFillColor.HasValue)
                writer.WriteAttributeString("GaugeFillColor", GaugeFillColor.Value.ToHexString());

            if(_border != null)
                _border.GenerateXmlAttributes(writer);

            if(_axis != null)
                _axis.GenerateXmlAttributes(writer);
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_colorRange != null)
                _colorRange.WriteXmlElement(writer);
        }

        /// <summary>
        /// ColorRange에 <see cref="ColorElement"/>를 추가합니다.
        /// </summary>
        public ColorElement AddColor(int? min, int? max, Color? code, string label) {
            var color = new ColorElement
                        {
                            MinValue = min,
                            MaxValue = max,
                            Code = code,
                            Label = label
                        };
            ColorRange.Add(color);
            return color;
        }
    }
}