using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.InheritanceMappings.TablePerClassHierarchy {
    [Serializable]
    public abstract class PlanBase : DataEntityBase<Int32> {
        public virtual string Title { get; set; }
        public virtual string Content { get; set; }

        public virtual string ReporterId { get; set; }
        public virtual DateTime? ReportDate { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(GetType(), ReporterId, ReportDate);
        }
    }
}