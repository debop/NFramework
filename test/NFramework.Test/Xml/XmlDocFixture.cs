using System;
using System.Xml;
using NUnit.Framework;

namespace NSoft.NFramework.Xml {
    [TestFixture]
    public class XmlDocFixture {
        [Test]
        public void CreateXmlDocument() {
            var doc = new XmlDoc();
            Assert.IsFalse(doc.IsValidDocument);
        }

        [Test]
        public void CreateXmlDocumentByString() {
            var doc = new XmlDoc("<root/>");
            Assert.IsTrue(doc.IsValidDocument);
            Console.WriteLine(doc.InnerXml);
        }

        [Test]
        public void AddElementTest() {
            var doc = new XmlDoc("<root/>");
            var child = doc.AddElementText(doc.DocumentElement, "child", "child element text");
            Assert.IsNotNull(child);

            var grandChild = doc.AddElementText(child, "grandChild", "child of child element text");
            Assert.IsNotNull(grandChild);

            Console.WriteLine("Child.OuterXml : " + child.OuterXml);
            Console.WriteLine("Child.InnerXml : " + child.InnerXml);
            Console.WriteLine("Child.InnerText : " + child.InnerText);
        }

        [Test]
        public void InsertElementTest() {
            var doc = new XmlDoc("<root/>");
            var node = doc.InsertBefore(doc.CreateXmlDeclaration("1.0", "utf-8", ""), doc.DocumentElement);
            Assert.IsNotNull(node);

            var firstElement = doc.AddElementText(doc.DocumentElement, "first", "");
            Assert.IsNotNull(firstElement);

            var second = doc.InsertElementBefore(firstElement, "second", "");
            Assert.IsNotNull(second);
        }

        [Test]
        public void AddNodeTest() {
            var doc = new XmlDoc("<root/>");
            Assert.IsTrue(XmlTool.IsValidDocument(doc));

            var element = doc.AddNode(doc.DocumentElement, XmlNodeType.Element, "Element", "ElementText");
            Assert.IsNotNull(element);
            Assert.AreEqual("Element", element.Name);

            // XmlComment 노드는 
            var comment = doc.AddNode(element, XmlNodeType.Comment, string.Empty, "CommentsText");
            Assert.AreEqual("#comment", comment.Name);
            Assert.AreEqual("CommentsText", comment.Value);
        }

        [Test]
        public void AddXmlAttributeTest() {
            var doc = new XmlDoc("<root/>");

            var idAttr = doc.AddAttribute(doc.DocumentElement, "id", "root id");
            var nameAttr = doc.AddAttribute(doc.DocumentElement, "name", "root name");

            Assert.AreEqual("id", idAttr.Name);
            Assert.AreEqual("root id", idAttr.Value);
            Assert.AreEqual("name", nameAttr.Name);
            Assert.AreEqual("root name", nameAttr.Value);

            idAttr = doc.GetAttributeNode(doc.DocumentElement, "id");
            nameAttr = doc.GetAttributeNode(doc.DocumentElement, "name");

            Assert.AreEqual("id", idAttr.Name);
            Assert.AreEqual("root id", idAttr.Value);
            Assert.AreEqual("name", nameAttr.Name);
            Assert.AreEqual("root name", nameAttr.Value);
        }
    }
}