namespace NSoft.NFramework.FusionCharts.Powers {
    /// <summary>
    /// Category element for Drag Node Chart
    /// </summary>
    public class DragNodeCategoryElement : CategoryElementBase {
        public int? X { get; set; }
        public bool? LineDashed { get; set; }
        public bool? ShowVerticalLine { get; set; }

        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(X.HasValue)
                writer.WriteAttributeString("X", X.ToString());

            if(LineDashed.HasValue)
                writer.WriteAttributeString("LineDashed", LineDashed.Value.GetHashCode().ToString());
            if(ShowVerticalLine.HasValue)
                writer.WriteAttributeString("ShowVerticalLine", ShowVerticalLine.Value.GetHashCode().ToString());
        }
    }
}