namespace NSoft.NFramework.FusionCharts.Powers {
    public class DragNodeCategoriesElement : CollectionElement<DragNodeCategoryElement> {
        public DragNodeCategoriesElement()
            : base("categories") {}

        private FontAttribute _fontAttr;

        public FontAttribute FontAttr {
            get { return _fontAttr ?? (_fontAttr = new FontAttribute()); }
            set { _fontAttr = value; }
        }

        private DashLineAttribute _verticalLineAttr;

        public DashLineAttribute VerticalLineAttr {
            get { return _verticalLineAttr ?? (_verticalLineAttr = new DashLineAttribute("verticalLine")); }
            set { _verticalLineAttr = value; }
        }

        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(_fontAttr != null)
                _fontAttr.GenerateXmlAttributes(writer);

            if(_verticalLineAttr != null)
                _verticalLineAttr.GenerateXmlAttributes(writer);
        }
    }
}