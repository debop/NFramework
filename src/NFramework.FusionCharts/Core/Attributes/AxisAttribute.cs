using System;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Axis Attribute
    /// </summary>
    [Serializable]
    public class AxisAttribute : ChartAttributeBase {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public AxisAttribute() : this(string.Empty) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="prefix">axis의 접두사</param>
        public AxisAttribute(string prefix) {
            Prefix = (prefix ?? string.Empty).Trim();
        }

        /// <summary>
        /// 축의 접두사
        /// </summary>
        protected string Prefix { get; private set; }

        /// <summary>
        /// 축 이름
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 축 이름 폭
        /// </summary>
        public int? NameWidth { get; set; }

        /// <summary>
        /// 최소 값
        /// </summary>
        public double? MinValue { get; set; }

        /// <summary>
        /// 최대 값
        /// </summary>
        public double? MaxValue { get; set; }

        /// <summary>
        /// 축 이름을 회전시킬 것인가?
        /// </summary>
        public bool? RotateName { get; set; }

        /// <summary>
        /// 값 보기 간격
        /// </summary>
        public double? ValuesStep { get; set; }

        /// <summary>
        /// 값 보기 여부
        /// </summary>
        public bool? ShowValues { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Name.IsNotWhiteSpace())
                writer.WriteAttributeString(Prefix + "Name", Name);

            if(NameWidth.HasValue)
                writer.WriteAttributeString(Prefix + "NameWidth", NameWidth.ToString());
            if(MinValue.HasValue)
                writer.WriteAttributeString(Prefix + "MinValue", MinValue.ToString());
            if(MaxValue.HasValue)
                writer.WriteAttributeString(Prefix + "MaxValue", MaxValue.ToString());

            if(RotateName.HasValue)
                writer.WriteAttributeString("Rotate" + Prefix + "Name", RotateName.GetHashCode().ToString());
            if(ShowValues.HasValue)
                writer.WriteAttributeString("Show" + Prefix + "Values", ShowValues.GetHashCode().ToString());

            if(ValuesStep.HasValue)
                writer.WriteAttributeString(Prefix + "ValuesStep", ValuesStep.ToString());
        }
    }
}