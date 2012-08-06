using System;
using System.Drawing;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// 범례 요소
    /// </summary>
    [Serializable]
    public class LegendItemElement : ChartElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="label"></param>
        public LegendItemElement(string label) : this(label, null) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="label"></param>
        /// <param name="color"></param>
        public LegendItemElement(string label, Color? color)
            : base("item") {
            Label = label;
            Color = color;
        }

        public string Label { get; set; }
        public Color? Color { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer"></param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Label.IsNotWhiteSpace())
                writer.WriteAttributeString("Label", Label);

            if(Color.HasValue)
                writer.WriteAttributeString("Color", Color.Value.ToHexString());
        }
    }
}