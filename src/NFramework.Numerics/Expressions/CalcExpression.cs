using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NCalc;

namespace NSoft.NFramework.Numerics.Expressions {
    /// <summary>
    /// NCalc 라이브러리를 이용하여 계산식에 대한 수행을 합니다.
    /// </summary>
    public static class CalcExpression {
        public static IEnumerable<double> UnaryEvaluate(this string expression, string parameterName, Func<double[]> parameterFunc) {
            expression.ShouldNotBeWhiteSpace("expression");
            var expr = new NCalc.Expression(expression, EvaluateOptions.IgnoreCase | EvaluateOptions.IterateParameters);
            expr.Parameters["x"] = parameterFunc();

            return ((IList)expr.Evaluate()).Cast<object>().Select(result => result.AsDouble());
        }
    }
}