namespace NSoft.NFramework.FusionCharts.Powers {
    /// <summary>
    /// The categories element lets you bunch together x-axis labels of the chart. 
    /// For a multi-series/combination chart, it's necessary to provide data labels using category elements under categories element.
    /// </summary>
    public class RadarCategoriesElement : CollectionElement<RadarCategoryElement> {
        /// <summary>
        /// 생성자
        /// </summary>
        public RadarCategoriesElement() : base("categories") {}

        private FontAttribute _fontAttr;

        /// <summary>
        /// 폰트 속성
        /// </summary>
        public virtual FontAttribute FontAttr {
            get { return _fontAttr ?? (_fontAttr = new FontAttribute()); }
            set { _fontAttr = value; }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(_fontAttr != null)
                _fontAttr.GenerateXmlAttributes(writer);
        }
    }
}