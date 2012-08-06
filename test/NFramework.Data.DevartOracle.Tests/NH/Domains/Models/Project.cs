using System;
using NSoft.NFramework.Data.NHibernateEx.Domain;

namespace NSoft.NFramework.Data.DevartOracle.NH.Domains.Models {
    [Serializable]
    public class Project : LocaledEntityBase<long, ProjectLocale> {
        public Project() {
            Field = new ProjectField(this);
        }

        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public virtual ProjectField Field { get; set; }

        public virtual DateTime? UpdateTimestamp { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Code);
        }
    }

    [Serializable]
    public class ProjectLocale : DataObjectBase, ILocaleValue {
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(Name, Description);
        }
    }
}