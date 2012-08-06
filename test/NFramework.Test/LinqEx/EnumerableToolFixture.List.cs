using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace NSoft.NFramework.LinqEx {
    [Microsoft.Silverlight.Testing.Tag("Linq")]
    [TestFixture]
    public class EnumerableToolFixture_List : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        [Test]
        public void RunEachAsync_Action_Test() {
            int actionCount = 0;
            Action<int> action = x => {
                                     Thread.Sleep(Rnd.Next(5, 100));
                                     Console.WriteLine(x);
                                     Interlocked.Increment(ref actionCount);
                                 };

            Enumerable
                .Range(0, 100)
                .RunEachAsync(action);

            Assert.AreEqual(100, actionCount);
        }

        [Test]
        public void RunEachAsync_Function_Test() {
            Func<int, double> @square = x => {
                                            Thread.Sleep(Rnd.Next(10, 100));
                                            return x * x;
                                        };

            Enumerable
                .Range(1, 100)
                .RunEachAsync(x => @square(x)).All(x => x > 0);
        }
    }
}