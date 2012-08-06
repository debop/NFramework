using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Northwind {
    [Serializable]
    public class Employee : DataEntityBase<int> {
        public Employee() {
            Territories = new List<Territory>();
        }

        public Employee(string firstName, string lastName) {
            FullName = new Name(firstName, lastName);
        }

        private IList<Employee> _staffMembers;

        public virtual IList<Employee> StaffMembers {
            get { return _staffMembers ?? (_staffMembers = new List<Employee>()); }
            protected set { _staffMembers = value; }
        }

        private IList<Order> _orders;

        public virtual IList<Order> Orders {
            get { return _orders ?? (_orders ?? new List<Order>()); }
            protected set { _orders = value; }
        }

        private Name _fullName;

        public virtual Name FullName {
            get { return _fullName ?? (_fullName = new Name()); }
            set { _fullName = value; }
        }

        public virtual string Title { get; set; }
        public virtual string TitleOfCourtesy { get; set; }

        [SoapElement(IsNullable = true)]
        public virtual DateTime? BirthDate { get; set; }

        [SoapElement(IsNullable = true)]
        public virtual DateTime? HireDate { get; set; }

        private AddressInfo _addressInfo;

        public virtual AddressInfo AddressInfo {
            get { return _addressInfo ?? (_addressInfo = new AddressInfo()); }
            set { _addressInfo = value; }
        }

        public virtual string HomePhone { get; set; }
        public virtual string Extension { get; set; }
        public virtual byte[] Photo { get; set; }
        public virtual string PhotoPath { get; set; }
        public virtual string Notes { get; set; }

        public virtual Employee ReportsTo { get; set; }

        private IList<Territory> _territories;

        public virtual IList<Territory> Territories {
            get { return _territories ?? (_territories = new List<Territory>()); }
            protected set { _territories = value; }
        }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(FullName, Title, HireDate);
        }

        public override string ToString() {
            return this.ObjectToString();
        }
    }
}