using System;
using System.Data;
using System.Data.Common;
using System.Text;
using NHibernate.Dialect.Schema;
using NHibernate.Util;

namespace NSoft.NFramework.Data.NHibernateEx.Dialect {
    /// <summary>
    /// SQL Server CE 를 사용할 때 사용하는 Dialect입니다. NHibernate에서 제공하는 것은 AnsiString을 지원하지 못하는데, 그 문제를 해결했습니다.
    /// 
    /// 참고 : http://netfrustrations.blogspot.com/2009/10/nhibernate-mapping-typeansistring.html
    /// </summary>
    [Obsolete("AnsiString을 자동으로 String을 매핑하지 못한다.")]
    public class MsSqlCeDialect : NHibernate.Dialect.Dialect {
        public MsSqlCeDialect() {
            RegisterColumnType(DbType.AnsiStringFixedLength, "NCHAR(255)");
            RegisterColumnType(DbType.AnsiStringFixedLength, 4000, "NCHAR($l)");
            RegisterColumnType(DbType.AnsiString, "NVARCHAR(255)");
            RegisterColumnType(DbType.AnsiString, 4000, "NVARCHAR($l)");
            RegisterColumnType(DbType.AnsiString, 1073741823, "NTEXT");
            RegisterColumnType(DbType.Binary, "VARBINARY(8000)");
            RegisterColumnType(DbType.Binary, 8000, "VARBINARY($l)");
            RegisterColumnType(DbType.Binary, 1073741823, "IMAGE");
            RegisterColumnType(DbType.Boolean, "BIT");
            RegisterColumnType(DbType.Byte, "TINYINT");
            RegisterColumnType(DbType.Currency, "MONEY");
            RegisterColumnType(DbType.DateTime, "DATETIME");
            RegisterColumnType(DbType.Decimal, "NUMERIC(19,5)");
            RegisterColumnType(DbType.Decimal, 19, "NUMERIC(19, $l)");
            RegisterColumnType(DbType.Double, "FLOAT");
            RegisterColumnType(DbType.Guid, "UNIQUEIDENTIFIER");
            RegisterColumnType(DbType.Int16, "SMALLINT");
            RegisterColumnType(DbType.Int32, "INT");
            RegisterColumnType(DbType.Int64, "BIGINT");
            RegisterColumnType(DbType.Single, "REAL"); //synonym for FLOAT(24) 
            RegisterColumnType(DbType.StringFixedLength, "NCHAR(255)");
            RegisterColumnType(DbType.StringFixedLength, 4000, "NCHAR($l)");
            RegisterColumnType(DbType.String, "NVARCHAR(255)");
            RegisterColumnType(DbType.String, 4000, "NVARCHAR($l)");
            RegisterColumnType(DbType.String, 1073741823, "NTEXT");
            RegisterColumnType(DbType.Time, "DATETIME");

            DefaultProperties[NHibernate.Cfg.Environment.ConnectionDriver] =
                "NSoft.NFramework.Data.NH.SqlServerCeDriver, NSoft.NFramework.Data";
            DefaultProperties[NHibernate.Cfg.Environment.PrepareSql] = "false";
        }

        public override string AddColumnString {
            get { return "add"; }
        }

        public override string NullColumnString {
            get { return " null"; }
        }

        public override bool QualifyIndexName {
            get { return false; }
        }

        public override string ForUpdateString {
            get { return string.Empty; }
        }

        public override bool SupportsIdentityColumns {
            get { return true; }
        }

        public override string IdentitySelectString {
            get { return "select @@IDENTITY"; }
        }

        public override string IdentityColumnString {
            get { return "IDENTITY NOT NULL"; }
        }

        public override bool SupportsLimit {
            get { return false; }
        }

        public override bool SupportsLimitOffset {
            get { return false; }
        }

        public override bool SupportsVariableLimit {
            get { return false; }
        }

        public override IDataBaseSchema GetDataBaseSchema(DbConnection connection) {
            return new MsSqlCeDataBaseSchema(connection);
        }

        public override string Qualify(string catalog, string schema, string table) {
            // SQL Server Compact doesn't support Schemas. So join schema name and table name with underscores
            // similar to the SQLLite dialect.

            var qualifiedName = new StringBuilder();
            bool quoted = false;

            if(!string.IsNullOrEmpty(catalog)) {
                qualifiedName.Append(catalog).Append(StringHelper.Dot);
            }

            var tableName = new StringBuilder();
            if(!string.IsNullOrEmpty(schema)) {
                if(schema.StartsWith(OpenQuote.ToString())) {
                    schema = schema.Substring(1, schema.Length - 1);
                    quoted = true;
                }
                if(schema.EndsWith(CloseQuote.ToString())) {
                    schema = schema.Substring(0, schema.Length - 1);
                    quoted = true;
                }
                tableName.Append(schema).Append(StringHelper.Underscore);
            }

            if(table.StartsWith(OpenQuote.ToString())) {
                table = table.Substring(1, table.Length - 1);
                quoted = true;
            }
            if(table.EndsWith(CloseQuote.ToString())) {
                table = table.Substring(0, table.Length - 1);
                quoted = true;
            }

            string name = tableName.Append(table).ToString();
            if(quoted)
                name = OpenQuote + name + CloseQuote;
            return qualifiedName.Append(name).ToString();
        }
    }
}