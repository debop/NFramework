using System;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Blur Style
    /// </summary>
    [Serializable]
    public class BlurStyle : ChartStyleElementBase {
        public BlurStyle(string name) : base(name, "Blur") {}

        /// <summary>
        /// The amount of horizontal blur. Valid values are 0 to 255. The default value is 4. Values that are a power of 2 (such as 2, 4, 8, 16 and 32) are optimized to render more quickly than other values.
        /// </summary>
        public int? BlurX { get; set; }

        /// <summary>
        /// The amount of vertical blur. Valid values are 0 to 255. The default value is 4. Values that are a power of 2 (such as 2, 4, 8, 16 and 32) are optimized to render more quickly than other values.
        /// </summary>
        public int? BlurY { get; set; }

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

            if(BlurX.HasValue)
                writer.WriteAttributeString("BlurX", BlurX.Value.ToString());
            if(BlurY.HasValue)
                writer.WriteAttributeString("BlurY", BlurY.Value.ToString());

            if(Quality.HasValue)
                writer.WriteAttributeString("Quality", Quality.Value.ToString());
        }
    }
}