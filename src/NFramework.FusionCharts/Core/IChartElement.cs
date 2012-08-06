using System.Xml;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Chart의 Element 를 표현하는 정보에 대한 Interface
    /// </summary>
    public interface IChartElement : IChartAttribute {
        /// <summary>
        /// element name
        /// </summary>
        string ElementName { get; }

        /// <summary>
        /// Chart Element 정보를 XmlElement 로 표현합니다.
        /// </summary>
        /// <param name="writer">객체 정보를 쓸 Writer</param>
        void WriteXmlElement(XmlWriter writer);
    }
}