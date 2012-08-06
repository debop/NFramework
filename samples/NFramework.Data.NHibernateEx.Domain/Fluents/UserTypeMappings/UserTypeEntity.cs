using System;
using NSoft.NFramework.TimePeriods;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.UserTypeMappings {
    [Serializable]
    public class UserTypeEntity : DataEntityBase<Int32> {
        public virtual string Name { get; set; }
        public virtual string Password { get; set; }
        public virtual string Password2 { get; set; }

        public virtual string CompressedString { get; set; }

        public virtual byte[] CompressedBlob { get; set; }

        private TimeRange _activePeriod;

        public virtual TimeRange ActivePeriod {
            get { return _activePeriod ?? (_activePeriod = new TimeRange()); }
            protected set { _activePeriod = value; }
        }

        public virtual YearAndWeek ActiveYearWeek { get; set; }

        public virtual Type LanguageType { get; set; }

        public virtual DateTime? UpdateTimestamp { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Name, Password);
        }
    }
}