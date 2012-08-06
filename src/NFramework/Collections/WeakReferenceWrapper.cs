using System;

namespace NSoft.NFramework.Collections {
    /// <summary>
    /// 객체에 대해 WeakReference를 표현합니다.
    /// </summary>
    [Serializable]
    public class WeakReferenceWrapper {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static WeakReferenceWrapper Wrap(object target) {
            return new WeakReferenceWrapper(target);
        }

        public static object Unwrap(object wrapped) {
            if(wrapped != null && wrapped is WeakReferenceWrapper)
                return ((WeakReferenceWrapper)wrapped).Target;

            return null;
        }

        private readonly WeakReference _reference;
        private readonly int _hashCode;

        public WeakReferenceWrapper(object target) {
            target.ShouldNotBeNull("target");

            _reference = new WeakReference(target);
            _hashCode = target.GetHashCode();
        }

        /// <summary>
        /// 원본 객체가 아직 살아있는지 (GC에 의해 청소되지 않았으면 True)
        /// </summary>
        public bool IsAlive {
            get { return _reference.IsAlive; }
        }

        /// <summary>
        /// GC에 의해 청소가 될 원본 객체
        /// </summary>
        public object Target {
            get { return _reference.Target; }
        }

        public override bool Equals(object obj) {
            if(obj == null)
                return false;

            if(this == obj)
                return true;

            var other = obj as WeakReferenceWrapper;
            if(other == null)
                return false;

            var target = Target;
            if(target == null) {
                // reference가 이미 소멸되었다. 결국 비교할 값이 없으므로 false를 반환한다.
                return false;
            }
            return (_hashCode == other._hashCode) && Equals(target, other.Target);
        }

        public override int GetHashCode() {
            return _hashCode;
        }
    }
}