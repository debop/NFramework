using System;
using System.Collections;
using System.Linq;
using NCalc;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Numerics.Expressions {
    /// <summary>
    /// NCalc 라이브러리에 대한 연습용 예제
    /// </summary>
    [TestFixture]
    public class NCalcFixture : AbstractNumericTestCase {
        [Test]
        public void PlusMinusTest() {
            var expr = new Expression("2 + 3 * 5");
            expr.Evaluate().Should().Be(17);

            expr = new Expression("48/(2*(9+3))");
            expr.Evaluate().Should().Be(2d);
        }

        [Test]
        public void ComplexExpressionTest() {
            var expr = new Expression(@"5 * Pow(2 * Sin(Pi/3) * Sin(Pi/4) - Atan(Pi/4), 2)");
            expr.Parameters["Pi"] = Math.PI;

            expr.Evaluate().AsDouble().Should().Be.GreaterThan(1.56);
        }

        [Test]
        public void DynamicParametersTest() {
            var expr = new Expression("Round(Pow(Pi, 2) + Pow(Pi, 2) + x, 2)");

            expr.Parameters["Pi2"] = new Expression("Pi * Pi");
            expr.Parameters["x"] = 10;

            expr.EvaluateParameter += delegate(string name, ParameterArgs args) {
                                          if(name == "Pi")
                                              args.Result = Math.PI;
                                      };

            Console.WriteLine("value = " + expr.Evaluate().AsDouble());
            expr.Evaluate().AsDouble().Should().Be.GreaterThan(0);
        }

        [Test]
        public void IterationTest() {
            var expr = new Expression("Sin(x)", EvaluateOptions.IgnoreCase | EvaluateOptions.IterateParameters);
            expr.Parameters["x"] = Enumerable.Range(0, 1000).Select(x => x / Math.PI).ToArray();

            foreach(var result in (IList)expr.Evaluate())
                Console.WriteLine(result.AsDouble());
        }

        [Test]
        public void MultiValuedParametersTest() {
            var expr = new Expression("Pow((a * b), c)", EvaluateOptions.IgnoreCase | EvaluateOptions.IterateParameters);

            expr.Parameters["a"] = new[] { 1, 2, 3, 4, 5 };
            expr.Parameters["b"] = new[] { 6, 7, 8, 9, 0 };
            expr.Parameters["c"] = 3;

            foreach(var result in (IList)expr.Evaluate()) {
                Console.WriteLine(result);
            }
        }
    }
}