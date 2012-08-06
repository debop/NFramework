using System;
using System.Collections.Generic;
using System.Xml;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Gantt Chart의 X 축인 시간 축을 표현한다
    /// </summary>
    [Serializable]
    [Obsolete("CategoriesElement를 사용하세요")]
    public class CategoryCollection : ChartElementBase {
        public CategoryCollection() : base("categories") {}

        public int? VerticalPadding { get; set; }

        private ItemAttribute _itemAttr;

        public virtual ItemAttribute ItemAttr {
            get { return _itemAttr ?? (_itemAttr = new ItemAttribute()); }
            set { _itemAttr = value; }
        }

        private IList<CategoryElement> _categoryElements;

        public virtual IList<CategoryElement> CategoryElements {
            get { return _categoryElements ?? (_categoryElements = new List<CategoryElement>()); }
            set { _categoryElements = value; }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(_itemAttr != null)
                _itemAttr.GenerateXmlAttributes(writer);

            if(VerticalPadding.HasValue)
                writer.WriteAttributeString("VerticalPadding", VerticalPadding.Value.ToString());
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_categoryElements != null && _categoryElements.Count > 0)
                foreach(var category in _categoryElements)
                    category.WriteXmlElement(writer);
        }
    }
}