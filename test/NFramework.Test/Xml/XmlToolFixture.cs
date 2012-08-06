using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using NSoft.NFramework.IO;
using NSoft.NFramework.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Xml {
    [TestFixture]
    public class XmlToolFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private static List<User> _users;

        public static List<User> Users {
            get { return _users ?? (_users = GetSampleUsers(100)); }
        }

        private static List<User> GetSampleUsers(int count) {
            var users = new List<User>();

            for(int i = 0; i < count; i++) {
                var user = new User
                           {
                               Id = Guid.NewGuid(),
                               Name = "User " + i.ToString("X2"),
                               Description = "Xml Serializer 테스트 "
                           };
                users.Add(user);
            }
            return users;
        }

        [Test]
        public void SerializeTest() {
            var user = Users[0];

            using(var ms = new MemoryStream()) {
                XmlTool.Serialize(user, ms);

                ms.SetStreamPosition();

                var str = ms.ToText();
                Assert.IsNotEmpty(str);

                Assert.IsTrue(str.Contains(user.Id.ToString()));
                Console.WriteLine("Serialized Usre=" + str);
            }
        }

        [Test]
        public void SerializeToXDocument() {
            XDocument xdoc;
            Assert.IsTrue(XmlTool.Serialize(Users, out xdoc));
            Assert.IsNotNull(xdoc);
            Assert.IsNotNull(xdoc.Root);
        }

        [Test]
        public void SerializeToXmlDocument() {
            XmlDocument doc;
            Assert.IsTrue(XmlTool.Serialize(Users, out doc));
        }

        [Test]
        public void DeserializeXml() {
            var user = Users[0];

            using(var ms = new MemoryStream()) {
                XmlTool.Serialize(user, ms);

                ms.SetStreamPosition();

                var xml = ms.ToText();
                // string xml = StringTool.ToString(ms);

                ms.SetStreamPosition();
                var user2 = XmlTool.Deserialize<User>(ms);

                Assert.AreEqual(user.Id, user2.Id);
                Assert.AreEqual(user.Name, user2.Name);
                Assert.AreEqual(user.Description, user2.Description);

                Assert.IsTrue(user.Equals(user2));
            }
        }

        [Test]
        public void DeserializeTest_Using_XmlDocument() {
            var user = Users[0];
            XmlDocument xmldoc;

            Assert.IsTrue(XmlTool.Serialize(user, out xmldoc));
            var deserialized = XmlTool.Deserialize<User>(xmldoc);

            Assert.AreEqual(user, deserialized);

            Assert.IsTrue(XmlTool.Serialize(Users, out xmldoc));
            var users2 = XmlTool.Deserialize<List<User>>(xmldoc);
            Assert.AreEqual(Users.Count, users2.Count);
            CollectionAssert.AreEqual(Users, users2);
        }

        [Test]
        public void DeserializeTest_Using_XDocument() {
            var user = Users[0];
            XDocument xdoc;
            Assert.IsTrue(XmlTool.Serialize(user, out xdoc));
            var deserialized = XmlTool.Deserialize<User>(xdoc);

            Assert.AreEqual(user, deserialized);

            Assert.IsTrue(XmlTool.Serialize(Users, out xdoc));
            var users2 = XmlTool.Deserialize<List<User>>(xdoc);
            Assert.AreEqual(Users.Count, users2.Count);
            CollectionAssert.AreEqual(Users, users2);
        }
    }
}