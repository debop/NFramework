using System;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Each annotation in the chart is defined using this element. For every annotation on the chart, there needs to be an <annotation> element
    /// </summary>
    [Serializable]
    public abstract class AnnotationElementBase : ChartElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        protected AnnotationElementBase() : base("annotation") {}

        public abstract AnnotationType Type { get; }

        public int? X { get; set; }
        public int? Y { get; set; }
        public string Color { get; set; }
        public int? Alpha { get; set; }
        public int? Thickness { get; set; }

        public int? XScale { get; set; }
        public int? YScale { get; set; }
        public int? Radius { get; set; }
        public int? YRadius { get; set; }
        public int? InnerRadius { get; set; }

        public int? ToX { get; set; }
        public int? ToY { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            writer.WriteAttributeString("type", Type.ToString());

            if(X.HasValue)
                writer.WriteAttributeString("x", X.ToString());
            if(Y.HasValue)
                writer.WriteAttributeString("y", Y.ToString());
            if(Color.IsNotWhiteSpace())
                writer.WriteAttributeString("Color", Color);
            if(Alpha.HasValue)
                writer.WriteAttributeString("Alpha", Alpha.ToString());
            if(Thickness.HasValue)
                writer.WriteAttributeString("Thickness", Thickness.ToString());

            if(XScale.HasValue)
                writer.WriteAttributeString("XScale", XScale.ToString());
            if(YScale.HasValue)
                writer.WriteAttributeString("YScale", YScale.ToString());
            if(Radius.HasValue)
                writer.WriteAttributeString("Radius", Radius.ToString());
            if(YRadius.HasValue)
                writer.WriteAttributeString("YRadius", YRadius.ToString());
            if(InnerRadius.HasValue)
                writer.WriteAttributeString("InnerRadius", InnerRadius.ToString());

            if(ToX.HasValue)
                writer.WriteAttributeString("ToX", ToX.ToString());
            if(ToY.HasValue)
                writer.WriteAttributeString("ToY", ToY.ToString());
        }
    }
}