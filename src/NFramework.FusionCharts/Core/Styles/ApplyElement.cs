using System;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Styles Element 밑에 적용할 Application에 대한 정보를 apply element로 표현한다.
    /// </summary>
    [Serializable]
    public class ApplyElement : ChartElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public ApplyElement() : base("apply") {}

        public string ToObject { get; set; }
        public string Styles { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(ToObject.IsNotWhiteSpace())
                writer.WriteAttributeString("ToObject", ToObject);
            if(Styles.IsNotWhiteSpace())
                writer.WriteAttributeString("Styles", Styles);
        }
    }
}