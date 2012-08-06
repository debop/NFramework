using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// 회사
    /// </summary>
    [Serializable]
    public class Company : LocaledMetadataEntityBase<Int32, Company.FCompanyLocale>, ICodeEntity, INamedEntity, IUpdateTimestampedEntity {
        public Company() {}

        public Company(string code, string name = null) {
            Code = code;
            Name = name ?? code;
        }

        public virtual string Code { get; set; }
        public virtual string Name { get; set; }

        public virtual bool? IsActive { get; set; }
        public virtual bool? IsDefault { get; set; }

        public virtual string Description { get; set; }
        public virtual string ExAttr { get; set; }

        private Iesi.Collections.Generic.ISet<User> _users;

        public virtual Iesi.Collections.Generic.ISet<User> Users {
            get { return _users ?? (_users = new Iesi.Collections.Generic.HashedSet<User>()); }
            protected set { _users = value; }
        }

        public virtual DateTime? UpdateTimestamp { get; set; }

        public virtual void AddUser(User user) {
            user.ShouldNotBeNull("user");

            Users.Add(user);
            user.Company = this;
        }


        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Code);
        }

        [Serializable]
        public class FCompanyLocale : DataObjectBase, ILocaleValue {
            public virtual string Name { get; set; }

            public virtual string Description { get; set; }

            public virtual string ExAttr { get; set; }

            public override int GetHashCode() {
                return HashTool.Compute(Name, Description, ExAttr);
            }
        }
    }
}