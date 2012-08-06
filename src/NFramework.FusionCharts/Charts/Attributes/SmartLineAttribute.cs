using System;
using System.Drawing;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// Smart labels/lines are data connector lines which connect the pie/doughnut slices to their respective labels without over-lapping even in cases where there are lots of labels located near each other.
    /// </summary>
    [Serializable]
    public class SmartLineAttribute : ChartAttributeBase {
        /// <summary>
        /// Whether to use smart labels or not. 
        /// </summary>
        public bool? EnableSmartLabels { get; set; }

        /// <summary>
        /// Whether to skip labels that are overlapping even when using smart labels. If not, they might overlap if there are too many labels. 
        /// </summary>
        public bool? SkipOverlapLabels { get; set; }

        /// <summary>
        /// The smart lines (smart label connector lines) can appear in two ways: Slanted or Straight. This attribute lets you choose between them. 
        /// </summary>
        public bool? IsSmartLineSlanted { get; set; }

        /// <summary>
        /// Color of smart label connector lines. 
        /// </summary>
        public Color? SmartLineColor { get; set; }

        /// <summary>
        /// Thickness of smart label connector lines. 
        /// </summary>
        public int? SmartLineThickness { get; set; }

        /// <summary>
        /// Alpha of smart label connector lines. 
        /// </summary>
        public int? SmartLineAlpha { get; set; }

        /// <summary>
        /// This attribute helps you set the distance of the label/value text boxes from the pie/doughnut edge. 
        /// </summary>
        public int? LabelDistance { get; set; }

        /// <summary>
        /// Clearance distance of a label (for sliced-in pies) from an adjacent sliced out pies. 
        /// </summary>
        public int? SmartLabelClearance { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML 속성으로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(EnableSmartLabels.HasValue)
                writer.WriteAttributeString("EnableSmartLabels", EnableSmartLabels.GetHashCode().ToString());
            if(SkipOverlapLabels.HasValue)
                writer.WriteAttributeString("SkipOverlapLabels", SkipOverlapLabels.GetHashCode().ToString());
            if(IsSmartLineSlanted.HasValue)
                writer.WriteAttributeString("IsSmartLineSlanted", IsSmartLineSlanted.GetHashCode().ToString());

            if(SmartLineColor.HasValue)
                writer.WriteAttributeString("SmartLineColor", SmartLineColor.Value.ToHexString());

            if(SmartLineThickness.HasValue)
                writer.WriteAttributeString("SmartLineThickness", SmartLineThickness.ToString());
            if(SmartLineAlpha.HasValue)
                writer.WriteAttributeString("SmartLineAlpha", SmartLineAlpha.ToString());
            if(LabelDistance.HasValue)
                writer.WriteAttributeString("LabelDistance", LabelDistance.ToString());
            if(SmartLabelClearance.HasValue)
                writer.WriteAttributeString("SmartLabelClearance", SmartLabelClearance.ToString());
        }
    }
}