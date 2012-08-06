using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// NHibernate 용 Entity 의 Base class
    /// </summary>
    /// <typeparam name="TId">Entity의 Identity 값의 수형</typeparam>
    [Serializable]
    public abstract class DataEntityBase<TId> : StateEntityBase, IDataEntity<TId> {
        private int? _hashCode;
        private TId _id; // = default(TId);

        /// <summary>
        /// Identity Value of Entity
        /// </summary>
        public virtual TId Id {
            get { return _id; }
            protected set {
                if(Equals(_id, value) == false)
                    _id = value;
            }
        }

        /// <summary>
        /// Persistent Object를 Transient Object로 만든다. (Persistent Object를 복제해서 새로운 Transient Object를 만들 때 사용한다.)
        /// </summary>
        public override void ToTransient() {
            base.ToTransient();

            _hashCode = null;
            _id = default(TId);
        }

        /// <summary>
        /// 현재 인스턴스를 문자열로 나타냅니다.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return string.Format("{0}# Id=[{1}]", GetType().FullName, Id);
        }

        /// <summary>
        /// 해시 코드를 구합니다.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            // Saved 된 거라면 Identity값만 주면 되고, 
            // Transient라면 꼭 GetHashCode를 override해서 Business적으로 구별되는 속성값으로 HashCode를 만들어야 한다.
            // 여기서는 단순무식하게 모든 속성정보를 다 나열하고 HashCode값을 구하므로 느리다.
            //
            // return (IsTransient) ? ToString().GetHashCode() : _id.GetHashCode();
            // return (IsTransient) ? base.GetHashCode() : _id.GetHashCode();

            // HashCode 계산은 성능에 많은 영향을 미친다. 이렇게 Cache를 해 놓으면 좋다.
            //
            if(IsTransient) {
                _hashCode = base.GetHashCode();
            }
            else if(_hashCode.HasValue == false) {
                _hashCode = Id.GetHashCode();
            }

            return _hashCode.Value;
        }

        /// <summary>
        /// Entity가 지정된 객체와 같은 Entity인지 판단한다.
        /// </summary>
        public override bool Equals(object obj) {
            return (obj != null) && (obj is IDataEntity<TId>) && Equals(obj as IDataEntity<TId>);
        }

        /// <summary>
        /// Entity가 같은지 검사한다.
        /// </summary>
        /// <param name="other">대상 Enitity</param>
        /// <returns></returns>
        /// <remarks>
        /// 두 Business Entity가 같으려면
        /// 1. 둘다 Persistent object이며, Id 값이 같다. (DB에 저장되어 있고, Id값이 같다) <br/>
        /// 2. 둘다 Transient일 경우 Entity의 HashCode 값이 같다면
        /// </remarks>
        public virtual bool Equals(IDataEntity<TId> other) {
            if(ReferenceEquals(other, null))
                return false;

            return HasSameNonDefaultIdAs(other) || ((IsTransient || other.IsTransient) && HasSameBusinessSignature(other));
        }

        /// <summary>
        /// 현재 Entity와 비교하고자 하는 Entity 모두 Id값이 기본 형식 값이 아니라 새로운 값이고,  두 Entity의 Id값이 같은지 검사.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool HasSameNonDefaultIdAs(IDataEntity<TId> entity) {
            return (Equals(Id, null) == false && Id.Equals(default(TId)) == false) &&
                   (Equals(entity.Id, null) == false && entity.Id.Equals(default(TId)) == false) &&
                   Id.Equals(entity.Id);
        }

        /// <summary>
        /// Entity가 Transient라면 Identity가 둘다 default(IdT) 값을 가지므로, 
        /// 두 entity가 Business 적으로 같은지 파악한다. 이것은 GetHashCode()로 구분한다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        private bool HasSameBusinessSignature(IDataEntity<TId> other) {
            return (ReferenceEquals(other, null) == false) && GetHashCode().Equals(other.GetHashCode());
        }

        /// <summary>
        /// Equal operator
        /// </summary>
        public static bool operator ==(DataEntityBase<TId> x, DataEntityBase<TId> y) {
            return Equals(x, y);
        }

        /// <summary>
        /// Not equal operator
        /// </summary>
        public static bool operator !=(DataEntityBase<TId> x, DataEntityBase<TId> y) {
            return !Equals(x, y);
        }
    }
}