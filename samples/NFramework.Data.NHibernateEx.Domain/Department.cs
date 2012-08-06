using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    [Serializable]
    public class Department : TreeNodeEntityBase<Department, Int32>, ICodeEntity, INamedEntity {
        public Department() {}

        public Department(Company company, string code, string name = null) {
            Company = company;
            Code = code;
            Name = name ?? code;
        }

        public virtual Company Company { get; set; }

        public virtual string Code { get; set; }
        public virtual string Name { get; set; }

        private Iesi.Collections.Generic.ISet<DepartmentMember> _members;

        public virtual Iesi.Collections.Generic.ISet<DepartmentMember> Members {
            get { return _members ?? (_members = new Iesi.Collections.Generic.HashedSet<DepartmentMember>()); }
            protected set { _members = value; }
        }

        public virtual IList<User> Users {
            get { return Members.Select(m => m.User).ToList(); }
        }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Code);
        }
    }
}