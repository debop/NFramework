using System;
using System.Data;
using System.Globalization;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace NSoft.NFramework.Data {
    public static partial class AdoTool {
        /// <summary>
        /// <paramref name="reader"/>의 내용을 읽어, <see cref="DataTable"/> 로 빌드합니다. <b>다 읽은 DataReader는 닫아버립니다.</b>
        /// </summary>
        /// <param name="db">DAAB Database instance</param>
        /// <param name="reader">읽을 IDataReader</param>
        /// <param name="firstResult">첫번째 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">읽을 최대 레코드 수</param>
        /// <returns>DataReader의 내용으로 채워진 DataTable</returns>
        /// <seealso cref="AdoDataAdapter"/>
        public static DataTable BuildDataTableFromDataReader(this Database db, IDataReader reader, int firstResult, int maxResults = 0) {
            return BuildDataTableFromDataReader(db, reader, null, firstResult, maxResults);
        }

        /// <summary>
        /// <paramref name="reader"/>의 내용을 읽어, <see cref="DataTable"/> 로 빌드합니다. <b>다 읽은 DataReader는 닫아버립니다.</b>
        /// </summary>
        /// <param name="db">DAAB Database instance</param>
        /// <param name="reader">읽을 IDataReader</param>
        /// <param name="dataTableFactory">DataTable 생성용 델리게이트</param>
        /// <param name="firstResult">첫번째 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">읽을 최대 레코드 수</param>
        /// <returns>DataReader의 내용으로 채워진 DataTable</returns>
        /// <seealso cref="AdoDataAdapter"/>
        public static DataTable BuildDataTableFromDataReader(this Database db, IDataReader reader,
                                                             Func<DataTable> dataTableFactory = null, int firstResult = 0,
                                                             int maxResults = 0) {
            if(IsDebugEnabled)
                log.Debug("AdoDataAdapter를 이용하여, IDataReader 내용을 읽어, DataTable로 빌드합니다");

            if(dataTableFactory == null)
                dataTableFactory = () => new DataTable { Locale = CultureInfo.InvariantCulture };

            var dataTable = dataTableFactory();

            if(reader != null) {
                var adapter = new AdoDataAdapter(db.GetDataAdapter());
                try {
                    adapter.Fill(new[] { dataTable }, reader, firstResult, maxResults);
                }
                catch(Exception ex) {
                    if(log.IsErrorEnabled) {
                        log.Error("DataReader로부터 DataTable로 Fill하는 동안 예외가 발생했습니다.");
                        log.Error(ex);
                    }
                    throw;
                }
                finally {
                    With.TryAction(reader.Dispose);
                    With.TryAction(adapter.Dispose);
                }

                if(IsDebugEnabled)
                    log.Debug("DataReader를 읽어 DataTable에 Load 했습니다!!! firstResult=[{0}], maxResults=[{1}]", firstResult, maxResults);
            }

            return dataTable;
        }
    }
}