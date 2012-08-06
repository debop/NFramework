namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// X축의 지표를 나타내는 요소의 컬렉션을 나타냅니다.
    /// </summary>
    public abstract class CategoriesElementBase : CollectionElement<ChartElementBase> {
        protected CategoriesElementBase() : base("categories") {}

        private FontAttribute _fontAttr;

        /// <summary>
        /// 폰트 속성
        /// </summary>
        public virtual FontAttribute FontAttr {
            get { return _fontAttr ?? (_fontAttr = new FontAttribute()); }
            set { _fontAttr = value; }
        }

        /// <summary>
        /// 속성 중 Attribute Node로 표현해야 한다.
        /// </summary>
        /// <param name="writer"></param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(_fontAttr != null)
                _fontAttr.GenerateXmlAttributes(writer);
        }
    }
}