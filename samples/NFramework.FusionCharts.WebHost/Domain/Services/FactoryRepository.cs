using System.Collections.Generic;
using System.Data;
using NSoft.NFramework.Data;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Data.SqlServer;
using NSoft.NFramework.FusionCharts.WebHost.Domain.Model;

namespace NSoft.NFramework.FusionCharts.WebHost.Domain.Services {
    public class FactoryRepository {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private static readonly object _syncLock = new object();
        public static INameMapper DefaultNameMap = new CapitalizeNameMapper();

        private static IAdoRepository _ado;

        /// <summary>
        ///  기본 Database를 사용하는 Repository (환경 설정 dataConfiguration의 defaultDatabase에 해당하는 DB를 사용한다.)
        /// </summary>
        public static IAdoRepository Ado {
            get {
                if(_ado == null)
                    lock(_syncLock)
                        if(_ado == null) {
                            var ado = new SqlRepositoryImpl();
                            System.Threading.Thread.MemoryBarrier();
                            _ado = ado;
                        }

                return _ado;

                // double-checking locking을 이용하여, 초기화를 수행합니다. (이게 속도가 더 느리고, 부하가 많이 간다)
                // LazyInitializer.EnsureInitialized(ref _ado, () => new SqlRepositoryImpl());
                // return _ado;
            }
        }

        public static DataTable FindAllFactoryMasterDataTable() {
            const string sql = @"SELECT FactoryId, FactoryName from Factory_Master";

            return Ado.ExecuteDataTableBySqlString(sql);
        }

        public static IList<FactoryMaster> FindAllFactoryMaster() {
            const string sql = @"SELECT FactoryId, FactoryName from Factory_Master";

            IList<FactoryMaster> masters = new List<FactoryMaster>();

            using(var reader = Ado.ExecuteReaderBySqlString(sql)) {
                while(reader.Read()) {
                    masters.Add(new FactoryMaster
                                {
                                    Id = reader.AsInt32("FactoryId"),
                                    Name = reader.AsString("FactoryName")
                                });
                }
            }
            return masters;
        }

        public static IList<FactoryOutput> FindAllFactoryOutputByFactoryId(int factoryId) {
            const string sql =
                @"
SELECT FactoryId, DatePro, Quantity 
  FROM Factory_Output 
 WHERE FactoryId=@FactoryId 
 ORDER BY DatePro";

            return Ado.ExecuteInstanceAsync<FactoryOutput>(DefaultNameMap, sql, new AdoParameter("FactoryId", factoryId)).Result;
        }
    }
}