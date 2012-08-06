using System;
using System.Threading;
using Castle.Core;
using NLog;
using NSoft.NFramework.Data.DataObjects.Northwind;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.Data.PostgreSql.Ado {

    public abstract class NpgsqlAdoFixtureBase : IoCSetupBase {

        #region << logger >>

        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly object _syncLock = new object();

        public const string SQL_ORDER_SELECT = "SELECT * FROM Orders";
        public const string SQL_ORDER_DETAIL_SELECT = "SELECT * FROM OrderDetails ";
        public const string SQL_EMPLOYEE_SELECT = "SELECT * FROM Employees ";
        public const string SQL_CUSTOMER_SELECT = "SELECT * FROM Customers ";
        public const string SQL_INVOICE_SELECT = "SELECT * FROM INVOICES ";

        public const string SQL_CUSTOMER_COUNT = "SELECT COUNT(*) FROM Customers";

        public const string SQL_ORDER_DETAIL_BY_ORDER_ID = "SELECT * FROM OrderDetails  WHERE OrderID = :OrderID ";

        public const string SQL_REGION_COUNT = @"SELECT COUNT(*) from Region";
        public const string SQL_REGION_INSERT = @"INSERT INTO Region values (101, 'Elbonia')";
        public const string SQL_REGION_INSERT2 = @"INSERT INTO Region values (102, 'Australia')";

        public const string SQL_REGION_SELECT = @"SELECT * FROM Region";
        public const string SQL_REGION_SELECT_SHARED_LOCK = @"SELECT * FROM Region";
        public const string SQL_REGION_DELETE = @"DELETE FROM Region where RegionID >= 100";

        public const string SQL_ORDER_BY_ORDER_DATE_AND_FREIGHT_SELECT =
            "SELECT * FROM Orders WHERE OrderDate < :OrderDate AND Freight < :Freight";

        public const string SQL_ORDER_BY_ORDERDATE_AND_FREIGHT_AND_CUSTOMER_SELECT =
            "SELECT * FROM Orders WHERE OrderDate < :OrderDate and Freight < :Freight and CustomerID = :CustomerID";

        public const string SP_CUSTOMER_ORDER_HISTORY = "SELECT * FROM CustOrderHist(:CustomerID)"; //"CustOrderHist";

        public static Type CustomerOrderHistoryType = typeof(CustomerOrderHistory);

        public IAdoRepository DefaultAdoRepository {
            get { return IoC.TryResolve(() => AdoRepositoryFactory.Instance.CreateRepositoryByProvider(), true, LifestyleType.Singleton); }
        }

        [ThreadStatic] private static IPostgreSqlRepository _northwindPostgreSqlRepository;

        public static IPostgreSqlRepository NorthwindPostgreSqlRepository {
            get {
                if(_northwindPostgreSqlRepository == null)
                    lock(_syncLock)
                        if(_northwindPostgreSqlRepository == null) {
                            var repository = IoC.Resolve<IPostgreSqlRepository>("NorthwindAdoRepository.PostgreSql");
                            Thread.MemoryBarrier();
                            _northwindPostgreSqlRepository = repository;
                        }

                return _northwindPostgreSqlRepository;
            }
        }

        private IAdoParameter _customerTestParameter;

        public IAdoParameter CustomerTestParameter {
            get { return _customerTestParameter ?? (_customerTestParameter = new AdoParameter("CustomerID", "ANATR")); }
        }

        private IAdoParameter _orderTestParameter;

        public IAdoParameter OrderTestParameter {
            get { return _orderTestParameter ?? (_orderTestParameter = new AdoParameter("OrderID", 11077)); }
        }

        private INameMap _nameMap;

        public INameMap NameMappings {
            get {
                return _nameMap ?? (_nameMap = new NameMap
                                               {
                                                   { "PRODUCTNAME", "ProductName" },
                                                   { "TOTAL", "Total" }
                                               }
                                   );
            }
        }

        private static INameMapper _capitalizeNameMapper;

        public static INameMapper CapitalizeMapper {
            get { return _capitalizeNameMapper ?? (_capitalizeNameMapper = new CapitalizeNameMapper()); }
        }

        private static INameMapper _trimMapper;

        public static INameMapper TrimMapper {
            get { return _trimMapper ?? (_trimMapper = new TrimNameMapper()); }
        }
    }
}