namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// LED Gauge 
    /// </summary>
    public class LEDGague : GaugeBase {
        /// <summary>
        /// This sets the size of each LED bar.
        /// </summary>
        public int? LedSize { get; set; }

        /// <summary>
        /// This sets the distance or the gap between two LED bars.
        /// </summary>
        public int? LedGap { get; set; }

        /// <summary>
        /// If you've defined color range for the chart, the LED bars take their fill color based on color range. But, if you want all the LED bars to use the color of the last color range (which matches current value), set this attribute to 1. 
        /// </summary>
        public bool? UseSameFillColor { get; set; }

        /// <summary>
        /// If you want all the background (non-activated) LED blocks to use the color of the last color range (which matches current value), set this attribute to 1. 
        /// </summary>
        public bool? UseSameFillBgColor { get; set; }

        /// <summary>
        /// 측정 값
        /// </summary>
        public double? Value { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(LedSize.HasValue)
                writer.WriteAttributeString("LedSize", LedSize.ToString());
            if(LedGap.HasValue)
                writer.WriteAttributeString("LedGap", LedGap.ToString());

            if(UseSameFillColor.HasValue)
                writer.WriteAttributeString("UseSameFillColor", UseSameFillColor.GetHashCode().ToString());
            if(UseSameFillBgColor.HasValue)
                writer.WriteAttributeString("UseSameFillBgColor", UseSameFillBgColor.GetHashCode().ToString());
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(System.Xml.XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(Value.HasValue)
                ChartUtil.WriteElementText(writer, "value", Value.ToString());
        }
    }
}