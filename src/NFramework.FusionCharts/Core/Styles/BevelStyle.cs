using System;
using System.Drawing;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Bevel 스타일
    /// </summary>
    [Serializable]
    public class BevelStyle : ChartStyleElementBase {
        public BevelStyle(string name) : base(name, "Bevel") {}

        /// <summary>
        /// The offset distance for the shadow (in pixels). The default value is 4
        /// </summary>
        public int? Distance { get; set; }

        /// <summary>
        /// The angle of the shadow. Valid values are 0 to 360˚. The default value is 45
        /// </summary>
        public int? Angle { get; set; }

        /// <summary>
        /// The shadow color of the bevel. Valid values are in hexadecimal format RRGGBB (without #). The default value is 000000
        /// </summary>
        public Color? ShadowColor { get; set; }

        /// <summary>
        /// The alpha transparency value of the shadow color. This value is specified as a normalized value from 0 to 100. For example, 25 set a transparency value of 25%. The default value is 50.
        /// </summary>
        public int? ShadowAlpha { get; set; }

        /// <summary>
        /// The highlight color of the bevel. Valid values are in hexadecimal format RRGGBB (without #). The default value is FFFFFF
        /// </summary>
        public Color? HighlightColor { get; set; }

        /// <summary>
        /// The alpha transparency value of the highlight color. The value is specified as a normalized value from 0 to 100. For example, 25 sets a transparency value of 25%. The default value is 50.
        /// </summary>
        public int? HighlightAlpha { get; set; }

        /// <summary>
        /// The amount of horizontal blur. Valid values are 0 to 255. The default value is 4. Values that are a power of 2 (such as 2, 4, 8, 16 and 32) are optimized to render more quickly than other values.
        /// </summary>
        public int? BlurX { get; set; }

        /// <summary>
        /// The amount of vertical blur. Valid values are 0 to 255. The default value is 4. Values that are a power of 2 (such as 2, 4, 8, 16 and 32) are optimized to render more quickly than other values.
        /// </summary>
        public int? BlurY { get; set; }

        /// <summary>
        /// The strength of the imprint or spread. The higher the value, the more color is imprinted and the stronger the contrast between the shadow and the background. Valid values are from 0 to 255. The default is 1.
        /// </summary>
        public int? Strength { get; set; }

        /// <summary>
        /// The number of times to apply the shadow effect. Valid values are 0 to 15. 
        /// The default value is 1, which is equivalent to low quality. A value of 2 is medium quality, 
        /// and a value of 3 is high quality. Shadow with lower values are rendered quicker
        /// </summary>
        public int? Quality { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Distance.HasValue)
                writer.WriteAttributeString("Distance", Distance.Value.ToString());
            if(Angle.HasValue)
                writer.WriteAttributeString("Angle", Angle.Value.ToString());
            if(ShadowColor.HasValue)
                writer.WriteAttributeString("ShadowColor", ShadowColor.Value.ToHexString());
            if(ShadowAlpha.HasValue)
                writer.WriteAttributeString("ShadowAlpha", ShadowAlpha.Value.ToString());
            if(HighlightColor.HasValue)
                writer.WriteAttributeString("HighlightColor", HighlightColor.Value.ToHexString());
            if(HighlightAlpha.HasValue)
                writer.WriteAttributeString("HighlightAlpha", HighlightAlpha.Value.ToString());
            if(BlurX.HasValue)
                writer.WriteAttributeString("BlurX", BlurX.Value.ToString());
            if(BlurY.HasValue)
                writer.WriteAttributeString("BlurY", BlurY.Value.ToString());
            if(Strength.HasValue)
                writer.WriteAttributeString("Strength", Strength.Value.ToString());
            if(Quality.HasValue)
                writer.WriteAttributeString("Quality", Quality.Value.ToString());
        }
    }
}