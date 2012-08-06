using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious.SimpleMappings {
    [Serializable]
    public class CPicture : DataEntityBase<Int32> {
        protected CPicture() {}

        public CPicture(CEmployee emp) {
            emp.ShouldNotBeNull("emp");
            Employee = emp;
            emp.Picture = this;
        }

        public virtual CEmployee Employee { get; set; }

        public virtual string Sign { get; set; }
        public virtual string Stamp { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Employee);
        }
    }
}