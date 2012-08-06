using System.IO;
using System.Text;
using System.Xml;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Chart의 속성 정보를 나타내는 클래스의 추상 클래스입니다.
    /// </summary>
    public abstract class ChartAttributeBase : ChartObjectBase, IChartAttribute {
        /// <summary>
        /// 속성 값들로, XML Attribute 정보를 생성합니다.
        /// </summary>
        public override string ToString() {
            var sb = new StringBuilder();

            using(var writer = new XmlTextWriter(new StringWriter(sb))) {
                GenerateXmlAttributes(writer);
                writer.Flush();
            }

            return sb.ToString();
        }
    }
}