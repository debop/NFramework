using System;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// Zero plane is a plane which separates the positive and negative numbers on a chart having both of them
    /// </summary>
    [Serializable]
    public class ZeroPaneAttribute : LineAttributeBase {
        public ZeroPaneAttribute() : base("ZeroPlane") {}

        /// <summary>
        /// Whether to show zero plane or not.
        /// </summary>
        public bool? Show { get; set; }

        /// <summary>
        /// Whether to draw a mesh or not. If set to 1, a mesh on the zero plane of the chart will be drawn.
        /// </summary>
        public bool? Mesh { get; set; }

        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Show.HasValue)
                writer.WriteAttributeString("show" + Prefix, Show.Value.GetHashCode().ToString());

            if(Mesh.HasValue)
                writer.WriteAttributeString(Prefix + "Mesh", Mesh.Value.GetHashCode().ToString());
        }
    }
}