using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;

namespace NSoft.NFramework.LinqEx.Dynamic {
    [TestFixture]
    public class DynamicFixture {
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 4)]
        public void ParseLambda_SimpleArithmetic(int x1, int y1) {
            const string exprString = "(x + y) * 2";

            var x = Expression.Parameter(typeof(int), "x");
            var y = Expression.Parameter(typeof(int), "y");

            var expr = DynamicExpression.ParseLambda(new[] { x, y }, null, exprString);
            var exprCompiled = expr.Compile();

            var dynamic = exprCompiled.DynamicInvoke(x1, y1);
            Console.WriteLine("({0}+{1})*2 = {2}", x1, y1, dynamic);
            Assert.AreEqual((x1 + y1) * 2, dynamic);
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 4)]
        public void ParseLambda_WithMath(int x1, int y1) {
            const string exprString = "Math.Pow(x, y)";

            var x = Expression.Parameter(typeof(int), "x");
            var y = Expression.Parameter(typeof(int), "y");

            var expr = DynamicExpression.ParseLambda(new[] { x, y }, null, exprString);
            var exprCompiled = expr.Compile();

            var dynamic = exprCompiled.DynamicInvoke(x1, y1);
            Console.WriteLine("(Math.Pow({0},{1}) = {2}", x1, y1, dynamic);
            Assert.AreEqual(Math.Pow(x1, y1), dynamic);
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 4)]
        public void ParseWithSymbols(int x1, int y1) {
            const string exprString = "iif(x > y, x, y)";

            var x = Expression.Parameter(typeof(int), "x");
            var y = Expression.Parameter(typeof(int), "y");

            var symbols = new Dictionary<string, object>
                          {
                              { "x", x },
                              { "y", y }
                          };

            var body = DynamicExpression.Parse(null, exprString, symbols);
            var expr = Expression.Lambda(body, new[] { x, y });

            var dynamic = expr.Compile().DynamicInvoke(x1, y1);

            Console.WriteLine("iif(x > y, x, y) = {2}, (x={0}, y={1})", x1, y1, dynamic);
            Assert.AreEqual((x1 > y1) ? x1 : y1, dynamic);
        }
    }
}