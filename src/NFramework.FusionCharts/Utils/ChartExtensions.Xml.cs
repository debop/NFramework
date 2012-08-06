using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Fasterflect;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.FusionCharts {
    public static partial class ChartExtensions {
        private const BindingFlags AllProperty = BindingFlags.Public | BindingFlags.Instance;
        private const BindingFlags CurrentPropertyOnly = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        public static Type ChartAttributeType = typeof(IChartAttribute);
        public static Type ChartElementType = typeof(IChartElement);

        public static string BuildChartXml(this IChart chart) {
            return BuildChartXml(chart, null, false);
        }

        public static string BuildChartXml(this IChart chart, bool indent) {
            return BuildChartXml(chart, null, indent);
        }

        public static string BuildChartXml(this IChart chart, Encoding encoding, bool indent) {
            chart.ShouldNotBeNull("chart");

            if(IsDebugEnabled)
                log.Debug(@"Generate DataXml is starting... chart type={0}, encoding={1}, indent={2}", chart.GetType().FullName,
                          encoding, indent);

            var builder = new StringBuilder(400);

            var xmlSettings = new XmlWriterSettings();

            if(encoding != null && encoding == Encoding.Unicode)
                xmlSettings.Encoding = encoding;
            else
                xmlSettings.OmitXmlDeclaration = true;

            xmlSettings.Indent = indent;

            if(xmlSettings.Indent)
                xmlSettings.IndentChars = "\t";

            // About ContextMenu 보이기
            chart.SetAboutMenuItemAttribute();

            // using(var writer = new XmlTextWriter(new StringWriter(builder)))
            using(var writer = XmlWriter.Create(new StringWriter(builder), xmlSettings)) {
                writer.WriteStartDocument();

                chart.WriteXmlElement(writer);

                // WriteChartElement(chart, writer);

                writer.WriteEndDocument();
            }

            if(IsDebugEnabled)
                log.Debug(@"Generate DataXml is finished. chart={0}, encoding={1}, indent={2}", chart.GetType().FullName, encoding,
                          indent);

            return builder.ToString();
        }

        /// <summary>
        /// <see cref="ChartAttributeBase"/>를 상속받는 수형의 인스턴스의 속성 정보를 <paramref name="writer"/>에 씁니다.
        /// 기존 <see cref="ChartAttributeBase"/>에는 GenerateXmlAttributes() 에서 모든 속성값을 코드상에서 생성하도록 했는데, 
        /// Reflection을 사용하여, 속성 명과 속성 값을 추출하고, XmlWriter에 씁니다.
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="writer"></param>
        [Obsolete("개발 중입니다.")]
        public static void WriteChartAttribute(this IChartAttribute attribute, XmlWriter writer) {
            WriteChartAttribute(attribute, AllProperty, writer);
        }

        /// <summary>
        /// <see cref="ChartAttributeBase"/>를 상속받는 수형의 인스턴스의 속성 정보를 <paramref name="writer"/>에 씁니다.
        /// 기존 <see cref="ChartAttributeBase"/>에는 GenerateXmlAttributes() 에서 모든 속성값을 코드상에서 생성하도록 했는데, 
        /// Reflection을 사용하여, 속성 명과 속성 값을 추출하고, XmlWriter에 씁니다.
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="bindingFlags"></param>
        /// <param name="writer"></param>
        [Obsolete("개발 중입니다.")]
        public static void WriteChartAttribute(this IChartAttribute attribute, BindingFlags bindingFlags, XmlWriter writer) {
            if(attribute == null)
                return;

            var attributeType = attribute.GetType();

            var fields = attributeType.Fields().ToList();
            var properties = attributeType.Properties(bindingFlags).ToList();
            var attributes = properties.Where(pi => pi.PropertyType.IsSameOrSubclassOf(ChartAttributeType)).ToArray();
            var elements =
                properties.Where(
                    pi =>
                    pi.PropertyType.IsSameOrSubclassOf(ChartElementType) ||
                    (pi.PropertyType.IsSameOrSubclassOf(typeof(IEnumerable<>)) && pi.PropertyType != typeof(string))).ToArray();

            var valueProperties = properties.Except(attributes).Except(elements).ToList();

            var writtenPropertyNames = new List<string>();

            var prefix = string.Empty;
            // NOTE: Prefix가 있는 Attribute는 중복이 생길 수 있다!!!
            if(attributeType.Properties().Any(pi => pi.Name == "Prefix")) {
                prefix = attribute.GetPropertyValue("Prefix").AsText();

                if(IsDebugEnabled)
                    log.Debug("[{0}]의 Prefix는 [{1}] 입니다.", attributeType.Name, prefix);
            }

            foreach(var pi in valueProperties.Where(pi => pi.GetGetMethod(true) != null)) {
                var name = pi.Name;

                if(name == "Prefix")
                    continue;
                if(name == "ElementName")
                    continue;

                if(IsDebugEnabled)
                    log.Debug(@"속성정보를 XmlAttribute로 빌드하려고 합니다... PropertyName=" + name);

                if(writtenPropertyNames.Contains(name) == false) {
                    var value = attribute.GetPropertyValue(name);

                    if(value != null) {
                        if(IsDebugEnabled)
                            log.Debug(@"속성 값을 Xml Attribute로 빌드합니다., Property Name=[{0}], Value=[{1}]", prefix + name,
                                      value.ToFusionXmlString());

                        writer.WriteAttributeString(prefix + name, value.ToFusionXmlString());
                        writtenPropertyNames.Add(name);
                    }
                }
            }

            foreach(var pi in attributes) {
                var name = pi.Name;

                var fieldName = FasterflectTool.GetMemberName(name, MemberNamingRule.CamelCaseUndercore);
                var fieldValueExists = fields.Any(fi => fi.Name == fieldName) && (attribute.TryGetFieldValue(fieldName) != null);

                if(fieldValueExists) {
                    var attr = attribute.TryGetPropertyValue(name);

                    if(attr != null && attr is IChartAttribute)
                        WriteChartAttribute((IChartAttribute)attr, bindingFlags, writer);
                }
            }
        }

        /// <summary>
        /// 기존 <see cref="ChartElementBase"/>에는 GenerateXmlElements() 에서 모든 속성값을 코드상에서 생성하도록 했는데, 
        /// Reflection을 사용하여, 속성 명과 속성 값을 추출하고, XmlWriter에 씁니다.
        /// <see cref="ChartElementBase"/> 를 상속받은 수형의 인스턴스의 속성 정보를 <paramref name="writer"/>에 씁니다.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="writer"></param>
        [Obsolete("개발 중입니다.")]
        public static void WriteChartElement(this IChartElement element, XmlWriter writer) {
            WriteChartElement(element, AllProperty, writer);
        }

        /// <summary>
        /// 기존 <see cref="ChartElementBase"/>에는 GenerateXmlElements() 에서 모든 속성값을 코드상에서 생성하도록 했는데, 
        /// Reflection을 사용하여, 속성 명과 속성 값을 추출하고, XmlWriter에 씁니다.
        /// <see cref="ChartElementBase"/> 를 상속받은 수형의 인스턴스의 속성 정보를 <paramref name="writer"/>에 씁니다.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="bindingFlags"></param>
        /// <param name="writer"></param>
        [Obsolete("개발 중입니다.")]
        public static void WriteChartElement(this IChartElement element, BindingFlags bindingFlags, XmlWriter writer) {
            if(element == null)
                return;

            var elementType = element.GetType();

            if(IsDebugEnabled)
                log.Debug(@"타입[{0}]에서 속성 중에 XmlElement로 빌드할 정보를 생성합니다.", elementType.FullName);

            var fields = elementType.Fields().ToList();
            var properties = elementType.Properties(bindingFlags).ToList();
            var elements =
                properties.Where(
                    pi =>
                    pi.PropertyType.IsSameOrSubclassOf(ChartElementType) ||
                    (pi.PropertyType.IsSameOrSubclassOf(typeof(IEnumerable<>)) && pi.PropertyType != typeof(string))).ToArray();

            foreach(var pi in elements) {
                var name = pi.Name;

                if(IsDebugEnabled)
                    log.Debug(@"IChartElement 속성 [{0}]을 Xml Element로 생성합니다.", name);

                var fieldName = FasterflectTool.GetMemberName(name, MemberNamingRule.CamelCaseUndercore);
                var fieldValueExists = fields.Any(fi => fi.Name == fieldName) && (element.TryGetFieldValue(fieldName) != null);

                if(fieldValueExists) {
                    var elem = element.TryGetPropertyValue(name);

                    if(elem != null) {
                        if(elem is IChartElement)
                            ((IChartElement)elem).WriteXmlElement(writer);

                        else if(elem is IEnumerable) {
                            foreach(var x in (IEnumerable)elem) {
                                if(x != null && x is IChartElement)
                                    ((IChartElement)x).WriteXmlElement(writer);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 인스턴스의 속성 값을 FusionChart Xml의 Attribute 값으로 사용하기 위해, 특수하게 사용합니다.
        /// (Color 는 Hex String으로 해야 하고,  FusionLink는 포맷이 다르다)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToFusionXmlString(this object value) {
            value.ShouldNotBeNull("value");

            var valueType = value.GetType().UnderlyingSystemType;

            if(valueType == typeof(Boolean))
                return value.GetHashCode().ToString();

            if(valueType.IsEnum)
                return value.GetHashCode().ToString();

            if(valueType == typeof(Color))
                return ((Color)value).ToHexString();

            return value.ToString();
        }
    }
}