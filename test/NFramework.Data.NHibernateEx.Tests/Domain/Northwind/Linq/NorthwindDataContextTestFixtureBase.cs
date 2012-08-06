using System.Linq;
using NHibernate.Linq;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.Tests.Domain.Northwind;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Domain.Northwind.Linq {
    [TestFixture]
    public abstract class NorthwindDataContextTestFixtureBase : NorthwindDbTestFixtureBase {
        protected NorthwindContext _northwindContext;

        // 좋은 방식은 아니다. CurrentSession은 항상 바뀔 수 있기 때문이다.
        public NorthwindContext Northwind {
            get { return _northwindContext ?? (_northwindContext = new NorthwindContext()); }
        }

        protected static IQueryable<T> GetEntityQuery<T>() {
            return UnitOfWork.CurrentSession.Query<T>();
        }
    }
}