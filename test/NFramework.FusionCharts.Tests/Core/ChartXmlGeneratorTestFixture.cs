using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.FusionCharts {
    // TODO: 현재는 제대로 작동하지 않지만, GenerateXmlAttribute, GenerateXmlElement를 자동으로 할 수 있을 것이다.

    /// <summary>
    /// 속성을 DynamicAccessor를 통해 자동으로 분해해서 XML을 생성합니다.
    /// </summary>
    [TestFixture]
    public class ChartXmlGeneratorTestFixture : ChartTestFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [Test]
        public void GenerateXmlByDynamicAccessor() {
            var accessor2 = DynamicAccessorFactory.CreateDynamicAccessor(typeof(CosmeticAttribute), false);

            var cosmetic = new CosmeticAttribute
                           {
                               BgSWF = "~/FusionCharts/BgChart.swf"
                           };

            var sb = new StringBuilder();

            using(var writer = new XmlTextWriter(new StringWriter(sb))) {
                writer.WriteStartDocument(true);
                writer.WriteStartElement("chart");

                GenerateXmlAttributes(writer, accessor2, cosmetic);

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            Console.WriteLine(sb.ToString());
        }

        private static void GenerateXmlAttributes<T>(XmlWriter writer, IDynamicAccessor accessor, T instance) where T : IChartObject {
            foreach(var propertyName in accessor.GetPropertyNames()) {
                var propertyValue = accessor.GetPropertyValue(instance, propertyName);
                if(propertyValue != null) {
                    Type propertyType = accessor.GetPropertyType(propertyName);

                    if(propertyType.IsSimpleType() || propertyType.IsValueType) {
                        if(IsDebugEnabled)
                            log.Debug("Property name={0}, type={1}, value={2}", propertyName, propertyType, propertyValue);

                        // NOTE : Color인 경우는 HexString으로, Enum 값은 HashCode 값으로...
                        if(propertyType == typeof(Color?)) {
                            var color = (Color?)propertyValue;
                            writer.WriteAttributeString(propertyName, color.Value.ToHexString());
                        }
                        else if(propertyType.IsEnum)
                            writer.WriteAttributeString(propertyName, propertyValue.GetHashCode().ToString());
                        else
                            writer.WriteAttributeString(propertyName, propertyValue.ToString());
                    }
                    else if(propertyType.IsSameOrSubclassOf(typeof(ChartAttributeBase))) {
                        var accessor2 = DynamicAccessorFactory.CreateDynamicAccessor(propertyType, false);
                        GenerateXmlAttributes(writer, accessor2, propertyValue as ChartAttributeBase);
                    }
                    else if(TypeTool.IsSameOrSubclassOrImplementedOf(propertyType, typeof(IEnumerable))) {
                        // Nothing to do.
                    }
                    else {
                        throw new NotSupportedException(string.Format("지원하지 않는 속성입니다.  property name={0}, type={1}", propertyName,
                                                                      propertyType.FullName));
                    }
                }
            }
        }
    }
}