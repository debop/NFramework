using System;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// AlternateHGrid, AlternateVGrid
    /// </summary>
    [Serializable]
    public class AlternateGridAttribute : ColorAttribute {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="name"></param>
        public AlternateGridAttribute(string name) : base(name) {}

        protected string Name { get; set; }

        /// <summary>
        /// Whether to show alternate colored vertical grid bands.
        /// </summary>
        public bool? Show { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Show.HasValue)
                writer.WriteAttributeString("show" + Prefix + "Color", Show.GetHashCode().ToString());
        }
    }
}