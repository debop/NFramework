using System;
using System.Drawing;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Glow Style을 나타내는 클래스입니다.
    /// </summary>
    [Serializable]
    public class GlowStyle : ChartStyleElementBase {
        public GlowStyle(string name) : base(name, "Glow") {}

        /// <summary>
        /// The color of the shadow in hex code (without #). The default value is CCCCCC
        /// </summary>
        public Color? Color { get; set; }

        /// <summary>
        /// The alpha transparency value for the shadow color. Valid values are 0 to 100. For example, 25 sets a transparency value of 25%.
        /// </summary>
        public int? Alpha { get; set; }

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

            if(Color.HasValue)
                writer.WriteAttributeString("Color", Color.Value.ToHexString());
            if(Alpha.HasValue)
                writer.WriteAttributeString("Alpha", Alpha.Value.ToString());
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