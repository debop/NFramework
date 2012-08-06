using System;
using System.Data.SqlTypes;

namespace NSoft.NFramework.Data.SqlServer {
    /// <summary>
    /// SQL Server의 SQL 수형과 .NET Language 수형간의 변환 함수를 제공합니다.
    /// </summary>
    public static partial class SqlTool {
        /// <summary>
        /// <paramref name="sqlValue"/>의 값을 Nullable 수형으로 반환한다. 실제 값이 Null인 경우는 Null을 반환한다.
        /// </summary>
        public static byte[] ToNullableValue(this SqlBinary sqlValue) {
            if(ConvertTool.IsNullOrDbNull(sqlValue))
                return null;

            return sqlValue.Value;
        }

        /// <summary>
        /// <paramref name="sqlValue"/>의 값을 Nullable 수형으로 반환한다. 실제 값이 Null인 경우는 Null을 반환한다.
        /// </summary>
        public static bool? ToNullableValue(this SqlBoolean sqlValue) {
            if(ConvertTool.IsNullOrDbNull(sqlValue))
                return null;

            return sqlValue.Value;
        }

        /// <summary>
        /// <paramref name="sqlValue"/>의 값을 Nullable 수형으로 반환한다. 실제 값이 Null인 경우는 Null을 반환한다.
        /// </summary>
        public static byte? ToNullableValue(this SqlByte sqlValue) {
            if(ConvertTool.IsNullOrDbNull(sqlValue))
                return null;

            return sqlValue.Value;
        }

        /// <summary>
        /// <paramref name="sqlValue"/>의 값을 Nullable 수형으로 반환한다. 실제 값이 Null인 경우는 Null을 반환한다.
        /// </summary>
        public static short? ToNullableValue(this SqlInt16 sqlValue) {
            if(ConvertTool.IsNullOrDbNull(sqlValue))
                return null;

            return sqlValue.Value;
        }

        /// <summary>
        /// <paramref name="sqlValue"/>의 값을 Nullable 수형으로 반환한다. 실제 값이 Null인 경우는 Null을 반환한다.
        /// </summary>
        public static int? ToNullableValue(this SqlInt32 sqlValue) {
            if(ConvertTool.IsNullOrDbNull(sqlValue))
                return null;

            return sqlValue.Value;
        }

        /// <summary>
        /// <paramref name="sqlValue"/>의 값을 Nullable 수형으로 반환한다. 실제 값이 Null인 경우는 Null을 반환한다.
        /// </summary>
        public static long? ToNullableValue(this SqlInt64 sqlValue) {
            if(ConvertTool.IsNullOrDbNull(sqlValue))
                return null;

            return sqlValue.Value;
        }

        /// <summary>
        /// <paramref name="sqlValue"/>의 값을 Nullable 수형으로 반환한다. 실제 값이 Null인 경우는 Null을 반환한다.
        /// </summary>
        public static decimal? ToNullableValue(this SqlMoney sqlValue) {
            if(ConvertTool.IsNullOrDbNull(sqlValue))
                return null;

            return sqlValue.Value;
        }

        /// <summary>
        /// <paramref name="sqlValue"/>의 값을 Nullable 수형으로 반환한다. 실제 값이 Null인 경우는 Null을 반환한다.
        /// </summary>
        public static float? ToNullableValue(this SqlSingle sqlValue) {
            if(ConvertTool.IsNullOrDbNull(sqlValue))
                return null;

            return sqlValue.Value;
        }

        /// <summary>
        /// <paramref name="sqlValue"/>의 값을 Nullable 수형으로 반환한다. 실제 값이 Null인 경우는 Null을 반환한다.
        /// </summary>
        public static double? ToNullableValue(this SqlDouble sqlValue) {
            if(ConvertTool.IsNullOrDbNull(sqlValue))
                return null;

            return sqlValue.Value;
        }

        /// <summary>
        /// <paramref name="sqlValue"/>의 값을 Nullable 수형으로 반환한다. 실제 값이 Null인 경우는 Null을 반환한다.
        /// </summary>
        public static string ToNullableValue(this SqlString sqlValue) {
            if(ConvertTool.IsNullOrDbNull(sqlValue))
                return null;

            return sqlValue.Value;
        }

        /// <summary>
        /// <paramref name="sqlValue"/>의 값을 Nullable 수형으로 반환한다. 실제 값이 Null인 경우는 Null을 반환한다.
        /// </summary>
        public static DateTime? ToNullableValue(this SqlDateTime sqlValue) {
            if(ConvertTool.IsNullOrDbNull(sqlValue))
                return null;

            return sqlValue.Value;
        }

