using System;
using System.Drawing;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Legend Attribute
    /// </summary>
    [Serializable]
    public class LegendAttribute : ChartAttributeBase {
        public const string LegendPrefix = @"Legend";

        public bool? Show { get; set; }
        public string Caption { get; set; }

        public Color? BorderColor { get; set; }
        public int? BorderThickness { get; set; }
        public int? BorderAlpha { get; set; }

        public Color? BgColor { get; set; }
        public int? BgAlpha { get; set; }

        public bool? Shadow { get; set; }
        public bool? AllowDrag { get; set; }

        public Color? ScrollBgColor { get; set; }
        public Color? ScrollBarColor { get; set; }
        public Color? ScrollBtnColor { get; set; }

        public bool? Reverse { get; set; }
        public int? Padding { get; set; }

        public bool? MarkerCircle { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Show.HasValue)
                writer.WriteAttributeString("Show" + LegendPrefix, Show.Value.GetHashCode().ToString());
            if(Caption.IsNotWhiteSpace())
                writer.WriteAttributeString(LegendPrefix + "Caption", Caption);

            if(BorderColor.HasValue)
                writer.WriteAttributeString(LegendPrefix + "BorderColor", BorderColor.Value.ToHexString());
            if(BorderThickness.HasValue)
                writer.WriteAttributeString(LegendPrefix + "BorderThickness", BorderThickness.Value.ToString());
            if(BorderAlpha.HasValue)
                writer.WriteAttributeString(LegendPrefix + "BorderAlpha", BorderAlpha.Value.ToString());

            if(BgColor.HasValue)
                writer.WriteAttributeString(LegendPrefix + "BgColor", BgColor.Value.ToHexString());
            if(BgAlpha.HasValue)
                writer.WriteAttributeString(LegendPrefix + "BgAlpha", BgAlpha.Value.ToString());

            if(Shadow.HasValue)
                writer.WriteAttributeString(LegendPrefix + "Shadow", Shadow.Value.GetHashCode().ToString());
            if(AllowDrag.HasValue)
                writer.WriteAttributeString(LegendPrefix + "AllowDrag", AllowDrag.Value.GetHashCode().ToString());

            if(ScrollBgColor.HasValue)
                writer.WriteAttributeString(LegendPrefix + "ScrollBgColor", ScrollBgColor.Value.ToHexString());
            if(ScrollBarColor.HasValue)
                writer.WriteAttributeString(LegendPrefix + "ScrollBarColor", ScrollBarColor.Value.ToHexString());
            if(ScrollBtnColor.HasValue)
                writer.WriteAttributeString(LegendPrefix + "ScrollBtnColor", ScrollBtnColor.Value.ToHexString());

            if(Reverse.HasValue)
                writer.WriteAttributeString("Reverse" + LegendPrefix, Reverse.Value.GetHashCode().ToString());
            if(Padding.HasValue)
                writer.WriteAttributeString(LegendPrefix + "Padding", Padding.Value.ToString());

            if(MarkerCircle.HasValue)
                writer.WriteAttributeString(LegendPrefix + "MarkerCircle", MarkerCircle.GetHashCode().ToString());
        }
    }
}