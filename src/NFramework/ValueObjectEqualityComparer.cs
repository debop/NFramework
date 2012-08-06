using System;
using System.Collections;
using NSoft.NFramework.TimePeriods;

namespace NSoft.NFramework {
    /// <summary>
    /// <see cref="IValueObject"/>의 인스턴스들이 같은지 비교합니다. 비교 방법은 HashCode 값이 같은지를 비교합니다.<br/>
    /// FluentNHibernate의 PersistenceSpecification에서 속성 값 비교를 위해 사용됩니다.<br/>
    /// 참고 : http://wiki.fluentnhibernate.org/Persistence_specification_testing#Testing_references 
    /// </summary>
    public class ValueObjectEqualityComparer : IEqualityComparer {
        public new bool Equals(object x, object y) {
            if(x == null && y == null)
                return true;

            if(x == null || y == null)
                return false;

            if(x is DateTime && y is DateTime)
                return ((DateTime)x).TrimToMillisecond().Equals(((DateTime)y).TrimToMillisecond());

            return x.GetHashCode().Equals(y.GetHashCode());
        }

        public int GetHashCode(object obj) {
            return HashTool.Compute(obj);
        }
    }
}