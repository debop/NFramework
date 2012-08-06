using System;
using System.Collections.Generic;
using NSoft.NFramework.Data.NHibernateEx.Domain;

namespace NSoft.NFramework.Data.MongoDB.NHCaches.Domains.Models {
    public interface IParent : IDataEntity<Guid> {
        // Guid Id { get; }
        int Version { get; set; }
        int Age { get; set; }
        string Name { get; set; }
        string Description { get; set; }

        IList<Child> Children { get; }
    }

    [Serializable]
    public class Parent : DataEntityBase<Guid>, IParent {
        public Parent() {
            // AssignedIdEntityBase 를 상속 받는 놈은 Id 값 Setting을 항상 SetIdentity로 할 것
            // SetIdentity(Guid.NewGuid());
            Id = Guid.NewGuid();

            Version = -1;
        }

        public virtual int Version { get; set; }
        public virtual int Age { get; set; }
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        private IList<Child> _children;

        public virtual IList<Child> Children {
            get { return _children ?? (_children = new List<Child>()); }
            protected set { _children = value; }
        }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Id, Version);
        }
    }
}