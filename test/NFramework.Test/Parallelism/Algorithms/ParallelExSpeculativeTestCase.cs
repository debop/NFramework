using System;
using System.Collections.Generic;
using System.Threading;
using NSoft.NFramework.Parallelism.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.Algorithms {
    /// <summary>
    /// Speculative retrive value ( function-race ) 를 위한 테스트입니다.
    /// </summary>
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class ParallelToolSpeculativeTestCase : ParallelToolTestCaseBase {
        private const int PowerNumber = 4;
        private static readonly Func<int, double>[] _functions = CreateSampleFunctions();

        /// <summary>
        /// Speculative Processing을 위해 실행시간이 다양한 여러개의 함수를 만듭니다. 
        /// </summary>
        /// <returns></returns>
        private static Func<int, double>[] CreateSampleFunctions() {
            var functions = new List<Func<int, double>>();

            for(int i = 0; i < TestCount; i++) {
                var min = Rnd.Next(1, 10);
                var max = Rnd.Next(15, 40);

                Func<int, double> func = x => {
                                             // 다양한 실행시간을 표현
                                             Thread.Sleep(Rnd.Next(min, max));
                                             return Math.Pow(x, PowerNumber);
                                         };

                functions.Add(func);
            }

            return functions.ToArray();
        }

        [Test]
        public void Can_SpeculativeFor() {
            var @functions = _functions;

            for(var i = 0; i < MaxTestCount; i++) {
                var testCount = i;
                var result = ParallelTool.SpeculativeFor(0, @functions.Length, n => @functions[n](testCount));
                Assert.AreEqual(Math.Pow(i, PowerNumber), result);
            }
        }

#if !SILVERLIGHT
        [Test]
        public void Can_SpeculativeForEach() {
            var @functions = _functions;

            for(int i = 0; i < MaxTestCount; i++) {
                var result = ParallelTool.SpeculativeForEach(@functions, i);
                Assert.AreEqual(Math.Pow(i, PowerNumber), result);
            }
        }

        [Test]
        public void Can_SpeculativeInvoke() {
            var @functions = _functions;

            for(var i = 0; i < MaxTestCount; i++) {
                var result = ParallelTool.SpeculativeInvoke<int, double>(i, @functions);
                Assert.AreEqual(Math.Pow(i, PowerNumber), result);
            }
        }
#endif
    }
}