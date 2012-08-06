using System.Data;

namespace NSoft.NFramework.Data {
    public static partial class AdoTool {
        /// <summary>
        /// 지정한 <see cref="IDataReader"/> 정보를 읽어 <see cref="AdoResultSet"/>으로 빌드합니다.
        /// </summary>
        /// <param name="reader">DataReader</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (1 이상, null|0 이면 최대 레코드)</param>
        /// <returns>빌드된 <see cref="AdoResultSet"/> 인스턴스</returns>
        public static AdoResultSet ToAdoResultSet(this IDataReader reader, int firstResult = 0, int maxResults = 0) {
            return new AdoResultSet(reader, firstResult, maxResults);
        }
    }
}