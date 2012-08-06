using System;
using System.Collections.Generic;
using NSoft.NFramework.LinqEx;

namespace NSoft.NFramework.Numerics.Utils {
    public abstract class AbstractMathToolFixtureBase {
        #region << logger >>

        protected static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        protected static readonly bool IsTraceEnabled = log.IsTraceEnabled;
        protected static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static IEnumerable<T> GetSerialList<T>(T seed, T step, int count) where T : IComparable<T> {
            return LinqTool.Generate<T>(seed, count, step);
        }
    }
}