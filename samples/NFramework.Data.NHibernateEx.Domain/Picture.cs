using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    [Serializable]
    public class Picture : DataEntityBase<Int32> {
        protected Picture() {}

        public Picture(NHEmployee emp) {
            emp.ShouldNotBeNull("emp");
            NhEmployee = emp;
            emp.Picture = this;
        }

        public virtual NHEmployee NhEmployee { get; set; }

        public virtual string Sign { get; set; }

        public virtual string Stamp { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(NhEmployee);
        }
    }
}