using System;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// <see cref="TaskElement"/> 들의 F-S 관계를 표현하기 위해 연결자를 나타낸다.
    /// </summary>
    [Serializable]
    public class ConnectorElement : ChartElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public ConnectorElement() : base("connector") {}

        public string FromTaskId { get; set; }
        public string ToTaskId { get; set; }

        public bool? FromTaskConnectStart { get; set; }
        public bool? ToTaskConnectStart { get; set; }

        private LineAttribute _lineAttr;

        public LineAttribute LineAttr {
            get { return _lineAttr ?? (_lineAttr = new LineAttribute()); }
            set { _lineAttr = value; }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(FromTaskId.IsNotWhiteSpace())
                writer.WriteAttributeString("FromTaskId", FromTaskId);
            if(ToTaskId.IsNotWhiteSpace())
                writer.WriteAttributeString("ToTaskId", ToTaskId);

            if(FromTaskConnectStart.HasValue)
                writer.WriteAttributeString("FromTaskConnectStart", FromTaskConnectStart.Value.GetHashCode().ToString());
            if(ToTaskConnectStart.HasValue)
                writer.WriteAttributeString("ToTaskConnectStart", ToTaskConnectStart.Value.GetHashCode().ToString());

            if(_lineAttr != null)
                _lineAttr.GenerateXmlAttributes(writer);
        }
    }
}