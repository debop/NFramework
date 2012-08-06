using System;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Trendline element
    /// </summary>
    [Serializable]
    public class LineElement : LineElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public LineElement() : base("line") {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="elementName"></param>
        protected LineElement(string elementName) : base(elementName) {}

        /// <summary>
        /// 표시명
        /// </summary>
        public string DisplayValue { get; set; }

        /// <summary>
        /// Whether the trend would display a line, or a zone (filled colored rectangle). 
        /// </summary>
        public bool? IsTrendZone { get; set; }

        /// <summary>
        /// Is Dashed
        /// </summary>
        public bool? IsDashed { get; set; }

        /// <summary>
        /// 1 ~ 10
        /// </summary>
        public int? DashLen { get; set; }

        /// <summary>
        /// 1 ~ 10
        /// </summary>
        public int? DashGap { get; set; }

        /// <summary>
        /// ToolTip Text
        /// </summary>
        public string ToolText { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(DisplayValue.IsNotWhiteSpace())
                writer.WriteAttributeString("displayValue", DisplayValue);
            if(IsTrendZone.HasValue)
                writer.WriteAttributeString("isTrendZone", IsTrendZone.GetHashCode().ToString());
            if(IsDashed.HasValue)
                writer.WriteAttributeString("isDashed", IsDashed.GetHashCode().ToString());
            if(DashLen.HasValue)
                writer.WriteAttributeString("dashLen", DashLen.Value.ToString());
            if(DashGap.HasValue)
                writer.WriteAttributeString("dashGap", DashGap.Value.ToString());

            if(ToolText.IsNotWhiteSpace())
                writer.WriteAttributeString("toolText", ToolText);
        }
    }
}