using System.Drawing;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Color, Alpha Attribute
    /// </summary>
    public abstract class ColorAttribute : ChartAttributeBase {
        /// <summary>
        /// Default constructor
        /// </summary>
        protected ColorAttribute() : this(string.Empty) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="prefix">접두사</param>
        protected ColorAttribute(string prefix) {
            Prefix = (prefix ?? string.Empty).Trim();
        }

        /// <summary>
        /// 접두사
        /// </summary>
        protected string Prefix { get; set; }

        /// <summary>
        /// Color
        /// </summary>
        public Color? Color { get; set; }

        /// <summary>
        /// Alpha
        /// </summary>
        public int? Alpha { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Color.HasValue)
                writer.WriteAttributeString(Prefix + "Color", Color.Value.ToHexString());

            if(Alpha.HasValue)
                writer.WriteAttributeString(Prefix + "Alpha", Alpha.Value.ToString());
        }
    }
}