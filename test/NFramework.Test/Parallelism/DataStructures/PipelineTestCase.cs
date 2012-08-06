using System;
using System.Linq;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.DataStructures {
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class PipelineTestCase : ParallelismFixtureBase {
        private readonly Func<int, double> @func1 = x => Math.Pow(x, 2) - Math.Sqrt(x) - Math.Log(x);
        private readonly Func<double, double> @func2 = x => x / 2 + Math.Log(x) + Math.Sqrt(x);
        private readonly Func<double, double> @func3 = x => x * x / 2.0 + Math.Log(x) - Math.Sqrt(x);
        private readonly Func<double, bool> @func4 = x => ((int)x % 2 == 0 && x > 100);

        [Test]
        public void PipelineTest() {
            var inputs = Enumerable.Range(1, 9999);

            // 일반적으로 degreeOfParallelism이 커질수록 성능이 좋아집니다.
            // 만약 Pipeline step이 아주 많다면, 수행 성능은 degreeOfParallelism에 더 민감해질 것입니다.
            //
            for(var i = 1; i <= Environment.ProcessorCount; i++) {
                var degreeOfParallelism = i;

                using(new OperationTimer("DegreeOfParallelism=" + degreeOfParallelism, true)) {
                    var pipeline =
                        Pipeline
                            .Create(@func1, degreeOfParallelism)
                            .Next(@func2, degreeOfParallelism)
                            .Next(@func3, degreeOfParallelism)
                            .Next(@func2, degreeOfParallelism)
                            .Next(@func3, degreeOfParallelism)
                            .Next(@func4, degreeOfParallelism);

                    var results = pipeline.Process(inputs).ToArray();
                }
            }
        }
    }
}