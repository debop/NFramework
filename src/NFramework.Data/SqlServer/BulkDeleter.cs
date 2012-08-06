using System;
using System.Data;
using System.Data.SqlClient;

namespace NSoft.NFramework.Data.SqlServer {
    /// <summary>
    /// <see cref="SqlBulkCopy"/>를 이용하여 Bulk Delete를 수행한다.
    /// </summary>
    /// <typeparam name="TEntity">엔티티 수형</typeparam>
    /// <typeparam name="TPrimaryKey">엔티티의 Primary Key 수형</typeparam>
    public class BulkDeleter<TEntity, TPrimaryKey> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly DataTable _table;
        private readonly string _tempTableName;
        private readonly string _procedureName;
        private readonly Func<TEntity, TPrimaryKey> _getPrimaryKey;
        private readonly Guid _guid = Guid.NewGuid();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tempTableName">임시 테이블 명</param>
        /// <param name="procedureName">실행할 Procedure 명</param>
        /// <param name="getPrimaryKey">Primary Key를 얻는 메소드</param>
        public BulkDeleter(string tempTableName, string procedureName, Func<TEntity, TPrimaryKey> getPrimaryKey) {
            _tempTableName = tempTableName;
            _procedureName = procedureName;
            _table = new DataTable(_tempTableName);
            _table.Columns.Add("Id", typeof(TPrimaryKey)).Unique = true;
            _table.Columns.Add("Guid", typeof(Guid));
            _getPrimaryKey = getPrimaryKey;
        }

        /// <summary>
        /// 삭제할 TABLE을 표현하는 엔티티 형식을 등록한다.
        /// </summary>
        /// <param name="entityType">Type of entity</param>
        public void ResisterForDeletion(TEntity entityType) {
            if(IsDebugEnabled)
                log.Debug("삭제할 Table을 표현하는 Entity의 Type을 등록합니다. EntityType=[{0}]", entityType);

            lock(_table)
                _table.Rows.Add(_getPrimaryKey(entityType), _guid);
        }

        /// <summary>
        /// 지정된 DB에 삭제를 수행한다.
        /// </summary>
        /// <param name="dbName">database connection string name</param>
        public void PerformDelete(string dbName) {
            var repository = AdoRepositoryFactory.Instance.CreateRepository(dbName);

            lock(_table) {
                // 1. SqlBulkCopy를 이용하여 메모리상으로 복사한 후, 
                // 2. 메모리상에 정보를 지우고, 
                // 3. TABLE의 내용을 삭제하는 Procedure를 호출한다.
                using(var bulkCopy = new SqlBulkCopy((SqlConnection)repository.Db.CreateConnection())) {
                    bulkCopy.DestinationTableName = _tempTableName;
                    bulkCopy.WriteToServer(_table);
                }
                _table.Rows.Clear();

                repository.ExecuteNonQueryByProcedure(_procedureName, new AdoParameter("guid", _guid, DbType.Guid));
            }
        }
    }
}