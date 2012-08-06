using System.Data;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.Data.SQLite {
    public abstract class SQLiteAdoFixtureBase : IoCSetupBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        public const string SelectOrder = @"SELECT * FROM Orders";
        public const string SelectOrderDetails = @"SELECT * FROM OrderDetails";
        public const string CountOrder = @"SELECT count(*) FROM Orders";

        public const string SelectCustomer = @"SELECT * FROM Customers";

        private static readonly object _syncLock = new object();

        public static IAdoRepository DefaultAdoRepository {
            get { return IoC.Resolve<IAdoRepository>("NorthwindAdoRepository.SQLite"); }
        }

        public static ISQLiteRepository DefaultSQLiteRepository {
            get { return IoC.Resolve<ISQLiteRepository>("NorthwindAdoRepository.SQLite"); }
        }

        public static ISQLiteRepository NorthwindRepository {
            get { return IoC.Resolve<ISQLiteRepository>("NorthwindAdoRepository.SQLite"); }
        }

        private IAdoParameter _customerTestParameter;

        public IAdoParameter CustomerTestParameter {
            get { return _customerTestParameter ?? (_customerTestParameter = new AdoParameter("CustomerId", "ANATR", DbType.String, 255)); }
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
    }
}