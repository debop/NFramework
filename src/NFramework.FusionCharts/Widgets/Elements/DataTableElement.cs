using System;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Gantt Chart의 Y 축에 부가적인 설명 테이블입니다.
    /// </summary>
    [Serializable]
    public class DataTableElement : CollectionElement<DataColumnElement> // ChartElementBase
    {
        public DataTableElement() : base("datatable") {}

        private ItemAttribute _headerAttr;

        public virtual ItemAttribute HeaderAttr {
            get { return _headerAttr ?? (_headerAttr = new ItemAttribute("header")); }
            set { _headerAttr = value; }
        }

        private ItemAttribute _itemAttr;

        public virtual ItemAttribute ItemAttr {
            get { return _itemAttr ?? (_itemAttr = new ItemAttribute()); }
            set { _itemAttr = value; }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(_headerAttr != null)
                _headerAttr.GenerateXmlAttributes(writer);

            if(_itemAttr != null)
                _itemAttr.GenerateXmlAttributes(writer);
        }
    }
}