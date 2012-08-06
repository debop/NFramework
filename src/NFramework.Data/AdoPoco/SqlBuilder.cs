using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.AdoPoco {
    /// <summary>
    /// SQL 문자열을 빌드하는 헬퍼 클래스입니다.
    /// </summary>
    public class SqlBuilder {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static SqlBuilder Builder {
            get { return new SqlBuilder(); }
        }

        private string _sql;
        private object[] _args;
        private SqlBuilder _rhs;
        private string _sqlFinal;
        private object[] _argsFinal;

        public SqlBuilder() {}

        public SqlBuilder(string sql, params object[] args) {
            sql.ShouldNotBeWhiteSpace("sql");

            if(IsDebugEnabled)
                log.Debug("SqlBuilder를 생성합니다. sql=[{0}], args=[{1}]", sql, args.CollectionToString());

            _sql = sql;
            _args = args;
        }

        /// <summary>
        /// 완성된 SQL 문장을 반환합니다.    
        /// </summary>
        public string SQL {
            get {
                Build();
                return _sqlFinal;
            }
        }

        /// <summary>
        /// 완성된 SQL 문장에 쓰인 인자 정보를 반환합니다.
        /// </summary>
        public object[] Arguments {
            get {
                Build();
                return _argsFinal;
            }
        }

        public SqlBuilder Append(SqlBuilder sql) {
            sql.ShouldNotBeNull("sql");

            if(_rhs != null)
                _rhs.Append(sql);
            else
                _rhs = sql;

            return this;
        }

        public SqlBuilder Append(string sql, params object[] args) {
            sql.ShouldNotBeWhiteSpace("sql");
            return Append(new SqlBuilder(sql, args));
        }

        public SqlBuilder Where(string sql, params object[] args) {
            var whereClause = string.Concat(SqlSR.WHERE, "(", sql, ")");
            return Append(new SqlBuilder(whereClause, args));
        }

        public SqlBuilder OrderBy(params object[] columns) {
            var orderByClause = SqlSR.ORDER_BY + columns.Select(x => x.ToString()).Join(", ");
            return Append(new SqlBuilder(orderByClause));
        }

        public SqlBuilder Select(params object[] columns) {
            var selectClause = SqlSR.SELECT + columns.Select(x => x.ToString()).Join(", ");
            return Append(new SqlBuilder(selectClause));
        }

        public SqlBuilder From(params object[] columns) {
            var fromClause = SqlSR.FROM + columns.Select(x => x.ToString()).Join(", ");
            return Append(new SqlBuilder(fromClause));
        }

        public SqlBuilder GroupBy(params object[] columns) {
            var fromClause = SqlSR.GROUP_BY + columns.Select(x => x.ToString()).Join(", ");
            return Append(new SqlBuilder(fromClause));
        }

        public SqlJoinClause InnerJoin(string table) {
            table.ShouldNotBeWhiteSpace("table");
            return JoinInternal(SqlSR.INNER_JOIN, table);
        }

        public SqlJoinClause LeftJoin(string table) {
            table.ShouldNotBeWhiteSpace("table");
            return JoinInternal(SqlSR.LEFT_JOIN, table);
        }

        public SqlJoinClause LeftOuterJoin(string table) {
            table.ShouldNotBeWhiteSpace("table");
            return JoinInternal(SqlSR.LEFT_OUTER_JOIN, table);
        }

        public SqlJoinClause RightJoin(string table) {
            table.ShouldNotBeWhiteSpace("table");
            return JoinInternal(SqlSR.RIGHT_JOIN, table);
        }

        public SqlJoinClause RightOuterJoin(string table) {
            table.ShouldNotBeWhiteSpace("table");
            return JoinInternal(SqlSR.RIGHT_OUTER_JOIN, table);
        }

        /// <summary>
        /// SQL 문장과 파라미터 정보를 이용하여 SQL 문장을 완성합니다.
        /// </summary>
        private void Build() {
            if(_sqlFinal.IsNotWhiteSpace())
                return;

            // Build it 
            var sb = new StringBuilder();
            var args = new List<object>();
            Build(sb, args);

            _sqlFinal = sb.ToString();
            _argsFinal = args.ToArray();
        }

        /// <summary>
        /// SQL 문장과 파라미터 정보를 이용하여 SQL 문장을 완성합니다.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="args"></param>
        /// <param name="lhs"></param>
        private void Build(StringBuilder sb, IList<object> args, SqlBuilder lhs = null) {
            if(_sql.IsNotWhiteSpace()) {
                if(sb.Length > 0)
                    sb.AppendLine();

                string sql = AdoProviderTool.ProcessParams(_sql, _args, args);

                if(Is(lhs, SqlSR.WHERE) && Is(this, SqlSR.WHERE))
                    sql = SqlSR.AND + sql.Substring(SqlSR.WHERE.Length);

                if(Is(lhs, SqlSR.ORDER_BY) && Is(this, SqlSR.ORDER_BY))
                    sql = ", " + sql.Substring(SqlSR.ORDER_BY.Length);

                sb.Append(sql);
            }

            //  do for rhs
            if(_rhs != null)
                _rhs.Build(sb, args, this);
        }

        /// <summary>
        /// <paramref name="sqlBuilder"/> 의  내부 Sql 문장에 <paramref name="sqlType"/>의 예약어로 시작하는지 검사합니다.
        /// </summary>
        /// <param name="sqlBuilder"></param>
        /// <param name="sqlType"></param>
        /// <returns></returns>
        private static bool Is(SqlBuilder sqlBuilder, string sqlType) {
            if(sqlBuilder == null)
                return false;

            return sqlBuilder._sql.IsNotWhiteSpace() &&
                   sqlBuilder._sql.StartsWith(sqlType, StringComparison.InvariantCultureIgnoreCase);
        }

        private SqlJoinClause JoinInternal(string joinType, string table) {
            return new SqlJoinClause(Append(new SqlBuilder(joinType + table)));
        }

        /// <summary>
        /// Join 과 관련된 절을 빌드하는 클래스입니다.
        /// </summary>
        public class SqlJoinClause {
            private readonly SqlBuilder _sqlBuilder;

            public SqlJoinClause(SqlBuilder sqlBuilder) {
                sqlBuilder.ShouldNotBeNull("sqlBuilder");
                _sqlBuilder = sqlBuilder;
            }

            /// <summary>
            /// Join 문의 ON 에 해당하는 Clause 를 추가합니다.
            /// </summary>
            /// <param name="onClause"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public SqlBuilder On(string onClause, params object[] args) {
                onClause.ShouldNotBeWhiteSpace("onClause");
                return _sqlBuilder.Append(SqlSR.ON + onClause, args);
            }
        }
    }
}