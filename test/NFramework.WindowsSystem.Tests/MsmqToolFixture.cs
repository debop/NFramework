using System;
using System.Messaging;
using NUnit.Framework;

namespace NSoft.NFramework.WindowsSystem {
    [TestFixture]
    public class MsmqToolFixture {
        private const string SamplePrivateQueue = @"NSoft.NFrameworkTest";

        private static MessageQueue GetPrivateQueue(string queueName) {
            return MsmqTool.CreatePrivateQueue(queueName, queueName, false);
        }

        [Test]
        public void CreatePrivateQueueTest() {
            var q = GetPrivateQueue(SamplePrivateQueue);

            Assert.IsNotNull(q);
            Assert.AreEqual(q.Label, SamplePrivateQueue);
        }

        [Test]
        public void SendMessageTest() {
            MsmqTool.Send(MsmqTool.PRIVATE_PATH + SamplePrivateQueue, "Simple Text", "Sample Label");
            MsmqTool.Send(MsmqTool.PRIVATE_PATH + SamplePrivateQueue, "Simple Text2", "Sample Label2");
        }

        [Test]
        public void GetAllMessageTest() {
            foreach(Message m in MsmqTool.GetAllMessages(MsmqTool.PRIVATE_PATH + SamplePrivateQueue))
                Console.WriteLine("Message Body: " + m.Body);
        }

        [Test]
        public void EnumerationTest() {
            var me =
                MsmqTool.GetMessageEnumerator(MsmqTool.PRIVATE_PATH + SamplePrivateQueue, new[] { typeof(string) });
            try {
                while(me.MoveNext()) {
                    var m = me.Current;
                    Console.WriteLine("Message Body: " + m.Body);

                    me.RemoveCurrent(); // Pick 이냐 Retrieve 냐
                }
            }
            finally {
                if(me != null)
                    me.Dispose();
            }
        }
    }
}