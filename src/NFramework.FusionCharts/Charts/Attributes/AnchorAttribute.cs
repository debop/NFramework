using System;
using System.Drawing;
using System.Xml;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// On line/area charts, anchors (or marker points) are polygons which appear at the joint of two consecutive lines/area points. These are indicators to show the position of data points. 
    /// The anchors handle tool tips and links for the data points. So, if you opt to not render anchors on a chart, the tool tips and links won't function. You can, however, hide them by setting alpha to 0 and still enable tool tips and links.
    /// </summary>
    [Serializable]
    public class AnchorAttribute : ChartAttributeBase {
        /// <summary>
        /// Whether to draw anchors on the chart. If the anchors are not shown, then the tool tip and links won't work.
        /// </summary>
        public bool? Draw { get; set; }

        /// <summary>
        /// 3 ~ 20
        /// This attribute sets the number of sides the anchor will have. For e.g., an anchor with 3 sides would represent a triangle, with 4 it would be a square and so on.
        /// </summary>
        public int? Sides { get; set; }

        /// <summary>
        /// In Pixels.
        /// This attribute sets the radius (in pixels) of the anchor. Greater the radius, bigger would be the anchor size.
        /// </summary>
        public int? Radius { get; set; }

        /// <summary>
        /// Lets you set the border color of anchors.
        /// </summary>
        public Color? BorderColor { get; set; }

        /// <summary>
        /// In Pixels.
        /// Helps you set border thickness of anchors.
        /// </summary>
        public int? BorderThickness { get; set; }

        /// <summary>
        /// the background color of anchors.
        /// </summary>
        public Color? BgColor { get; set; }

        /// <summary>
        /// 0 ~ 100
        /// the alpha of entire anchors. If you need to hide the anchors on chart but still enable tool tips, set this as 0.
        /// </summary>
        public int? Alpha { get; set; }

        /// <summary>
        /// 0 ~ 100
        /// the alpha of anchor background.
        /// </summary>
        public int? BgAlpha { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML 속성으로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Draw.HasValue)
                writer.WriteAttributeString("DrawAnchors", Draw.GetHashCode().ToString());
            if(Sides.HasValue)
                writer.WriteAttributeString("AnchorSides", Sides.ToString());
            if(Radius.HasValue)
                writer.WriteAttributeString("AnchorRadius", Radius.ToString());
            if(BorderColor.HasValue)
                writer.WriteAttributeString("AnchorBorderColor", BorderColor.Value.ToHexString());
            if(BorderThickness.HasValue)
                writer.WriteAttributeString("AnchorBorderThickness", BorderThickness.ToString());
            if(BgColor.HasValue)
                writer.WriteAttributeString("AnchorBgColor", BgColor.Value.ToHexString());
            if(Alpha.HasValue)
                writer.WriteAttributeString("AnchorAlpha", Alpha.ToString());
            if(BgAlpha.HasValue)
                writer.WriteAttributeString("AnchorBgAlpha", BgAlpha.ToString());
        }
    }
}