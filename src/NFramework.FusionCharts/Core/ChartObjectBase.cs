using System.Collections.Generic;
using System.Xml;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// 모든 FusionChart의 Chart, Element, Attribute의 기본 요소입니다.
    /// </summary>
    public abstract class ChartObjectBase : IChartObject {
        private IDictionary<string, string> _exProperties;

        /// <summary>
        /// Class에서 미처 준비하지 못한 속성 정보를 추가로 제공하기 위한 저장소입니다. 이 값들은 chart data xml의 attribute 로 생성됩니다.
        /// </summary>
        public IDictionary<string, string> ExProperties {
            get { return _exProperties ?? (_exProperties = new Dictionary<string, string>()); }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public virtual void GenerateXmlAttributes(XmlWriter writer) {
            if(_exProperties != null)
                foreach(var entry in _exProperties)
                    writer.WriteAttributeString(entry.Key, entry.Value);
        }
    }
}