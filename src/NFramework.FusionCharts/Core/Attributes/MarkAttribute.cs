using System;
using System.Drawing;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// 마크 속성
    /// </summary>
    [Serializable]
    public class MarkAttribute : ChartAttributeBase {
        public MarkAttribute() : this(string.Empty) {}

        public MarkAttribute(string prefix) {
            Prefix = (prefix ?? string.Empty).Trim();
        }

        protected string Prefix { get; private set; }

        public int? Number { get; set; }
        public Color? Color { get; set; }
        public int? Alpha { get; set; }
        public int? Height { get; set; }
        public int? Thickness { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Number.HasValue)
                writer.WriteAttributeString(Prefix + "Number", Number.Value.ToString());
            if(Color.HasValue)
                writer.WriteAttributeString(Prefix + "Color", Color.Value.ToHexString());
            if(Alpha.HasValue)
                writer.WriteAttributeString(Prefix + "Alpha", Alpha.Value.ToString());
            if(Height.HasValue)
                writer.WriteAttributeString(Prefix + "Height", Height.Value.ToString());
            if(Thickness.HasValue)
                writer.WriteAttributeString(Prefix + "Thickness", Thickness.Value.ToString());
        }
    }
}