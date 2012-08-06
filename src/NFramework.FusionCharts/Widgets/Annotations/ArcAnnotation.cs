namespace NSoft.NFramework.FusionCharts.Widgets {
    public class ArcAnnotation : RectangleAnnotation {
        public override AnnotationType Type {
            get { return AnnotationType.Arc; }
        }

        /// <summary>
        /// This attribute sets the number of sides that the required polygon would have. 
        /// For example, to draw a triangle we need to set it to 3. Similarly, to draw a pentagon, you can set it as 5.
        /// </summary>
        public int? Sides { get; set; }

        /// <summary>
        /// This attribute is used to set the starting angle for a circle, arc or polygon. 
        /// For example, if you need to draw a semi-circle or a semi-circular arc, then set startAngle="0" and endAngle="180" for the respective annotation. 
        /// Similarly for a polygon, if you need to draw a diamond shape, then set sides="4" and the startAngle="90".
        /// </summary>
        public int? StartAngle { get; set; }

        /// <summary>
        /// This attribute is used to set the ending angle for a circle or ar
        /// </summary>
        public int? EndAngle { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Sides.HasValue)
                writer.WriteAttributeString("Sides", Sides.ToString());
            if(StartAngle.HasValue)
                writer.WriteAttributeString("StartAngle", StartAngle.ToString());
            if(EndAngle.HasValue)
                writer.WriteAttributeString("EndAngle", EndAngle.ToString());
        }
    }
}