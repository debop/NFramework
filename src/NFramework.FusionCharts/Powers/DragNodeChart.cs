using System.Drawing;
using NSoft.NFramework.FusionCharts.Charts;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Powers {
    /// <summary>
    /// Drag-node Chart
    /// </summary>
    public class DragNodeChart : FusionChartBase {
        public bool? ViewMode { get; set; }
        public bool? EnableLink { get; set; }

        /// <summary>
        /// If you want the user to be able to submit the data as form, you need to set this attribute to 1. If you're using JavaScript methods to get the data from chart, you can hide the button by setting this attribute to 0. 
        /// </summary>
        public bool? ShowFormBtn { get; set; }

        /// <summary>
        ///  URL of your server side script to which you want to submit data. You can either use relative path or absolute path. The name of form variable which is to be requested in this page is strXML. 
        /// </summary>
        public string FormAction { get; set; }

        /// <summary>
        /// Method of form submission - POST or GET. We recommend POST method if you're submitting data as XML. 
        /// </summary>
        public string FormMethod { get; set; }

        public string FormTarget { get; set; }

        public string FormBtnTitle { get; set; }

        public int? FormBtnWidth { get; set; }
        public Color? FormBtnBorderColor { get; set; }
        public Color? FormBtnBgColor { get; set; }

        public int? BtnPadding { get; set; }

        /// <summary>
        /// 속성 중 Attribute Node로 표현해야 한다.
        /// </summary>
        /// <param name="writer"></param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(ViewMode.HasValue)
                writer.WriteAttributeString("ViewMode", ViewMode.Value.GetHashCode().ToString());
            if(EnableLink.HasValue)
                writer.WriteAttributeString("EnableLink", EnableLink.Value.GetHashCode().ToString());

            if(ShowFormBtn.HasValue)
                writer.WriteAttributeString("ShowFormBtn", ShowFormBtn.Value.GetHashCode().ToString());
            if(FormAction.IsNotWhiteSpace())
                writer.WriteAttributeString("FormAction", FormAction);
            if(FormMethod.IsNotWhiteSpace())
                writer.WriteAttributeString("FormMethod", FormMethod);
            if(FormTarget.IsNotWhiteSpace())
                writer.WriteAttributeString("FormTarget", FormTarget);
            if(FormBtnTitle.IsNotWhiteSpace())
                writer.WriteAttributeString("FormBtnTitle", FormBtnTitle);

            if(FormBtnWidth.HasValue)
                writer.WriteAttributeString("FormBtnWidth", FormBtnWidth.Value.ToString());
            if(FormBtnBorderColor.HasValue)
                writer.WriteAttributeString("FormBtnBorderColor", FormBtnBorderColor.Value.ToHexString());
            if(FormBtnBgColor.HasValue)
                writer.WriteAttributeString("FormBtnBgColor", FormBtnBgColor.Value.ToHexString());

            if(BtnPadding.HasValue)
                writer.WriteAttributeString("BtnPadding", BtnPadding.Value.ToString());
        }

        private DragNodeCategoriesElement _categories;

        public new DragNodeCategoriesElement Categories {
            get { return _categories ?? (_categories = new DragNodeCategoriesElement()); }
            set { _categories = value; }
        }

        private DragNodeDataSetElement _dataSet;

        public DragNodeDataSetElement DataSet {
            get { return _dataSet ?? (_dataSet = new DragNodeDataSetElement()); }
            set { _dataSet = value; }
        }

        private ConnectorsElement _connectors;

        public ConnectorsElement Connectors {
            get { return _connectors ?? (_connectors = new ConnectorsElement()); }
            set { _connectors = value; }
        }

        protected override void GenerateXmlElements(System.Xml.XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_categories != null)
                _categories.WriteXmlElement(writer);

            if(_dataSet != null)
                _dataSet.WriteXmlElement(writer);

            if(_connectors != null)
                _connectors.WriteXmlElement(writer);
        }
    }
}