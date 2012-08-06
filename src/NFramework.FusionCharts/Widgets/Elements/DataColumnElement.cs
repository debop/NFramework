using System;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Gantt Chart Y축의 부가 항목을 나타냅니다.
    /// </summary>
    [Serializable]
    public class DataColumnElement : CollectionElement<DataColumnTextElement> // ChartElementBase
    {
        public DataColumnElement() : base("datacolumn") {}

        public string HeaderText { get; set; }

        /// <summary>
        /// FusionChart Link Format 
        /// </summary>
        private FusionLink _helperLink;

        public FusionLink HelperLink {
            get { return _helperLink ?? (_helperLink = new FusionLink("helperLink")); }
            set { _helperLink = value; }
        }

        public int? Width { get; set; }

        private ItemAttribute _headerAttr;

        public ItemAttribute HeaderAttr {
            get { return _headerAttr ?? (_headerAttr = new ItemAttribute("header")); }
            set { _headerAttr = value; }
        }

        private ItemAttribute _itemAttr;

        public ItemAttribute ItemAttr {
            get { return _itemAttr ?? (_itemAttr = new ItemAttribute()); }
            set { _itemAttr = value; }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(HeaderText.IsNotWhiteSpace())
                writer.WriteAttributeString("HeaderText", HeaderText);

            if(_helperLink != null)
                _helperLink.GenerateXmlAttributes(writer);

            if(Width.HasValue)
                writer.WriteAttributeString("Width", Width.ToString());

            if(_headerAttr != null)
                _headerAttr.GenerateXmlAttributes(writer);
            if(_itemAttr != null)
                _itemAttr.GenerateXmlAttributes(writer);
        }
    }
}