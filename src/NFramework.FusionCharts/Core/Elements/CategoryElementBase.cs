using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Category Element의 기본 클래스
    /// </summary>
    public abstract class CategoryElementBase : ChartElementBase {
        protected CategoryElementBase() : base("category") {}

        public bool? ShowLabel { get; set; }
        public string ToolText { get; set; }
        public string Label { get; set; }
        public bool? ShowLabelBorder { get; set; }

        public HorizontalPosition? LabelPosition { get; set; }
        public FusionTextAlign? LabelHAlign { get; set; }
        public FusionVerticalAlign? LabelVAlign { get; set; }

        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(ShowLabel.HasValue)
                writer.WriteAttributeString("showLabel", ShowLabel.GetHashCode().ToString());
            if(ToolText.IsNotWhiteSpace())
                writer.WriteAttributeString("toolText", ToolText);
            if(Label.IsNotWhiteSpace())
                writer.WriteAttributeString("label", Label);

            if(ShowLabelBorder.HasValue)
                writer.WriteAttributeString("showLabelBorder", ShowLabelBorder.GetHashCode().ToString());

            if(LabelPosition.HasValue)
                writer.WriteAttributeString("LabelPosition", LabelPosition.GetHashCode().ToString());
            if(LabelHAlign.HasValue)
                writer.WriteAttributeString("LabelHAlign", LabelHAlign.GetHashCode().ToString());
            if(LabelVAlign.HasValue)
                writer.WriteAttributeString("LabelVAlign", LabelVAlign.GetHashCode().ToString());
        }
    }
}