using System;
using NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes;
using NSoft.NFramework.TimePeriods;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// <see cref="AriaEncryptStringUserType"/> 을 사용하여 Password를 저장할 때 암호화하고, Entity를 만들 때 복호화한다.
    /// </summary>
    [Serializable]
    public class User : MetadataEntityBase<Int64>, IUpdateTimestampedEntity {
        protected User() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="company">회사</param>
        /// <param name="code">사용자 코드</param>
        /// <param name="name">사용자 명</param>
        public User(Company company, string code, string name = null) {
            company.ShouldNotBeNull("company");
            code.ShouldNotBeNull("code");

            Company = company;
            Code = code;
            Name = name ?? code;
        }

        public virtual string Code { get; set; }

        public virtual string Name { get; set; }

        /// <summary>
        /// Encrypted password
        /// </summary>
        public virtual string Password { get; set; }

        public virtual string Password2 { get; set; }

        /// <summary>
        /// Compressed string
        /// </summary>
        public virtual string Data { get; set; }

        public virtual byte[] Blob { get; set; }

        public virtual TimeRange ActivePeriod { get; set; }

        public virtual YearAndWeek ActiveYearWeek { get; set; }

        public virtual PeriodTime PeriodTime { get; set; }

        public virtual Company Company { get; set; }

        public virtual Type LanguageType { get; set; }

        public virtual DateTime? UpdateTimestamp { get; set; }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Company, Code);
        }

        public override string ToString() {
            return string.Format("User# Id=[{0}], Code=[{1}], Name=[{2}], Password=[{3}], Company=[{4}]", Id, Code, Name, Password,
                                 Company);
        }
    }
}