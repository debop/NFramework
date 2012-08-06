using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// This element helps you define an annotation group, thus allowing you to group together a number of annotations.
    /// All the annotations in the chart have to be placed in an annotation group.
    /// A chart can have any number of annotation groups, and each group can have any number of annotations within.
    /// </summary>
    public class AnnotationGroupElement : CollectionElement<AnnotationElementBase> {
        public AnnotationGroupElement() : base("annotationGroup") {}

        public string Id { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
        public int? Alpha { get; set; }
        public int? XScale { get; set; }
        public int? YScale { get; set; }
        public int? OrigW { get; set; }
        public int? OrigH { get; set; }
        public bool? AutoScale { get; set; }
        public bool? ConstrainedScale { get; set; }
        public bool? ScaleImages { get; set; }
        public bool? ScaleText { get; set; }
        public int? GrpXShift { get; set; }
        public int? GrpYShift { get; set; }

        public bool? ShowBelow { get; set; }
        public bool? Visible { get; set; }

        public string ToolText { get; set; }

        private FusionLink _link;

        public FusionLink Link {
            get { return _link ?? (_link = new FusionLink()); }
            set { _link = value; }
        }

        /// <summary>
        /// 속성 중 Attribute Node로 표현해야 한다.
        /// </summary>
        /// <param name="writer"></param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Id.IsNotWhiteSpace())
                writer.WriteAttributeString("Id", Id);
            if(X.HasValue)
                writer.WriteAttributeString("x", X.ToString());
            if(Y.HasValue)
                writer.WriteAttributeString("y", Y.ToString());
            if(Alpha.HasValue)
                writer.WriteAttributeString("Alpha", Alpha.ToString());
            if(XScale.HasValue)
                writer.WriteAttributeString("XScale", XScale.ToString());
            if(YScale.HasValue)
                writer.WriteAttributeString("YScale", YScale.ToString());
            if(OrigW.HasValue)
                writer.WriteAttributeString("origW", OrigW.ToString());
            if(OrigH.HasValue)
                writer.WriteAttributeString("origH", OrigH.ToString());
            if(AutoScale.HasValue)
                writer.WriteAttributeString("AutoScale", AutoScale.GetHashCode().ToString());
            if(ConstrainedScale.HasValue)
                writer.WriteAttributeString("ConstrainedScale", ConstrainedScale.GetHashCode().ToString());
            if(ScaleImages.HasValue)
                writer.WriteAttributeString("ScaleImages", ScaleImages.GetHashCode().ToString());
            if(ScaleText.HasValue)
                writer.WriteAttributeString("ScaleText", ScaleText.GetHashCode().ToString());
            if(GrpXShift.HasValue)
                writer.WriteAttributeString("GrpXShift", GrpXShift.ToString());
            if(GrpYShift.HasValue)
                writer.WriteAttributeString("GrpXShift", GrpXShift.ToString());

            if(ShowBelow.HasValue)
                writer.WriteAttributeString("ShowBelow", ShowBelow.GetHashCode().ToString());
            if(Visible.HasValue)
                writer.WriteAttributeString("Visible", Visible.GetHashCode().ToString());

            if(ToolText.IsNotWhiteSpace())
                writer.WriteAttributeString("ToolText", ToolText);

            if(_link != null)
                _link.GenerateXmlAttributes(writer);
        }
    }
}