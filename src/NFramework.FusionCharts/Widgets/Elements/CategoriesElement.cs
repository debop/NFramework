using System.Drawing;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Gantt Chart의 X 축인 시간 축을 표현한다
    /// </summary>
    public class CategoriesElement : CollectionElement<CategoryElement> {
        public CategoriesElement() : base("categories") {}

        public virtual int? VerticalPadding { get; set; }

        public Color? BgColor { get; set; }
        public int? BgAlpha { get; set; }
        public string Label { get; set; }
        public FusionTextAlign? Align { get; set; }
        public FusionVerticalAlign? VAlign { get; set; }

        private FontAttribute _fontAttr;

        public virtual FontAttribute FontAttr {
            get { return _fontAttr ?? (_fontAttr = new FontAttribute()); }
            set { _fontAttr = value; }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(VerticalPadding.HasValue)
                writer.WriteAttributeString("VerticalPadding", VerticalPadding.Value.ToString());

            if(BgColor.HasValue)
                writer.WriteAttributeString("BgColor", BgColor.Value.ToHexString());
            if(BgAlpha.HasValue)
                writer.WriteAttributeString("BgAlpha", BgAlpha.Value.ToString());
            if(Label.IsNotWhiteSpace())
                writer.WriteAttributeString("Label", Label);
            if(Align.HasValue)
                writer.WriteAttributeString("Align", Align.Value.ToString().ToLower());
            if(VAlign.HasValue)
                writer.WriteAttributeString("VAlign", VAlign.Value.ToString().ToLower());

            if(_fontAttr != null)
                _fontAttr.GenerateXmlAttributes(writer);
        }
    }
}