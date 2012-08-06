using System;
using System.Linq.Expressions;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework {
    /// <summary>
    /// Guard 패턴을 이용하여, 조건에 맞는 상황인지를 검사합니다. 
    /// </summary>
    public static partial class Guard {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static void Assert(bool condition) {
            if(condition)
                return;

            throw new InvalidOperationException();
        }

        public static void Assert(bool condition, string fmt, params object[] args) {
            if(condition)
                return;

            throw new InvalidOperationException(string.Format(fmt, args));
        }

        public static void Assert<TException>(bool condition) where TException : Exception {
            if(condition)
                return;

            throw ActivatorTool.CreateInstance<TException>();
        }

        public static void Assert<TException>(bool condition, string fmt, params object[] args) where TException : Exception {
            if(condition)
                return;

            throw ActivatorTool.CreateInstance<TException>(new[] { string.Format(fmt, args) });
        }

        public static void Assert(this Expression<Func<bool>> expression) {
            Assert(expression, "[{0}] 식이 false를 반환했습니다.", expression.Body);
        }

        public static void Assert(this Expression<Func<bool>> expression, string fmt, params object[] args) {
            expression.ShouldNotBeNull("expression");

            if(expression.Compile().Invoke())
                return;

            throw new InvalidOperationException(string.Format(fmt, args));
        }

        public static void Assert<TException>(this Expression<Func<bool>> expression) where TException : Exception {
            Assert(expression, "[{0}] 식이 false를 반환했습니다.", expression.Body);
        }

        public static void Assert<TException>(this Expression<Func<bool>> expression, string fmt, params object[] args)
            where TException : Exception {
            expression.ShouldNotBeNull("expression");

            if(expression.Compile().Invoke())
                return;

            throw ActivatorTool.CreateInstance<TException>(new[] { string.Format(fmt, args) });
        }
    }
}