using System.Linq;
using NSoft.NFramework.Reflections;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Domain.Northwind.Linq {
    [TestFixture]
    public class NorthwindDataContextTestFixture : NorthwindDataContextTestFixtureBase {
        [Test]
        public void SubstringFunction() {
            var query = from e in Northwind.Employees
                        where e.FullName.FirstName.Substring(1, 2) == "An"
                        select e;

            var list = query.ToList();
            Assert.IsTrue(list.Count > 0);
            ObjectDumper.Write(query);
        }

        [Test]
        public void LeftFunction() {
            var query = from e in Northwind.Employees
                        where e.FullName.FirstName.Substring(1, 2) == "An"
                        select e.FullName.FirstName.Substring(3);

            var list = query.ToList();
            Assert.IsTrue(list.Count > 0);
            ObjectDumper.Write(query);
        }

        [Test]
        public void StartsWithFunction() {
            var query = from e in Northwind.Employees
                        where e.FullName.FirstName.StartsWith("AN")
                        select e;

            var list = query.ToList();

            ObjectDumper.Write(query);
            Assert.IsTrue(list.Count > 0);
        }
    }
}