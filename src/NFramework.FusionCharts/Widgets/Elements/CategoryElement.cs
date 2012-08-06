using System;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Gantt Chart의 X 축을 시간축으로 표현하는 Element이다.
    /// </summary>
    [Serializable]
    public class CategoryElement : ChartElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public CategoryElement() : base("category") {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="start">시작일</param>
        /// <param name="end">완료일</param>
        public CategoryElement(DateTime start, DateTime end)
            : this() {
            Start = start;
            End = end;
        }

        /// <summary>
        /// 시작 시각
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// 완료 시각
        /// </summary>
        public DateTime? End { get; set; }

        private FusionLink _link;

        /// <summary>
        /// FusionChart Link Format 
        /// </summary>
        public virtual FusionLink Link {
            get { return _link ?? (_link = new FusionLink()); }
            set { _link = value; }
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

            if(_itemAttr != null)
                _itemAttr.GenerateXmlAttributes(writer);

            if(Start.HasValue)
                writer.WriteAttributeString("Start", Start.Value.ToSortableString(true));
            if(End.HasValue)
                writer.WriteAttributeString("End", End.Value.ToSortableString(true));

            if(_link != null)
                _link.GenerateXmlAttributes(writer);
        }
    }
}