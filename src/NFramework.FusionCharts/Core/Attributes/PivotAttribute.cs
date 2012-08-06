using System;
using System.Drawing;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Pivot Attributes
    /// </summary>
    [Serializable]
    public class PivotAttribute : ChartAttributeBase {
        public int? Radius { get; set; }
        public Color? FillColor { get; set; }
        public int? FillAlpha { get; set; }
        public int? FillAngle { get; set; }
        public int? FillType { get; set; }

        /// <summary>
        /// Gradient fill mix formula
        /// </summary>
        public string FillMix { get; set; }

        /// <summary>
        /// Gradient fill ratio. (number separated by commas)
        /// </summary>
        public string FillRatio { get; set; }

        public bool? ShowPivotBorder { get; set; }
        public int? BorderThickness { get; set; }
        public Color? BorderColor { get; set; }
        public int? BorderAlpha { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Radius.HasValue)
                writer.WriteAttributeString("Radius", Radius.Value.ToString());
            if(FillColor.HasValue)
                writer.WriteAttributeString("FillColor", FillColor.Value.ToHexString());
            if(FillAlpha.HasValue)
                writer.WriteAttributeString("FillAlpha", FillAlpha.Value.ToString());
            if(FillAngle.HasValue)
                writer.WriteAttributeString("FillAngle", FillAngle.Value.ToString());
            if(FillType.HasValue)
                writer.WriteAttributeString("FillType", FillType.Value.ToString());
            if(FillMix.IsNotWhiteSpace())
                writer.WriteAttributeString("FillMix", FillMix);
            if(FillRatio.IsNotWhiteSpace())
                writer.WriteAttributeString("FillRatio", FillRatio);

            if(ShowPivotBorder.HasValue)
                writer.WriteAttributeString("ShowPivotBorder", ShowPivotBorder.Value.GetHashCode().ToString());
            if(BorderThickness.HasValue)
                writer.WriteAttributeString("BorderThickness", BorderThickness.Value.ToString());
            if(BorderColor.HasValue)
                writer.WriteAttributeString("BorderColor", BorderColor.Value.ToString());
            if(BorderAlpha.HasValue)
                writer.WriteAttributeString("BorderAlpha", BorderAlpha.Value.ToString());
        }
    }
}