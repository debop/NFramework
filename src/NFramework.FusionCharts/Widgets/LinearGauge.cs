namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// FusionWidgets의 Linear Gague 입니다.
    /// </summary>
    public class LinearGauge : GaugeBase {
        /// <summary>
        /// Whether to show pointer value above/below the pointer? 
        /// </summary>
        public bool? ValueAbovePoint { get; set; }

        /// <summary>
        /// Whether to show the pointer on top side or bottom side of chart? 
        /// </summary>
        public bool? PointOnTop { get; set; }

        public int? GaugeRoundRadius { get; set; }

        private PointerAttribute _pointer;

        public virtual PointerAttribute Pointer {
            get { return _pointer ?? (_pointer = new PointerAttribute("pointer")); }
            set { _pointer = value; }
        }

        private CollectionElement<PointerElement> _pointers;

        public virtual CollectionElement<PointerElement> Pointers {
            get { return _pointers ?? (_pointers = new CollectionElement<PointerElement>("pointers")); }
            set { _pointers = value; }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(ValueAbovePoint.HasValue)
                writer.WriteAttributeString("ValueAbovePoint", ValueAbovePoint.GetHashCode().ToString());
            if(PointOnTop.HasValue)
                writer.WriteAttributeString("PointOnTop", PointOnTop.GetHashCode().ToString());

            if(_pointer != null)
                _pointer.GenerateXmlAttributes(writer);
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(System.Xml.XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_pointers != null)
                _pointers.WriteXmlElement(writer);
        }
    }
}