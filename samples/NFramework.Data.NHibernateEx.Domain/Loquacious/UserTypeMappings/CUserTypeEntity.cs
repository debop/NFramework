using System;
using NHibernate.UserTypes;
using NSoft.NFramework.TimePeriods;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    /// <summary>
    /// NHibernate <see cref="IUserType"/> 및 <see cref="ICompositeUserType"/> 을 가지는 Entity를 표현합니다.
    /// </summary>
    [Serializable]
    public class CUserTypeEntity : DataEntityBase<Int32> {
        public string Name { get; set; }

        public string Password { get; set; }
        public string Password2 { get; set; }

        public string CompressedString { get; set; }

        public byte[] CompressedBlob { get; set; }

        private TimeRange _activePeriod;

        public TimeRange ActivePeriod {
            get { return _activePeriod ?? (_activePeriod = new TimeRange()); }
            set { _activePeriod = value; }
        }

        public YearAndWeek ActiveYearWeek { get; set; }

        public Type LanguageType { get; set; }

        public DateTime? UpdateTimestamp { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Name, Password);
        }
    }
}