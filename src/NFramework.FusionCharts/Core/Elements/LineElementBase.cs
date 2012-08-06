using System;
using System.Drawing;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Line 형태의 Element의 추상 클래스
    /// </summary>
    [Serializable]
    public abstract class LineElementBase : ChartElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        protected LineElementBase() : base("line") {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="elementName"></param>
        protected LineElementBase(string elementName) : base(elementName) {}

        /// <summary>
        /// Line ScrollColor
        /// </summary>
        public Color? Color { get; set; }

        /// <summary>
        /// Alpha 값 (0 ~ 100)
        /// </summary>
        public int? Alpha { get; set; }

        /// <summary>
        /// 라인 두께
        /// </summary>
        public int? Thickness { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Color.HasValue)
                writer.WriteAttributeString("Color", Color.Value.ToHexString());
            if(Alpha.HasValue)
                writer.WriteAttributeString("Alpha", Alpha.ToString());
            if(Thickness.HasValue)
                writer.WriteAttributeString("Thickness", Thickness.ToString());
        }
    }
}