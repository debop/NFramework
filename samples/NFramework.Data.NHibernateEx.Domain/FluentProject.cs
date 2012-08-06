using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    [Serializable]
    public class FluentProject : LocaledEntityBase<long, FluentProjectLocale> {
        public FluentProject() {
            Field = new FluentProjectField(this);
        }

        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public virtual FluentProjectField Field { get; set; }

        public virtual DateTime? UpdateTimestamp { get; set; }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Code);
        }
    }

    [Serializable]
    public class FluentProjectLocale : DataObjectBase, ILocaleValue {
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(Name, Description);
        }
    }
}