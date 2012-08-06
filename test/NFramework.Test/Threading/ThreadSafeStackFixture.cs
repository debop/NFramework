using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.Collections.Concurrent;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Threading {
    [TestFixture]
    public class ThreadSafeStackFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        private const int MaxNumber = 10000;
        private ThreadSafeStack<int> stack;

        protected override void OnSetUp() {
            base.OnSetUp();
            stack = new ThreadSafeStack<int>();
        }

        [Test]
        public void PushTest() {
            for(var i = 0; i < 100; i++)
                stack.Push(Rnd.Next(MaxNumber));

            for(var i = 0; i < 100; i++)
                stack.Pop().Should().Be.GreaterThanOrEqualTo(0).And.Be.LessThan(MaxNumber);

            stack.Count.Should().Be(0);
        }

        [Test]
        public void PopTest() {
            Task.Factory.StartNew(() => {
                                      Thread.Sleep(100);

                                      for(var i = 0; i < 100; i++) {
                                          Thread.Sleep(10);
                                          stack.Push(Rnd.Next(MaxNumber));
                                      }
                                  });


            for(var i = 0; i < 100; i++)
                stack.Pop().Should().Be.GreaterThanOrEqualTo(0).And.Be.LessThan(MaxNumber);

            stack.Count.Should().Be(0);
        }

        [Test]
        public void TryPopTeset() {
            var mre = new ManualResetEventSlim(false);

            Task.Factory.StartNew(() => {
                                      Thread.Sleep(100);

                                      for(var i = 0; i < 100; i++) {
                                          Thread.Sleep(10);
                                          stack.Push(Rnd.Next(MaxNumber));
                                      }
                                      mre.Set();
                                  });

            for(var i = 0; i < 100; i++) {
                Thread.Sleep(10);

                int item;
                if(stack.TryPop(out item))
                    item.Should().Be.GreaterThanOrEqualTo(0).And.Be.LessThan(MaxNumber);
                else
                    item.Should("item").Be(0);
            }

            mre.Wait();
            stack.Count.Should().Not.Be(0);
        }
    }
}