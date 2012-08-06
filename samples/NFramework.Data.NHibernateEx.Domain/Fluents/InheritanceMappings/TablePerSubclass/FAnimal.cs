using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.InheritanceMappings.TablePerSubclass {
    [Serializable]
    public class FAnimal : DataEntityBase<Int32> {
        public virtual string Name { get; set; }

        public virtual int? Age { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(GetType().FullName, Name, Age);
        }
    }
}