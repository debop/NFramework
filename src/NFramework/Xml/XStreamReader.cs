using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Linq;

namespace NSoft.NFramework.Xml {
    /// <summary>
    /// Silverlight, Windows Phone 에서 사용하기 쉽게 <see cref="XmlReader"/> 를 IEnumerable{XElement} 형식으로 열거할 수 있도록 해 줍니다.
    /// </summary>
    /// <example>
    /// <code>
    /// var rss =
    ///		XStreamReader.Load("http://services.social.microsoft.com/feeds/feed/CSharpHeadlines")
    ///			.Descendants("item")
    ///			.Select(x => new
    ///						 {
    ///							 Title = x.Element("title").GetValue(),
    ///							 Description = x.Element("description").GetValue(),
    ///							 PubDate = x.Element("pubDate").GetValue().AsDateTime()
    ///						 })
    /// 		.ToList();
    /// </code>
    /// </example>
    public class XStreamReader {
        /// <summary>
        /// <paramref name="stream"/>으로부터 정보를 읽어들이는 XStreamReader를 생성합니다.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static XStreamReader Load(Stream stream) {
            return new XStreamReader(() => XmlReader.Create(stream));
        }

        /// <summary>
        /// <paramref name="uri"/>으로부터 정보를 읽어들이는 XStreamReader를 생성합니다.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static XStreamReader Load(string uri) {
            return new XStreamReader(() => XmlReader.Create(uri));
        }

        /// <summary>
        /// <paramref name="textReader"/>으로부터 정보를 읽어들이는 XStreamReader를 생성합니다.
        /// </summary>
        /// <param name="textReader"></param>
        /// <returns></returns>
        public static XStreamReader Load(TextReader textReader) {
            return new XStreamReader(() => XmlReader.Create(textReader));
        }

        /// <summary>
        /// <paramref name="reader"/>으로부터 정보를 읽어들이는 XStreamReader를 생성합니다.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static XStreamReader Load(XmlReader reader) {
            return new XStreamReader(() => reader);
        }

        /// <summary>
        /// <paramref name="text"/>를 파싱하여 XStreamReader를 생성합니다.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static XStreamReader Parse(string text) {
            return new XStreamReader(() => XmlReader.Create(new StringReader(text)));
        }

        // instance

        private readonly Func<XmlReader> _readerFactory;

        private XStreamReader(Expression<Func<XmlReader>> readerFactory) {
            readerFactory.ShouldNotBeNull("readerFactory");
            _readerFactory = readerFactory.Compile();
        }

        private void MoveToNextElement(XmlReader reader) {
            while(reader.Read() && reader.NodeType != XmlNodeType.Element) {}
        }

        private void MoveToNextFollowing(XmlReader reader) {
            var depth = reader.Depth;

            if(reader.NodeType == XmlNodeType.Element && !reader.IsEmptyElement) {
                while(reader.Read() && depth < reader.Depth) {}
            }
            MoveToNextElement(reader);
        }

        /// <summary>
        /// <paramref name="name"/>의 Attribute를 반환합니다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XAttribute Attribute(XName name) {
            return Attributes(name).FirstOrDefault();
        }

        /// <summary>
        /// <paramref name="name"/>에 해당하는 Attribute 들을 열거합니다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<XAttribute> Attributes(XName name) {
            return Attributes().Where(x => x.Name == name);
        }

        /// <summary>
        /// 모든 Attribute 들을 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<XAttribute> Attributes() {
            using(var reader = _readerFactory()) {
                reader.MoveToContent();
                while(reader.MoveToNextAttribute()) {
                    XNamespace ns = reader.NamespaceURI;
                    XName name = ns + reader.Name.Split(':').Last();
                    yield return new XAttribute(name, reader.Value);
                }
            }
        }

        /// <summary>
        /// Element Name 이 <paramref name="name"/>인 첫번째 XElement를 반환합니다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XElement Element(XName name) {
            return Elements(name).FirstOrDefault();
        }

        /// <summary>
        /// 모든 XElement를 반환합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<XElement> Elements() {
            using(var reader = _readerFactory()) {
                reader.MoveToContent();
                MoveToNextElement(reader);
                while(!reader.EOF) {
                    yield return XElement.Load(reader.ReadSubtree());
                    MoveToNextFollowing(reader);
                }
            }
        }

        /// <summary>
        /// Element Name 이 <paramref name="name"/>인 XElement를 열거합니다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<XElement> Elements(XName name) {
            return Elements().Where(x => x.Name == name);
        }

        /// <summary>
        /// Element Name 이 <paramref name="name"/>인 모든 자손 XElement들을 열거합니다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<XElement> Descendants(XName name) {
            using(var reader = _readerFactory()) {
                while(reader.ReadToFollowing(name.LocalName, name.NamespaceName)) {
                    yield return XElement.Load(reader.ReadSubtree());
                }
            }
        }
    }
}