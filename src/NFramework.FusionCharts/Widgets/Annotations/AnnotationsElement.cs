namespace NSoft.NFramework.FusionCharts.Widgets {
    public class AnnotationsElement : CollectionElement<AnnotationGroupElement> {
        public AnnotationsElement() : base("annotations") {}

        public int? OrigW { get; set; }
        public int? OrigH { get; set; }
        public bool? AutoScale { get; set; }
        public bool? ConstrainedScale { get; set; }
        public bool? ScaleImages { get; set; }
        public bool? ScaleText { get; set; }
        public int? XShift { get; set; }
        public int? YShift { get; set; }

        /// <summary>
        /// 속성 중 Attribute Node로 표현해야 한다.
        /// </summary>
        /// <param name="writer"></param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

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

            if(XShift.HasValue)
                writer.WriteAttributeString("XShift", XShift.ToString());
            if(YShift.HasValue)
                writer.WriteAttributeString("YShift", YShift.ToString());
        }
    }
}