using System;
using System.Linq.Expressions;

namespace NSoft.NFramework.LinqEx {
    public static partial class LinqTool {
        /// <summary>
        /// 지정된 Anonymous Method를 명확한 LambdaExpression으로 반환한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        /// <example>
        ///		<code>
        ///         // Addition function and expression
        ///			Func{int, int, int} func = (int a, int b) => a + b;
        ///         Expression{Func{int, int, int}} expr = (int a, int b) => a + b;
        ///    
        ///         // 다음 코드는 애미하다
        ///			var func = (int a, int b) => a + b;
        /// 
        ///			// 이를 명확하게 하기 위해 
        ///         var func = LinqTool.Func( (int a, int b) => a + b);	 // Function
        ///         var expr = LinqTool.Expr( (int a, int b) => a + b);  // Expression
        /// 
        /// 
        ///			// using anonymous types is possible
        ///			var func = LinqTool.Func( (int a, int b) => new { Sum = a+b, Mul = a*b } );
        ///			var func = LinqTool.Expr( (int a, int b) => new { Sum = a+b, Mul = a*b } );
        ///		</code>
        /// </example>
        public static Expression<Func<T, TResult>> Expr<T, TResult>(Expression<Func<T, TResult>> expr) {
            return expr;
        }

        /// <summary>
        /// 지정된 Anonymous function을 명확한 Func{T, TResult} delegate로 반환한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Func<T, TResult> Func<T, TResult>(Func<T, TResult> expr) {
            return expr;
        }

        /// <summary>
        /// <paramref name="expression"/>을 실행하여, Runtime 시의 값을 반환합니다.
        /// </summary>
        /// <param name="expression">실행할 Expression</param>
        /// <param name="args">실행 시 인자값</param>
        /// <returns>Expression을 동적으로 실행하여 얻은 값</returns>
        public static object Evaluate(this Expression expression, params object[] args) {
            var valueExpr = Expression.Lambda(expression).Compile();
            return valueExpr.DynamicInvoke(args);
        }

        /// <summary>
        /// <paramref name="expression"/>에서 속성명을 추출합니다.
        /// Retrieves the name of the property from a member expression
        /// NOTE: NHibernate.Impl.ExpressionProcessor 에 있는 것인데, NHibernate을 사용하지 않는 경우에도 사용하기 위해 업어옴
        /// </summary>
        /// <param name="expression">An expression tree that can contain either a member, or a conversion from a member.
        /// If the member is referenced from a null valued object, then the container is treated as an alias.</param>
        /// <returns>The name of the member property</returns>
        /// <example>
        /// <code>
        ///		Expression&lt;Func&lt;User,object&gt;&gt; expr = u=>u.Name;
        ///		var propertyName = ExpressionUtil.FindMemberName( expr.Body );
        ///     Assert.AreEqual("Name", propertyName);
        /// </code>
        /// </example>
        public static string FindMemberName(this Expression expression) {
            if(expression is MemberExpression) {
                var memberExpression = (MemberExpression)expression;

                if(memberExpression.Expression.NodeType == ExpressionType.MemberAccess ||
                   memberExpression.Expression.NodeType == ExpressionType.Call) {
                    if(IsNullableOfT(memberExpression.Member.DeclaringType)) {
                        // it's a Nullable<T>, so ignore any .Value
                        if(memberExpression.Member.Name == "Value")
                            return FindMemberName(memberExpression.Expression);
                    }

                    return FindMemberName(memberExpression.Expression) + "." + memberExpression.Member.Name;
                }

                return memberExpression.Member.Name;
            }

            if(expression is UnaryExpression) {
                var unaryExpression = (UnaryExpression)expression;

                if(unaryExpression.NodeType != ExpressionType.Convert)
                    throw new Exception("Cannot interpret member from " + expression.ToString());

                return FindMemberName(unaryExpression.Operand);
            }

            if(expression is MethodCallExpression) {
                var methodCallExpression = (MethodCallExpression)expression;

                if(methodCallExpression.Method.Name == "GetType")
                    return ClassMember(methodCallExpression.Object);

                if(methodCallExpression.Method.Name == "get_Item")
                    return FindMemberName(methodCallExpression.Object);

                if(methodCallExpression.Method.Name == "First")
                    return FindMemberName(methodCallExpression.Arguments[0]);

                throw new InvalidOperationException("Unrecognised method call in epression " + expression.ToString());
            }

            throw new InvalidOperationException("Could not determine member from " + expression.ToString());
        }

        /// <summary>
        /// <paramref name="expression"/>에서 속성명을 추출합니다. (최종적인 속성명만을 추출합니다. 예: user.Company.Code ==> "Code" )
        /// Retrieves the name of the property from a member expression (without leading member access)
        /// NOTE: NHibernate.Impl.ExpressionProcessor 에 있는 것인데, NHibernate을 사용하지 않는 경우에도 사용하기 위해 업어옴
        /// </summary>
        /// <example>
        /// <code>
        ///		Expression&lt;Func&lt;User,object&gt;&gt; expr = u=>u.Company.Name;
        ///		var propertyName = ExpressionUtil.FindPropertyName( expr.Body );
        ///     Assert.AreEqual("Name", propertyName);
        /// </code>
        /// </example>
        public static string FindPropertyName(this Expression expression) {
            string memberExpression = FindMemberName(expression);
            int periodPosition = memberExpression.LastIndexOf('.') + 1;

            return (periodPosition <= 0)
                       ? memberExpression
                       : memberExpression.Substring(periodPosition);
        }

        private static string ClassMember(Expression expression) {
            return
                expression.NodeType == ExpressionType.MemberAccess
                    ? FindMemberName(expression) + ".class"
                    : @"class";
        }

        /// <summary>
        /// 지정된 타입이 Nullable{T} 수형인지 파악합니다
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsNullableOfT(Type type) {
            return
                type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}