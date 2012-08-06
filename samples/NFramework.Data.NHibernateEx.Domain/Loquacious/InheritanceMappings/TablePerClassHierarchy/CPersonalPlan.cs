using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    [Serializable]
    public class CPersonalPlan : CPlanBase {
        public virtual string UserCode { get; set; }

        public virtual TimeSpan? ResourceTime { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(base.GetHashCode(), UserCode);
        }
    }
}