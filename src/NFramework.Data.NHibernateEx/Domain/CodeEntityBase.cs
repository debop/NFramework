using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// Code를 기본 속성으로 가지는 엔티티입니다.
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    [Serializable]
    public abstract class CodeEntityBase<TId> : DataEntityBase<TId>, ICodeEntity {
        protected CodeEntityBase() {}

        protected CodeEntityBase(string code) {
            code.ShouldNotBeNull("code");
            Code = code;
        }

        /// <summary>
        /// 엔티티의 Business Identity를 위한 Code 정보 (예: UserCode, OrderCode 등)
        /// </summary>
        public virtual string Code { get; set; }

        /// <summary>
        /// 현재 개체가 동일한 형식의 다른 개체와 같은지 여부를 나타냅니다.
        /// </summary>
        /// <returns>
        /// 현재 개체가 <paramref name="other"/> 매개 변수와 같으면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        /// <param name="other">이 개체와 비교할 개체입니다.</param>
        public bool Equals(ICodeEntity other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Code);
        }
    }
}