using System;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// Double 수형을 값으로 나타내는 Set Element
    /// </summary>
    [Serializable]
    public class ValueSetElement : SetElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public ValueSetElement() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="elementName"></param>
        public ValueSetElement(string elementName) : base(elementName) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="value"></param>
        public ValueSetElement(double? value) {
            Value = value;
        }

        /// <summary>
        /// 변량 값
        /// </summary>
        public double? Value { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Value.HasValue)
                writer.WriteAttributeString("value", Value.ToString());
        }
    }
}