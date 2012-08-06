using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// {T} 수형의 Element들을 자식으로 가지는 Collection Element 입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class CollectionElement<T> : List<T>, IChartElement where T : IChartElement {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="elementName">요소 명</param>
        public CollectionElement(string elementName) {
            Guard.Assert(elementName.IsNotWhiteSpace(), @"elementName is null or empty string.");
            ElementName = elementName.Trim();
        }

        /// <summary>
        /// 요소 이름
        /// </summary>
        public string ElementName { get; private set; }

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

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        private void GenerateXmlElements(XmlWriter writer) {
            foreach(var element in this)
                element.WriteXmlElement(writer);
        }

        /// <summary>
        /// Chart Element 정보를 지정한 XmlWriter에 씁니다.
        /// </summary>
        /// <param name="writer">객체 정보를 쓸 Writer</param>
        public void WriteXmlElement(XmlWriter writer) {
            writer.ShouldNotBeNull("writer");

            writer.WriteStartElement(ElementName);

            GenerateXmlAttributes(writer);
            GenerateXmlElements(writer);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Xml Text 를 생성합니다.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            var sb = new StringBuilder();

            using(var writer = new XmlTextWriter(new StringWriter(sb))) {
                WriteXmlElement(writer);
                writer.Flush();
            }

            return sb.ToString();
        }
    }
}