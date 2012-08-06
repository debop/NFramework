using System;
using System.Collections.Generic;

namespace NSoft.NFramework.FusionCharts.Widgets {
    [Obsolete("속성이 없는 Collection은 CollectionElement{T}를 직접 사용하시고, 속성이 있으면, 상속받은 Element class를 정의하여 사용하세요.")]
    [Serializable]
    public class LegendElement : ChartElementBase {
        public LegendElement()
            : base("legend") {}

        private IList<LegendItemElement> _items;

        public virtual IList<LegendItemElement> Items {
            get { return _items ?? (_items = new List<LegendItemElement>()); }
        }

        protected override void GenerateXmlElements(System.Xml.XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_items != null) {
                foreach(var item in _items)
                    item.WriteXmlElement(writer);
            }
        }
    }
}