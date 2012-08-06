using System;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// Pie Chart의 고유 Attribute
    /// </summary>
    [Serializable]
    public class PieAttribute : ChartAttributeBase {
        /// <summary>
        /// Pixels. 
        /// If you've opted to slice a particular pie/doughnut slice, using this attribute you can control the distance between the slice and the center of chart. 
        /// </summary>
        public int? SlicingDistance { get; set; }

        /// <summary>
        /// Pixels. 
        /// This attribute lets you explicitly set the outer radius of the chart. FusionCharts automatically calculates the best fit pie radius for the chart. This attribute is useful if you want to enforce one of your own values. 
        /// </summary>
        public int? PieRadius { get; set; }

        /// <summary>
        /// 0 ~ 360.
        /// By default, the pie chart starts from angle 0 i.e., the first pie starts plotting from 0 angle. If you want to change the starting angle of the chart, use this attribute. 
        /// </summary>
        public int? StartingAngle { get; set; }

        /// <summary>
        /// The doughnut/pie charts have three modes: Slicing, Rotation and Link. If any links are defined, the chart works in Link mode. Otherwise, it starts in Slicing mode. If you need to enable rotation by default, set this attribute to 1. 
        /// </summary>
        public bool? EnableRotation { get; set; }

        /// <summary>
        /// 0 ~ 100.
        /// Alpha of the pie inner face 
        /// </summary>
        public int? PieInnerFaceAlpha { get; set; }

        /// <summary>
        /// 0 ~ 100.
        /// Alpha of the pie outer face 
        /// </summary>
        public int? PieOuterFaceAlpha { get; set; }

        /// <summary>
        /// 30 ~ 80. 
        /// This attribute alters the y-perspective of the pie in percentage figures. 100 percent means the full pie face is visible and 0 percent means only the side face is visible. 
        /// </summary>
        public int? PieYScale { get; set; }

        /// <summary>
        /// In Pixels. This attribute controls the pie 3D Depth. 
        /// </summary>
        public int? PieSliceDepth { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML 속성으로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(SlicingDistance.HasValue)
                writer.WriteAttributeString("SlicingDistance", SlicingDistance.ToString());
            if(PieRadius.HasValue)
                writer.WriteAttributeString("PieRadius", PieRadius.ToString());
            if(StartingAngle.HasValue)
                writer.WriteAttributeString("StartingAngle", StartingAngle.ToString());
            if(EnableRotation.HasValue)
                writer.WriteAttributeString("EnableRotation", EnableRotation.GetHashCode().ToString());
            if(PieInnerFaceAlpha.HasValue)
                writer.WriteAttributeString("PieInnerFaceAlpha", PieInnerFaceAlpha.ToString());
            if(PieOuterFaceAlpha.HasValue)
                writer.WriteAttributeString("PieOuterFaceAlpha", PieOuterFaceAlpha.ToString());
            if(PieYScale.HasValue)
                writer.WriteAttributeString("PieYScale", PieYScale.ToString());
            if(PieSliceDepth.HasValue)
                writer.WriteAttributeString("PieSliceDepth", PieSliceDepth.ToString());
        }
    }
}