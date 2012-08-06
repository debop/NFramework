namespace NSoft.NFramework.FusionCharts.Widgets {
    public class LineAnnotationElement : AnnotationElementBase {
        public override AnnotationType Type {
            get { return AnnotationType.Line; }
        }

        public bool? Dashed { get; set; }
        public int? DashLen { get; set; }
        public int? DashGap { get; set; }

        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Dashed.HasValue)
                writer.WriteAttributeString("Dashed", Dashed.GetHashCode().ToString());
            if(DashLen.HasValue)
                writer.WriteAttributeString("DashLen", DashLen.ToString());
            if(DashGap.HasValue)
                writer.WriteAttributeString("DashGap", DashGap.ToString());
        }
    }
}