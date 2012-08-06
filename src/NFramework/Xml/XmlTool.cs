using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Xml;
using NLog;
using NSoft.NFramework.IO;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Xml {
    public static partial class XmlTool {
        #region << logger >>

        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// XML 형식의 문자열의 기본 인코딩 방식 (<see cref="Encoding.UTF8"/>)
        /// </summary>
        public static readonly Encoding XmlEncoding = Encoding.UTF8;

        /// <summary>
        /// XML 포맷의 문자열을 만들때 들여쓰기 값
        /// </summary>
        public const int XmlIndent = 4;

        private static readonly IList<XmlNodeType> NodeTypes;

        static XmlTool() {
            // 자식을 가질 수 있는 노드들에 정보
            //
            NodeTypes = new List<XmlNodeType>
                        {
                            XmlNodeType.Element,
                            XmlNodeType.Document,
                            XmlNodeType.DocumentFragment,
                            XmlNodeType.EntityReference
                        };
        }

        /// <summary>
        /// Xml Document가 유효한 것인지 판단한다.
        /// </summary>
        /// <param name="document">검사할 <see cref="XmlDocument"/> 인스턴스</param>
        /// <returns></returns>
        /// <remarks>
        ///	지정된 document가 null이 아니고, document의 root element가 null이 아니면 유효한 document라 판단한다.
        /// </remarks>
        public static bool IsValidDocument(this XmlDocument document) {
            return (IsNotValidDocument(document) == false);
        }

        public static bool IsNotValidDocument(this XmlDocument document) {
            return (document == null || document.DocumentElement == null);
        }

        /// <summary>
        /// XmlNode가 유효한 것인지 판단한다.
        /// </summary>
        /// <remarks>
        /// 지정된 노드가 Null이 아니고, 노드를 소유한 <see cref="System.Xml.XmlDocument"/> 개체가 null이 아니면 유효한 노드이다.
        /// </remarks>
        /// <param name="node"></param>
        /// <returns></returns>
        /// 
        public static bool IsValidNode(this XmlNode node) {
            return (IsNotValidNode(node) == false);
        }

        /// <summary>
        /// XmlNode가 유효하지 않을 때 참을 반환합니다.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsNotValidNode(this XmlNode node) {
            return (node == null || node.OwnerDocument == null);
        }

        /// <summary>
        /// <paramref name="node"/>가 자식 노드를 가질 수 있는 지 검사한다.
        /// </summary>
        /// <param name="node">검사할 노드</param>
        /// <returns><paramref name="node"/>가 자식을 가질 수 있는 노드면 true, 아니면 false를 반환</returns>
        public static bool IsNodeCanHaveChildNode(this XmlNode node) {
            CheckNull(node);
            return NodeTypes.Contains(node.NodeType);
        }

        /// <summary>
        /// <paramref name="node"/>가 null인지 검사한다.
        /// </summary>
        /// <param name="node">검사할 노드</param>
        /// <exception cref="XmlException">
        /// <paramref name="node"/>가 null 이거나, Owner Document가 null인 경우
        /// </exception>
        public static void CheckNull(this XmlNode node) {
            if(IsNotValidNode(node))
                throw new XmlException("node is null or owner document is null. node=" + node);
        }

        /// <summary>
        /// <paramref name="element"/>가 null인지 검사한다.
        /// </summary>
        /// <param name="element">검사할 XmlElement</param>
        /// <exception cref="XmlException">
        /// <paramref name="element"/>가 null 이거나, Owner Document가 null인 경우
        /// </exception>
        public static void CheckNullElement(this XmlElement element) {
            if(IsNotValidNode(element))
                throw new XmlException("element is null or owner document is null. element=" + element);
        }

        /// <summary>
        /// <paramref name="document"/>가 유효한지 검사한다.
        /// </summary>
        /// <param name="document">검사할 XmlDocument 인스턴스</param>
        /// <exception cref="XmlException">
        /// <paramref name="document"/>가 null이거나 <paramref name="document"/>.DocumentElement가 null인 경우
        /// </exception>
        public static void CheckDocument(this XmlDocument document) {
            if(IsNotValidDocument(document))
                throw new XmlException("document is null or invalid. document=" + document);
        }

        /// <summary>
        ///  지정된 xml 문자열로 XmlDocument 객체를 만든다.
        /// </summary>
        /// <param name="xmlText">Xml 문자열 또는 Uri string</param>
        /// <returns></returns>
        public static XmlDoc CreateXmlDocument(this string xmlText) {
            xmlText.ShouldNotBeWhiteSpace("xmlText");

            return new XmlDoc(xmlText);
        }

        /// <summary>
        /// Stream 객체로부터 XmlDocument를 생성한다.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>XmlDoc 인스턴스, 실패시에는 null</returns>
        public static XmlDoc CreateXmlDocument(this Stream stream) {
            stream.ShouldNotBeNull("stream");
            stream.SetStreamPosition(0);
            return new XmlDoc(stream);
        }

        /// <summary>
        /// <see cref="CreateXmlDocument(string)"/>과 같은 기능을 합니다.
        /// </summary>
        /// <param name="xmlText">xml 형식의 문자열</param>
        /// <returns></returns>
        [Obsolete("CreateXmlDocument를 사용하십시요.")]
        public static XmlDoc GetXmlDocument(this string xmlText) {
            return CreateXmlDocument(xmlText);
        }

        /// <summary>
        /// <see cref="CreateXmlDocument(Stream)"/>과 같은 기능을 합니다.
        /// </summary>
        [Obsolete("CreateXmlDocument를 사용하십시요")]
        public static XmlDoc GetXmlDocument(this Stream stream) {
            return CreateXmlDocument(stream);
        }

        /// <summary>
        /// 지정된 XmlElement에 지정된 이름과 값을 가진 Attribute Node를 추가한다.
        /// </summary>
        /// <param name="element">Attribute를 추가할 Element</param>
        /// <param name="attrName">속성 명</param>
        /// <param name="attrValue">속성 값</param>
        public static void AddAttribute(this XmlElement element, string attrName, string attrValue) {
            CheckNullElement(element);

            element.SetAttribute(attrName, attrValue);
        }

        /// <summary>
        /// 지정된 xml element 의 속정 값을 가져온다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="attrName"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static T GetAttributeValue<T>(this XmlElement element, string attrName, T defValue = default(T)) {
            CheckNullElement(element);
            return element.GetAttribute(attrName).AsValue(defValue);
        }

        /// <summary>
        /// 해당 XmlElement의 지정된 속성 값을 Int32로 반환한다.
        /// </summary>
        public static Int32 AttributeToInt32(this XmlElement element, string attributeName, int defValue = 0) {
            CheckNullElement(element);
            return element.GetAttribute(attributeName).AsInt(defValue);
        }

        /// <summary>
        /// 해당 XmlElement의 지정된 속성명을 가진 속성의 값을 문자열로 변환한다.
        /// </summary>
        public static string AttributeToString(this XmlElement element, string attributeName, string defValue = "") {
            CheckNullElement(element);
            return element.GetAttribute(attributeName).AsText(defValue);
        }

        /// <summary>
        /// 해당	XmlElement의 특성 값을 System.Boolean으로 반환한다.
        /// </summary>
        public static bool AttributeToBoolean(this XmlElement element, string attributeName, bool defValue = false) {
            CheckNullElement(element);
            return element.GetAttribute(attributeName).AsBool(defValue);
        }

        /// <summary>
        /// 해당 XmlElement의 특성 값을 System.DateTime 으로 반환한다.
        /// </summary>
        public static DateTime AttributeToDateTime(this XmlElement element, string attributeName, DateTime defValue) {
            CheckNullElement(element);
            return element.GetAttribute(attributeName).AsDateTime(defValue);
        }

        /// <summary>
        /// XML string 을 XML TEXT FORMAT에 맞게 encoding을 수행한다.
        /// </summary>
        /// <remarks>
        ///	<pre>&amp;, &lt;, &gt; 등 Xml에 쓰이는 특수 문자를 변환해 준다.</pre>
        /// </remarks>
        public static string XmlEncode(this string xml) {
            return XmlEncodeEx(xml);
        }

        /// <summary>
        /// XML 문법에 맞게 특수 문자들을 encoding 한다. 
        /// </summary>
        public static string XmlEncodeEx(this string xml) {
            if(xml == null)
                return String.Empty;

            if(xml.IsWhiteSpace())
                return xml;

            var sb = new StringBuilder(xml);

            sb.Replace("\"", "&quot;");
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            sb.Replace("'", "&quot;");

            return sb.ToString();

            //var encoded = xml.Replace("\"", "&quot;");
            //encoded = encoded.Replace("&", "&amp;");
            //encoded = encoded.Replace("<", "&lt;");
            //encoded = encoded.Replace(">", "&gt;");
            //encoded = encoded.Replace("'", "&quot;");

            //return encoded;
        }

        /// <summary>
        /// Xml Text 내용중에 이중 Encoding된 것들 즉 XmlText Node에 Xml format 문자열이 있는 경우
        /// 이를 Decoding한다.
        /// </summary>
        /// <remarks>아직 개발중입니다. 사용하지 마십시요</remarks>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string XmlDecode(this string xml) {
            return xml;
        }

        /// <summary>
        /// XmlTextNode에 있는 문자열을 디코딩한다.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string DecodeText(this string s) {
            if(s == null)
                return String.Empty;

            if(s.IsWhiteSpace())
                return s;

            var sb = new StringBuilder(s);

            sb.Replace("&amp;lt;", "&lt;");
            sb.Replace("&amp;gt;", "&gt;");
            sb.Replace("&amp;quot;", "&quot;");

            return sb.ToString();

            //if(s.IsNotWhiteSpace())
            //{
            //    s = s.Replace("&amp;lt;", "&lt;");
            //    s = s.Replace("&amp;gt;", "&gt;");
            //    s = s.Replace("&amp;quot;", "&quot;");
            //}
            //return s;
        }

        /// <summary>
        /// 태그 값을 Decoding한다.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        private static string DecodeTag(this string tag) {
            if(tag == null)
                return String.Empty;

            if(tag.IsWhiteSpace())
                return tag;

            while(true) {
                var spos = tag.IndexOf("&quot;");
                if(spos < 0)
                    break;

                var epos = tag.IndexOf("&quot;", spos + 6);
                if(epos < 0)
                    break;

                var tmp = tag.Substring(spos, epos - spos + 6);
                var buf = DecodeText(tmp.Substring(6, tmp.Length - 12));
                tag = tag.Replace(tmp, '\"' + buf + '\"');

                // spos = epos + 6;
            }

            tag = tag.Replace("&amp;", "&");
            tag = tag.Replace("&lt;", "<");
            tag = tag.Replace("&gt;", ">");

            return tag;
        }

        /// <summary>
        /// Xml Text 형식의 문자열을 일반 문자열로 변환한다.
        /// </summary>
        /// <remarks>아직 개발중입니다. 사용하지 마십시요</remarks>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string DecodeXml(this string src) {
            if(src.IsWhiteSpace())
                return src;

            var tmp = src;

            var result = new StringBuilder();

            var LT = tmp.IndexOf("&lt;");
            while(LT > 0) {
                var GT = tmp.IndexOf("&gt;");
                if(GT > 0) {
                    result.Append(DecodeTag(tmp.Substring(LT, GT + 4)));
                    tmp = tmp.Substring(GT + 4, tmp.Length - GT - 4);
                    LT = tmp.IndexOf("&lt;");
                    if(LT > 0) {
                        result.Append(DecodeText(tmp.Substring(LT, GT + 4)));
                        tmp = tmp.Substring(LT, tmp.Length - LT);
                        LT = 0;
                    }
                }
                else {
                    tmp = tmp.Substring(LT + 4, tmp.Length - LT - 4);
                    LT = tmp.IndexOf("&lt;");
                }
            }
            if(tmp.Length > 0)
                result.Append(tmp);

            return result.ToString();
        }

        /// <summary>
        /// XML string 을 XmlElement로 변환해서 parentNode node에 child로 추가한다.
        /// </summary>
        /// <param name="parentNode">xml을 추가할 node</param>
        /// <param name="xml">XML format의 string : 주의사항 XmlFragment가 아니다 즉 이 XML 또한 하나의 ROOT만 있어야 한다.</param>
        /// <returns>새로추가된 XmlElement 인스턴스</returns>
        public static XmlElement AddElement(this XmlNode parentNode, string xml) {
            CheckNull(parentNode);

            XmlElement element = null;

            if(parentNode.OwnerDocument != null) {
                var document = CreateXmlDocument(xml);

                if(IsValidDocument(document)) {
                    element = document.DocumentElement.CloneNode(true) as XmlElement;
                    parentNode.AppendChild(element);
                }
            }

            return element;
        }

        /// <summary>
        /// parentNode XmlElement 밑에 newElement를 추가한다.
        /// </summary>
        /// <param name="parentNode">대상 XML Element</param>
        /// <param name="newElement">추가될 XmlElement</param>
        /// <returns></returns>
        public static XmlElement AddElement(this XmlNode parentNode, XmlElement newElement) {
            CheckNull(parentNode);

            var element = newElement.CloneNode(true) as XmlElement;
            parentNode.AppendChild(element);
            return element;
        }

        /// <summary>
        /// parentNode에 새로운 XmlElement를 추가한다.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static XmlElement AddElement(this XmlNode parentNode, string name, string value) {
            CheckNull(parentNode);

            var element = parentNode.OwnerDocument.CreateElement(name);

            if(value != null)
                element.InnerText = value.AsText();

            parentNode.AppendChild(element);
            return element;
        }

        /// <summary>
        /// XmlNodeList의 Node들을 parentNode에 추가한다.
        /// </summary>
        /// <param name="parentNode">대상 Element</param>
        /// <param name="elementList">XmlNodeList 형식의 Element (같은 Level의 Element만 써야한다.)</param>
        /// <returns>추가된 XmlNode의 갯수</returns>
        public static int AddElementList(this XmlNode parentNode, XmlNodeList elementList) {
            var count = 0;

            if(elementList == null || elementList.Count == 0)
                return count;

            CheckNull(parentNode);

            XmlDocumentFragment fragment = parentNode.OwnerDocument.CreateDocumentFragment();

            foreach(XmlNode srcNode in elementList) {
                fragment.AppendChild(srcNode.CloneNode(true));
                count++;
            }
            parentNode.AppendChild(fragment);

            return count;
        }

        /// <summary>
        /// 주어진 xml 문자열로 XmlDocumentFragment를 만들고 이를 parentNode에 추가한다.
        /// 이때 xml 문자열은 Root Node를 가질 필요 없다.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="xmlText"></param>
        public static void AddXmlFragment(this XmlNode parentNode, string xmlText) {
            CheckNull(parentNode);

            if(xmlText.IsEmpty())
                throw new ArgumentNullException("xmlText", "추가할 내용이 없습니다.");

            var document = CreateXmlDocument("<RW_XML_FRAGMENT/>");
            AddElement(document.DocumentElement, xmlText);

            AddElementList(parentNode, document.DocumentElement.ChildNodes);
        }

        /// <summary>
        /// XPATH에 해당하는 Element를 찾아서 InnerText 값을 반환한다. 
        /// (여러개의 Element일 경우에는 제일 처음 검색된 것을 사용한다.)
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xpath"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetElementText(this XmlDocument document, string xpath, string defaultValue = "") {
            var result = defaultValue;

            if(document != null && xpath.IsNotWhiteSpace()) {
                var node = document.SelectSingleNode(xpath);
                if(node != null)
                    result = node.InnerText;
            }
            return result;
        }

        /// <summary>
        /// XPATH에 해당하는 Element들을 찾아서 InnerText 값을 StringCollection에 담아 반환한다.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xpath"></param>
        /// <returns>찾는 Element가 없을 시에는 빈 컬렉션 반환</returns>
        public static StringCollection GetElementTextToArrayList(this XmlDocument document, string xpath) {
            var sc = new StringCollection();
            GetElementTextList(document, xpath, sc);
            return sc;
        }

        /// <summary>
        /// XPATH에 해당하는 Element들을 찾아서 InnerText 값을 StringCollection에 담아 반환한다.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xpath">찾고자하는 경로</param>
        /// <param name="list">결과를 담을 컬렉션</param>
        public static void GetElementTextList(this XmlDocument document, string xpath, StringCollection list) {
            CheckDocument(document);
            list.ShouldNotBeNull("list");

            var nodeList = document.SelectNodes(xpath);
            foreach(XmlNode node in nodeList)
                list.Add(node.InnerText);
        }

        /// <summary>
        /// XPATH에 해당하는 Element들을 찾아서 InnerText 값을 리스트에 담아 반환한다.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xpath"></param>
        /// <param name="list"></param>
        public static void GetElementTextList(this XmlDocument document, string xpath, List<string> list) {
            CheckDocument(document);

            list.ShouldNotBeNull("list");

            var nodeList = document.SelectNodes(xpath);

            foreach(XmlNode node in nodeList)
                list.Add(node.InnerText);
        }

        /// <summary>
        /// Xpath로 찾은 XmlNode들을 삭제한다.
        /// </summary>
        /// <param name="document">조작할 XmlDocument</param>
        /// <param name="xpath">삭제할 노드를 찾기 위한 XPath 구문</param>
        /// <returns>삭제된 Node의 수, 없으면 0</returns>
        public static int RemoveSelection(this XmlDocument document, string xpath) {
            CheckDocument(document);

            int count = 0;

            if(xpath.IsNotWhiteSpace()) {
                var nodeList = document.SelectNodes(xpath);

                foreach(XmlNode node in nodeList)
                    if(node.ParentNode != null) {
                        node.ParentNode.RemoveChild(node);
                        count++;
                    }
            }
            return count;
        }
    }
}