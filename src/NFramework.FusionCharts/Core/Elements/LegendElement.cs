using System;
using System.Drawing;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Legend Element
    /// </summary>
    [Serializable]
    public class LegendElement : ChartElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public LegendElement() : base("legend") {}

        /// <summary>
        /// 라벨
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 색
        /// </summary>
        public Color? Color { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Label.IsNotWhiteSpace())
                writer.WriteAttributeString("Label", Label);
            if(Color.HasValue)
                writer.WriteAttributeString("Color", Color.Value.ToHexString());
        }
    }
}