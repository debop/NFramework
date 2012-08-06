using System.Data;
using NSoft.NFramework.Data;
using NSoft.NFramework.DataServices.Messages;

namespace NSoft.NFramework.DataServices.Clients {
    public abstract class AbstractServicesFixture : AbstractDataServiceFixture {
        #region << logger >>

        protected static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        protected static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string ProductName = "Northwind";

        public const string SQL_ORDER_DETAILS = "Order Details, GetAll";
        public const string SQL_ORDERS = "Order, GetAll";
        public const string SQL_CUSTOMERS = "Customer, GetAll";

        public const string SP_CUSTOMER_ORDER_HISTORY = "Order, CustomerOrderHistory";

        public const bool NEED_TRANSACTION = false;
        public static readonly object _sync = new object();

        public virtual bool AsParallel {
            get { return false; }
        }

        public virtual RequestMessage CreateRequestMessage() {
            return new RequestMessage
                   {
                       Transactional = NEED_TRANSACTION,
                       AsParallel = AsParallel
                   };
        }

        private IAdoParameter _customerTestParameter;

        public IAdoParameter CustomerTestParameter {
            get { return _customerTestParameter ?? (_customerTestParameter = new AdoParameter("CustomerId", "ANATR", DbType.String)); }
        }
    }
}