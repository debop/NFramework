using System;
using System.Drawing;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Font Style 을 나타내는 클래스입니다.
    /// </summary>
    [Serializable]
    public class FontStyle : ChartStyleElementBase {
        public FontStyle(string name) : base(name, "Font") {}

        public string Font { get; set; }
        public int? Size { get; set; }
        public Color? Color { get; set; }
        public bool? Bold { get; set; }
        public bool? Italic { get; set; }
        public bool? Underline { get; set; }

        public Color? BgColor { get; set; }
        public Color? BorderColor { get; set; }

        /// <summary>
        /// Helps you set whether the text should be rendered as HTML or plain text.
        /// </summary>
        public bool? IsHTML { get; set; }

        /// <summary>
        /// The left margin of the text, in points
        /// </summary>
        public int? LeftMargin { get; set; }

        /// <summary>
        /// The amount of space that is uniformly distributed between characters of the text
        /// </summary>
        public int? LetterSpacing { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Font.IsNotWhiteSpace())
                writer.WriteAttributeString("Font", Font);
            if(Size.HasValue)
                writer.WriteAttributeString("Size", Size.Value.ToString());
            if(Color.HasValue)
                writer.WriteAttributeString("Color", Color.Value.ToHexString());
            if(Bold.HasValue)
                writer.WriteAttributeString("Bold", Bold.Value.GetHashCode().ToString());
            if(Italic.HasValue)
                writer.WriteAttributeString("Italic", Italic.Value.GetHashCode().ToString());
            if(Underline.HasValue)
                writer.WriteAttributeString("Underline", Underline.Value.GetHashCode().ToString());

            if(BgColor.HasValue)
                writer.WriteAttributeString("BgColor", BgColor.Value.ToHexString());
            if(BorderColor.HasValue)
                writer.WriteAttributeString("BorderColor", BorderColor.Value.ToHexString());

            if(IsHTML.HasValue)
                writer.WriteAttributeString("IsHTML", IsHTML.GetHashCode().ToString());

            if(LeftMargin.HasValue)
                writer.WriteAttributeString("LeftMargin", LeftMargin.Value.GetHashCode().ToString());
            if(LetterSpacing.HasValue)
                writer.WriteAttributeString("LetterSpacing", LetterSpacing.Value.ToString());
        }
    }
}