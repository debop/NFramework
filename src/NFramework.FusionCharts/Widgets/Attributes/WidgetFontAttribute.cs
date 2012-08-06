using System;
using System.Drawing;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Font Attributes. Widgets 에 있는 Chart중에는 IsBold, IsItalic, IsUnderline을 쓰는 것도 있지만, Is가 안 붙은 것도 있다. 
    /// 그래서 구분을 위해 따로 정의하였다.
    /// </summary>
    /// <seealso cref="FontAttribute"/>
    [Serializable]
    public class WidgetFontAttribute : ChartAttributeBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public WidgetFontAttribute() : this(string.Empty) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="prefix"></param>
        public WidgetFontAttribute(string prefix) {
            Prefix = (prefix ?? string.Empty).Trim();
        }

        protected string Prefix { get; set; }

        public string Font { get; set; }
        public Color? FontColor { get; set; }
        public string FontSize { get; set; }

        public bool? Bold { get; set; }
        public bool? Italic { get; set; }
        public bool? Underline { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML 속성으로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Font.IsNotWhiteSpace())
                writer.WriteAttributeString(Prefix + "Font", Font);
            if(FontColor.HasValue)
                writer.WriteAttributeString(Prefix + "FontColor", FontColor.Value.ToHexString());
            if(FontSize.IsNotWhiteSpace())
                writer.WriteAttributeString(Prefix + "FontSize", FontSize);

            if(Bold.HasValue)
                writer.WriteAttributeString(Prefix + "Bold", Bold.Value.GetHashCode().ToString());
            if(Italic.HasValue)
                writer.WriteAttributeString(Prefix + "Italic", Italic.Value.GetHashCode().ToString());
            if(Underline.HasValue)
                writer.WriteAttributeString(Prefix + "Underline", Underline.Value.GetHashCode().ToString());
        }
    }
}