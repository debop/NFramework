using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Mappings {
    [Serializable]
    public class Role : DataEntityBase<int> {
        public virtual string Name { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual AnotherEntity Entity { get; set; }
        public virtual Role ParentRole { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Name);
        }
    }
}