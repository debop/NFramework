using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    [Serializable]
    public class Child : DataEntityBase<Guid> {
        // 테스트를 위해 
        public Child() {
            Id = Guid.NewGuid();
            Version = -1;
            Name = "Some Child";
        }

        public Child(string name, Parent parent) : this() {
            Name = name;
            Parent = parent;
        }

        public Child(Guid id, string name, Parent parent) : this(name, parent) {
            Id = id;
        }

        public virtual int Version { get; set; }
        public virtual string Name { get; set; }

        public virtual Parent Parent { get; set; }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Parent, Name);
        }
    }
}