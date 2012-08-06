using System;
using System.Text;
using System.Threading.Tasks;
using NSoft.NFramework.IO;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.Extensions.APM {
    [TestFixture]
    public class FileAsyncTestCase : ParallelismFixtureBase {
        private static readonly string Message = @"동해물과 백두산이 마르고 닳도록... ~!@#$%^&*()_+ Hello World." + Environment.NewLine;
        private static readonly byte[] MessageBuffer = Encoding.UTF8.GetBytes(Message);

        private const int RunCount = 30;

        [Test]
        public void Can_WriteAsync_And_ReadAllText() {
            if(IsDebugEnabled)
                log.Debug("Write Asynchronous and Real all text...");

            var filename = FileTool.GetTempFileName("ASYNC_");

            using(var fs = FileAsync.OpenWrite(filename)) {
                Task chainTask = null;

                for(int i = 0; i < RunCount; i++) {
                    var lineNo = i.ToString() + ": ";
                    var buffer = Encoding.UTF8.GetBytes(lineNo);

                    if(chainTask == null)
                        chainTask = fs.WriteAsync(buffer, 0, buffer.Length);
                    else
                        chainTask.ContinueWith(_ => fs.WriteAsync(buffer, 0, buffer.Length));

                    chainTask = chainTask.ContinueWith(_ => fs.WriteAsync(MessageBuffer, 0, MessageBuffer.Length));
                }

                chainTask.Wait();
            }

            var fileText = With.TryFunctionAsync(() => FileAsync.ReadAllText(filename, Encoding.UTF8).Result, () => string.Empty);
            Assert.IsNotEmpty(fileText);

            Console.WriteLine("Contents = " + Environment.NewLine + fileText);

            With.TryAction(() => FileTool.DeleteFile(filename));
        }

        [Test]
        public void ThreadTest() {
            TestTool.RunTasks(3, Can_WriteAsync_And_ReadAllText);
        }
    }
}