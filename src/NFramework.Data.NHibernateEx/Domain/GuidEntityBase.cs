using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// Identity 수형이 <see cref="Guid"/>인 엔티티입니다.
    /// </summary>
    [Serializable]
    public abstract class GuidEntityBase : DataEntityBase<Guid> {
        /// <summary>
        /// 기본 생성자
        /// </summary>
        protected GuidEntityBase() {
            Id = Guid.Empty;
        }
    }
}