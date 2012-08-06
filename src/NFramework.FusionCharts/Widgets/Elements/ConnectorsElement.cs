namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Connector (<see cref="ConnectorElement"/>) 의 집합을 나타내는 클래스입니다.
    /// </summary>
    public class ConnectorsElement : CollectionElement<ConnectorElement> {
        /// <summary>
        /// 생성자
        /// </summary>
        public ConnectorsElement() : base("connectors") {}

        private LineAttribute _lineAttr;

        public virtual LineAttribute LineAttr {
            get { return _lineAttr ?? (_lineAttr = new LineAttribute()); }
            set { _lineAttr = value; }
        }

        public virtual void AddConnector(string fromTaskId, string toTaskId, bool? fromTaskConnectStart,
                                         bool? toTaskConnectStart) {
            Add(new ConnectorElement
                {
                    FromTaskId = fromTaskId,
                    ToTaskId = toTaskId,
                    FromTaskConnectStart = fromTaskConnectStart,
                    ToTaskConnectStart = toTaskConnectStart
                });
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(_lineAttr != null)
                _lineAttr.GenerateXmlAttributes(writer);
        }
    }
}