using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// <see cref="IDataReader"/>의 확장 메소드입니다. (<see cref="AdoDataReader"/> 보다 여기 있는 확장 메소드를 사용하시기 바랍니다. AsValue{T} 형식으로요^^
    /// </summary>
    public static class DataReaderTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        /// <summary>
        /// IDataReader의 필드명 컬렉션을 가져옵니다.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static IList<string> GetFieldNames(this IDataReader reader) {
            return Enumerable.Range(0, reader.FieldCount).Select(i => reader.GetName(i)).ToList();
        }

        /// <summary>
        /// IDataReader의 컬럼명의 값을 가져옵니다. 없으면 <paramref name="valueFactory"/>의 반환값을 반환합니다.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public static object AsValue(this IDataReader reader, string columnName, Func<object> valueFactory = null) {
            return AsValue(reader, reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// IDataReader의 컬럼명의 값을 가져옵니다. 없으면 <paramref name="valueFactory"/>의 반환값을 반환합니다.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="index"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public static object AsValue(this IDataReader reader, int index, Func<object> valueFactory = null) {
            valueFactory = valueFactory ?? (() => null);
            return (reader.IsDBNull(index) == false) ? reader.GetValue(index) : valueFactory();
        }

        /// <summary>
        /// IDataReader의 컬럼명의 값을 가져옵니다. 없으면 <paramref name="valueFactory"/>의 반환값을 반환합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public static T AsValue<T>(this IDataReader reader, string columnName, Func<T> valueFactory = null) {
            valueFactory = valueFactory ?? (() => default(T));
            return AsValue<T>(reader, reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// IDataReader의 컬럼명의 값을 가져옵니다. 없으면 <paramref name="valueFactory"/>의 반환값을 반환합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="index"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public static T AsValue<T>(this IDataReader reader, int index, Func<T> valueFactory = null) {
            valueFactory = valueFactory ?? (() => default(T));

            return
                With.TryFunction(() => reader.IsDBNull(index) ? valueFactory() : ConvertTool.AsValue<T>(reader.GetValue(index)),
                                 exceptionAction:
                                     ex => {
                                         if(IsInfoEnabled) {
                                             log.Info("IDataReader로부터 값을 얻는데 실패했습니다. index=[{0}]", index);
                                             log.Info(ex);
                                         }
                                     });
        }

        /// <summary>
        /// IDataReader의 컬럼명의 값을 가져옵니다. 없으면 <paramref name="valueFactory"/>의 반환값을 반환합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public static T? AsValueNullable<T>(this IDataReader reader, string columnName, Func<T?> valueFactory = null) where T : struct {
            valueFactory = valueFactory ?? (() => default(T?));
            return AsValueNullable<T>(reader, reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// IDataReader의 컬럼명의 값을 가져옵니다. 없으면 <paramref name="valueFactory"/>의 반환값을 반환합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="index"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public static T? AsValueNullable<T>(this IDataReader reader, int index, Func<T?> valueFactory = null) where T : struct {
            valueFactory = valueFactory ?? (() => default(T?));
            return
                With.TryFunction(
                    () => reader.IsDBNull(index) ? null : ConvertTool.AsValueNullable<T>(reader.GetValue(index), valueFactory),
                    exceptionAction:
                        ex => {
                            if(IsInfoEnabled) {
                                log.Info("IDataReader로부터 값을 얻는데 실패했습니다. index=[{0}]", index);
                                log.Info(ex);
                            }
                        });
        }

        /// <summary>
        /// IDataReader의 컬럼명의 값을 문자열로 반환합니다.없으면 <paramref name="valueFactory"/>의 반환값을 반환합니다.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public static string AsString(this IDataReader reader, string columnName, Func<string> valueFactory = null) {
            return AsString(reader, reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// IDataReader의 컬럼명의 값을 문자열로 반환합니다.없으면 <paramref name="valueFactory"/>의 반환값을 반환합니다.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="index"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public static string AsString(this IDataReader reader, int index, Func<string> valueFactory = null) {
            valueFactory = valueFactory ?? (() => string.Empty);
            return reader.AsValue<string>(index, valueFactory);
        }

        /// <summary>
        /// Boolean 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">Column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, default(bool) 값을 반환한다.</returns>
        public static bool AsBool(this IDataReader reader, int index, Func<bool> valueFactory = null) {
            return reader.AsValue<bool>(index, valueFactory);
        }

        /// <summary>
        /// Boolean 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">Column Name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value</returns>
        public static bool AsBool(this IDataReader reader, string columnName, Func<bool> valueFactory = null) {
            return reader.AsValue<bool>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// Boolean 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">Column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, null 값을 반환한다.</returns>
        public static bool? AsBoolNullable(this IDataReader reader, int index, Func<bool?> valueFactory = null) {
            return reader.AsValueNullable<bool>(index, valueFactory);
        }

        /// <summary>
        /// Boolean 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">Column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, null 값을 반환한다.</returns>
        public static bool? AsBoolNullable(this IDataReader reader, string columnName, Func<bool?> valueFactory = null) {
            return reader.AsValueNullable<bool>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// System.Byte 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">Column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, default(byte) 값을 반환한다.</returns>
        public static byte AsByte(this IDataReader reader, int index, Func<byte> valueFactory = null) {
            return reader.AsValue<byte>(index, valueFactory);
        }

        /// <summary>
        /// System.Byte 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">Column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, default(byte) 값을 반환한다.</returns>
        public static byte AsByte(this IDataReader reader, string columnName, Func<byte> valueFactory = null) {
            return reader.AsValue<byte>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// System.Byte 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">Column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, null 값을 반환한다.</returns>
        public static byte? AsByteNullable(this IDataReader reader, int index, Func<byte?> valueFactory = null) {
            return reader.AsValueNullable<byte>(index, valueFactory);
        }

        /// <summary>
        /// System.Byte 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">Column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, null 값을 반환한다.</returns>
        public static byte? AsByteNullable(this IDataReader reader, string columnName, Func<byte?> valueFactory = null) {
            return reader.AsValueNullable<byte>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// 이진 데이타를 읽어서 버퍼에 저장한다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnIndex">Column index</param>
        /// <param name="fieldOffset">offset of column data</param>
        /// <param name="buffer">buffer to store data.</param>
        /// <param name="bufferoffset">buffer offset</param>
        /// <param name="length">length to retrieve</param>
        /// <returns>length to be retrieved</returns>
        public static long AsBytes(this IDataReader reader, int columnIndex, long fieldOffset, byte[] buffer, int bufferoffset,
                                   int length) {
            return reader.IsDBNull(columnIndex) ? 0 : reader.GetBytes(columnIndex, fieldOffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// 이진 데이타를 읽어서 버퍼에 저장한다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">Column name</param>
        /// <param name="fieldOffset">offset of column data</param>
        /// <param name="buffer">buffer to store data.</param>
        /// <param name="bufferoffset">buffer offset</param>
        /// <param name="length">length to retrieve</param>
        /// <returns>length to be retrieved</returns>
        public static long AsBytes(this IDataReader reader, string columnName, long fieldOffset, byte[] buffer, int bufferoffset,
                                   int length) {
            return reader.AsBytes(reader.GetOrdinal(columnName), fieldOffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// System.Char 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">Column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, default(char) 값을 반환한다.</returns>
        public static char AsChar(this IDataReader reader, int index, Func<char> valueFactory = null) {
            return reader.AsValue<char>(index, valueFactory);
        }

        /// <summary>
        /// System.Char 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">Column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, default(char) 값을 반환한다.</returns>
        public static char AsChar(this IDataReader reader, string columnName, Func<char> valueFactory = null) {
            return reader.AsValue<char>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// System.Char 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">Column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, null 값을 반환한다.</returns>
        public static char? AsCharNullable(this IDataReader reader, int index, Func<char?> valueFactory = null) {
            return reader.AsValueNullable<char>(index, valueFactory);
        }

        /// <summary>
        /// System.Char 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">Column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, null 값을 반환한다.</returns>
        public static char? AsCharNullable(this IDataReader reader, string columnName, Func<char?> valueFactory = null) {
            return reader.AsValueNullable<char>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// 이진 데이타를 읽어서 버퍼에 저장한다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnIndex">Column index</param>
        /// <param name="fieldOffset">offset of column data</param>
        /// <param name="buffer">buffer to store data.</param>
        /// <param name="bufferoffset">buffer offset</param>
        /// <param name="length">length to retrieve</param>
        /// <returns>length to be retrieved</returns>
        public static long AsChars(this IDataReader reader, int columnIndex, long fieldOffset, char[] buffer, int bufferoffset,
                                   int length) {
            return reader.IsDBNull(columnIndex) ? 0 : reader.GetChars(columnIndex, fieldOffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// 이진 데이타를 읽어서 버퍼에 저장한다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">Column name</param>
        /// <param name="fieldOffset">offset of column data</param>
        /// <param name="buffer">buffer to store data.</param>
        /// <param name="bufferoffset">buffer offset</param>
        /// <param name="length">length to retrieve</param>
        /// <returns>length to be retrieved</returns>
        public static long AsChars(this IDataReader reader, string columnName, long fieldOffset, char[] buffer, int bufferoffset,
                                   int length) {
            return reader.AsChars(reader.GetOrdinal(columnName), fieldOffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// DateTime 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">Column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, default(char) 값을 반환한다.</returns>
        public static DateTime AsDateTime(this IDataReader reader, int index, Func<DateTime> valueFactory = null) {
            return reader.AsValue<DateTime>(index, valueFactory);
        }

        /// <summary>
        /// DateTime 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">Column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, default(char) 값을 반환한다.</returns>
        public static DateTime AsDateTime(this IDataReader reader, string columnName, Func<DateTime> valueFactory = null) {
            return reader.AsValue<DateTime>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// DateTime 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">Column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, null 값을 반환한다.</returns>
        public static DateTime? AsDateTimeNullable(this IDataReader reader, int index, Func<DateTime?> valueFactory = null) {
            return reader.AsValueNullable<DateTime>(index, valueFactory);
        }

        /// <summary>
        /// DateTime 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">Column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, null 값을 반환한다.</returns>
        public static DateTime? AsDateTimeNullable(this IDataReader reader, string columnName, Func<DateTime?> valueFactory = null) {
            return reader.AsValueNullable<DateTime>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// Decimal 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">Column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, default(char) 값을 반환한다.</returns>
        public static Decimal AsDecimal(this IDataReader reader, int index, Func<Decimal> valueFactory = null) {
            return reader.AsValue<Decimal>(index, valueFactory);
        }

        /// <summary>
        /// Decimal 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">Column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, default(char) 값을 반환한다.</returns>
        public static Decimal AsDecimal(this IDataReader reader, string columnName, Func<Decimal> valueFactory = null) {
            return reader.AsValue<Decimal>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// Decimal 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">Column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, null 값을 반환한다.</returns>
        public static Decimal? AsDecimalNullable(this IDataReader reader, int index, Func<Decimal?> valueFactory = null) {
            return reader.AsValueNullable<Decimal>(index, valueFactory);
        }

        /// <summary>
        /// Decimal 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">Column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, null 값을 반환한다.</returns>
        public static Decimal? AsDecimalNullable(this IDataReader reader, string columnName, Func<Decimal?> valueFactory = null) {
            return reader.AsValueNullable<Decimal>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// Double 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">Column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, default(char) 값을 반환한다.</returns>
        public static double AsDouble(this IDataReader reader, int index, Func<double> valueFactory = null) {
            return reader.AsValue<double>(index, valueFactory);
        }

        /// <summary>
        /// Double 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">Column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, default(char) 값을 반환한다.</returns>
        public static double AsDouble(this IDataReader reader, string columnName, Func<double> valueFactory = null) {
            return reader.AsValue<double>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// Double 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">Column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, null 값을 반환한다.</returns>
        public static double? AsDoubleNullable(this IDataReader reader, int index, Func<double?> valueFactory = null) {
            return reader.AsValueNullable<double>(index, valueFactory);
        }

        /// <summary>
        /// Double 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">Column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, null 값을 반환한다.</returns>
        public static double? AsDoubleNullable(this IDataReader reader, string columnName, Func<double?> valueFactory = null) {
            return reader.AsValueNullable<double>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// Double 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">Column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, default(char) 값을 반환한다.</returns>
        public static float AsFloat(this IDataReader reader, int index, Func<float> valueFactory = null) {
            return reader.AsValue<float>(index, valueFactory);
        }

        /// <summary>
        /// System.Char 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">Column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, default(char) 값을 반환한다.</returns>
        public static float AsFloat(this IDataReader reader, string columnName, Func<float> valueFactory = null) {
            return reader.AsValue<float>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// System.Char 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">Column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, null 값을 반환한다.</returns>
        public static float? AsFloatNullable(this IDataReader reader, int index, Func<float?> valueFactory = null) {
            return reader.AsValueNullable<float>(index, valueFactory);
        }

        /// <summary>
        /// System.Char 형식으로 값을 읽는다.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">Column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>Column value, 값이 DbNull이면, null 값을 반환한다.</returns>
        public static float? AsFloatNullable(this IDataReader reader, string columnName, Func<float?> valueFactory = null) {
            return reader.AsValueNullable<float>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// Get Guid Value
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>if value is DBNull, return Guid.Empty</returns>
        public static Guid AsGuid(this IDataReader reader, int index, Func<Guid> valueFactory = null) {
            return reader.AsValue<Guid>(index, valueFactory ?? (() => Guid.Empty));
        }

        /// <summary>
        /// Get Guid Value
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>if value is DBNull, return Guid.Empty</returns>
        public static Guid AsGuid(this IDataReader reader, string columnName, Func<Guid> valueFactory = null) {
            return reader.AsValue<Guid>(reader.GetOrdinal(columnName), valueFactory ?? (() => Guid.Empty));
        }

        /// <summary>
        /// Get Guid Value
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>if value is DBNull, return null.</returns>
        public static Guid? AsGuidNullable(this IDataReader reader, int index, Func<Guid?> valueFactory = null) {
            return reader.AsValueNullable<Guid>(index, valueFactory);
        }

        /// <summary>
        /// Get Guid Value
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>if value is DBNull, return null</returns>
        public static Guid? AsGuidNullable(this IDataReader reader, string columnName, Func<Guid?> valueFactory = null) {
            return reader.AsValueNullable<Guid>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// Get Int16 Value
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>if value is DBNull, return Guid.Empty</returns>
        public static Int16 AsInt16(this IDataReader reader, int index, Func<Int16> valueFactory = null) {
            return reader.AsValue<Int16>(index, valueFactory);
        }

        /// <summary>
        /// Get Int16 Value
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>if value is DBNull, return Guid.Empty</returns>
        public static Int16 AsInt16(this IDataReader reader, string columnName, Func<Int16> valueFactory = null) {
            return reader.AsValue<Int16>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// Get Int16 Value
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>if value is DBNull, return null.</returns>
        public static Int16? AsInt16Nullable(this IDataReader reader, int index, Func<Int16?> valueFactory = null) {
            return reader.AsValueNullable<Int16>(index, valueFactory);
        }

        /// <summary>
        /// Get Int16 Value
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>if value is DBNull, return null</returns>
        public static Int16? AsInt16Nullable(this IDataReader reader, string columnName, Func<Int16?> valueFactory = null) {
            return reader.AsValueNullable<Int16>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// Get Int32 Value
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>if value is DBNull, return Guid.Empty</returns>
        public static Int32 AsInt32(this IDataReader reader, int index, Func<Int32> valueFactory = null) {
            return reader.AsValue<Int32>(index, valueFactory);
        }

        /// <summary>
        /// Get Int32 Value
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>if value is DBNull, return Guid.Empty</returns>
        public static Int32 AsInt32(this IDataReader reader, string columnName, Func<Int32> valueFactory = null) {
            return reader.AsValue<Int32>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// Get Int32 Value
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>if value is DBNull, return null.</returns>
        public static Int32? AsInt32Nullable(this IDataReader reader, int index, Func<Int32?> valueFactory = null) {
            return reader.AsValueNullable<Int32>(index, valueFactory);
        }

        /// <summary>
        /// Get Int32 Value
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>if value is DBNull, return null</returns>
        public static Int32? AsInt32Nullable(this IDataReader reader, string columnName, Func<Int32?> valueFactory = null) {
            return reader.AsValueNullable<Int32>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// Get Int64 Value
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>if value is DBNull, return Guid.Empty</returns>
        public static Int64 AsInt64(this IDataReader reader, int index, Func<Int64> valueFactory = null) {
            return reader.AsValue<Int64>(index, valueFactory);
        }

        /// <summary>
        /// Get Int64 Value
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>if value is DBNull, return Guid.Empty</returns>
        public static Int64 AsInt64(this IDataReader reader, string columnName, Func<Int64> valueFactory = null) {
            return reader.AsValue<Int64>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// Get Int64 Value
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>if value is DBNull, return null.</returns>
        public static Int64? AsInt64Nullable(this IDataReader reader, int index, Func<Int64?> valueFactory = null) {
            return reader.AsValueNullable<Int64>(index, valueFactory);
        }

        /// <summary>
        /// Get Int64 Value
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>if value is DBNull, return null</returns>
        public static Int64? AsInt64Nullable(this IDataReader reader, string columnName, Func<Int64?> valueFactory = null) {
            return reader.AsValueNullable<Int64>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// Get TimeSpan Value
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>if value is DBNull, return TimeSpan.Zero</returns>
        public static TimeSpan AsTimeSpan(this IDataReader reader, int index, Func<TimeSpan> valueFactory = null) {
            return reader.AsValue<TimeSpan>(index, valueFactory ?? (() => TimeSpan.Zero));
        }

        /// <summary>
        /// Get TimeSpan Value
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>if value is DBNull, return TimeSpan.Zero</returns>
        public static TimeSpan AsTimeSpan(this IDataReader reader, string columnName, Func<TimeSpan> valueFactory = null) {
            return reader.AsValue<TimeSpan>(reader.GetOrdinal(columnName), valueFactory ?? (() => TimeSpan.Zero));
        }

        /// <summary>
        /// Get TimeSpan Value
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="index">column index</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>if value is DBNull, return null.</returns>
        public static TimeSpan? AsTimeSpanNullable(this IDataReader reader, int index, Func<TimeSpan?> valueFactory = null) {
            return reader.AsValueNullable<TimeSpan>(index, valueFactory);
        }

        /// <summary>
        /// Get TimeSpan Value
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">column name</param>
        /// <param name="valueFactory">기본값 설정 함수</param>
        /// <returns>if value is DBNull, return null</returns>
        public static TimeSpan? AsTimeSpanNullable(this IDataReader reader, string columnName, Func<TimeSpan?> valueFactory = null) {
            return reader.AsValueNullable<TimeSpan>(reader.GetOrdinal(columnName), valueFactory);
        }

        /// <summary>
        /// Return whether the specified field is set to null.
        /// </summary>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="columnName">column name</param>
        /// <returns>true if the specified field is set to null; otherwise, false.</returns>
        public static bool IsDBNull(this IDataReader reader, string columnName) {
            return reader.IsDBNull(reader.GetOrdinal(columnName));
        }

        /// <summary>
        /// DataReader가 가진 모든 Record의 내용을 전부 보여준다. (내용을 다 보여준 후에는 더 이상 조작할 수 없다.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 이 함수를 호출하면 더이상 IDataReader를 fetching 할 수 없다.<br/>
        /// Unit Test 시나 Debuging시에만 사용해야 한다.
        /// </remarks>
        /// <param name="reader">IDataReader 인스턴스</param>
        /// <param name="showDetails">내부 내용을 모두 보여줄 것인가?</param>
        public static string ToString(this IDataReader reader, bool showDetails) {
            if(showDetails == false)
                return reader.ToString();

            var sb = new StringBuilder();
            var fieldCount = reader.FieldCount;
            var row = 0;

            while(reader.Read()) {
                sb.AppendFormat("[{0}] ", row++);

                for(var i = 0; i < fieldCount; i++) {
                    if(i > 0)
                        sb.Append("|");
                    sb.AppendFormat("{0}=[{1}]", reader.GetName(i), reader.GetValue(i));
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}