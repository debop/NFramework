using System;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Line Attribute
    /// </summary>
    [Serializable]
    public class LineAttribute : LineAttributeBase {
        /// <summary>
        /// Default construcotr
        /// </summary>
        public LineAttribute() {}

        /// <summary>
        /// Construcotr
        /// </summary>
        /// <param name="prefix"></param>
        public LineAttribute(string prefix) : base(prefix) {}

        /// <summary>
        /// Dash로 보일 것인지 여부
        /// </summary>
        public bool? IsDashed { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(IsDashed.HasValue)
                writer.WriteAttributeString(Prefix + "IsDashed", IsDashed.Value.GetHashCode().ToString());
        }
    }
}