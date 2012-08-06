using System;

namespace NSoft.NFramework.Data.NHibernateEx.UnitOfWorks.MultipleUnitOfWorkArtifacts {
    [Serializable]
    public class DomainObjectFromDatabase2 : DataObjectBase {
        public DomainObjectFromDatabase2() {}

        public DomainObjectFromDatabase2(string name) {
            Name = name;
        }

        public virtual Int32 Id { get; set; }

        public virtual String Name { get; set; }

        public override int GetHashCode() {
            return Name.CalcHash();
        }
    }
}