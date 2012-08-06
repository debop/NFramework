using System;
using System.Collections.Generic;
using Castle.Components.Validator;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Mappings {
    /// <summary>
    /// 회사 정보
    /// </summary>
    [Serializable]
    public class LocaledCompany : LocaledMetadataEntityBase<Int32, LocaledCompanyLocale>, IUpdateTimestampedEntity {
        protected LocaledCompany() {
            UpdateTimestamp = DateTime.Now;
        }

        public LocaledCompany(string code)
            : this() {
            code.ShouldNotBeWhiteSpace("code");

            Code = code;
            Name = code;

            IsActive = true;
        }

        /// <summary>
        /// 코드
        /// </summary>
        [ValidateNonEmpty]
        public virtual string Code { get; protected set; }

        /// <summary>
        /// 회사명
        /// </summary>
        [ValidateNonEmpty]
        public virtual string Name { get; set; }

        /// <summary>
        /// 설명
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 사용여부
        /// </summary>
        public virtual bool? IsActive { get; set; }

        /// <summary>
        /// 확정 속성
        /// </summary>
        public virtual string ExAttr { get; set; }

        /// <summary>
        /// 최종갱신일
        /// </summary>
        public virtual DateTime? UpdateTimestamp { get; set; }

        private IList<User> _users;

        /// <summary>
        /// 소속 직원들
        /// </summary>
        public virtual IList<User> Users {
            get { return _users ?? (_users = new List<User>()); }
            protected set { _users = value; }
        }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Code);
        }

        public override string ToString() {
            return string.Format("Company# Id=[{0}],Code=[{1}], Name=[{2}], IsActive=[{3}]", Id, Code, Name, IsActive);
        }
    }

    /// <summary>
    /// 회사 지역화 정보
    /// </summary>
    public class LocaledCompanyLocale : DataObjectBase, ILocaleValue {
        /// <summary>
        /// 회사명
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 설명
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 확정 속성
        /// </summary>
        public virtual string ExAttr { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(Name, Description, ExAttr);
        }
    }
}