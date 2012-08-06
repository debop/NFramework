using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    public static partial class ChartUtil {
        /// <summary>
        /// 지정된 Writer에 Element를 씁니다.
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="elementName">생성할 Element의 Name</param>
        /// <param name="text">Xml Element의 Text</param>
        public static void WriteElementText(XmlWriter writer, string elementName, string text) {
            Guard.Assert(writer != null, @"writer is null.");
            Guard.Assert(elementName.IsNotWhiteSpace(), "elementName is null or empty string.");

            writer.WriteStartElement(elementName);
            writer.WriteString(text);
            writer.WriteEndElement();
        }
    }
}