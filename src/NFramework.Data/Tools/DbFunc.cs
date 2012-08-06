using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlTypes;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// Database 관련 Helper class
    /// </summary>
    public static partial class DbFunc {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 환경설정에 설정된 ConnectionStrings 섹션의 <see cref="ConnectionStringSettings"/> 의 이름을 가져온다.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetDatabaseNames() {
            return ConfigTool.GetDatabaseNames();
        }

        /// <summary>
        /// 환경설정에 설정된 ConnectionStrings 섹션의 <see cref="ConnectionStringSettings"/> 을 모두 가져온다.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ConnectionStringSettings> GetConnectionStringSettings() {
            return ConfigTool.GetConnectionStringSettings();
        }

        /// <summary>
        /// 지정된 문자열의 값들을 왼쪽으로 맞추고, 지정된 길이만큼의 나머지 여백은 공백 로 채웁니다. 
        /// <see cref="string.PadRight(int,char)"/>와 같은 기능을 수행합니다.
        /// </summary>
        /// <example>
        /// <code>
        /// LSet("abc", 10); // => "abc_______"
        /// </code>
        /// </example>
        public static string LSet(string value, int length) {
            if(value.IsEmpty())
                return " ".Replicate(length);

            return (value.Length >= length) ? value : value.PadRight(length, ' ');
        }

        /// <summary>
        /// 지정된 문자열의 값들을 오른쪽으로 맞추고, 지정된 길이만큼의 안쪽 여백은 공백 로 채웁니다. 
        /// <see cref="string.PadRight(int,char)"/>와 같은 기능을 수행합니다.
        /// </summary>
        /// <example>
        /// <code>
        /// LSet("abc", 10); // => "_______abc"
        /// </code>
        /// </example>
        public static string RSet(string value, int length) {
            if(value.IsEmpty())
                return " ".Replicate(length);

            return (value.Length >= length) ? value : value.PadLeft(length, ' ');
        }

        /// <summary>
        /// <see cref="DbType"/> 을 .NET Language Type으로 매핑합니다.
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns>닷넷 Language Type, 변환 정보가 없으면 typeof(object) 를 반환</returns>
        public static Type GetLanguageType(this DbType dbType) {
            switch(dbType) {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    return typeof(string);

                case DbType.Binary:
                    return typeof(byte[]);

                case DbType.Byte:
                case DbType.SByte:
                    return typeof(byte);

                case DbType.Boolean:
                    return typeof(bool);

                case DbType.Currency:
                case DbType.Decimal:
                    return typeof(decimal);

                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                    return typeof(DateTime);

                case DbType.Double:
                case DbType.VarNumeric:
                    return typeof(double);

                case DbType.Guid:
                    return typeof(Guid);

                case DbType.Int16:
                    return typeof(short);

                case DbType.Int32:
                    return typeof(int);

                case DbType.Int64:
                    return typeof(long);

                case DbType.Object:
                    return typeof(object);

                case DbType.Single:
                    return typeof(float);

                case DbType.Time:
                    return typeof(TimeSpan);

                case DbType.UInt16:
                    return typeof(System.UInt16);
                case DbType.UInt32:
                    return typeof(System.UInt32);
                case DbType.UInt64:
                    return typeof(System.UInt64);

                case DbType.Xml:
                    return typeof(XmlNode);

                case DbType.DateTimeOffset:
                    return typeof(int);

                default:
                    return typeof(object);
            }
        }

        /// <summary>
        /// 닷넷 Language <see cref="Type"/>을 Database의 <see cref="DbType"/>으로 매핑한다.
        /// </summary>
        /// <param name="languageType"></param>
        /// <returns></returns>
        public static DbType GetDbType(this Type languageType) {
            var typeCode = Type.GetTypeCode(languageType);
            switch(typeCode) {
                case TypeCode.Boolean:
                    return DbType.Boolean;

                case TypeCode.Byte:
                case TypeCode.Char:
                    return DbType.Byte;

                case TypeCode.DBNull:
                    return DbType.Object;

                case TypeCode.DateTime:
                    return DbType.DateTime;

                case TypeCode.Decimal:
                    return DbType.Decimal;

                case TypeCode.Double:
                    return DbType.Double;

                case TypeCode.Empty:
                    return DbType.Object;

                case TypeCode.Int16:
                    return DbType.Int16;

                case TypeCode.Int32:
                    return DbType.Int32;

                case TypeCode.Int64:
                    return DbType.Int64;

                case TypeCode.Object:
                    return DbType.Object;

                case TypeCode.SByte:
                    return DbType.SByte;

                case TypeCode.Single:
                    return DbType.Single;

                case TypeCode.String:
                    return DbType.String;

                case TypeCode.UInt16:
                    return DbType.UInt16;

                case TypeCode.UInt32:
                    return DbType.UInt32;

                case TypeCode.UInt64:
                    return DbType.UInt64;
            }
            switch(languageType.Name.ToLower()) {
                case "guid":
                    return DbType.Guid;
                case "string":
                    return DbType.String;
                case "integer":
                case "int32":
                case "int":
                    return DbType.Int32;
                case "decimal":
                    return DbType.Decimal;
                case "byte[]":
                    return DbType.Binary;
                case "object":
                    return DbType.Object;
                case "byte":
                    return DbType.Byte;
                case "bool":
                    return DbType.Boolean;
                case "datetime":
                    return DbType.DateTime;
                case "timespan":
                    return DbType.Time;
                case "double":
                    return DbType.Double;
                case "short":
                case "Int16":
                    return DbType.Int16;
                case "long":
                case "int64":
                    return DbType.Int64;
                case "float":
                    return DbType.Single;

                default:
                    return DbType.Object;
            }
        }

        /// <summary>
        /// 지정된 <see cref="DbType"/>이 닷넷의 <see cref="String"/> 형식인지 검사한다.
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static bool IsStringType(this DbType dbType) {
            return GetLanguageType(dbType).IsAssignableFrom(typeof(string));
        }

        /// <summary>
        /// 지정된 레코드의 해당 컬럼의 값을 문자열로 반환한다. 값이 없을 때에는 빈 문자열을 반환한다.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public static string CellToString(this DataRow row, string column) {
            return row[column].AsText();
        }

        /// <summary>
        /// 지정된 값을 지정된 <see cref="DbType"/>과 같은 형식으로 변환한다.
        /// </summary>
        /// <param name="dbType">원하는 <see cref="DbType"/></param>
        /// <param name="value">변경할 값</param>
        /// <returns>지정된 <see cref="DbType"/>으로 변경된 값</returns>
        public static object ConvertToDbTypedValue(this DbType dbType, object value) {
            // value 가 null이면 Db형식에 상관없이 NULL값을 반환한다.
            if(Equals(value, null))
                return DBNull.Value;

            dbType.ShouldNotBeNull("dbType");

            try {
                switch(dbType) {
                    case DbType.Time:
                        return value.AsTimeSpanNullable();

                    case DbType.Date:
                    case DbType.DateTime:
                    case DbType.DateTime2:
                        return value.AsDateTimeNullable();

                    case DbType.Int16:
                        return value.AsShortNullable();

                    case DbType.Int32:
                        return value.AsIntNullable();

                    case DbType.Int64:
                        return value.AsLongNullable();

                    case DbType.Decimal:
                        return value.AsDecimalNullable();

                    case DbType.Currency:
                    case DbType.Double:
                        return value.AsDoubleNullable();

                    case DbType.Single:
                        return value.AsFloatNullable();

                    case DbType.Guid:
                        return value.AsGuidNullable();

                    case DbType.Byte:
                        return value.AsByteNullable();

                    case DbType.SByte:
                        return value.AsValue<sbyte>();


                    case DbType.String:
                    case DbType.StringFixedLength:
                        return value.AsText();

                    default:
                        return value;
                }
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("Cannot Parse value to proper DbType=[{0}]", dbType);
                    log.Error(ex);
                }
                throw;
            }
        }

        /// <summary>
        /// 객체가 null 이거나 DBNull.Value이거나, INullable.IsNull 이면 true를 반환한다.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNull(object value) {
            if(value == null)
                return true;

            if(value is INullable && ((INullable)value).IsNull)
                return true;

            if(value == DBNull.Value)
                return true;

            return false;
        }
    }
}