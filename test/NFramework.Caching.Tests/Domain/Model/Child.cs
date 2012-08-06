using System;
using NSoft.NFramework.Data.NHibernateEx.Domain;

namespace NSoft.NFramework.Caching.Domain.Model {
    [Serializable]
    public class Child : DataEntityBase<Guid> {
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
        public virtual string Description { get; set; }
        public virtual Parent Parent { get; set; }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Parent, Name);
        }
    }
}