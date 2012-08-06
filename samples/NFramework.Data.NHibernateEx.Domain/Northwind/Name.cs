using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Northwind {
    [Serializable]
    public class Name : DataObjectBase {
        public static readonly Name Empty = new Name();

        public Name() {}

        public Name(string firstName, string lastName) {
            FirstName = firstName;
            LastName = lastName;
        }

        public virtual string FirstName { get; set; }
        public virtual string MiddleName { get; set; }
        public virtual string LastName { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(FirstName, MiddleName, LastName);
        }

        public override string ToString() {
            return string.Format("Name# FirstName={0}, MiddleName={1}, LastName={2}", FirstName, MiddleName, LastName);
        }
    }
}