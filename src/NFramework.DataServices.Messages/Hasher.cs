using System.Collections;

namespace NSoft.NFramework.DataServices.Messages {
    /// <summary>
    /// Hash 값을 계산하는 Tool 입니다.
    /// </summary>
    internal static class Hasher {
        public const int NullValue = 0;
        public const int OneValue = 1;
        public const int HashValue = 31;

        public static int Compute(object obj) {
            return (obj == null) ? NullValue : obj.GetHashCode();
        }

        public static int Compute(object obj1, object obj2) {
            unchecked {
                return Compute(obj1) * HashValue + Compute(obj2);
            }
        }

        public static int Compute(object obj1, object obj2, object obj3) {
            unchecked {
                return Compute(obj1, obj2) * HashValue + Compute(obj3);
            }
        }

        public static int Compute(IEnumerable objs) {
            var hash = NullValue;

            unchecked {
                foreach(var obj in objs) {
                    hash += hash * HashValue + Compute(obj);
                }
            }

            return hash;
        }

        public static int Compute(params object[] objs) {
            var hash = NullValue;

            unchecked {
                foreach(var obj in objs) {
                    hash += hash * HashValue + Compute(obj);
                }
            }

            return hash;
        }
    }
}