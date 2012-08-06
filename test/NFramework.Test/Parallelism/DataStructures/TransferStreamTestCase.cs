using System.IO;
using System.Text;
using NSoft.NFramework.Tools;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Parallelism.DataStructures {
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class TransferStreamTestCase : ParallelismFixtureBase {
        private const int TestCount = 100;
        private const string Message = @"동해물과 백두산이 마르고 닳도록 Oh Happy Day!";

        [Test]
        public void WriteToStreamUsingBackgroundThread() {
            var buffer = Encoding.UTF8.GetBytes(Message);

            using(var targetStream = new MemoryStream()) {
                var transferStream = new TransferStream(targetStream);

                // 실제 targetStream에 쓰는 작업이 아니라 쓰기를 예약한 것입니다.
                //
                for(int i = 0; i < TestCount; i++) {
                    // 쓰기 요청은 했지만 실제로 targetStream에 쓰여지는 작업는 비동기적으로 수행되므로 아직 쓰여지지 않았다.
                    Assert.AreNotEqual(buffer.Length * TestCount, (int)targetStream.Length);

                    transferStream.Write(buffer, 0, buffer.Length);
                }

                // 이 때에 모든 쓰기가 완료될 것이고, 대상 Stream에 완전히 복사가 됩니다.
                transferStream.Close();

                Assert.AreEqual(buffer.Length * TestCount, (int)targetStream.Length);

                targetStream.ToText(Encoding.UTF8).Contains(Message).Should().Be.True();
            }
        }

        [Test]
        public void WriteToStreamUsingBackgroundThread_WriteToClosedTransferStream() {
            var buffer = Encoding.UTF8.GetBytes(Message);

            using(var targetStream = new MemoryStream()) {
                var transferStream = new TransferStream(targetStream);

                // 실제 targetStream에 쓰는 작업이 아니라 쓰기를 예약한 것입니다.
                //
                for(int i = 0; i < TestCount; i++) {
                    // 쓰기 요청은 했지만 실제로 targetStream에 쓰여지는 작업는 비동기적으로 수행되므로 아직 쓰여지지 않았다.
                    Assert.AreNotEqual(buffer.Length * TestCount, (int)targetStream.Length);

                    transferStream.Write(buffer, 0, buffer.Length);
                }

                // 이 때에 모든 쓰기가 완료될 것이고, 대상 Stream에 완전히 복사가 됩니다.
                transferStream.Close();

                // 여기서 예외가 발생해야 한다??? 아니다. close 가 되면 쓰기를 취소하고 그냥 반환한다.
                //
                transferStream.Write(buffer, 0, buffer.Length);

                Assert.AreEqual(buffer.Length * TestCount, (int)targetStream.Length);

                targetStream.ToText(Encoding.UTF8).Contains(Message).Should().Be.True();
            }
        }

        [Test]
        public void Thread_Test() {
            TestTool.RunTasks(4,
                              WriteToStreamUsingBackgroundThread,
                              WriteToStreamUsingBackgroundThread_WriteToClosedTransferStream,
                              WriteToStreamUsingBackgroundThread,
                              WriteToStreamUsingBackgroundThread_WriteToClosedTransferStream);
        }
    }
}