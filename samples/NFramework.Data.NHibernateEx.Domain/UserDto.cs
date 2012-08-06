using System;
using NSoft.NFramework.TimePeriods;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    [Serializable]
    public class UserDto : IEquatable<UserDto> {
        public virtual Int64 Id { get; set; }
        public virtual string Name { get; set; }

        /// <summary>
        /// Encrypted password
        /// </summary>
        public virtual string Password { get; set; }

        /// <summary>
        /// Compressed string
        /// </summary>
        public virtual string Data { get; set; }

        public virtual byte[] Blob { get; set; }

        public virtual TimeRange ActivePeriod { get; set; }

        public virtual YearAndWeek WeekOfYearInfo { get; set; }

        public virtual PeriodTime PeriodTime { get; set; }

        public virtual string CompanyCode { get; set; }

        public virtual Type LanguageType { get; set; }

        public virtual DateTime? UpdateTimestamp { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(CompanyCode, Name);
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is UserDto) && Equals(obj as UserDto);
        }

        public bool Equals(UserDto other) {
            return (other != null) && (GetHashCode().Equals(other.GetHashCode()));
        }

        public override string ToString() {
            return string.Format("UserDto# Id=[{0}], Name=[{1}], Password=[{2}], CompanyCode=[{3}]", Id, Name, Password, CompanyCode);
        }
    }
}