using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Tools {
    /// <summary>
    /// Enum 관련 Helper Class
    /// </summary>
    public static class EnumTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// 지정된 Enum 수형의 모든 Enum값을 반환한다.
        /// </summary>
        /// <typeparam name="T">Enum 수형</typeparam>
        /// <returns>Enum 수형의 모든 값</returns>
        [CLSCompliant(false)]
        public static IEnumerable<T> GetEnumValues<T>() where T : IConvertible, IComparable {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// 지정된 Enum 값의 Enum 수형의 모든 Enum 값을 반환한다.
        /// </summary>
        /// <param name="enumValue">Enum 값</param>
        /// <returns>모든 Enum 값</returns>
        public static IEnumerable<object> GetEnumValues(object enumValue) {
            enumValue.ShouldNotBeNull("enumValue");
            return Enum.GetValues(enumValue.GetType()).Cast<object>();
        }
    }
}