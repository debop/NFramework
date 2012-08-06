using System;
using System.Collections.Generic;
using System.Xml;

namespace NSoft.NFramework.FusionCharts.Widgets {
    [Serializable]
    [Obsolete("ConnectorsElement를 사용하세요")]
    public class ConnectorCollection : ChartElementBase {
        public ConnectorCollection()
            : base("connectors") {}

        private LineAttribute _lineAttr;

        public virtual LineAttribute LineAttr {
            get { return _lineAttr ?? (_lineAttr = new LineAttribute()); }
        }

        private IList<ConnectorElement> _connectorElements;

        public virtual IList<ConnectorElement> ConnectorElements {
            get { return _connectorElements ?? (_connectorElements = new List<ConnectorElement>()); }
        }

        /// <summary>
        /// 속성 중 Attribute Node로 표현해야 한다.
        /// </summary>
        /// <param name="writer"></param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(_lineAttr != null)
                _lineAttr.GenerateXmlAttributes(writer);
        }

        /// <summary>
        /// Xml Element Node 속성을 XmlElement로 저장합니다. 꼭 <see cref="ChartObjectBase.GenerateXmlAttributes"/> 수행 후에 해야 한다.
        /// </summary>
        /// <param name="writer"></param>
        protected override void GenerateXmlElements(XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_connectorElements != null)
                foreach(var connector in _connectorElements)
                    connector.WriteXmlElement(writer);
        }
    }
}