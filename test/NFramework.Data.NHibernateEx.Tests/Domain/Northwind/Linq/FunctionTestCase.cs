using System.Linq;
using NHibernate.Linq;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain.Northwind;
using NSoft.NFramework.Reflections;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.Domain.Northwind.Linq {
    // NOTE: 따로 하면 테스트를 모두 통과하는데, 전체 테스트 시에는 예외가 발생한다. 짐작가는 부분은 초기화시에 문제가 있는 듯^^
    //
    [TestFixture]
    public class FunctionTestCase : NorthwindDataContextTestFixtureBase {
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
            var query = from e in UnitOfWork.CurrentSession.Query<Employee>()
                        where e.FullName.FirstName.StartsWith("AN")
                        select e;

            query.Count().Should().Be.GreaterThan(0);
            ObjectDumper.Write(query);
        }

        [Test]
        public void CharIndexFunction() {
            var query =
                UnitOfWork.CurrentSession
                    .Query<Employee>()
                    .Where(e => e.FullName.FirstName.IndexOf('A') == 1)
                    .Select(e => e.FullName.FirstName);

            query.Count().Should().Be.GreaterThan(0);
            ObjectDumper.Write(query);
        }

        [Test]
        public void IndexOfFunctionExpression() {
            var query =
                UnitOfWork.CurrentSession
                    .Query<Employee>()
                    .Where(e => e.FullName.FirstName.IndexOf('A') == 1)
                    .Select(e => e.FullName.FirstName);

            query.Count().Should().Be.GreaterThan(0);
            ObjectDumper.Write(query);
        }

        [Test]
        public void IndexOfFunctionProjection() {
            var query =
                UnitOfWork.CurrentSession
                    .Query<Employee>()
                    .Where(e => e.FullName.FirstName.Contains("a"))
                    .Select(e => e.FullName.FirstName.IndexOf('A', 3));

            query.Count().Should().Be.GreaterThan(0);
            ObjectDumper.Write(query);
        }

        [Test]
        public void TwoFunctionExpression() {
            var query =
                UnitOfWork.CurrentSession
                    .Query<Employee>()
                    .Where(e => e.FullName.FirstName.IndexOf("A") == e.BirthDate.Value.Month);

            query.Count().Should().Be.GreaterThan(0);
            ObjectDumper.Write(query);
        }
    }
}