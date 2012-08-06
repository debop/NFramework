using System;
using System.Linq;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data {
    public static partial class AdoTool {
        // 공백이거나 개행문자가 붙어야 하는데...
        private static readonly string[] SqlStringTokens
            = new[]
              {
                  "from ", "select ", "insert ", "update ", "delete ", "top ", "set ", "where ",
                  "order by ", "group by ", "exec ", "execute ", "run ", "drop ", "create ", "alter ", "replace ",
                  "table ", "view ", "if ", "proc ", "procedure ", "dbcc ", " count(", " isnull(",
                  "pragma "
              };

        private static readonly string[] InvalidCountKeywords = new[] { SQL_DISTINCT, SQL_TOP, SQL_LIMIT, SQL_OFFSET };

        /// <summary>
        /// 지정한 쿼리문이 일반적인 쿼리문인지, Stored Procedure 이름인지 판단한다.
        /// </summary>
        /// <remarks>
        /// DAAB의 Database에는 GetSqlStringCommand, GetProcedureCommand 만 지원한다. 
        /// 동적으로 실행할 쿼리문이 결정될 때에는 GetCommand 와 같이 일반 SQL 문장과 Procedure Name을 구분하지 않고, Command를 빌드해주는 함수가 필요하다.
        /// </remarks>
        /// <param name="query">검사할 SQL 문장</param>
        /// <returns>일반 SQL 문장이면 true, Stored Procedure Name이면 false 반환</returns>
        /// <see cref="IAdoRepository.GetCommand(string,bool)"/>
        public static bool IsSqlString(string query) {
            query.ShouldNotBeWhiteSpace("query");

            string sql = query.ToLower();
            var isSqlString = SqlStringTokens.Any(sql.Contains);

            if(IsDebugEnabled)
                log.Debug("쿼리 문장인가요? isSqlString=[{0}], query=[{1}]", isSqlString, query);

            return isSqlString;
        }

        /// <summary>
        /// SELECT sql statement
        /// </summary>
        public const string SQL_SELECT = "SELECT ";

        /// <summary>
        /// DISTINCT sql statement
        /// </summary>
        public const string SQL_DISTINCT = "DISTINCT";

        /// <summary>
        /// FROM sql statement
        /// </summary>
        public const string SQL_FROM = "FROM ";

        /// <summary>
        /// TOP sql statement
        /// </summary>
        public const string SQL_TOP = "TOP";

        /// <summary>
        /// LIMIT
        /// </summary>
        public const string SQL_LIMIT = "LIMIT";

        /// <summary>
        /// Offset
        /// </summary>
        public const string SQL_OFFSET = "OFFSET";

        /// <summary>
        /// COUNT(*) sql statement
        /// </summary>
        public const string SQL_COUNT = "COUNT(*) ";

        /// <summary>
        /// ORDER BY sql statement
        /// </summary>
        public const string SQL_ORDER_BY = "ORDER BY ";

        /// <summary>
        /// 조회용 문장으로부터 결과 셋의 Count를 세는 쿼리 문장으로 변환합니다.
        /// NOTE : 현재 지원되지 않는 코드는 DISTINCT, TOP 이 들어간 코드입니다.
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public static string GetCountingSqlString(string selectSql) {
            if(IsDebugEnabled)
                log.Debug("조회용 SQL 문장에서 Count를 수행하는 SQL 문장으로 변환합니다... selectSql=[{0}]", selectSql);

            selectSql.ShouldNotBeWhiteSpace("selectSql");

            try {
                Guard.Assert(CanConvertableSqlStringToCounting(selectSql),
                             "지정된 문자열이 Count() 를 조회를 수행할 수 있는 SQL 문자열이 아닙니다. selectSql=[{0}]", selectSql);

                var sql = selectSql.ToUpper();

                var firstColumnIndex = sql.IndexOf(SQL_SELECT);
                var fromIndex = sql.IndexOf(SQL_FROM);

                var subsql = sql.Substring(firstColumnIndex + SQL_SELECT.Length).Trim();

                if(InvalidCountKeywords.Any(word => subsql.StartsWith(word) || subsql.Contains(word)))
                    throw new NotSupportedException(InvalidCountKeywords.CollectionToString() + " 가 들어간 조회문자열의 변경은 지원하지 않습니다.");

                Guard.Assert(firstColumnIndex > -1 && fromIndex > -1, "조회용 문자에서 SELECT 단어와 FROM 단어를 찾을 수 없습니다.");

                // Column 에 Subquery가 있는 경우를 대비해서 SELECT FROM 쌍을 제거한다.
                //
                int tryCount = 0;
                var startIndex = firstColumnIndex + SQL_SELECT.Length;

                while(startIndex > 0 && startIndex < sql.Length && ++tryCount < 4000) {
                    var subSelectIndex = sql.IndexOf(SQL_SELECT, startIndex);
                    var subFromIndex = sql.IndexOf(SQL_FROM, startIndex);

                    // SubQuery에 SELECT 문장이 없으므로 더 이상 작업이 필요없다.
                    if(subSelectIndex < startIndex) {
                        fromIndex = subFromIndex;
                        break;
                    }

                    if(subFromIndex < startIndex)
                        break;

                    if(subSelectIndex < subFromIndex)
                        startIndex = subFromIndex + SQL_FROM.Length;
                }

                firstColumnIndex += SQL_SELECT.Length;

                var countSql = selectSql.Substring(0, firstColumnIndex) + SQL_COUNT + selectSql.Substring(fromIndex);

                var orderByIndex = countSql.ToUpper().LastIndexOf(SQL_ORDER_BY);
                if(orderByIndex > 0)
                    countSql = countSql.Substring(0, orderByIndex - 1); // orderByIndex 에서 -1 을 하는 것은 ORDER BY의 앞의 공백을 제거하기 위해서이다.

                if(IsDebugEnabled)
                    log.Debug("Count 수행 SQL 문장. countSql=[{0}]", countSql);

                return countSql;
            }
            catch(Exception ex) {
                if(log.IsInfoEnabled) {
                    log.Info("조회 SQL 변환에 실패했습니다. 지원하지 않는 단어가 포함되어 있습니다. DISTINCT, TOP 등... ");
                    log.Info("selectedSql=[{0}]", selectSql);
                    log.Info(ex);
                }

                throw;
            }
        }

        /// <summary>
        /// 조회용 쿼리문을 Count 쿼리문으로 변환 가능한지 판단한다. (DISTINCT, TOP 이 들어가면 안됩니다.)
        /// </summary>
        /// <param name="selectSql"></param>
        /// <returns></returns>
        public static bool CanConvertableSqlStringToCounting(string selectSql) {
            if(IsDebugEnabled)
                log.Debug("조회용 SQL 문장을 Counting SQL 문장으로 변환이 가능한지 검사합니다. selectSql=[{0}]", selectSql);

            if(selectSql.IsWhiteSpace())
                return false;

            var sql = selectSql.ToUpper();
            var canConvertable = sql.Contains(SQL_SELECT) && sql.Contains(SQL_FROM);

            if(IsDebugEnabled) {
                log.Debug("조회용 SQL 문장을 Counting SQL 문장으로 변환이 가능한지 1차 검사했습니다. 검사결과=[{0}]", canConvertable);
                log.Debug("selectSql=[{0}]", selectSql);
            }

            return canConvertable;
        }
    }
}