using System;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework {
    public static partial class ConvertTool {
        private static readonly Type DBNullType = typeof(DBNull);
        private static readonly Type DateTimeType = typeof(DateTime);

        /// <summary>
        /// <paramref name="source"/> 인스턴스를 <paramref name="targetType"/> 형식으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="targetType">대상 형식</param>
        /// <returns>대상 형식으로 변환된 객체</returns>
        public static object AsValue(this object source, Type targetType) {
            // DBNull 형식으로 변경 시에는 DBNull.Value 를 반환하고, 그 이외에는 null을 반환한다.
            return AsValue(source, targetType, () => (targetType == DBNullType) ? DBNull.Value : null);
        }

        /// <summary>
        /// <paramref name="source"/> 인스턴스를 <paramref name="targetType"/> 형식으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="targetType">대상 형식</param>
        /// <param name="defaultValue">대상 형식으로 변환 실패 시, 반환하는 기본값</param>
        /// <returns>대상 형식으로 변환된 객체</returns>
        public static object AsValue(this object source, Type targetType, object defaultValue) {
            return AsValue(source, targetType, () => defaultValue);
        }

        /// <summary>
        /// <paramref name="source"/> 인스턴스를 <paramref name="targetType"/> 형식으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="targetType">대상 형식</param>
        /// <param name="defaultValueFactory">대상 형식으로 변환 실패 시, 기본값을 제공하는 함수</param>
        /// <returns>대상 형식으로 변환된 객체</returns>
        public static object AsValue(this object source, Type targetType, Func<object> defaultValueFactory) {
            targetType.ShouldNotBeNull("targetType");
            defaultValueFactory.ShouldNotBeNull("defaultValueFactory");

            if(IsNullOrDbNull(source))
                return defaultValueFactory();

            var sourceType = source.GetType();

            if((sourceType == targetType))
                return source;

            // NOTE: source가 AsString()에 의해 값을 표현해야 하는 수형만 가능합니다.

            var isConverted = false;
            object convertedValue;

            if(sourceType == DateTimeType)
                isConverted = TryConvertValue(source, targetType, out convertedValue);
            else
                isConverted = TryConvertValue(source.ToString(), targetType, out convertedValue);

            if(isConverted) {
                if(IsDebugEnabled)
                    log.Debug("원본 인스턴스 (Value=[{0}], Type=[{1}]), 대상 형식[{2}], 결과 인스턴스 (Value=[{3}], Type=[{4}]) 로 변환 성공!!!",
                              source, sourceType.Name, targetType.Name, convertedValue, convertedValue.GetType());

                return convertedValue;
            }

            if(IsDebugEnabled)
                log.Debug("원본 인스턴스 (Value=[{0}], Type=[{1}]), 대상 형식[{2}]으로 변환하는데 실패했습니다!!! 기본값을 반환합니다.", source, sourceType.FullName,
                          targetType.FullName);

            return defaultValueFactory();
        }

        /// <summary>
        /// <paramref name="source"/>를 T 수형으로 변환합니다.
        /// </summary>
        /// <typeparam name="T">변환할 수형</typeparam>
        /// <param name="source">변환할 객체</param>
        /// <param name="defaultValue">변환 실패시, source가 null 일 경우, 기본값</param>
        /// <returns>변환된 값</returns>
        public static T AsValue<T>(this object source, T defaultValue = default(T)) {
            if(IsNullOrDbNull(source))
                return defaultValue;

            return DefValue<T>(source, defaultValue);
        }

        /// <summary>
        /// <paramref name="source"/>를 T 수형으로 변환합니다.
        /// </summary>
        /// <typeparam name="T">변환할 수형</typeparam>
        /// <param name="source">변환할 객체</param>
        /// <param name="defaultValueFactory">기본값을 제공하는 함수</param>
        /// <returns>변환된 값</returns>
        public static T AsValue<T>(this object source, Func<T> defaultValueFactory) {
            defaultValueFactory.ShouldNotBeNull("defaultFactory");

            if(IsNullOrDbNull(source))
                return defaultValueFactory();

            return DefValue<T>(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/>를 Nullable{T} 수형으로 변환합니다.
        /// </summary>
        /// <typeparam name="T">변환할 수형</typeparam>
        /// <param name="source">변환할 객체</param>
        /// <returns>변환된 값</returns>
        public static T? AsValueNullable<T>(this object source) where T : struct {
            return AsValueNullable<T>(source, () => (T?)null);
        }

        /// <summary>
        /// <paramref name="source"/>를 Nullable{T} 수형으로 변환합니다.
        /// </summary>
        /// <typeparam name="T">변환할 수형</typeparam>
        /// <param name="source">변환할 객체</param>
        /// <param name="defaultValueFactory">기본값 반환 함수</param>
        /// <returns>변환된 값</returns>
        public static T? AsValueNullable<T>(this object source, Func<T?> defaultValueFactory) where T : struct {
            defaultValueFactory.ShouldNotBeNull("defaultValueFactory");

            if(IsNullOrDbNull(source))
                return defaultValueFactory();

            return AsValue<T>(source);
        }

        /// <summary>
        /// <paramref name="source"/>를 enum 값으로 변경합니다.
        /// </summary>
        /// <param name="source">변환할 객체</param>
        /// <param name="defaultValue">변환 실패시 기본 enum 값</param>
        /// <returns>변환된 enum 값</returns>
        public static Enum AsEnum(this object source, Enum defaultValue) {
            return ConvertEnum(source, defaultValue);
        }

        /// <summary>
        /// <paramref name="source"/>를 enum 값으로 변경합니다.
        /// </summary>
        /// <param name="source">변환할 객체</param>
        /// <param name="defaultValue">변환 실패시 기본 enum 값</param>
        /// <returns>변환된 enum 값</returns>
        public static T AsEnum<T>(this object source, T defaultValue) where T : struct, IFormattable {
            return ConvertEnum(source, defaultValue);
        }

        /// <summary>
        /// <paramref name="source"/>를 enum 값으로 변경합니다.
        /// </summary>
        /// <typeparam name="T">enum 수형</typeparam>
        /// <param name="source">변환할 객체</param>
        /// <param name="defaultValue">변환 실패시 기본 enum 값</param>
        /// <returns>변환된 enum 값</returns>
        public static T? AsEnumNullable<T>(this object source, T defaultValue) where T : struct, IFormattable {
            if(source == null)
                return null;

            return (T?)ConvertEnum(source, defaultValue);
        }

        /// <summary>
        /// <paramref name="source"/>를 bool 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValue">변환 실퍠시에 반환할 값</param>
        /// <returns>변환된 boolean 값</returns>
        public static bool AsBool(this object source, bool defaultValue = false) {
            if(IsNullOrDbNull(source) || (source is string && ((string)source).IsWhiteSpace()))
                return defaultValue;

            if(source is string)
                return BooleanTool.AsBoolean((string)source);

            if(source.GetType().IsNumeric())
                return BooleanTool.AsBoolean(source.AsInt());

            return AsValue<bool>(source, defaultValue);
        }

        /// <summary>
        /// <paramref name="source"/>를 bool 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실퍠시에 반환할 값을 생성하는 Factory</param>
        /// <returns>변환된 boolean 값</returns>
        public static bool AsBool(this object source, Func<bool> defaultValueFactory) {
            return AsValue<bool>(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/>를 bool? 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <returns>변환된 boolean? 값</returns>
        public static bool? AsBoolNullable(this object source) {
            return AsValueNullable<bool>(source);
        }

        /// <summary>
        /// <paramref name="source"/>를 bool? 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실퍠시에 반환할 값을 생성하는 Factory</param>
        /// <returns>변환된 boolean? 값</returns>
        public static bool? AsBoolNullable(this object source, Func<bool?> defaultValueFactory) {
            return AsValueNullable<bool>(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/>를 char 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <returns>변환된 char 값</returns>
        public static char AsChar(this object source) {
            return AsValue<char>(source);
        }

        /// <summary>
        /// <paramref name="source"/>를 char 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValue">변환 실퍠시에 반환할 값</param>
        /// <returns>변환된 char 값</returns>
        public static char AsChar(this object source, char defaultValue) {
            return AsValue<char>(source, defaultValue);
        }

        /// <summary>
        /// <paramref name="source"/>를 char 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실퍠시에 반환할 값을 생성하는 Factory</param>
        /// <returns>변환된 char 값</returns>
        public static char AsChar(this object source, Func<char> defaultValueFactory) {
            return AsValue<char>(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/>를 char? 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <returns>변환된 char? 값</returns>
        public static char? AsCharNullable(this object source) {
            return AsValueNullable<char>(source);
        }

        /// <summary>
        /// <paramref name="source"/>를 char? 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실퍠시에 반환할 값을 생성하는 Factory</param>
        /// <returns>변환된 char? 값</returns>
        public static char? AsCharNullable(this object source, Func<char?> defaultValueFactory) {
            return AsValueNullable<char>(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/>를 byte 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <returns>변환된 byte 값</returns>
        public static byte AsByte(this object source) {
            return AsValue<byte>(source);
        }

        /// <summary>
        /// <paramref name="source"/>를 byte 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValue">변환 실퍠시에 반환할 값</param>
        /// <returns>변환된 byte 값</returns>
        public static byte AsByte(this object source, byte defaultValue) {
            return AsValue(source, defaultValue);
        }

        /// <summary>
        /// <paramref name="source"/>를 byte 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실퍠시에 반환할 값을 생성하는 Factory</param>
        /// <returns>변환된 byte 값</returns>
        public static byte AsByte(this object source, Func<byte> defaultValueFactory) {
            return AsValue(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/>를 byte? 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <returns>변환된 byte? 값</returns>
        public static byte? AsByteNullable(this object source) {
            return AsValueNullable<byte>(source);
        }

        /// <summary>
        /// <paramref name="source"/>를 byte? 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실퍠시에 반환할 값을 생성하는 Factory</param>
        /// <returns>변환된 byte? 값</returns>
        public static byte? AsByteNullable(this object source, Func<byte?> defaultValueFactory) {
            return AsValueNullable<byte>(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="Int16"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValue">변환 실패시 반환할 기본값</param>
        /// <returns>변환된 Int16 값</returns>
        public static short AsShort(this object source, short defaultValue = 0) {
            return AsValue<short>(source, defaultValue);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="Int16"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실패시 반환할 기본값 생성자</param>
        /// <returns>변환된 Int16 값</returns>
        public static short AsShort(this object source, Func<short> defaultValueFactory) {
            return AsValue<short>(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="Nullable{Int16}"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <returns>변환된 <see cref="Nullable{Int16}"/> 값</returns>
        public static short? AsShortNullable(this object source) {
            return AsValueNullable<short>(source);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="Nullable{Int16}"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실패시 반환할 기본값 생성자</param>
        /// <returns>변환된 <see cref="Nullable{Int16}"/> 값</returns>
        public static short? AsShortNullable(this object source, Func<short?> defaultValueFactory) {
            return AsValueNullable<short>(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="Int32"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValue">변환 실패시 반환할 기본값</param>
        /// <returns>변환된 Int32 값</returns>
        public static int AsInt(this object source, int defaultValue = 0) {
            return AsValue<int>(source, defaultValue);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="Int32"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실패시 반환할 기본값 생성자</param>
        /// <returns>변환된 Int32 값</returns>
        public static int AsInt(this object source, Func<int> defaultValueFactory) {
            return AsValue<int>(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="Nullable{Int32}"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <returns>변환된 <see cref="Nullable{Int32}"/> 값</returns>
        public static int? AsIntNullable(this object source) {
            return AsValueNullable<int>(source);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="Nullable{Int32}"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실패시 반환할 기본값 생성자</param>
        /// <returns>변환된 <see cref="Nullable{Int32}"/> 값</returns>
        public static int? AsIntNullable(this object source, Func<int?> defaultValueFactory) {
            return AsValueNullable<int>(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="Int64"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValue">변환 실패시 반환할 기본값</param>
        /// <returns>변환된 Int64 값</returns>
        public static long AsLong(this object source, long defaultValue = 0L) {
            return AsValue<long>(source, defaultValue);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="Int64"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실패시 반환할 기본값 생성자</param>
        /// <returns>변환된 Int64 값</returns>
        public static long AsLong(this object source, Func<long> defaultValueFactory) {
            return AsValue<long>(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="Nullable{Int64}"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <returns>변환된 <see cref="Nullable{Int64}"/> 값</returns>
        public static long? AsLongNullable(this object source) {
            return AsValueNullable<long>(source);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="Nullable{Int64}"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실패시 반환할 기본값 생성자</param>
        /// <returns>변환된 <see cref="Nullable{Int64}"/> 값</returns>
        public static long? AsLongNullable(this object source, Func<long?> defaultValueFactory) {
            return AsValueNullable<long>(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="Decimal"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValue">변환 실패시 반환할 기본값</param>
        /// <returns>변환된 Decimal 값</returns>
        public static decimal AsDecimal(this object source, decimal defaultValue = 0m) {
            return AsValue(source, defaultValue);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="Decimal"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실패시 반환할 기본값 생성자</param>
        /// <returns>변환된 Decimal 값</returns>
        public static decimal AsDecimal(this object source, Func<decimal> defaultValueFactory) {
            return AsValue(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="Nullable{Decimal}"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <returns>변환된 <see cref="Nullable{Decimal}"/> 값</returns>
        public static decimal? AsDecimalNullable(this object source) {
            return AsValueNullable<decimal>(source);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="Nullable{Decimal}"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실패시 반환할 기본값 생성자</param>
        /// <returns>변환된 <see cref="Nullable{Decimal}"/> 값</returns>
        public static decimal? AsDecimalNullable(this object source, Func<decimal?> defaultValueFactory) {
            return AsValueNullable<decimal>(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="float"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValue">변환 실패시 반환할 기본값</param>
        /// <returns>변환된 float 값</returns>
        public static float AsFloat(this object source, float defaultValue = 0f) {
            return AsValue(source, defaultValue);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="float"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실패시 반환할 기본값 생성자</param>
        /// <returns>변환된 float 값</returns>
        public static float AsFloat(this object source, Func<float> defaultValueFactory) {
            return AsValue(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="Nullable{Single}"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <returns>변환된 <see cref="Nullable{Single}"/> 값</returns>
        public static float? AsFloatNullable(this object source) {
            return AsValueNullable<float>(source);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="Nullable{Single}"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실패시 반환할 기본값 생성자</param>
        /// <returns>변환된 <see cref="Nullable{Single}"/> 값</returns>
        public static float? AsFloatNullable(this object source, Func<float?> defaultValueFactory) {
            return AsValueNullable<float>(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="double"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValue">변환 실패시 반환할 기본값</param>
        /// <returns>변환된 double 값</returns>
        public static double AsDouble(this object source, double defaultValue = 0d) {
            return AsValue<double>(source, defaultValue);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="double"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실패시 반환할 기본값 생성자</param>
        /// <returns>변환된 double 값</returns>
        public static double AsDouble(this object source, Func<double> defaultValueFactory) {
            return AsValue<double>(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="Nullable{Double}"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <returns>변환된 <see cref="Nullable{Double}"/> 값</returns>
        public static double? AsDoubleNullable(this object source) {
            return AsValueNullable<double>(source);
        }

        /// <summary>
        /// <paramref name="source"/> 의 값을 <see cref="Nullable{Double}"/> 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실패시 반환할 기본값 생성자</param>
        /// <returns>변환된 <see cref="Nullable{Double}"/> 값</returns>
        public static double? AsDoubleNullable(this object source, Func<double?> defaultValueFactory) {
            return AsValueNullable<double>(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/>를 DateTime 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <returns>변환된 DateTime 값</returns>
        public static DateTime AsDateTime(this object source) {
            return AsValue<DateTime>(source, () => DateTime.MinValue);
        }

        /// <summary>
        /// <paramref name="source"/>를 DateTime 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValue">변환 실퍠시에 반환할 값</param>
        /// <returns>변환된 DateTime 값</returns>
        public static DateTime AsDateTime(this object source, DateTime defaultValue) {
            return AsValue(source, defaultValue);
        }

        /// <summary>
        /// <paramref name="source"/>를 DateTime 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실퍠시에 반환할 값을 생성하는 Factory</param>
        /// <returns>변환된 DateTime 값</returns>
        public static DateTime AsDateTime(this object source, Func<DateTime> defaultValueFactory) {
            return AsValue(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/>를 DateTime? 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <returns>변환된 DateTime? 값</returns>
        public static DateTime? AsDateTimeNullable(this object source) {
            return AsValueNullable<DateTime>(source);
        }

        /// <summary>
        /// <paramref name="source"/>를 DateTime? 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실퍠시에 반환할 값을 생성하는 Factory</param>
        /// <returns>변환된 DateTime? 값</returns>
        public static DateTime? AsDateTimeNullable(this object source, Func<DateTime?> defaultValueFactory) {
            return AsValueNullable<DateTime>(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/>를 TimeSpan 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <returns>변환된 TimeSpan 값</returns>
        public static TimeSpan AsTimeSpan(this object source) {
            return AsValue<TimeSpan>(source, () => TimeSpan.Zero);
        }

        /// <summary>
        /// <paramref name="source"/>를 TimeSpan 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValue">변환 실퍠시에 반환할 값</param>
        /// <returns>변환된 TimeSpan 값</returns>
        public static TimeSpan AsTimeSpan(this object source, TimeSpan defaultValue) {
            return AsValue<TimeSpan>(source, defaultValue);
        }

        /// <summary>
        /// <paramref name="source"/>를 TimeSpan 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실퍠시에 반환할 값을 생성하는 Factory</param>
        /// <returns>변환된 TimeSpan 값</returns>
        public static TimeSpan AsTimeSpan(this object source, Func<TimeSpan> defaultValueFactory) {
            return AsValue<TimeSpan>(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/>를 TimeSpan? 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <returns>변환된 TimeSpan? 값</returns>
        public static TimeSpan? AsTimeSpanNullable(this object source) {
            return AsValueNullable<TimeSpan>(source);
        }

        /// <summary>
        /// <paramref name="source"/>를 TimeSpan? 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실퍠시에 반환할 값을 생성하는 Factory</param>
        /// <returns>변환된 TimeSpan? 값</returns>
        public static TimeSpan? AsTimeSpanNullable(this object source, Func<TimeSpan?> defaultValueFactory) {
            return AsValueNullable<TimeSpan>(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/>를 Guid 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <returns>변환된 Guid 값</returns>
        public static Guid AsGuid(this object source) {
            return AsValue<Guid>(source, () => Guid.Empty);
        }

        /// <summary>
        /// <paramref name="source"/>를 Guid 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValue">변환 실퍠시에 반환할 값</param>
        /// <returns>변환된 Guid 값</returns>
        public static Guid AsGuid(this object source, Guid defaultValue) {
            return AsValue<Guid>(source, defaultValue);
        }

        /// <summary>
        /// <paramref name="source"/>를 Guid 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실퍠시에 반환할 값을 생성하는 Factory</param>
        /// <returns>변환된 Guid 값</returns>
        public static Guid AsGuid(this object source, Func<Guid> defaultValueFactory) {
            return AsValue<Guid>(source, defaultValueFactory);
        }

        /// <summary>
        /// <paramref name="source"/>를 Guid? 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <returns>변환된 Guid? 값</returns>
        public static Guid? AsGuidNullable(this object source) {
            return AsValueNullable<Guid>(source);
        }

        /// <summary>
        /// <paramref name="source"/>를 Guid? 값으로 변환합니다.
        /// </summary>
        /// <param name="source">원본 값</param>
        /// <param name="defaultValueFactory">변환 실퍠시에 반환할 값을 생성하는 Factory</param>
        /// <returns>변환된 Guid? 값</returns>
        public static Guid? AsGuidNullable(this object source, Func<Guid?> defaultValueFactory) {
            return AsValueNullable<Guid>(source, defaultValueFactory);
        }

        /// <summary>
        /// 인스턴스를 문자열로 변환합니다. NULL이거나 변환 실패시에는 <paramref name="defaultValue"/>를 반환합니다.
        /// </summary>
        /// <param name="source">인스턴스</param>
        /// <param name="defaultValue">기본 문자열</param>
        /// <returns>인스턴스를 문자열로 변환한 값</returns>
        /// <seealso cref="AsValue(object,System.Type,object)"/>
        public static string AsText(this object source, string defaultValue = "") {
            if(IsNullOrDbNull(source))
                return defaultValue;

            return source.ToString();

            // return AsValue<string>(source, defaultValue);
        }

        /// <summary>
        /// 인스턴스를 문자열로 변환합니다.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static string AsText(this object source, IFormatProvider provider) {
            return IsNullOrDbNull(source) ? string.Empty : source.ToString().ToString(provider);
        }

        /// <summary>
        /// 인스턴스를 문자열로 변환합니다. NULL이거나 변환 실패시에는 <paramref name="defaultValueFactory"/>를 이용하여 기본값을 생성하여 반환합니다.
        /// </summary>
        /// <param name="source">인스턴스</param>
        /// <param name="defaultValueFactory">기본값 생성 함수</param>
        /// <returns>인스턴스를 문자열로 변환한 값</returns>
        /// <seealso cref="AsValue(object,System.Type,System.Func{object})"/>
        /// <seealso cref="AsValue{T}(object,System.Func{T})"/>
        public static string AsText(this object source, Func<string> defaultValueFactory) {
            defaultValueFactory.ShouldNotBeNull("defaultValueFactory");

            if(IsNullOrDbNull(source))
                return defaultValueFactory();

            return source.ToString();

            // return AsValue<string>(source, defaultValueFactory);
        }
    }
}