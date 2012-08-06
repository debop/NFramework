using System;

namespace NSoft.NFramework.FusionCharts.Charts {
    [Serializable]
    public class XYPlotSetElement : XYSetElement {
        public XYPlotSetElement() {}
        public XYPlotSetElement(double x, double y) : base(x, y) {}
        public XYPlotSetElement(double x, double y, double z) : base(x, y, z) {}
    }

    //public class XYPlotSetElement : SetElementBase
    //{
    //    public double? X { get; set; }
    //    public double? Y { get; set; }
    //    public double? Z { get; set; }

    //    public override void GenerateXmlAttributes(System.Xml.XmlWriter writer)
    //    {
    //        base.GenerateXmlAttributes(writer);

    //        if (X.HasValue)
    //            writer.WriteAttributeString("x", X.ToString());
    //        if (Y.HasValue)
    //            writer.WriteAttributeString("y", Y.ToString());
    //        if (Z.HasValue)
    //            writer.WriteAttributeString("z", Z.ToString());
    //    }
    //}
}