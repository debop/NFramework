using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework {
    /// <summary>
    /// Hash 값을 계산하는 Tool 입니다.
    /// </summary>
    public static class HashTool {
        public const int NullValue = 0;
        public const int OneValue = 1;
        public const int Factor = 31;

        private static readonly int EmptyStringHashCode = string.Empty.GetHashCode();

        /// <summary>
        /// 지정된 객체들의 HashCode를 계산합니다.
        /// </summary>
        public static int Compute(object obj) {
            unchecked {
                return (obj != null) ? obj.GetHashCode() : NullValue;
            }
        }

        /// <summary>
        /// 지정된 객체들의 HashCode를 계산합니다.
        /// </summary>
        public static int Compute(object obj1, object obj2) {
            unchecked {
                return Compute(obj1) * Factor + Compute(obj2);
            }
        }

        /// <summary>
        /// 지정된 객체들의 HashCode를 계산합니다.
        /// </summary>
        public static int Compute(object obj1, object obj2, object obj3) {
            unchecked {
                return Compute(obj1, obj2) * Factor + Compute(obj3);
            }
        }

        /// <summary>
        /// 지정된 객체들의 HashCode를 계산합니다.
        /// </summary>
        public static int Compute(object obj1, object obj2, object obj3, object obj4) {
            unchecked {
                return Compute(obj1, obj2, obj3) * Factor + Compute(obj4);
            }
        }

        /// <summary>
        /// 지정된 객체들의 HashCode를 계산합니다.
        /// </summary>
        /// <param name="objs">객체를 입력하세요 (GetHashCode를 넣지 마세요)</param>
        /// <returns></returns>
        public static int Compute(params object[] objs) {
            unchecked {
                int hash = NullValue;

                if(objs != null)
                    foreach(var obj in objs)
                        hash = hash * Factor + Compute(obj);

                return hash;
            }
        }

        /// <summary>
        /// 지정된 객체들의 HashCode를 계산합니다.
        /// </summary>
        /// <param name="objs">객체를 입력하세요 (GetHashCode를 넣지 마세요)</param>
        /// <returns></returns>
        public static int Compute(IEnumerable objs) {
            unchecked {
                int hash = NullValue;

                if(objs != null)
                    foreach(var obj in objs)
                        hash = hash * Factor + Compute(obj);

                return hash;
            }
        }

        public static int Compute<T>(IEnumerable<T> sequence) {
            unchecked {
                int hash = NullValue;

                if(sequence != null)
                    foreach(T item in sequence)
                        hash = hash * Factor + Compute(item);

                return hash;
            }
        }

        /// <summary>
        /// 문자열의 HashCode를 반환합니다. null 이거나 빈문자열이면 string.Empty.GetHashCode() 값을 반환합니다. (0은 아닙니다)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int CalcHash(this string str) {
            return str.IsEmpty() ? EmptyStringHashCode : str.GetHashCode();
        }

        /// <summary>
        /// 인스턴스의 특정 속성들로부터 HashCode를 생성합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="target">해시코드를 얻고자하는 대상 객체</param>
        /// <param name="propertyExprs">해시코드 조합용 속성들</param>
        /// <returns></returns>
        public static int CalcHash<T>(this T target, params Expression<Func<T, object>>[] propertyExprs) {
            if(Equals(target, default(T)))
                return 0;

#if !SILVERLIGHT

            if(propertyExprs == null || propertyExprs.Length == 0)
                return target.GetHashCode();

            var propertyNames = propertyExprs.Select(expr => expr.Body.FindMemberName()).ToArray();
            var accessor = DynamicAccessorFactory.CreateDynamicAccessor<T>(MapPropertyOptions.Safety);

            return Compute(propertyNames.Select(name => accessor.GetPropertyValue(target, name)).ToArray());
#else
			return target.GetHashCode();
#endif
        }
    }
}