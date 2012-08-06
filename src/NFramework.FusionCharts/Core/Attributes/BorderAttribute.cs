using System;
using System.Drawing;
using System.Xml;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Border Attributes
    /// </summary>
    [Serializable]
    public class BorderAttribute : ChartAttributeBase {
        /// <summary>
        /// Default constructor
        /// </summary>
        public BorderAttribute() : this(string.Empty) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="prefix"></param>
        public BorderAttribute(string prefix) {
            Prefix = (prefix ?? string.Empty).Trim();
        }

        public string Prefix { get; private set; }

        public bool? Show { get; set; }
        public Color? Color { get; set; }
        public int? Thickness { get; set; }
        public int? Alpha { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);
            // var hasPrefix = Prefix.IsNotWhiteSpace();

            if(Show.HasValue)
                writer.WriteAttributeString("show" + Prefix + "Border", Show.Value.GetHashCode().ToString());

            if(Color.HasValue)
                writer.WriteAttributeString(Prefix + "BorderColor", Color.Value.ToHexString());

            if(Thickness.HasValue)
                writer.WriteAttributeString(Prefix + "BorderThickness", Thickness.Value.ToString());

            if(Alpha.HasValue)
                writer.WriteAttributeString(Prefix + "BorderAlpha", Alpha.Value.ToString());
        }
    }
}