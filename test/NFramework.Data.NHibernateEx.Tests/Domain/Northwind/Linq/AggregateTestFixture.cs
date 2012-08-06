using System;
using System.Linq;
using System.Text;
using NSoft.NFramework.Data.NHibernateEx.Domain.Northwind;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Domain.Northwind.Linq {
    [TestFixture]
    public class AggregateTestFixture : NorthwindDataContextTestFixtureBase {
        [Test]
        public void AggregateWithStartsWith() {
            var query =
                Northwind.Customers
                    .Where(c => c.Id.StartsWith("A"))
                    .Select(c => c.Id)
                    .Aggregate((prev, next) => string.Concat(prev, ",", next));

            Console.WriteLine(query);
            Assert.AreEqual("ALFKI,ANATR,ANTON,AROUT", query.ToString());
        }

        [Test]
        public void AggregateWithContains() {
            var query = Northwind.Customers
                .Where(c => c.Id.Contains("CH"))
                .Select(c => c.Id)
                .Aggregate((prev, next) => string.Concat(prev, ",", next));

            Console.WriteLine(query);
            Assert.AreEqual("CHOPS,RANCH", query.ToString());
        }

        [Test]
        public void AggregateWithEquals() {
            var searchIds = new string[] { "ALFKI", "ANATR", "ANTON" };

            var query = Northwind.Customers
                .Where(c => searchIds.Contains(c.Id))
                .Select(c => c.Id)
                .Aggregate((prev, next) => (prev + "," + next));

            Console.WriteLine(query);
            Assert.AreEqual("ALFKI,ANATR,ANTON", query);
        }

        [Test]
        public void AggregateWithMonthFunction() {
            var date = new DateTime(2007, 1, 1);

            var query = GetEntityQuery<Employee>()
                .Where(e => e.BirthDate != null && e.BirthDate.Value.Month == date.Month)
                .Select(e => e.FullName)
                .Aggregate(new StringBuilder(), (sb, name) => sb.Length > 0 ? sb.Append(",").Append(name) : sb.Append(name));

            Console.WriteLine("{0} Birthdays:", date.ToString("MMMM"));
            Console.WriteLine(query);
        }
    }
}