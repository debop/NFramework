using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.Data.DevartOracle.Ado {
    public abstract class OracleFixtureBase : IoCSetupBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        // DevArt 사의 예제를 실행시켜야 합니다.

        public const string SelectDept = @"SELECT * FROM DEPT";
        public const string SelectEmp = @"SELECT * FROM EMP";
        public const string CountEmp = @"SELECT count(*) FROM EMP";

        public const string SelectDemoStates = @"SELECT * FROM DEMO_STATES";
        public const string SelectDemoCustomers = @"SELECT * FROM DEMO_CUSTOMERS";
        public const string SelectDemoOrders = "SELECT * FROM DEMO_ORDERS";

        public const string SP_GET_EMPLOYEES_BY_DEPT = @"GET_EMPLOYEES_BY_DEPT";

        private static readonly object _syncLock = new object();

        public static IAdoRepository DefaultAdoRepository {
            get { return IoC.Resolve<IAdoRepository>("DefaultAdoRepository"); }
        }

        public static IOracleRepository DefaultOracleRepository {
            get { return IoC.Resolve<IOracleRepository>("OracleRepository.Default"); }
        }

        public static IOracleRepository RealWebRepository {
            get { return IoC.Resolve<IOracleRepository>("OracleRepository.NSoft"); }
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