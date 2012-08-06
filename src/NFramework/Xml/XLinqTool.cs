using System.Xml.Linq;

namespace NSoft.NFramework.Xml {
    /// <summary>
    /// XLinq 관련 Utility Class 입니다.
    /// </summary>
    public static class XLinqTool {
        /// <summary>
        /// <paramref name="element"/>의 Value 값을 반환합니다. 없으면, <paramref name="defaultValue"/>를 반환합니다.
        /// </summary>
        public static string GetElementValue(this XElement element, string defaultValue = "") {
            return (element != null) ? element.Value : defaultValue;
        }

        /// <summary>
        /// <paramref name="attribute"/>의 Value 값을 반환합니다. 없으면, <paramref name="defaultValue"/>를 반환합니다.
        /// </summary>
        public static string GetAttrValue(this XAttribute attribute, string defaultValue = "") {
            return (attribute != null) ? attribute.Value : defaultValue;
        }
    }
}