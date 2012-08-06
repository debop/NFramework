using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    [Serializable]
    public class Address : DataObjectBase {
        public virtual string AddressLine1 { get; set; }
        public virtual string AddressLine2 { get; set; }
        public virtual string City { get; set; }
        public virtual string Country { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(Country, City, AddressLine1, AddressLine2);
        }
    }
}