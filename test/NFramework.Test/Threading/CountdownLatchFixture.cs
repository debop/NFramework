using System;
using System.Threading;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Threading {
    [TestFixture]
    public class CountdownLatchFixture : AbstractFixture {
        public const int ThreadCount = 5;

        [Test]
        public void WaitForEventWithTimeout() {
            TestTool.RunTasks(ThreadCount,
                              () => {
                                  var countdown = new CountdownLatch(5);
                                  bool result = countdown.WaitOne(TimeSpan.FromMilliseconds(5));
                                  Assert.IsFalse(result);
                              });
        }

        [Test]
        public void WaintForEventToRun() {
            const int count = 50;

            TestTool.RunTasks(ThreadCount,
                              () => {
                                  var countdown = new CountdownLatch(count);
                                  var done = new bool[count];


                                  for(int i = 0; i < count; i++) {
                                      var j = i;
                                      ThreadPool.QueueUserWorkItem(delegate {
                                                                       done[j] = true;
                                                                       countdown.Set();
                                                                   });
                                  }

                                  bool result = countdown.WaitOne();
                                  Assert.IsTrue(result);

                                  for(var i = 0; i < count; i++)
                                      Assert.IsTrue(done[i], "{0} was not set to true", i);
                              });
        }

        [Test]
        public void WaitForCustomersThrowsIfGetsNegativeCustomers() {
            // 당연히 음수값이 들어가면 예외가 발생하지롱.
            Assert.Throws<InvalidOperationException>(() => new CountdownLatch(-2).WaitOne());
        }

        [Test]
        public void WaitForConsumersDoesNotWaitIfConsumersAreZero() {
            TestTool.RunTasks(ThreadCount,
                              () => {
                                  var result = new CountdownLatch(0).WaitOne();
                                  Assert.IsTrue(result);
                              });
        }

        [Test]
        public void ResetWaitForConsumers() {
            TestTool.RunTasks(ThreadCount,
                              () => {
                                  var countdown = new CountdownLatch(0);
                                  var result = countdown.WaitOne();
                                  Assert.IsTrue(result);

                                  countdown.Reset(5);
                                  result = countdown.WaitOne(TimeSpan.FromMilliseconds(5));
                                  Assert.IsFalse(result);

                                  for(var i = 0; i < 5; i++) {
                                      ThreadPool.QueueUserWorkItem(state => countdown.Set());
                                  }
                                  result = countdown.WaitOne();
                                  Assert.IsTrue(result);
                              });
        }
    }
}