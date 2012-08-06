using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NSoft.NFramework.IO;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Xml {
    /// <summary>
    /// RealWeb XmlDocument Class
    /// System.Xml.XmlDocument로부터 상속 받음<br/>
    /// XmlDocument를 다루기위한 Helper Function들을 추가함
    /// </summary>
    /// <remarks>
    /// 현재 Namespace URI등은 함수에서 지원하지 않으므로 XmlDocument 에서 직접 사용해야 합니다.
    /// </remarks>
    /// TODO: Namespace URI 등을 지원하기 위한 함수 추가
    [Serializable]
    public class XmlDoc : XmlDocument {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public XmlDoc() {}

        /// <summary>
        /// 주어진 xml 문자열이 순수 XML string이거나 URI 이거나 
        /// 모두 load하여 XmlDocument를 구성한다.
        /// </summary>
        /// <param name="xmlText">xml text이거나 xml 파일경로, URI 등이 된다.</param>
        public XmlDoc(string xmlText) {
            if(IsDebugEnabled)
                log.Debug("Initialize a new instance of XmlDoc with xmlText. xmlText=[{0}]", xmlText.EllipsisChar(80));

            if(xmlText.IsNotWhiteSpace()) {
                if(xmlText.IndexOf(">", StringComparison.Ordinal) > 0)
                    LoadXml(xmlText);
                else
                    Load(xmlText);
            }
        }

        /// <summary>
        /// 생성자 ( System.Uri 개체의 AbsoluteUri 속성을 통해 XmlDocument를 생성합니다.
        /// </summary>
        /// <param name="uri"><c>System.Uri</c>개체</param>
        public XmlDoc(Uri uri) {
            uri.ShouldNotBeNull("uri");

            if(IsDebugEnabled)
                log.Debug("Initialize a new instance of XmlDoc with uri=[{0}]", uri);

            Load(uri.AbsolutePath);
        }

        /// <summary>
        /// 생성자 - <paramref name="document"/>을 읽어서 새로운 <see cref="XmlDocument"/> 인스턴스를 생성한다.
        /// </summary>
        /// <param name="document">원본 XmlDocument</param>
        public XmlDoc(XmlDocument document) {
            document.ShouldNotBeNull("document");
            Guard.Assert(document.IsValidDocument(), "Invalid content of xml document.");

            LoadXml(document.OuterXml);
        }

        /// <summary>
        /// <paramref name="stream"/> 정보를 읽어서 XmlDocument 인스턴스를 생성한다.
        /// </summary>
        /// <param name="stream">원본 Stream 객체</param>
        public XmlDoc(Stream stream) {
            stream.ShouldNotBeNull("strem");
            stream.SetStreamPosition();

            Load(stream);
        }

        /// <summary>
        /// XmlDocument.DocumentElement 즉 XmlDocument Root Node를 반환한다.
        /// </summary>
        public XmlElement Root {
            get { return DocumentElement; }
        }

        /// <summary>
        /// XmlDocument의 Xml 문자열을 반환한다.
        /// </summary>
        public string Xml {
            get { return OuterXml; }
        }

        /// <summary>
        /// XmlDocument의 내용을 일반 Text형식으로 반환한다. (XmlDocument.OuterXml)
        /// </summary>
        public string Text {
            get { return OuterXml; }
        }

        /// <summary>
        /// XmlDocument의 xml 내용을 MemoryStream 형태로 보낸다.
        /// </summary>
        public Stream XmlStream {
            get { return OuterXml.ToStream(XmlTool.XmlEncoding); }
        }

        /// <summary>
        /// XmlDocument를 읽을 수 있는 XmlNodeReader를 반환한다.
        /// </summary>
        public XmlNodeReader Reader {
            get { return new XmlNodeReader(this); }
        }

        /// <summary>
        /// 유효한 XmlDocument 객체인가 판단.
        /// </summary>
        public bool IsValidDocument {
            get { return this.IsValidDocument(); }
        }

        /// <summary>
        /// Create a new instance of XmlElement with name and text
        /// </summary>
        /// <param name="name">XmlElement의 이름</param>
        /// <param name="text">XmlElement의 Inner Text</param>
        /// <returns>생성된 XmlElement</returns>
        protected virtual XmlElement CreateElementText(string name, string text) {
            name.ShouldNotBeWhiteSpace("name");

            if(IsDebugEnabled)
                log.Debug("Create XmlElement with Text. name=[{0}], text=[{1}]", name, text);

            var result = CreateElement(name);

            if(text.IsNotWhiteSpace())
                result.InnerText = text;

            return result;
        }

        /// <summary>
        /// <paramref name="xml"/> 로부터 새로운 XmlElement를 생성한다.
        /// </summary>
        /// <param name="xml">Xml 형식의 문자열</param>
        /// <returns>생성된 <see cref="XmlElement"/> 객체, 실패시에는 null 반환</returns>
        [Obsolete("구현되지 않았음.")]
        protected virtual XmlElement CreateElementXml(string xml) {
            // TODO : 다시 구현해야 한다.
            //
            throw new NotImplementedException();
            // 잘못된 것 같다.
            //			XmlElement result = null;
            //			XmlDocument xmlDoc = null;
            //
            //			xmlDoc = RwXml.CreateXmlDocument(xml);
            //			if (RwXml.IsValidDocument(xmlDoc))
            //				result = (XmlElement)xmlDoc.DocumentElement.CloneNode(true);
            //
            //			return result;
        }

        /// <summary>
        /// <paramref name="xpath"/>에 해당하는 XmlElement를 XmlDocument 전체에서 검색하여 반환한다.
        /// </summary>
        /// <param name="xpath">검색 식</param>
        /// <returns>검색한 XmlElement, 없으면 null 반환</returns>
        protected virtual XmlElement FindElement(string xpath) {
            return FindNode(xpath) as XmlElement;
        }

        /// <summary>
        /// <paramref name="xpath"/>에 해당하는 XmlElement를 지정된 <paramref name="parentNode"/> 자식 노드에서 검색하여 반환한다.
        /// </summary>
        /// <param name="parentNode">검색 기준이 되는 parent node</param>
        /// <param name="xpath">검색 식</param>
        /// <returns>검색한 XmlElement, 없으면 null 반환</returns>
        protected virtual XmlElement FindElement(XmlNode parentNode, string xpath) {
            parentNode.ShouldNotBeNull("parentNode");

            if(IsDebugEnabled)
                log.Debug("요소 찾기... parentNode=[{0}], xpath=[{1}]", parentNode.Name, xpath);

            if(xpath.IsNotWhiteSpace())
                return parentNode.SelectSingleNode(xpath) as XmlElement;

            return null;
        }

        /// <summary>
        /// 이 <see cref="XmlDoc"/>에서 <paramref name="xpath"/>에 해당하는 모든 XmlElement를 리스트로 반환한다.
        /// </summary>
        /// <param name="xpath">검색 식</param>
        /// <returns>XmlNodeList</returns>
        protected virtual XmlNodeList FindElements(string xpath) {
            if(IsDebugEnabled)
                log.Debug("요소들 찾기... xpath=[{0}]", xpath);

            if(xpath.IsNotWhiteSpace())
                return SelectNodes(xpath);

            return null;
        }

        /// <summary>
        /// <paramref name="parentNode"/>에서 <paramref name="xpath"/>에 해당하는 모든 XmlElement를 리스트로 반환한다.
        /// </summary>
        /// <param name="parentNode">기준이 되는 parent node</param>
        /// <param name="xpath">검색 식</param>
        /// <returns>XmlNodeList</returns>
        protected virtual XmlNodeList FindElements(XmlNode parentNode, string xpath) {
            parentNode.ShouldNotBeNull("parentNode");

            if(IsDebugEnabled)
                log.Debug("요소 찾기... parentNode=[{0}], xpath=[{1}]", parentNode.Name, xpath);

            if(xpath.IsNotWhiteSpace())
                return parentNode.SelectNodes(xpath);

            return null;
        }

        /// <summary>
        /// <paramref name="node"/>가 자식 노드를 가질 수 있는지 판단한다.
        /// </summary>
        /// <param name="node">검사할 노드</param>
        /// <returns>자식노드를 가질 수 있는지 여부</returns>
        protected virtual bool IsNodeCanHaveChildNode(XmlNode node) {
            return node.IsNodeCanHaveChildNode();
        }

        /// <summary>
        /// XmlNode를 찾는다.
        /// </summary>
        /// <param name="xpath">XPATH 형식 ("//ROOT") </param>
        public XmlNode FindNode(string xpath) {
            return FindNode(DocumentElement, xpath);
        }

        /// <summary>
        /// 지정된 노드내에서 XmlNode를 찾는다.
        /// </summary>
        /// <param name="parentNode">부모 노드</param>
        /// <param name="xpath">XPATH 형식 ("ROOT") </param>
        public XmlNode FindNode(XmlNode parentNode, string xpath) {
            parentNode.ShouldNotBeNull("parentNode");

            if(IsDebugEnabled)
                log.Debug("노드 찾기... parentNode=[{0}], xpath=[{1}]", parentNode.Name, xpath);

            return parentNode.SelectSingleNode(xpath);
        }

        /// <summary>
        /// <paramref name="parentNode"/>에 새로운 Attribute를 추가한다.
        /// </summary>
        /// <param name="parentNode">부모 노드</param>
        /// <param name="name">특성 이름</param>
        /// <param name="attrValue">특성 값</param>
        /// <param name="replace">기존 존재시 대체 여부</param>
        /// <returns>성공시 추가된 XmlAttribute, 실패시에는 null</returns>
        /// <exception cref="ArgumentNullException">parentNode가 null일 때</exception>
        public XmlAttribute AddAttribute(XmlNode parentNode, string name, object attrValue, bool replace) {
            parentNode.ShouldNotBeNull("parentNode");
            name.ShouldNotBeWhiteSpace("name");

            if(IsDebugEnabled)
                log.Debug("노드에 새로운 Attribute를 추가합니다... parentNode=[{0}], name=[{1}], attrValue=[{2}], replace=[{3}]",
                          parentNode.Name, name, attrValue, replace);

            var result = (XmlAttribute)parentNode.Attributes.GetNamedItem(name.Replace(" ", "_"));

            if(result == null) {
                result = CreateAttribute(name);
                if(attrValue != null)
                    result.Value = attrValue.ToString();
                parentNode.Attributes.Append(result);
            }
            else if(replace && attrValue != null) {
                result.Value = attrValue.ToString();
            }

            return result;
        }

        /// <summary>
        /// <paramref name="parentNode"/>에 새로운 Attribute를 추가한다.
        /// </summary>
        /// <param name="parentNode">부모 노드</param>
        /// <param name="name">특성 이름</param>
        /// <param name="attrValue">특성 값</param>
        /// <returns></returns>
        public XmlAttribute AddAttribute(XmlNode parentNode, string name, object attrValue) {
            return AddAttribute(parentNode, name, attrValue, true);
        }

        /// <summary>
        /// parentNode에 Attribute를 추가한다.
        /// </summary>
        /// <param name="parentNode">부모 노드</param>
        /// <param name="srcAttribute">원본 XmlAttribute 객체</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">parentNode가 null일 때</exception>
        public XmlAttribute AddAttribute(XmlNode parentNode, XmlAttribute srcAttribute) {
            parentNode.ShouldNotBeNull("parentNode");
            srcAttribute.ShouldNotBeNull("srcAttribute");

            if(IsDebugEnabled)
                log.Debug("Add instance of Addtribute to parent node. parentNode=[{0}], srcAttribute=[{1}]",
                          parentNode.Name, srcAttribute.Name);

            return (XmlAttribute)parentNode.Attributes.SetNamedItem(srcAttribute);
        }

        /// <summary>
        /// XmlCDataSection Node를 부모 노드에 추가한다. (&lt;![CDATA[ xxxx ]]&gt;)
        /// </summary>
        /// <param name="parentNode">부모노드</param>
        /// <param name="cdata">CDataSection의 값</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">parentNode가 null일 때</exception>
        public XmlCDataSection AddCDataSection(XmlNode parentNode, string cdata) {
            parentNode.ShouldNotBeNull("parentNode");

            if(IsDebugEnabled)
                log.Debug("Add CDataSection node... parentNode=[{0}], cdata=[{1}]", parentNode.Name, cdata);

            var cds = CreateCDataSection(cdata);
            parentNode.AppendChild(cds);

            return cds;
        }

        /// <summary>
        /// XmlCommend Node를 추가한다.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="comment">Commant Text</param>
        /// <returns></returns>
        public XmlComment AddComment(XmlNode parentNode, string comment) {
            parentNode.ShouldNotBeNull("parentNode");

            if(IsDebugEnabled)
                log.Debug("Add Comment node... parentNode=[{0}], comment=[{1}]", parentNode.Name, comment);

            var result = CreateComment(comment);
            return parentNode.AppendChild(result) as XmlComment;
        }

        /// <summary>
        /// Xml 문자열을 XmlDocument로 만들어서 Root Node를 부모 노드의 자식 노드로 추가한다.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="xmlText"></param>
        /// <returns>추가에 성공한 노드, 실패하면 null이 반환된다.</returns>
        public XmlElement AddElementXml(XmlNode parentNode, string xmlText) {
            parentNode.ShouldNotBeNull("parentNode");

            if(IsDebugEnabled)
                log.Debug("새로운 XmlElement를 추가합니다... parentNode=[{0}], xmlText=[{1}]", parentNode.Name, xmlText);

            XmlElement result = null;

            try {
                XmlDocument document = xmlText.CreateXmlDocument();
                if(document.IsValidDocument()) {
                    result = document.DocumentElement.CloneNode(true) as XmlElement;
                    if(result != null)
                        parentNode.AppendChild(result);
                }
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("새로운 XmlElement를 추가하는데 실패했습니다.");
                    log.Warn(ex);
                }

                result = null;
            }
            return result;
        }

        /// <summary>
        /// XmlElement를 생성하여 부모 노드에 추가한다.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="name">Element Tag Name (Element의 TagName은 문자, '_', ':' 만 가능하다.)</param>
        /// <param name="text">Element Text Value</param>
        /// <returns>추가된 XmlElement</returns>
        public XmlElement AddElementText(XmlNode parentNode, string name, string text) {
            parentNode.ShouldNotBeNull("parentNode");

            if(IsDebugEnabled)
                log.Debug("XmlElement를 추가합니다... parentNode=[{0}], name=[{1}], text=[{2}]", parentNode.Name, name, text);

            var result = CreateElementText(name, text);
            parentNode.AppendChild(result);

            return result;
        }

        /// <summary>
        /// 부모 노드에 원본 노드의 복사본을 추가한다.
        /// </summary>
        /// <param name="parentNode">부모 노드</param>
        /// <param name="srcNode">복사할 대상 요소</param>
        /// <returns>새로 복사해서 부모노드에 추가한 요소(<see cref="XmlElement"/>)</returns>
        public XmlElement AddElement(XmlNode parentNode, XmlElement srcNode) {
            parentNode.ShouldNotBeNull("parentNode");
            srcNode.ShouldNotBeNull("srcNode");

            if(IsDebugEnabled)
                log.Debug("XmlElement를 추가합니다... parentNode=[{0}], srcNode=[{1}]", parentNode.Name, srcNode.Name);

            var result = srcNode.CloneNode(true) as XmlElement;

            if(result != null) {
                parentNode.AppendChild(result);

                if(IsDebugEnabled)
                    log.Debug("XmlElement를 추가했습니다!!! 추가된 XmlElement=[{0}]", result);
            }

            return result;
        }

        /// <summary>
        /// 부모노드에 지정된 이름과 지정된 Text 값을 가진 <see cref="XmlElement"/>를 Parent Node에 추가한다.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="name"></param>
        /// <param name="text">XmlElement의 Text 속성</param>
        /// <returns></returns>
        public XmlElement AddElement(XmlNode parentNode, string name, string text) {
            return AddElementText(parentNode, name, text);
        }

        /// <summary>
        /// 부모노드에 <see cref="XmlEntity"/>를 추가한다.
        /// </summary>
        /// <param name="parentNode">부모 노드</param>
        /// <param name="name">Entity 노드 이름</param>
        /// <returns>추가된 <see cref="XmlEntity"/> 노드</returns>
        public XmlEntity AddEntity(XmlNode parentNode, string name) {
            parentNode.ShouldNotBeNull("parentNode");

            if(IsDebugEnabled)
                log.Debug("Add XmlEntity node... parentNode=[{0}], name=[{1}]", parentNode.Name, name);

            var result = (XmlEntity)CreateNode(XmlNodeType.Entity, name, null);

            if(DocumentType != null)
                DocumentType.Entities.SetNamedItem(result);

            parentNode.AppendChild(result);

            return result;
        }

        /// <summary>
        /// <paramref name="parentNode"/>에 <see cref="XmlEntityReference"/>를 추가한다.
        /// </summary>
        /// <param name="parentNode">대상 노드</param>
        /// <param name="name">추가할 <see cref="XmlEntityReference"/>의 이름</param>
        /// <returns>추가된 <see cref="XmlEntityReference"/></returns>
        public XmlEntityReference AddEntityRef(XmlNode parentNode, string name) {
            parentNode.ShouldNotBeNull("parentNode");

            if(IsDebugEnabled)
                log.Debug("Add XmlEntityReference node... parentNode=[{0}], name=[{1}]", parentNode.Name, name);

            var result = CreateEntityReference(name);
            parentNode.AppendChild(result);

            return result;
        }

        /// <summary>
        /// XML Element로 파일 내용을 넣는다<br/>
        /// 파일내용은 bin.base64 형태로 XML string을 만들어서 XmlElement의 InnerText로 넣는다.
        /// </summary>
        /// <param name="parentNode">부모 노드</param>
        /// <param name="name">새로운 XML Element TagName</param>
        /// <param name="filename">DOM에 포함시킬 파일 이름(fullpath)</param>
        /// <returns>추가된 XmlElement</returns>
        /// <remarks>
        ///	파일 내용을 XML DOM에 넣어서 기존의 MSXML2.IXmlHttp를 사용하면 속도문제가 크다.<br />
        ///	.NET Version에서는 XMLHttp를 WebHttpRequest/WebHttpResponse를 사용하므로 문제없다. 
        ///	Client용 Application 중 MSXML 2,3,4를 사용하면 속도문제는 해결되지 않는다.
        /// </remarks>
        public XmlElement AddFileElement(XmlNode parentNode, string name, string filename) {
            parentNode.ShouldNotBeNull("parentNode");

            if(IsDebugEnabled)
                log.Debug("Add XmlElement that has file contents. parentNode=[{0}], name=[{1}], filename=[{2}]",
                          parentNode.Name, name, filename);

            if(File.Exists(filename) == false)
                throw new FileNotFoundException("File not founded", filename);

            var element = AddElementText(parentNode, name, string.Empty);

            if(element != null) {
                AddAttribute(element, "dt", "bin.base64");
                AddAttribute(element, "filePath", filename);
                AddAttribute(element, "length", filename.GetFileSize());

                element.InnerText = FileTool.ToByteArray(filename).Base64Encode();
            }

            return element;
        }

        /// <summary>
        /// 부모 노드에 노드를 추가한다.
        /// </summary>
        /// <param name="parentNode">부모노드</param>
        /// <param name="nodeType">System.Xml.XmlNodeType</param>
        /// <param name="name">Tag Name</param>
        /// <param name="text">Node Value or Text</param>
        /// <returns>추가된 System.Xml.XmlNode</returns>
        public XmlNode AddNode(XmlNode parentNode, XmlNodeType nodeType, string name, string text) {
            parentNode.ShouldNotBeNull("parentNode");

            XmlNode result;

            switch(nodeType) {
                case XmlNodeType.Element:
                    result = AddElementText(parentNode, name, text);
                    break;
                case XmlNodeType.Attribute:
                    result = AddAttribute(parentNode, name, text);
                    break;
                case XmlNodeType.Text:
                    result = AddTextNode(parentNode, text);
                    break;
                case XmlNodeType.CDATA:
                    result = AddCDataSection(parentNode, text);
                    break;
                case XmlNodeType.EntityReference:
                    result = AddEntityRef(parentNode, name);
                    break;
                case XmlNodeType.ProcessingInstruction:
                    result = AddPI(parentNode, name, text);
                    break;
                case XmlNodeType.Comment:
                    result = AddComment(parentNode, text);
                    break;
                case XmlNodeType.Entity:
                    result = AddEntity(parentNode, text);
                    break;

                case XmlNodeType.Document:
                case XmlNodeType.DocumentType:
                case XmlNodeType.DocumentFragment:
                case XmlNodeType.Notation:
                    throw new NotSupportedException("제공하지 않는 NodeType 입니다.");

                default:
                    throw new InvalidOperationException("알 수 없는 NodeType 입니다.");
            }

            return result;
        }

        /// <summary>
        /// Processing Instruction Node를 추가한다.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="target"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public XmlProcessingInstruction AddPI(XmlNode parentNode, string target, string data) {
            parentNode.ShouldNotBeNull("parentNode");

            if(IsDebugEnabled)
                log.Debug("Add XmlProcessingInstruction node. parentNode=[{0}], target=[{1}], data=[{2}]",
                          parentNode.Name, target, data);

            var result = CreateProcessingInstruction(target, data);
            parentNode.AppendChild(result);

            return result;
        }

        /// <summary>
        /// Text Node를 추가한다.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public XmlText AddTextNode(XmlNode parentNode, string text) {
            parentNode.ShouldNotBeNull("parentNode");

            if(IsDebugEnabled)
                log.Debug("Add XmlText node. parentNode=[{0}], text=[{1}]", parentNode.Name, text);

            var result = CreateTextNode(text);
            parentNode.AppendChild(result);

            return result;
        }

        /// <summary>
        /// Element 내의 지정한 이름의 Attribute를 반환한다.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <returns>Attribute 객체, 없으면 null 반환</returns>
        public XmlAttribute GetAttributeNode(XmlElement node, string name) {
            return (node != null) ? node.GetAttributeNode(name) : null;
        }

        /// <summary>
        /// XmlElement를 찾는다.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public XmlElement GetElement(XmlNode parentNode, string xpath) {
            return FindElement(parentNode, xpath);
        }

        /// <summary>
        /// <paramref name="xpath"/>로 검색한 XmlElement 노드를 반환한다. - <see cref="XmlDoc.FindElement(string)"/>와 같다.
        /// </summary>
        /// <param name="xpath">검색 식</param>
        /// <returns>검색한 XmlElement 인스턴스, 없으면 null</returns>
        public XmlElement GetElement(string xpath) {
            return FindElement(xpath);
        }

        /// <summary>
        /// XmlElement를 찾아서 InnerText값을 반환한다.
        /// </summary>
        /// <param name="xpath">검색 식</param>
        /// <returns>검색한 XmlElement의 Text 속성 값, 없으면 빈 문자열 반환</returns>
        public string GetElementText(string xpath) {
            return GetElementText(this, xpath);
        }

        /// <summary>
        /// <paramref name="parentNode"/>를 기준으로 XmlElement를 찾아서 InnerText값을 반환한다.
        /// </summary>
        /// <param name="parentNode">검색 기준이 되는 parent node</param>
        /// <param name="xpath">검색 식</param>
        /// <returns>검색한 XmlElement의 Text 속성 값, 없으면 빈 문자열 반환</returns>
        public string GetElementText(XmlNode parentNode, string xpath) {
            parentNode.ShouldNotBeNull("parentNode");

            var element = FindElement(parentNode, xpath);

            return (element != null) ? element.InnerText : string.Empty;
        }

        /// <summary>
        /// xpath에 해당하는 XmlElement 들을 찾아서 Element의 Text 를 string array로 반환한다.
        /// </summary>
        /// <returns>문자열 배열, 찾은 내용이 없으면 길이가 0인 문자열 배열</returns>
        public string[] GetElementTextArray(string xpath) {
            var texts = new List<string>();

            var nodeList = FindElements(xpath);

            if(nodeList != null) {
                for(int i = 0; i < nodeList.Count; i++) {
                    var element = nodeList[i] as XmlElement;
                    var text = (element != null) ? element.InnerText : string.Empty;
                    texts.Add(text);
                }
            }
            return texts.ToArray();
        }

        /// <summary>
        /// 새로운 XmlElement를 refNode 앞에 추가한다.
        /// </summary>
        /// <returns>실패시에는 null을 반환한다.</returns>
        public XmlElement InsertElementBefore(XmlElement refNode, string name, string text) {
            refNode.ShouldNotBeNull("refNode");
            name.ShouldNotBeWhiteSpace("name");

            if(IsDebugEnabled)
                log.Debug("Insert new XmlElement before reference node. refNode=[{0}], name=[{1}], text=[{2}]",
                          refNode.Name, name, text);


            return InsertElementBefore(refNode,
                                       CreateElement(name, text));
        }

        /// <summary>
        /// 새로운 XmlElement를 refNode 앞에 추가한다.
        /// </summary>
        public XmlElement InsertElementBefore(XmlElement refNode, XmlElement newNode) {
            refNode.ShouldNotBeNull("refNode");

            if(refNode.ParentNode != null && newNode != null)
                return refNode.ParentNode.InsertBefore(newNode, refNode) as XmlElement;

            throw new XmlException("Invalid reference Node, Invalid Inserting Node");
        }

        /// <summary>
        /// 새로운 XmlElement를 refNode 앞에 추가한다.
        /// </summary>
        public XmlElement InsertElementBefore(XmlElement refNode, string xml) {
            refNode.ShouldNotBeNull("refNode");

            XmlElement result = null;

            var document = xml.CreateXmlDocument();

            if(document.IsValidDocument())
                result = InsertElementBefore(refNode, document.Root);

            return result;
        }

        /// <summary>
        /// 속도가 느리기 때문에 사용하지 안는 것이 좋다.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsValidNodeIndex(int index) {
            return ((index >= 0) && (index < SelectNodes("//").Count));
        }

        /// <summary>
        /// 지정된 Element를 삭제한다.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>삭제여부</returns>
        public bool RemoveElement(XmlElement element) {
            if(element == null)
                return false;

            var result = false;

            if(element.ParentNode != null) {
                element.ParentNode.RemoveChild(element);
                result = true;
            }

            if(IsDebugEnabled)
                log.Debug("Remove XmlElement node. element name=[{0}], removed=[{1}]", element.Name, result);

            return result;
        }

        /// <summary>
        /// Element Node를 찾아서 삭제한다.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="xpath"></param>
        /// <returns>제거 여부</returns>
        public bool RemoveElement(XmlNode parentNode, string xpath) {
            if(parentNode == null)
                return false;

            if(IsDebugEnabled)
                log.Debug("RemoveElement: parentNode=[{0}], xpath=[{1}]", parentNode, xpath);

            var list = parentNode.SelectNodes(xpath);

            if(list != null) {
                for(var i = list.Count - 1; i >= 0; i--) {
                    var node = list[i];

                    if(node != null)
                        parentNode.RemoveChild(node);
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// NamedNodeMap에서 해당 node를 삭제한다.
        /// </summary>
        public XmlNode RemoveNamedItem(XmlNamedNodeMap nodeMap, XmlNode node) {
            return nodeMap.RemoveNamedItem(node.Name);
        }

        /// <summary>
        /// NamedNodeMap(대부분 XmlAttributeCollection)에서 해당 node를 삭제한다.
        /// </summary>
        /// <param name="nodeMap"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public XmlNode RemoveNamedItem(XmlNamedNodeMap nodeMap, string nodeName) {
            return nodeMap.RemoveNamedItem(nodeName);
        }

        /// <summary>
        /// xpath로 찾은 모든 XmlNode를 삭제한다.
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns>제거된 노드의 수</returns>
        public int RemoveSelection(string xpath) {
            if(xpath != null)
                return XmlTool.RemoveSelection(this, xpath);

            return 0;
        }

        /// <summary>
        /// File 정보를 담고 있는 XmlElement를 filename으로 저장한다.
        /// </summary>
        /// <param name="fileNode"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public string SaveFileElement(XmlElement fileNode, string filename) {
            return SaveFileElement(fileNode, filename, true);
        }

        /// <summary>
        /// File 정보를 담고 있는 XmlElement를 filename으로 저장한다.
        /// </summary>
        /// <param name="fileNode">파일 Stream 을 가지고 있는 Node</param>
        /// <param name="filename">저장할 파일 이름</param>
        /// <param name="overwrite">겹쳐쓰기 여부</param>
        /// <returns>저장된 파일 이름</returns>
        public string SaveFileElement(XmlElement fileNode, string filename, bool overwrite) {
            fileNode.ShouldNotBeNull("fileNode");

            if(IsDebugEnabled)
                log.Debug("파일정보를 담은 요소를 파일로 저장합니다... fileNode=[{0}], filename=[{1}], overwrite=[{2}]", fileNode.Name, filename,
                          overwrite);

            var buff = Convert.FromBase64String(fileNode.InnerText);

            if(!overwrite && File.Exists(filename))
                filename = filename.FindNewFileName();

            Path.GetDirectoryName(filename).CreateDirectory();

            using(var bs = FileTool.GetBufferedFileStream(filename, FileOpenMode.ReadWrite))
                bs.Write(buff, 0, buff.Length);

            if(IsDebugEnabled)
                log.Debug("파일정보를 담은 요소를 파일로 저장했습니다!!! filename=[{0}]", filename);

            return filename;
        }

        /// <summary>
        /// Xml 내용을 파일로 저장한다.
        /// </summary>
        /// <param name="filename">전체 경로 명</param>
        public void SaveToFile(string filename) {
            // Directory가 있는지 검사.
            Path.GetDirectoryName(filename).CreateDirectory();

            Save(filename);
        }

        /// <summary>
        /// Xml 내용을 System Temporary Directory에 저장한다.
        /// </summary>
        /// <param name="filename">저장할 파일명 (전체 경로가 아닌 파일명만 적는다.)</param>
        public void SaveToTemp(string filename) {
            string path = Path.Combine(FileTool.GetTempPath(), filename.ExtractFileName());
            SaveToFile(path);
        }

        /// <summary>
        /// xpath로 찾은 Element의 InnerText값을 지정한다.
        /// </summary>
        /// <param name="xpath">XPath</param>
        /// <param name="text">탐색한 XmlElement에 설정할 Text값</param>
        /// <exception cref="XmlException">해당 Element를 찾지 못했을 때</exception>
        public void SetElementText(string xpath, string text) {
            var element = FindElement(xpath);

            if(element != null)
                element.InnerText = text;
            else {
                if(log.IsWarnEnabled)
                    log.Warn("Fail to set Text of XmlElement. specified node not found. xpath=[{0}], text=[{1}]", xpath, text);
            }
        }

        /// <summary>
        /// XmlNamedNodeMap에 Node를 추가한다.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodeMap"></param>
        /// <returns>실패시 null 반환</returns>
        public XmlNode SetNamedItem(XmlNamedNodeMap nodeMap, XmlNode node) {
            if(nodeMap != null && node != null)
                return nodeMap.SetNamedItem(node);

            return null;
        }

        /// <summary>
        /// XmlNamedNodeMap에 Node를 추가한다.
        /// </summary>
        /// <returns>실패시 null 반환</returns>
        public XmlNode SetNamedItem(XmlNamedNodeMap nodeMap, string name, string value) {
            if(nodeMap != null && name != null) {
                XmlNode node = CreateAttribute(name);

                if(value != null)
                    node.Value = value;

                SetNamedItem(nodeMap, node);

                return node;
            }

            return null;
        }

        /// <summary>
        /// Element의 TagName은 문자, '_', ':' 만 가능하다. 
        /// 되도록 Element Tag Name에 특수 문자를 사용하지 마시요.
        /// </summary>
        /// <param name="tagName">Element의 TagName</param>
        /// <returns>사용 가능여부</returns>
        /// <remarks>
        ///	이 함수는 되도록 사용하지 마십시요. DOM을 생성해서 TEST하는 것이므로 속도가 매우 느립니다.
        /// </remarks>
        public static bool IsValidTagName(string tagName) {
            bool result;
            var xml = string.Format("<{0}/>", tagName);

            try {
                XmlDocument document = xml.CreateXmlDocument();
                result = document.IsValidDocument();
            }
            catch {
                result = false;
            }
            return result;
        }
    }
}