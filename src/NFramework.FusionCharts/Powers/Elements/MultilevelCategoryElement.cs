using System.Collections.Generic;
using System.Drawing;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Powers {
    /// <summary>
    /// Multi-level Pie Chart 등에서 사용하는 category 입니다. 자식으로 category를 가집니다.
    /// </summary>
    /// <seealso cref="MultilevelPie"/>
    public class MultilevelCategoryElement : CategoryElementBase {
        /// <summary>
        /// If you do not want a symmetrical distribution for all the pies, you can allot numeric values to individual pies. This can be done using the value attribute of each category. You'll need to define the value for all the categories (pies) then. The sweep angle of the pie would depend on its value.
        /// If you just want a symmetrical multi-level pie chart, you can ignore the value.
        /// </summary>
        public double? Value { get; set; }

        /// <summary>
        ///	 Color for the pie.
        /// </summary>
        public Color? Color { get; set; }

        /// <summary>
        /// Alpha for the pie.
        /// </summary>
        public int? Alpha { get; set; }

        /// <summary>
        /// Tool text for the pie.
        /// </summary>
        public string HoverText { get; set; }

        private FusionLink _link;

        /// <summary>
        /// Link for the pie.
        /// </summary>
        public FusionLink Link {
            get { return _link ?? (_link = new FusionLink()); }
            set { _link = value; }
        }

        private IList<MultilevelCategoryElement> _children;

        public IList<MultilevelCategoryElement> Children {
            get { return _children ?? (_children = new List<MultilevelCategoryElement>()); }
            set { _children = value; }
        }

        /// <summary>
        /// Child category를 추가합니다.
        /// </summary>
        /// <param name="childCategory"></param>
        public MultilevelCategoryElement AddChildCategory(MultilevelCategoryElement childCategory) {
            Children.Add(childCategory);
            return this;
        }

        /// <summary>
        /// Child Category를 추가합니다.
        /// </summary>
        /// <param name="childCategories"></param>
        public MultilevelCategoryElement AddChildCategoryRange(params MultilevelCategoryElement[] childCategories) {
            if(childCategories != null && childCategories.Length > 0)
                foreach(var child in childCategories)
                    Children.Add(child);

            return this;
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Value.HasValue)
                writer.WriteAttributeString("value", Value.ToString());
            if(Color.HasValue)
                writer.WriteAttributeString("color", Color.Value.ToHexString());
            if(Alpha.HasValue)
                writer.WriteAttributeString("alpha", Alpha.ToString());
            if(HoverText.IsNotWhiteSpace())
                writer.WriteAttributeString("hoverText", HoverText);

            if(_link != null)
                _link.GenerateXmlAttributes(writer);
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(System.Xml.XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_children != null && _children.Count > 0)
                foreach(var child in _children)
                    child.WriteXmlElement(writer);
        }
    }
}