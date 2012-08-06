using System;
using NHibernate;
using NHibernate.Dialect.Function;

namespace NSoft.NFramework.Data.NHibernateEx.Dialect {
    [Obsolete("AnsiString을 자동으로 String을 매핑하지 못한다.")]
    public class MsSqlCe40Dialect : MsSqlCeDialect {
        public MsSqlCe40Dialect() {
            RegisterFunction("concat", new VarArgsSQLFunction(NHibernateUtil.String, "(", "+", ")"));
        }

        public override bool SupportsLimit {
            get { return true; }
        }

        public override bool SupportsLimitOffset {
            get { return true; }
        }

        //public override SqlString GetLimitString(SqlString queryString, SqlString offset, SqlString limit)
        //{
        //    if(queryString.IndexOfCaseInsensitive(" ORDER BY ") < 0)
        //    {
        //        queryString = queryString.Append(" ORDER BY GETDATE()");
        //    }

        //    return queryString.Append(string.Format(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, limit));
        //}
    }
}