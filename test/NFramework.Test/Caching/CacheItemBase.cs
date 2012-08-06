using System;

namespace NSoft.NFramework.Caching {
    /// <summary>
    /// 캐시에 저장될 요소의 기본적인 클래스 정보
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    [Serializable]
    public abstract class CacheItemBase<TId> : IEquatable<CacheItemBase<TId>> where TId : struct {
        protected CacheItemBase(TId id) {
            Id = id;
            CreateDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Item의 Identity
        /// </summary>
        public virtual TId Id { get; set; }

        /// <summary>
        /// 요소가 생성된 시각
        /// </summary>
        public DateTime? CreateDate { get; set; }

        public bool Equals(CacheItemBase<TId> other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is CacheItemBase<TId>) && Equals((CacheItemBase<TId>)obj);
        }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }

        public override string ToString() {
            return string.Format("{0}# Id=[{1}]", GetType().Name, Id);
        }
    }
}