using System;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// Category 를 나타내는 클래스입니다.
    /// </summary>
    [Serializable]
    public class CategoryElement : CategoryElementBase {
        /// <summary>
        /// Line 위치
        /// </summary>
        public virtual HorizontalPosition? LinePosition { get; set; }

        private DashLineAttribute _lineAttr;

        /// <summary>
        /// Line 속성
        /// </summary>
        public virtual DashLineAttribute LineAttr {
            get { return _lineAttr ?? (_lineAttr = new DashLineAttribute()); }
            set { _lineAttr = value; }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(LinePosition.HasValue)
                writer.WriteAttributeString("LinePosition", LinePosition.GetHashCode().ToString());

            if(_lineAttr != null)
                _lineAttr.GenerateXmlAttributes(writer);
        }
    }
}