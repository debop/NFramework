using System;
using System.Data;
using System.Text;
using Castle.Core;
using NSoft.NFramework.Data.DataObjects.Northwind;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.Data {
    // NOTE : 테스트를 위해서는 Samples\Database\Northwind for Rcl.Data Testing.sql 를 실행해야 합니다.
    public abstract class AdoFixtureBase : IoCSetUpBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly object _syncLock = new object();

        public const string DbName = "Northwind";
        public const string SQL_ORDER_SELECT = "SELECT * FROM dbo.Orders WITH (NOLOCK) ";
        public const string SQL_ORDER_DETAIL_SELECT = "SELECT * FROM dbo.[Order Details] WITH (NOLOCK) ";
        public const string SQL_EMPLOYEE_SELECT = "SELECT * FROM dbo.Employees WITH (NOLOCK)";
        public const string SQL_CUSTOMER_SELECT = "SELECT * FROM dbo.Customers WITH (NOLOCK)";

        public const string SQL_INVOICE_SELECT = "SELECT * FROM INVOICES ";

        public const string SQL_ORDER_DETAILS_BY_ORDER_ID = "SELECT * FROM [Order Details] WITH (NOLOCK) WHERE OrderID=@OrderID";

        public const string SQL_REGION_COUNT = @"SELECT count(*) from dbo.Region WITH (NOLOCK) ";
        public const string SQL_REGION_INSERT = @"INSERT into dbo.Region values (101, 'Elbonia')";
        public const string SQL_REGION_INSERT2 = @"INSERT into dbo.Region values (102, 'Australia')";

        public const string SQL_REGION_SELECT = @"SELECT * from dbo.Region with (nolock)";
        public const string SQL_REGION_SELECT_SHARED_LOCK = @"SELECT * from dbo.Region";
        public const string SQL_REGION_DELETE = @"DELETE FROM dbo.Region where RegionID >= 100";

        public const string ErrorQueryString = "RAISERROR('Error in SQL', 16, 1)";

        public static Type CustomerOrderHistoryType = typeof(CustomerOrderHistory);

        public IAdoRepository DefaultAdoRepository {
            get { return IoC.TryResolve(() => AdoRepositoryFactory.Instance.CreateRepositoryByProvider(), true, LifestyleType.Singleton); }
        }

        private static IAdoRepository _northwindRepository;

        public static IAdoRepository NorthwindAdoRepository {
            get {
                if(_northwindRepository == null)
                    lock(_syncLock)
                        if(_northwindRepository == null) {
                            var repository = IoC.Resolve<IAdoRepository>("NorthwindAdoRepository.SqlServer");
                            System.Threading.Thread.MemoryBarrier();
                            _northwindRepository = repository;
                        }

                return _northwindRepository;
            }
        }

        private IAdoParameter _customerTestParameter;

        public IAdoParameter CustomerTestParameter {
            get { return _customerTestParameter ?? (_customerTestParameter = new AdoParameter("CustomerId", "ANATR", DbType.String)); }
        }

        private IAdoParameter _orderTestParameter;

        public IAdoParameter OrderTestParameter {
            get { return _orderTestParameter ?? (_orderTestParameter = new AdoParameter("OrderId", 11077, DbType.Int32)); }
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

        public static int TotalCount() {
            return AdoRepository.ExecuteScalar(SQL_REGION_COUNT).AsInt(0);
        }

        #region << Stored Procedure for Testing : Northwind >>

        public static void CreateStoredProcedures() {
            var query = @"create procedure RegionSelect as select * from Region Order by RegionId";

            NorthwindAdoRepository.ExecuteNonQueryBySqlString(query);

            query = @"create procedure RegionInsert(@RegionID int, @RegionDescription varchar(100)) as "
                    + @"Insert into Region values(@RegionID, @RegionDescription)";

            NorthwindAdoRepository.ExecuteNonQueryBySqlString(query);

            query = @"create procedure RegionUpdate(@RegionID int, @RegionDescription varchar(100)) as "
                    + @"Update Region set RegionDescription=@RegionDescription where RegionID=@RegionID";

            NorthwindAdoRepository.ExecuteNonQueryBySqlString(query);

            query = @"create procedure RegionDelete(@RegionID int) as "
                    + @"Delete Region where RegionID=@RegionID";

            NorthwindAdoRepository.ExecuteNonQueryBySqlString(query);
        }

        public static void DeleteStoredProcedures() {
            var queryBuilder = new StringBuilder();
            queryBuilder
                .Append("if object_id('RegionSelect') <> 0 drop procedure RegionSelect;")
                .Append("if object_id('RegionInsert') <> 0 drop procedure RegionInsert;")
                .Append("if object_id('RegionUpdate') <> 0 drop procedure RegionUpdate;")
                .Append("if object_id('RegionDelete') <> 0 drop procedure RegionDelete;");

            NorthwindAdoRepository.ExecuteNonQueryBySqlString(queryBuilder.ToString());
        }

        #endregion
    }
}