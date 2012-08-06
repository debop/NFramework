using System;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious.SimpleMappings {
    [Serializable]
    public class CEmployee : DataEntityBase<Int32> {
        public virtual string LastName { get; set; }
        public virtual string FirstName { get; set; }

        public virtual CStore Store { get; set; }

        public virtual CPicture Picture { get; set; }

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