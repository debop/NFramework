using System;
using System.Linq;
using System.Threading;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.ForTesting {
    /// <summary>
    /// Thread 갯수가 많을 수록 ThreadPool을 사용하면, 성능이 향상됩니다. (성능테스트시에는 log 끄고, Release 모드로 해야 제대로 알 수 있습니다.
    /// </summary>
    [TestFixture]
    public class ThreadStressUtiltiyFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private const int LowerBound = 0;
        private const int UpperBound = 1000;

        [TestFixtureSetUp]
        public void ClassSetUp() {
            ThreadPool.SetMinThreads(100, 2000);
        }

        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void ThreadTestWithThreadPool(int numThreads) {
            using(new OperationTimer("Using Thread pool"))
            using(var resetEvent = new ManualResetEvent(false)) {
                int workerThreads = numThreads;

                for(var thread = 0; thread < numThreads; thread++) {
                    int thread1 = thread;

                    if(IsDebugEnabled)
                        log.Debug("새로운 Thread를 ThreadPool에 추가합니다. Thread=" + thread);

                    ThreadPool.QueueUserWorkItem(state => {
                                                     for(int i = LowerBound; i < UpperBound; i++) {
                                                         if((i % numThreads) == thread1) {
                                                             if(IsDebugEnabled)
                                                                 log.Debug("FindRoot({0}) returns {1}", i, Hero.FindRoot(i));
                                                         }
                                                     }

                                                     if(Interlocked.Decrement(ref workerThreads) == 0)
                                                         resetEvent.Set();
                                                 }
                        );
                }
                resetEvent.WaitOne();
            }
        }

        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void ThreadTestWithThreadPoolAndManualResetEventSlim(int numThreads) {
            using(new OperationTimer("Using Thread pool"))
            using(var resetEvent = new ManualResetEventSlim()) {
                int workerThreads = numThreads;

                for(var thread = 0; thread < numThreads; thread++) {
                    int thread1 = thread;

                    if(IsDebugEnabled)
                        log.Debug("새로운 Thread를 ThreadPool에 추가합니다. Thread=" + thread);


                    ThreadPool.QueueUserWorkItem(state => {
                                                     for(int i = LowerBound; i < UpperBound; i++) {
                                                         if((i % numThreads) == thread1) {
                                                             if(IsDebugEnabled)
                                                                 log.Debug("FindRoot({0}) returns {1}", i, Hero.FindRoot(i));
                                                         }
                                                     }

                                                     if(Interlocked.Decrement(ref workerThreads) == 0)
                                                         resetEvent.Set();
                                                 }
                        );
                }
                resetEvent.Wait();
            }
        }

        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void ThreadTestWithThreadCollection(int numThreads) {
            using(new OperationTimer("Using Thread Collection"))
            using(var resetEvent = new AutoResetEvent(false)) {
                int workerThreads = numThreads;

                for(var thread = 0; thread < numThreads; thread++) {
                    int thread1 = thread;

                    var t = new Thread(() => {
                                           for(int i = LowerBound; i < UpperBound; i++) {
                                               if((i % numThreads) == thread1) {
                                                   if(IsDebugEnabled)
                                                       log.Debug("FindRoot({0}) returns {1}", i, Hero.FindRoot(i));
                                               }
                                           }

                                           if(Interlocked.Decrement(ref workerThreads) == 0)
                                               resetEvent.Set();
                                       }
                        );
                    t.Start();
                }
                resetEvent.WaitOne();
            }
        }

        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void ThreadTestWithThreadCollectionAndManualResetEventSlim(int numThreads) {
            using(new OperationTimer("Using Thread Collection"))
            using(var resetEvent = new ManualResetEventSlim()) {
                int workerThreads = numThreads;

                for(var thread = 0; thread < numThreads; thread++) {
                    int thread1 = thread;

                    new Thread(() => {
                                   for(int i = LowerBound; i < UpperBound; i++) {
                                       if((i % numThreads) == thread1) {
                                           if(IsDebugEnabled)
                                               log.Debug("FindRoot({0}) returns {1}", i, Hero.FindRoot(i));
                                       }
                                   }

                                   if(Interlocked.Decrement(ref workerThreads) == 0)
                                       resetEvent.Set();
                               }
                        )
                        .Start();
                }
                resetEvent.Wait();
            }
        }

        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void ThreadTestByThreadPool(int numThreads) {
            var states = Enumerable.Range(0, numThreads).Cast<object>();

            using(new OperationTimer("Using ThreadStressByThreadPool"))
                TestTool
                    .ThreadStressByThreadPool(state => {
                                                  for(var i = LowerBound; i < UpperBound; i++) {
                                                      if((i % numThreads) == (int)state)
                                                          if(IsDebugEnabled)
                                                              log.Debug("FindRoot({0}) returns {1}", i, Hero.FindRoot(i));
                                                  }
                                              },
                                              states);
        }

        public static class Hero {
            private const double Tolerance = 1.0E-8;

            public static double FindRoot(double number) {
                var guess = 1.0;
                var error = Math.Abs(guess * guess - number);

                while(error > Tolerance) {
                    guess = (number / guess + guess) / 2.0;
                    error = Math.Abs(guess * guess - number);
                }
                return guess;
            }
        }
    }
}