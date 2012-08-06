using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Powers {
    /// <summary>
    /// Each category element represents an x-axis data label
    /// </summary>
    public class CandleStickCategoryElement : ChartElementBase {
        public CandleStickCategoryElement()
            : base("category") {}

        /// <summary>
        /// The candle stick chart has both x and y axis as numeric. This attribute lets you define the x value (numerical position on the x-axis) where this category name would be placed.
        /// </summary>
        public double? X { get; set; }

        /// <summary>
        /// This attribute determines the label for the data item. The label appears on the x-axis of chart.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Whether the vertical line should appear as dashed?
        /// </summary>
        public bool? LineDashed { get; set; }

        /// <summary>
        /// Whether the vertical line should be shown for this category
        /// </summary>
        public bool? ShowVerticalLine { get; set; }

        /// <summary>
        /// You can individually opt to show/hide labels of individual data items using this attribute.
        /// </summary>
        public bool? ShowLabel { get; set; }

        /// <summary>
        /// The label of each category shows up on the x-axis. However, there might be cases where you want to display short label (or abbreviated label) on the axis and show the full label as tool tip. This attribute lets you specify the tool tip text for the label.
        /// </summary>
        public string ToolText { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(X.HasValue)
                writer.WriteAttributeString("x", X.ToString());
            if(Label.IsNotWhiteSpace())
                writer.WriteAttributeString("label", Label);
            if(LineDashed.HasValue)
                writer.WriteAttributeString("lineDashed", LineDashed.GetHashCode().ToString());
            if(ShowVerticalLine.HasValue)
                writer.WriteAttributeString("showVerticalLine", ShowVerticalLine.GetHashCode().ToString());
            if(ShowLabel.HasValue)
                writer.WriteAttributeString("showLabel", ShowLabel.GetHashCode().ToString());
            if(ToolText.IsNotWhiteSpace())
                writer.WriteAttributeString("toolText", ToolText);
        }
    }
}