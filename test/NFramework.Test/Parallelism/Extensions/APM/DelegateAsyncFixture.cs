using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.Extensions.APM {
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class DelegateAsyncFixture : ParallelismFixtureBase {
#if !SILVERLIGHT
        private const int IterationCount = 1000;

        [Test]
        public void DelegateAsync_Run_Action() {
            Action<int> @power = x => {
                                     Thread.Sleep(Rnd.Next(Rnd.Next(1, 10), Rnd.Next(11, 50)));

                                     var result = Math.Pow(x, 5);
                                     if(IsDebugEnabled)
                                         log.Debug("@power({0}) = {1}", x, result);
                                 };

            var tasks = new List<Task>();

            for(var i = 0; i < IterationCount; i++) {
                if(IsDebugEnabled)
                    log.Debug("@power({0}) called...", i);

                Thread.Sleep(0);

                // 함수를 BeginInvoke, EndInvoke로 비동기 실행할 수 있습니다.
                //
                var task = DelegateAsync.Run(@power, i);
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            Assert.IsTrue(tasks.All(task => task.IsCompleted));
        }

        [Test]
        public void DelegateAsync_Run_Function() {
            Func<int, double> @power = x => {
                                           Thread.Sleep(Rnd.Next(Rnd.Next(1, 10), Rnd.Next(11, 50)));

                                           var result = Math.Pow(x, 5);

                                           if(IsDebugEnabled)
                                               log.Debug("@power({0}) = {1}", x, result);

                                           return result;
                                       };

            var tasks = new List<Task<double>>();

            for(var i = 0; i < IterationCount; i++) {
                if(IsDebugEnabled)
                    log.Debug("@power({0}) called...", i);

                // Thread.Sleep(0);

                // 함수를 BeginInvoke, EndInvoke로 비동기 실행할 수 있습니다.
                //
                tasks.Add(DelegateAsync.Run(@power, i));
            }

            Task.WaitAll(tasks.ToArray());

            Assert.IsTrue(tasks.All(task => task.IsCompleted));
            tasks.ForEach(task => {
                              if(IsDebugEnabled)
                                  log.Debug("계산 결과=" + task.Result);
                          });
        }

        /// <summary>
        /// Factorize 처럼 DelegateAsync class에서 지원하지 않는 시그니쳐의 함수는 Wrapping해서 작업하면 됩니다.
        /// </summary>
        [Test]
        public void Wrapping_Factorize_Async() {
            const int number = 1000589023;
            var factor = new[] { 0, 0 };

            Func<int, bool> @factorize = x => {
                                             var temp = factor;
                                             return Factorize(x, ref temp[0], ref temp[1]);
                                         };

            //var task = DelegateAsync.Run(@factorize, number);
            //task.Wait();

            // Assert.IsTrue(task.Result);
            //if(IsDebugEnabled)
            //	log.Debug("Factors of {0} : {1} {2} - {3}", number, factor[0], factor[1], task.Result);

            var factorialized = DelegateAsync.Run(@factorize, number);
            if(IsDebugEnabled)
                log.Debug("Factors of {0} : {1} {2} - {3}", number, factor[0], factor[1], factorialized);
        }

        [Test]
        public void ThreadTest() {
            TestTool.RunTasks(4,
                              () => {
                                  DelegateAsync_Run_Action();
                                  DelegateAsync_Run_Function();
                                  Wrapping_Factorize_Async();
                              });
        }

        /// <summary>
        /// Factorize
        /// </summary>
        /// <param name="number"></param>
        /// <param name="primefactor1"></param>
        /// <param name="primefactor2"></param>
        /// <returns></returns>
        public static bool Factorize(int number, ref int primefactor1, ref int primefactor2) {
            primefactor1 = 1;
            primefactor2 = number;

            for(var i = 2; i < number; i++) {
                if((number % i) == 0) {
                    primefactor1 = i;
                    primefactor2 = number / i;
                    break;
                }
            }
            if(primefactor1 == 1)
                return false;

            return true;
        }
#endif
    }
}