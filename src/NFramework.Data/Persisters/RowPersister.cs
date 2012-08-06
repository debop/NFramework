using System;
using System.Data;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.Persisters {
    /// <summary>
    /// DataRow로 부터 Persistent object를 빌드하는 Persister입니다.
    /// </summary>
    /// <typeparam name="TPersistent"></typeparam>
    public class RowPersister<TPersistent> : AdoPersisterBase<DataRow, TPersistent>, IRowPersister<TPersistent> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        public RowPersister() : base(new TrimNameMapper()) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="nameMapper">name mapper</param>
        /// <param name="persistentFactory">Persistent object 생성 delegate</param>
        public RowPersister(INameMapper nameMapper, Func<TPersistent> persistentFactory = null) : base(nameMapper, persistentFactory) {}

        /// <summary>
        /// DataSource로부터 새로운 Persistent object를 빌드합니다.
        /// </summary>
        /// <param name="dataRow">데이타 소스</param>
        /// <returns>Persistent object</returns>
        public override TPersistent Persist(DataRow dataRow) {
            dataRow.ShouldNotBeNull("dataRow");

            if(IsDebugEnabled)
                log.Debug("Build Persistent object of Type [{0}] from DataRow... Name Mapping method=[{1}]",
                          typeof(TPersistent).FullName, NameMapper.GetType().FullName);

            var persistent = FactoryFunction();

            foreach(DataColumn col in dataRow.Table.Columns) {
                var propertyName = NameMapper.MapToPropertyName(col.ColumnName);

                if(propertyName.IsNotWhiteSpace()) {
                    if(IsDebugEnabled)
                        log.Debug("속성명 [{0}]에 DataRow의 컬럼명 [{1}]의 값 [{2}]를 설정합니다...", propertyName, col.ColumnName, dataRow[col]);

                    DynamicAccessor.SetPropertyValue(persistent, propertyName, dataRow[col]);
                }
            }

            return persistent;
        }
    }
}