using System.Xml;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Chart의 구성원이 되는 모든 Class의 기본 인터페이스입니다.
    /// </summary>
    public interface IChartObject {
        /// <summary>
        /// 속성 정보를 XmlAttribute로 표현합니다.
        /// </summary>
        /// <param name="writer"></param>
        void GenerateXmlAttributes(XmlWriter writer);
    }
}