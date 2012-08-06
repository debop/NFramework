namespace NSoft.NFramework.FusionCharts.Powers {
    /// <summary>
    /// <see cref="CandleStickCategoryElement"/> 컬렉션을 나타내는 클래스 입니다.
    /// </summary>
    public class CandleStickCategoriesElement : CollectionElement<CandleStickCategoryElement> {
        /// <summary>
        /// 생성자
        /// </summary>
        public CandleStickCategoriesElement() : base("categories") {}

        private FontAttribute _fontAttr;

        public virtual FontAttribute FontAttr {
            get { return _fontAttr ?? (_fontAttr = new FontAttribute()); }
            set { _fontAttr = value; }
        }

        private DashLineAttribute _verticalLine;

        public DashLineAttribute VerticalLine {
            get { return _verticalLine ?? (_verticalLine = new DashLineAttribute("verticalLine")); }
            set { _verticalLine = value; }
        }

        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(_fontAttr != null)
                _fontAttr.GenerateXmlAttributes(writer);

            if(_verticalLine != null)
                _verticalLine.GenerateXmlAttributes(writer);
        }

        /// <summary>
        /// <see cref="CandleStickCategoryElement"/>를 추가합니다.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="x"></param>
        public virtual CandleStickCategoryElement Add(double? x, string label) {
            return Add(x, label, null, null, null, null);
        }

        /// <summary>
        /// <see cref="CandleStickCategoryElement"/>를 추가합니다.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="label"></param>
        /// <param name="showLabel"></param>
        /// <param name="showVerticalLine"></param>
        /// <param name="lineDashed"></param>
        /// <param name="toolText"></param>
        public virtual CandleStickCategoryElement Add(double? x, string label,
                                                      bool? showLabel, bool? showVerticalLine,
                                                      bool? lineDashed, string toolText) {
            var category = new CandleStickCategoryElement
                           {
                               X = x,
                               Label = label,
                               ShowLabel = showLabel,
                               ShowVerticalLine = showVerticalLine,
                               LineDashed = lineDashed,
                               ToolText = toolText
                           };
            Add(category);

            return category;
        }
    }
}