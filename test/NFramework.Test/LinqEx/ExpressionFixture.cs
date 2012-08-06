using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace NSoft.NFramework.LinqEx.Queries {
    [TestFixture]
    public class ExpressionFixture {
        private Expression<Func<int, int>> calc;
        private Expression<Func<int, int>> expr;
        private Func<int, int> func;

        [TestFixtureSetUp]
        public void ClassSetUp() {
            calc = (i => i * 10);
            expr = LinqTool.Expr<int, int>(i => calc.Invoke(i) + 4);
            func = LinqTool.Func<int, int>(i => calc.Invoke(i) + 2);
        }

        [Test]
        public void DynamicInvoke() {
            Console.WriteLine("Call expr.Invoke(4): {0}", expr.Invoke(4));

            Assert.AreEqual(44, expr.Invoke(4)); // Expression을 수행한다.
            Assert.AreEqual(44, expr.Compile().Invoke(4)); // Delegate를 수행한다.

            Console.WriteLine("Call func(4): {0}", func(4));
            Assert.AreEqual(42, func(4));
        }

        /// <summary>
        /// <see cref="ExpressionTool"/>
        /// </summary>
        [Test]
        public void ExpressionExpand() {
            Expression<Func<int, int>> criteria1 = (i => i * 10);

            // i => critical.Compile().Invoke(i) + 4; 수행시에는 Expand()가 덜 효과적이다. 즉 순수 Expression으로 줘야 Expand를 제대로 한다.
            Expression<Func<int, int>> criteria2 = (i => criteria1.Invoke(i) + 4);

            Console.WriteLine("Originail Expression: " + criteria2);

            var expanded = criteria2.Expand();

            Console.WriteLine("Expanded Expression: " + expanded);

            Assert.AreEqual(40, criteria1.Invoke(4));
            Assert.AreEqual(44, criteria2.Invoke(4));
            Assert.AreEqual(44, expanded.Invoke(4));
        }

        [Test]
        public void ExpressionExpand2() {
            // expand the expression
            // Expand() is extension method from ExpressionTool class

            Expression<Func<int, int>> calc = (i => i * 10);
            Expression<Func<int, int>> expr = (i => calc.Invoke(i) + 4);
            var expanded = expr.Expand();

            // Original : i => Add(i => Multiply(i, 10).Invoke(i), 4)
            // Expanded : i => Add(Multiply(i, 10), 4)
            Console.WriteLine("Original Expression: " + expr);
            Console.WriteLine("Expanded Expression: " + expanded);

            Assert.AreEqual(44, expr.Invoke(4));
            Assert.AreEqual(44, expr.Compile().Invoke(4));
            Assert.AreEqual(44, expanded.Invoke(4));
            Assert.AreEqual(44, expanded.Compile().Invoke(4));
        }
    }
}