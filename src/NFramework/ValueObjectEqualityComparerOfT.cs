using System.Collections.Generic;

namespace NSoft.NFramework.ValueObjects {
    /// <summary>
    /// <see cref="IValueObject"/>의 인스턴스들이 같은지 비교합니다. 비교 방법은 HashCode 값이 같은지를 비교합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ValueObjectEqualityComparer<T> : IEqualityComparer<T> where T : IValueObject {
        public bool Equals(T x, T y) {
            if(ReferenceEquals(x, null) && ReferenceEquals(y, null))
                return true;

            if(ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return x.GetHashCode().Equals(y.GetHashCode());
        }

        public int GetHashCode(T obj) {
            return HashTool.Compute(obj);
        }
    }
}