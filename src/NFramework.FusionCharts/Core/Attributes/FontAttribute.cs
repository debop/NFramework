using System;
using System.Drawing;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Font Attributes
    /// </summary>
    [Serializable]
    public class FontAttribute : ChartAttributeBase {
        /// <summary>
        /// Constructor
        /// </summary>
        public FontAttribute() : this(string.Empty) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="prefix">접두사</param>
        public FontAttribute(string prefix) {
            Prefix = (prefix ?? string.Empty).Trim();
        }

        internal string Prefix { get; set; }

        /// <summary>
        /// 폰트 명
        /// </summary>
        public string Font { get; set; }

        /// <summary>
        /// 폰트 색상
        /// </summary>
        public Color? FontColor { get; set; }

        /// <summary>
        /// 폰트 크기
        /// </summary>
        public string FontSize { get; set; }

        /// <summary>
        /// 굵게
        /// </summary>
        public bool? IsBold { get; set; }

        /// <summary>
        /// 기울기
        /// </summary>
        public bool? IsItalic { get; set; }

        /// <summary>
        /// 밑줄
        /// </summary>
        public bool? IsUnderline { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
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

            if(IsBold.HasValue)
                writer.WriteAttributeString(Prefix + "IsBold", IsBold.Value.GetHashCode().ToString());
            if(IsItalic.HasValue)
                writer.WriteAttributeString(Prefix + "IsItalic", IsItalic.Value.GetHashCode().ToString());
            if(IsUnderline.HasValue)
                writer.WriteAttributeString(Prefix + "IsUnderline", IsUnderline.Value.GetHashCode().ToString());
        }
    }
}