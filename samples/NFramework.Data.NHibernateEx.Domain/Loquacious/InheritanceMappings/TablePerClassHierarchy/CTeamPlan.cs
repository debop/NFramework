using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    [Serializable]
    public class CTeamPlan : CPlanBase {
        public virtual string TeamCode { get; set; }

        public virtual string SubContent { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(base.GetHashCode(), TeamCode);
        }
    }
}