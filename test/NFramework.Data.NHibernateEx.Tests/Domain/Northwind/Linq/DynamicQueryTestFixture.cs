using System;
using System.Linq;
using NSoft.NFramework.Data.NHibernateEx.Domain.Northwind;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Domain.Northwind.Linq {
    [TestFixture]
    public class DynamicQueryTestFixture : NorthwindDataContextTestFixtureBase {
        [Test]
        public void CanQueryWithDynamicOrderBy() {
            var query = GetEntityQuery<Customer>().OrderBy(c => c.ContactName);
            var list = query.ToList();

            Console.WriteLine(query);

            list.ForEach(c => Console.WriteLine(c.ContactName));

            var previousName = string.Empty;
            list.ForEach(c => {
                             Assert.IsTrue(previousName.CompareTo(c.ContactName) < 0);
                             previousName = c.ContactName;
                         });
        }
    }
}