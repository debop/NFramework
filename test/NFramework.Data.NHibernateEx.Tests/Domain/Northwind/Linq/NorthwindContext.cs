using System.Linq;
using NHibernate.Linq;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain.Northwind;

namespace NSoft.NFramework.Data.Domain.Northwind.Linq {
    public class NorthwindContext {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public IQueryable<Category> Categories {
            get { return UnitOfWork.CurrentSession.Query<Category>(); }
        }

        public IQueryable<Customer> Customers {
            get { return UnitOfWork.CurrentSession.Query<Customer>(); }
        }

        public IQueryable<Employee> Employees {
            get { return UnitOfWork.CurrentSession.Query<Employee>(); }
        }

        public IQueryable<Order> Orders {
            get { return UnitOfWork.CurrentSession.Query<Order>(); }
        }

        public IQueryable<OrderDetail> OrderDetails {
            get { return UnitOfWork.CurrentSession.Query<OrderDetail>(); }
        }

        public IQueryable<Product> Products {
            get { return UnitOfWork.CurrentSession.Query<Product>(); }
        }

        public IQueryable<Shipper> Shippers {
            get { return UnitOfWork.CurrentSession.Query<Shipper>(); }
        }

        public IQueryable<Supplier> Suppliers {
            get { return UnitOfWork.CurrentSession.Query<Supplier>(); }
        }
    }
}