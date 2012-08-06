using System;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    [Serializable]
    public class NHEmployee : DataEntityBase<Int32> {
        public virtual string LastName { get; set; }
        public virtual string FirstName { get; set; }

        public virtual Store Store { get; set; }

        public virtual Picture Picture { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(LastName, FirstName);
        }

        public override string ToString() {
            return this.ObjectToString();
        }
    }
}