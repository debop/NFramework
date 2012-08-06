using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public interface IParent : IDataEntity<Guid> {
        // Guid Id { get; }
        int Version { get; set; }
        int Age { get; set; }
        string Name { get; set; }

        IList<Child> Children { get; }
    }

    [Serializable]
    public class Parent : DataEntityBase<Guid>, IParent {
        protected Parent() {}

        public Parent(Guid id) {
            Id = id;
            Version = -1;
        }

        public virtual int Version { get; set; }
        public virtual int Age { get; set; }
        public virtual string Name { get; set; }

        private IList<Child> _children;

        public virtual IList<Child> Children {
            get { return _children ?? (_children = new List<Child>()); }
        }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Name, Age);
        }

        public override string ToString() {
            return string.Format("Parent# Name=[{0}], Age=[{1}], Version=[{2}]", Name, Age, Version);
        }
    }
}