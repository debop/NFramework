using System;
using System.Xml;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// 값이 double 수형인 라인의 요소를 나타내는 클래스입니다.
    /// </summary>
    [Serializable]
    public class DoubleLineElement : LineElement {
        /// <summary>
        /// The starting value for the trendline. Say, if you want to plot a slanted trendline from value 102 to 109, the startValue would be 102.
        /// </summary>
        public double? StartValue { get; set; }

        /// <summary>
        /// The ending y-axis value for the trendline. Say, if you want to plot a slanted trendline from value 102 to 109, the endValue would be 109. If you do not specify a value for endValue, it would automatically assume the same value as startValue.
        /// </summary>
        public double? EndValue { get; set; }

        /// <summary>
        /// 최상위에 나타낼 것인가?
        /// </summary>
        public bool? ShowOnTop { get; set; }

        /// <summary>
        /// Value 값을 오른쪽에 나타낼 것인가?
        /// </summary>
        public bool? ValueOnRight { get; set; }

        /// <summary>
        /// 속성 중 Attribute Node로 표현해야 한다.
        /// </summary>
        /// <param name="writer"></param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(StartValue.HasValue)
                writer.WriteAttributeString("startValue", StartValue.ToString());
            if(EndValue.HasValue)
                writer.WriteAttributeString("endValue", EndValue.ToString());

            if(ShowOnTop.HasValue)
                writer.WriteAttributeString("showOnTop", ShowOnTop.GetHashCode().ToString());
            if(ValueOnRight.HasValue)
                writer.WriteAttributeString("valueOnRight", ValueOnRight.GetHashCode().ToString());
        }
    }
}