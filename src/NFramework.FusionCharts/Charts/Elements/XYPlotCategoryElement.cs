using System;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// XY Plot Chart에서 사용하는 Category 정보
    /// </summary>
    [Serializable]
    public class XYPlotCategoryElement : CategoryElement {
        public int? X { get; set; }
        public bool? ShowVeriticalLine { get; set; }
        public bool? LineDashed { get; set; }

        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(X.HasValue)
                writer.WriteAttributeString("x", X.Value.ToString());
            if(ShowVeriticalLine.HasValue)
                writer.WriteAttributeString("showVerticalLine", ShowVeriticalLine.GetHashCode().ToString());
            if(LineDashed.HasValue)
                writer.WriteAttributeString("lineDashed", LineDashed.GetHashCode().ToString());
        }
    }
}