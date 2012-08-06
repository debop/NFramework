using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NSoft.NFramework {
    /// <summary>
    /// Enum 형식에 대한 Comparer입니다. Lambda Expression을 이용하여, 비교자를 구현했습니다.
    /// <see cref="Comparer{T}.Default"/>를 사용했을 때보다 속도가 10배 이상 빨라집니다.
    /// </summary>
    /// <remarks>
    /// 참고 : http://www.netgore.com/docs/a01975_source.html
    /// 원본 : http://www.codeproject.com/KB/cs/EnumComparer.aspx
    /// </remarks>
    /// <typeparam name="T">사용자 enum 형식</typeparam>
    [Serializable]
    public class EnumComparer<T> : IEqualityComparer<T> where T : struct {
        private static readonly ICollection<Type> SupportedUnderlyingTypes =
            new[] { typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong) };

        private static readonly EnumComparer<T> _instance = new EnumComparer<T>();

        private readonly Func<T, T, bool> _equalsFunc;
        private readonly Func<T, int> _getHashCodeFunc;

        private static Func<T, T, bool> GenerateEqualsFunc() {
            var exprX = Expression.Parameter(typeof(T), "x");
            var exprY = Expression.Parameter(typeof(T), "y");

            var equalExpr = Expression.Equal(exprX, exprY);
            return Expression.Lambda<Func<T, T, bool>>(equalExpr, new[] { exprX, exprY }).Compile();
        }

        private static Func<T, int> GenerateGetHashCodeFunc() {
            var instanceParam = Expression.Parameter(typeof(T), "instance");
            var underlyingType = Enum.GetUnderlyingType(typeof(T));
            var convertExpr = Expression.Convert(instanceParam, underlyingType);

            var getHashCodeMethod = underlyingType.GetMethod("GetHashCode");
            var getHashCodeExpr = Expression.Call(convertExpr, getHashCodeMethod);

            return Expression.Lambda<Func<T, int>>(getHashCodeExpr, new[] { instanceParam }).Compile();
        }

        /// <summary>
        /// 정적 생성자
        /// </summary>
        static EnumComparer() {
            Guard.Assert(typeof(T).IsEnum, @"The type parameter [{0}] is not Enum type.", typeof(T));

            // underlying type이 지원되는 수형인지 파악한다.
            var underlyingType = Enum.GetUnderlyingType(typeof(T));
            Guard.Assert(SupportedUnderlyingTypes.Contains(underlyingType),
                         "Enum [{0}] contains an unsupported underlying Enum type of [{1}]",
                         typeof(T), underlyingType);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        protected EnumComparer() {
            _equalsFunc = GenerateEqualsFunc();
            _getHashCodeFunc = GenerateGetHashCodeFunc();
        }

        /// <summary>
        /// Instance of EnumComparer{T}
        /// </summary>
        public static EnumComparer<T> Instance {
            get { return _instance; }
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <param name="x">The first object of type {T} to compare.</param>
        /// <param name="y">The second object of type {T} to compare.</param>
        public bool Equals(T x, T y) {
            return _equalsFunc(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        /// <param name="instance">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The type of <paramref name="instance"/> is a reference type and <paramref name="instance"/> is null.
        /// </exception>
        public int GetHashCode(T instance) {
            return _getHashCodeFunc(instance);
        }
    }
}