using System;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Angular Guage에서 사용하는 Dial 정보
    /// </summary>
    [Serializable]
    public class DialElement : ChartElementBase {
        public DialElement() : base("dial") {}

        /// <summary>
        /// Each dial can have a unique ID, using which it can be referred to in JavaScript and real-time data updates.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Numerical value for the dial that will be shown on the chart.
        /// </summary>
        public double? Value { get; set; }

        /// <summary>
        /// Whether to show value for this dial?
        /// </summary>
        public bool? ShowValue { get; set; }

        /// <summary>
        /// Whether to place value textbox x-position?
        /// </summary>
        public double? ValueX { get; set; }

        /// <summary>
        /// Whether to place value textbox y-position?
        /// </summary>
        public double? ValueY { get; set; }

        /// <summary>
        /// Whether this dial would be editable?
        /// </summary>
        public bool? EditMode { get; set; }

        private BorderAttribute _border;

        /// <summary>
        /// Border Attributes
        /// </summary>
        public BorderAttribute Border {
            get { return _border ?? (_border = new BorderAttribute()); }
            set { _border = value; }
        }

        /// <summary>
        /// List of colors separated by comma.
        /// Background color for dial. You can either specify a single color (e.g., CCCCCC) for solid fill or multiple colors separated by comma to attain a gradient fill.
        /// </summary>
        public string BgColor { get; set; }

        /// <summary>
        /// Radius for the dial (in pixels).
        /// </summary>
        public int? Radius { get; set; }

        /// <summary>
        /// Width of the bottom part of the dial (the part connected to pivot).
        /// </summary>
        public int? BaseWidth { get; set; }

        /// <summary>
        /// Width of the top part of dial.
        /// </summary>
        public int? TopWidth { get; set; }

        /// <summary>
        /// If you want the dial to extend beyond the pivot (in opposite side) for more realistic looks, you can set the extension distance (in pixels) using this attribute.
        /// </summary>
        public int? RearExtension { get; set; }

        /// <summary>
        /// Each dial can have custom tool text using this attribute.
        /// </summary>
        public string ToolText { get; set; }

        private FusionLink _link;

        /// <summary>
        /// Each dial can be linked to an external URL using this attribute.
        /// </summary>
        public FusionLink Link {
            get { return _link ?? (_link = new FusionLink()); }
            set { _link = value; }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Id.IsNotWhiteSpace())
                writer.WriteAttributeString("Id", Id);
            if(Value.HasValue)
                writer.WriteAttributeString("Value", Value.ToString());

            if(ShowValue.HasValue)
                writer.WriteAttributeString("ShowValue", ShowValue.GetHashCode().ToString());
            if(ValueX.HasValue)
                writer.WriteAttributeString("ValueX", ValueX.ToString());
            if(ValueY.HasValue)
                writer.WriteAttributeString("ValueY", ValueY.ToString());
            if(EditMode.HasValue)
                writer.WriteAttributeString("EditMode", EditMode.GetHashCode().ToString());

            if(_border != null)
                _border.GenerateXmlAttributes(writer);

            if(BgColor.IsNotWhiteSpace())
                writer.WriteAttributeString("BgColor", BgColor);
            if(Radius.HasValue)
                writer.WriteAttributeString("Radius", Radius.ToString());
            if(BaseWidth.HasValue)
                writer.WriteAttributeString("BaseWidth", BaseWidth.ToString());
            if(TopWidth.HasValue)
                writer.WriteAttributeString("TopWidth", TopWidth.ToString());
            if(RearExtension.HasValue)
                writer.WriteAttributeString("RearExtension", RearExtension.ToString());
            if(ToolText.IsNotWhiteSpace())
                writer.WriteAttributeString("ToolText", ToolText);

            if(_link != null)
                _link.GenerateXmlAttributes(writer);
        }
    }
}