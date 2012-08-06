namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.InheritanceMappings.TablePerClassHierarchy {
    public class TeamPlan : PlanBase {
        public virtual string TeamCode { get; set; }

        public virtual string SubContent { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(TeamCode, base.GetHashCode());
        }
    }
}