        /// <summary>
        /// <paramref name="sqlValue"/>의 값을 Nullable 수형으로 반환한다. 실제 값이 Null인 경우는 Null을 반환한다.
        /// </summary>
        public static Guid? ToNullableValue(this SqlGuid sqlValue) {
            if(ConvertTool.IsNullOrDbNull(sqlValue))
                return null;

            return sqlValue.Value;
        }

        /// <summary>
        /// <paramref name="nullableValue"/>를 해당하는 Sql 수형으로 변환합니다. null 값인 경우 기본 클래스를 반환합니다.
        /// </summary>
        public static SqlBinary FromNullableValue(this byte[] nullableValue) {
            return (nullableValue != null) ? new SqlBinary(nullableValue) : new SqlBinary();
        }

        /// <summary>
        /// <paramref name="nullableValue"/>를 해당하는 Sql 수형으로 변환합니다. null 값인 경우 기본 클래스를 반환합니다.
        /// </summary>
        public static SqlBoolean FromNullableValue(this bool? nullableValue) {
            return (nullableValue.HasValue) ? new SqlBoolean(nullableValue.Value) : new SqlBoolean();
        }

        /// <summary>
        /// <paramref name="nullableValue"/>를 해당하는 Sql 수형으로 변환합니다. null 값인 경우 기본 클래스를 반환합니다.
        /// </summary>
        public static SqlByte FromNullableValue(this byte? nullableValue) {
            return (nullableValue.HasValue) ? new SqlByte(nullableValue.Value) : new SqlByte();
        }

        /// <summary>
        /// <paramref name="nullableValue"/>를 해당하는 Sql 수형으로 변환합니다. null 값인 경우 기본 클래스를 반환합니다.
        /// </summary>
        public static SqlInt16 FromNullableValue(this short? nullableValue) {
            return (nullableValue.HasValue) ? new SqlInt16(nullableValue.Value) : new SqlInt16();
        }

        /// <summary>
        /// <paramref name="nullableValue"/>를 해당하는 Sql 수형으로 변환합니다. null 값인 경우 기본 클래스를 반환합니다.
        /// </summary>
        public static SqlInt32 FromNullableValue(this int? nullableValue) {
            return (nullableValue.HasValue) ? new SqlInt32(nullableValue.Value) : new SqlInt32();
        }

        /// <summary>
        /// <paramref name="nullableValue"/>를 해당하는 Sql 수형으로 변환합니다. null 값인 경우 기본 클래스를 반환합니다.
        /// </summary>
        public static SqlInt64 FromNullableValue(this long? nullableValue) {
            return (nullableValue.HasValue) ? new SqlInt64(nullableValue.Value) : new SqlInt64();
        }

        /// <summary>
        /// <paramref name="nullableValue"/>를 해당하는 Sql 수형으로 변환합니다. null 값인 경우 기본 클래스를 반환합니다.
        /// </summary>
        public static SqlMoney FromNullableValue(this decimal? nullableValue) {
            return (nullableValue.HasValue) ? new SqlMoney(nullableValue.Value) : new SqlMoney();
        }

        /// <summary>
        /// <paramref name="nullableValue"/>를 해당하는 Sql 수형으로 변환합니다. null 값인 경우 기본 클래스를 반환합니다.
        /// </summary>
        public static SqlSingle FromNullableValue(this float? nullableValue) {
            return (nullableValue.HasValue) ? new SqlSingle(nullableValue.Value) : new SqlSingle();
        }

        /// <summary>
        /// <paramref name="nullableValue"/>를 해당하는 Sql 수형으로 변환합니다. null 값인 경우 기본 클래스를 반환합니다.
        /// </summary>
        public static SqlDouble FromNullableValue(this double? nullableValue) {
            return (nullableValue.HasValue) ? new SqlDouble(nullableValue.Value) : new SqlDouble();
        }

        /// <summary>
        /// <paramref name="nullableValue"/>를 해당하는 Sql 수형으로 변환합니다. null 값인 경우 기본 클래스를 반환합니다.
        /// </summary>
        public static SqlString FromNullableValue(this string nullableValue) {
            return (nullableValue != null) ? new SqlString(nullableValue) : new SqlString();
        }

        /// <summary>
        /// <paramref name="nullableValue"/>를 해당하는 Sql 수형으로 변환합니다. null 값인 경우 기본 클래스를 반환합니다.
        /// </summary>
        public static SqlDateTime FromNullableValue(this DateTime? nullableValue) {
            return (nullableValue.HasValue) ? new SqlDateTime(nullableValue.Value) : new SqlDateTime();
        }

        /// <summary>
        /// <paramref name="nullableValue"/>를 해당하는 Sql 수형으로 변환합니다. null 값인 경우 기본 클래스를 반환합니다.
        /// </summary>
        public static SqlGuid FromNullableValue(this Guid? nullableValue) {
            return (nullableValue.HasValue) ? new SqlGuid(nullableValue.Value) : new SqlGuid();
        }
    }
}