using System;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Data 컬럼을 나타내는 Element
    /// </summary>
    [Serializable]
    public class DataColumnTextElement : ChartElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public DataColumnTextElement() : base("text") {}

        public string Label { get; set; }

        private FusionLink _link;

        /// <summary>
        /// FusionChart Link Format 
        /// </summary>
        public FusionLink Link {
            get { return _link ?? (_link = new FusionLink()); }
            set { _link = value; }
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

            if(Label.IsNotWhiteSpace())
                writer.WriteAttributeString("label", Label);

            if(_link != null)
                _link.GenerateXmlAttributes(writer);
            if(_itemAttr != null)
                _itemAttr.GenerateXmlAttributes(writer);
        }
    }
}