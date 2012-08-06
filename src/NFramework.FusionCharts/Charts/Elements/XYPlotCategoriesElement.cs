namespace NSoft.NFramework.FusionCharts.Charts {
    public class XYPlotCategoriesElement : CategoriesElement {
        private DashLineAttribute _lineAttr;

        public virtual DashLineAttribute LineAttr {
            get { return _lineAttr ?? (_lineAttr = new DashLineAttribute("verticalLine")); }
            set { _lineAttr = value; }
        }

        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(_lineAttr != null)
                _lineAttr.GenerateXmlAttributes(writer);
        }
    }
}