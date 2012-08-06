using System.Drawing;
using System.Xml;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Line 속성을 나타내는 클래스의 추상 클래스입니다.
    /// </summary>
    public abstract class LineAttributeBase : ChartAttributeBase {
        /// <summary>
        /// Default constructor
        /// </summary>
        protected LineAttributeBase() : this("line") {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="prefix"></param>
        protected LineAttributeBase(string prefix) {
            Prefix = (prefix ?? string.Empty).Trim();
        }

        protected string Prefix { get; private set; }

        /// <summary>
        /// Line ScrollColor
        /// </summary>
        public Color? Color { get; set; }

        /// <summary>
        /// Alpha 값 (0 ~ 100)
        /// </summary>
        public int? Alpha { get; set; }

        /// <summary>
        ///  라인 두께
        /// </summary>
        public int? Thickness { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Color.HasValue)
                writer.WriteAttributeString(Prefix + "Color", Color.Value.ToHexString());
            if(Alpha.HasValue)
                writer.WriteAttributeString(Prefix + "Alpha", Alpha.Value.ToString());
            if(Thickness.HasValue)
                writer.WriteAttributeString(Prefix + "Thickness", Thickness.Value.ToString());
        }
    }
}