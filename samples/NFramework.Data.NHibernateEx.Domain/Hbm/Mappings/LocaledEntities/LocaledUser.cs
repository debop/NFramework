using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Mappings {
    [Serializable]
    public class LocaledUser : LocaledMetadataEntityBase<Int32, LocaledUserLocale>, IUpdateTimestampedEntity {
        protected LocaledUser() {}

        public LocaledUser(LocaledCompany localedCompany, string code) {
            localedCompany.ShouldNotBeNull("company");
            code.ShouldNotBeWhiteSpace("code");

            Company = localedCompany;
            Code = code;
        }

        public virtual LocaledCompany Company { get; set; }

        public virtual string Code { get; protected set; }

        public virtual string Name { get; set; }

        /// <summary>
        /// 사용여부
        /// </summary>
        public virtual bool? IsActive { get; set; }

        /// <summary>
        /// 설명
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 확정 속성
        /// </summary>
        public virtual string ExAttr { get; set; }

        /// <summary>
        /// Json 직렬화로 저장할 정보
        /// </summary>
        public virtual object JsonData { get; set; }

        public virtual DateTime? UpdateTimestamp { get; set; }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Company, Code);
        }
    }

    public class LocaledUserLocale : DataObjectBase, ILocaleValue {
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string ExAttr { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(Name, Description, ExAttr);
        }
    }
}