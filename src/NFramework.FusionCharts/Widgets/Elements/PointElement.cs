using System;
using System.Drawing;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Point 요소
    /// </summary>
    [Serializable]
    public class PointElement : ChartElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public PointElement() : base("point") {}

        public int? StartValue { get; set; }
        public int? EndValue { get; set; }
        public string DisplayValue { get; set; }
        public bool? ValueInside { get; set; }

        public Color? Color { get; set; }
        public int? Thickness { get; set; }

        public bool? ShowBorder { get; set; }
        public Color? BorderColor { get; set; }

        public int? Radius { get; set; }
        public int? InnerRadius { get; set; }

        public bool? Dashed { get; set; }
        public int? DashLen { get; set; }
        public int? DashGap { get; set; }

        public bool? UseMarker { get; set; }
        public Color? MarkerColor { get; set; }
        public Color? MarkerBorderColor { get; set; }
        public int? MarkerRadius { get; set; }
        public string MarkerTooltext { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(StartValue.HasValue)
                writer.WriteAttributeString("StartValue", StartValue.ToString());
            if(EndValue.HasValue)
                writer.WriteAttributeString("EndValue", EndValue.ToString());
            if(DisplayValue.IsNotWhiteSpace())
                writer.WriteAttributeString("DisplayValue", DisplayValue);
            if(ValueInside.HasValue)
                writer.WriteAttributeString("ValueInside", ValueInside.GetHashCode().ToString());

            if(Color.HasValue)
                writer.WriteAttributeString("Color", Color.Value.ToHexString());
            if(Thickness.HasValue)
                writer.WriteAttributeString("Thickness", Thickness.ToString());

            if(ShowBorder.HasValue)
                writer.WriteAttributeString("ShowBorder", ShowBorder.GetHashCode().ToString());
            if(BorderColor.HasValue)
                writer.WriteAttributeString("BorderColor", BorderColor.Value.ToHexString());

            if(Radius.HasValue)
                writer.WriteAttributeString("Radius", Radius.ToString());
            if(InnerRadius.HasValue)
                writer.WriteAttributeString("InnerRadius", InnerRadius.ToString());

            if(Dashed.HasValue)
                writer.WriteAttributeString("Dashed", Dashed.GetHashCode().ToString());
            if(DashLen.HasValue)
                writer.WriteAttributeString("DashLen", DashLen.ToString());
            if(DashGap.HasValue)
                writer.WriteAttributeString("DashGap", DashGap.ToString());

            if(UseMarker.HasValue)
                writer.WriteAttributeString("UseMarker", UseMarker.GetHashCode().ToString());
            if(MarkerColor.HasValue)
                writer.WriteAttributeString("MarkerColor", MarkerColor.Value.ToHexString());
            if(MarkerBorderColor.HasValue)
                writer.WriteAttributeString("MarkerBorderColor", MarkerBorderColor.Value.ToHexString());
            if(MarkerRadius.HasValue)
                writer.WriteAttributeString("MarkerRadius", MarkerRadius.ToString());
            if(MarkerTooltext.IsNotWhiteSpace())
                writer.WriteAttributeString("MarkerTooltext", MarkerTooltext);
        }
    }
}