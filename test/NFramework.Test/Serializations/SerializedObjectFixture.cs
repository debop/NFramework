using System;
using System.Xml.Serialization;
using NSoft.NFramework.Serializations.SerializedObjects;
using NSoft.NFramework.TimePeriods;
using NSoft.NFramework.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Serializations {
    [TestFixture]
    public class SerializedObjectFixture {
        //! Bson, Json은 DateTime 값 변환시에 문제가 좀 있다!!! msec 은 포함하지 않는게 좋다.
        //
        private static readonly DateTime Now = DateTime.Now.TrimToMillisecond();

        private static SerializableObject GetSerializableObject(string title) {
            return new SerializableObject
                   {
                       Title = title,
                       Content = "내용은 뭐 있겠나? ".Replicate(100),
                       SendTime = Now,
                       IsRead = false,
                       Sender = "debop",
                       Receiver = "midoogi"
                   };
        }

        private static XmlSerializableObject GetXmlSerializableObject(string title) {
            return new XmlSerializableObject
                   {
                       Title = title,
                       Content = "내용은 뭐 있겠나? ".Replicate(100),
                       SendTime = Now,
                       IsRead = false,
                       Sender = "debop",
                       Receiver = "midoogi"
                   };
        }

        [Test]
        public void JsonSerializedObjectTest() {
            var graph = GetSerializableObject("Json Serializable");

            var serialized = new JsonSerializedObject(graph);

            serialized.Should().Not.Be.Null();
            serialized.Method.Should().Be(SerializationMethod.Json);
            serialized.ObjectTypeName.Should().Contain("SerializableObject");

            var deserialized = (SerializableObject)serialized.GetDeserializedObject();

            deserialized.Should().Not.Be.Null();
            deserialized.Title.Should().Be(graph.Title);
        }

        [Test]
        public void XmlSerializedObjectTest() {
            var graph = GetXmlSerializableObject("Xml Serializable");

            var serialized = new XmlSerializedObject(graph);

            serialized.Should().Not.Be.Null();
            serialized.Method.Should().Be(SerializationMethod.Xml);
            serialized.ObjectTypeName.Should().Contain("XmlSerializableObject");

            var deserialized = (XmlSerializableObject)serialized.GetDeserializedObject();

            deserialized.Should().Not.Be.Null();
            deserialized.Title.Should().Be(graph.Title);
        }

        [Serializable]
        private class SerializableObject : ValueObjectBase {
            #region << logger >>

            [NonSerialized] private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

            private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

            #endregion

            public string Sender { get; set; }

            public string Receiver { get; set; }

            public string Title { get; set; }

            public string Content { get; set; }

            public DateTime? SendTime { get; set; }

            public DateTime? ReceiveTime { get; set; }

            public bool? IsRead { get; set; }
        }

        //! NOTE: XmlSerialization을 위해서는 public 이어야 한다. Json, Bson은 private도 가능하다.
        [Serializable]
        [XmlRoot(ElementName = "SerializableObject", Namespace = "ns.realweb21.com")]
        public class XmlSerializableObject : ValueObjectBase {
            #region << logger >>

            [NonSerialized] private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

            private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

            #endregion

            [XmlAttribute]
            public string Sender { get; set; }

            [XmlAttribute]
            public string Receiver { get; set; }

            [XmlElement]
            public string Title { get; set; }

            [XmlElement]
            public string Content { get; set; }

            [XmlAttribute]
            public DateTime SendTime { get; set; }

            [XmlAttribute]
            public DateTime ReceiveTime { get; set; }

            [XmlAttribute]
            public bool IsRead { get; set; }
        }
    }
}