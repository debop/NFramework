using System.IO;
using System.Text;
using System.Xml;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Chart의 Element로 구성되는 정보를 표현하는 기본 클래스입니다.
    /// </summary>
    public abstract class ChartElementBase : ChartObjectBase, IChartElement {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        protected ChartElementBase(string elementName) {
            ElementName = elementName;
        }

        /// <summary>
        /// element name
        /// </summary>
        public string ElementName { get; private set; }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected virtual void GenerateXmlElements(XmlWriter writer) {
            // Nothing to do.
        }

        /// <summary>
        /// Chart Object의 속성을 Xml로 변환합니다.
        /// </summary>
        /// <param name="writer">Element를 추가할 Writer</param>
        public void WriteXmlElement(XmlWriter writer) {
            if(IsDebugEnabled)
                log.Debug(@"Chart 요소 (Type=[{0}], Name=[{1}]) 를 XmlElement로 생성합니다...", GetType().Name, ElementName);

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