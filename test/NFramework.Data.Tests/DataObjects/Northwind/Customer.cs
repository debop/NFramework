using System;

namespace NSoft.NFramework.Data.DataObjects.Northwind {
    [Serializable]
    public class Customer : DataObjectBase {
        public string CustomerID { get; set; }
        public string CompanyName { get; set; }

        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(CustomerID);
        }
    }
}