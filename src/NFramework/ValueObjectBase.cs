using System;

namespace NSoft.NFramework {
    /// <summary>
    /// DDD의 Value Object를 표현하는 추상 클래스입니다.
    /// </summary>
    [Serializable]
    public abstract class ValueObjectBase : IValueObject, IEquatable<ValueObjectBase> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public virtual bool Equals(ValueObjectBase other) {
            return Equals((IValueObject)other);
        }

        public virtual bool Equals(IValueObject other) {
            return (other != null) &&
                   (GetType() == other.GetType()) &&
                   GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is IValueObject) && Equals((IValueObject)obj);
        }

        public override int GetHashCode() {
            return HashTool.NullValue;
        }
    }
}