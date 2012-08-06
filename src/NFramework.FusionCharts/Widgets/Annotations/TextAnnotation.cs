using System.Drawing;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Text Annotation
    /// </summary>
    public class TextAnnotation : AnnotationElementBase {
        #region Overrides of AnnotationElementBase

        public override AnnotationType Type {
            get { return AnnotationType.Text; }
        }

        #endregion

        /// <summary>
        /// Herein, you specify the text that you want to displayed using the text annotation.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// This attribute sets the horizontal alignment of the text. Default value: center.
        /// </summary>
        public FusionTextAlign? Align { get; set; }

        /// <summary>
        /// This attribute sets the vertical alignment of the text. Default value: middle.
        /// </summary>
        public FusionVerticalAlign? VAlign { get; set; }

        private WidgetFontAttribute _fontAttr;

        public WidgetFontAttribute FontAttr {
            get { return _fontAttr ?? (_fontAttr = new WidgetFontAttribute()); }
            set { _fontAttr = value; }
        }

        /// <summary>
        /// The letter spacing for the text. Default value: 0.
        /// </summary>
        public int? LetterSpacing { get; set; }

        /// <summary>
        /// The margin to the left of the text
        /// </summary>
        public int? LeftMargin { get; set; }

        /// <summary>
        /// The background color of the box in which the text is placed.
        /// </summary>
        public Color? BgColor { get; set; }

        /// <summary>
        /// The border color of the text box.
        /// </summary>
        public Color? BorderColor { get; set; }

        /// <summary>
        /// This attribute lets you define whether you want the text to be wrapped or not to the next line.
        /// </summary>
        public bool? Wrap { get; set; }

        /// <summary>
        /// The maximum width after occupying which, the remaining text(if any) would be wrapped to the next line.
        /// </summary>
        public int? WrapWidth { get; set; }

        /// <summary>
        /// The maximum possible height you would need for the texbox if you have set wrapping on.
        /// </summary>
        public int? WrapHeight { get; set; }

        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Label.IsNotWhiteSpace())
                writer.WriteAttributeString("label", Label);
            if(Align.HasValue)
                writer.WriteAttributeString("Align", Align.ToString());
            if(VAlign.HasValue)
                writer.WriteAttributeString("VAlign", VAlign.ToString());

            if(_fontAttr != null)
                _fontAttr.GenerateXmlAttributes(writer);

            if(LetterSpacing.HasValue)
                writer.WriteAttributeString("LetterSpacing", LetterSpacing.ToString());
            if(LeftMargin.HasValue)
                writer.WriteAttributeString("LeftMargin", LeftMargin.ToString());
            if(BgColor.HasValue)
                writer.WriteAttributeString("BgColor", BgColor.Value.ToHexString());
            if(BorderColor.HasValue)
                writer.WriteAttributeString("BorderColor", BorderColor.Value.ToHexString());
            if(Wrap.HasValue)
                writer.WriteAttributeString("Wrap", Wrap.GetHashCode().ToString());
            if(WrapWidth.HasValue)
                writer.WriteAttributeString("WrapWidth", WrapWidth.ToString());
            if(WrapHeight.HasValue)
                writer.WriteAttributeString("WrapHeight", WrapHeight.ToString());
        }
    }
}