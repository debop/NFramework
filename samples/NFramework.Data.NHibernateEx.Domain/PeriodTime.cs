using System;
using NSoft.NFramework.TimePeriods;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    [Serializable]
    public class PeriodTime : DataObjectBase {
        public virtual TimeRange Period { get; set; }

        public virtual YearAndWeek YearWeek { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(Period, YearWeek);
        }

        public override string ToString() {
            return string.Format("PeriodTime# Period=[{0}], YearWeek=[{1}]", Period, YearWeek);
        }
    }
}