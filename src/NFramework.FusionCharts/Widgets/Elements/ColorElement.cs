using System.Drawing;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Guage 등에서 사용하는 구간을 색으로 나타낼 때 사용한다.
    /// </summary>
    /// <seealso cref="ColorRangeElement"/>
    public class ColorElement : ChartElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public ColorElement() : base("color") {}

        /// <summary>
        /// Indicates the starting value of that color range. The minValue of the first color range should be equal to the chart's lowerLimit, and the minValue of the succeeding &lt;color&gt; element should be equal to the current &lt;color&gt; element's maxValue.
        /// </summary>
        public int? MinValue { get; set; }

        /// <summary>
        /// Indicates the end value of that color range. The maxValue of the last color range should be equal to the chart's upperLimit.
        /// </summary>
        public int? MaxValue { get; set; }

        /// <summary>
        /// 라벨
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Actual color (hex color) of the color range using which it will be filled.
        /// </summary>
        public Color? Code { get; set; }

        /// <summary>
        /// Sets the transparency of each range in the color range.
        /// </summary>
        public int? Alpha { get; set; }

        /// <summary>
        /// Border color of that particular color in the range. If you want to specify a different border for each range in the color range, you can use this attribute.
        /// </summary>
        public Color? BorderColor { get; set; }

        /// <summary>
        /// Border alpha for the color range.
        /// </summary>
        public int? BorderAlpha { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(MinValue.HasValue)
                writer.WriteAttributeString("minValue", MinValue.ToString());
            if(MaxValue.HasValue)
                writer.WriteAttributeString("maxValue", MaxValue.ToString());

            if(Label.IsNotWhiteSpace())
                writer.WriteAttributeString("label", Label);

            if(Code.HasValue)
                writer.WriteAttributeString("Code", Code.Value.ToHexString());
            if(Alpha.HasValue)
                writer.WriteAttributeString("Alpha", Alpha.ToString());
            if(BorderColor.HasValue)
                writer.WriteAttributeString("BorderColor", BorderColor.Value.ToHexString());
            if(BorderAlpha.HasValue)
                writer.WriteAttributeString("BorderAlpha", BorderAlpha.ToString());
        }
    }
}