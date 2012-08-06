using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Northwind {
    [Serializable]
    public class AddressInfo : DataObjectBase {
        public string Address { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string PostalCode { get; set; }

        public CountryCode? Country { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(Address, City, PostalCode);
        }

        public override string ToString() {
            return string.Format("AddressInfo# Address=[{0}], City=[{1}], Region=[{2}], PostalCode=[{3}], Country=[{4}]",
                                 Address, City, Region, PostalCode, Country);
        }
    }
}