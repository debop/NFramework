using System;
using System.ComponentModel;
using System.Globalization;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework {
    public static partial class ConvertTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsTraceEnabled = log.IsTraceEnabled;
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string ConvertErrorMsgFormat = "값 변환에 실패했습니다. 기본값을 반환합니다. value=[{0}], defaultValue=[{1}], Return Type=[{2}]";

        /// <summary>
        /// 값을 변환한다. 지정된 value가 null일 경우 defaultValue를 반환한다.
        /// </summary>
        /// <param name="value">추출할 값</param>
        /// <param name="defaultValue">추출을 실패시의 기본값</param>
        /// <returns>추출을 실패시에는 defaultValue를 반환한다.</returns>
        public static object DefValue(object value, object defaultValue) {
            return IsNullOrDbNull(value) ? defaultValue : value;
        }

        /// <summary>
        /// Generic을 이용하여 값 변환을 사용한다.
        /// </summary>
        /// <remarks>
        /// 변환 실패시 혹은 값이 null인 경우 지정된 defaultValue를 반환한다.
        /// </remarks>
        /// <typeparam name="T">추출할 값의 수형</typeparam>
        /// <param name="value">추출할 값</param>
        /// <param name="defaultValue">추출 실패시의 기본값</param>
        /// <param name="useCurrentCulture">현재 AppDomain의 CultureInfo를 사용할 것인가.</param>
        /// <returns></returns>
        public static T DefValue<T>(object value, T defaultValue = default(T), bool useCurrentCulture = false) {
            // value 가 null이면, 기본값 반환
            if(IsNullOrDbNull(value))
                return defaultValue;

            // value가 빈 문자열로 대체된다면, 기본값으로 반환
            //
            if(value.ToString().IsEmpty()) {
                if(typeof(T) == typeof(string)) {
                    object result = string.Empty;
                    return (T)result;
                }
                return defaultValue;
            }


            return DefValue(value, () => defaultValue, useCurrentCulture);
        }

        /// <summary>
        /// <paramref name="value"/>를 <typeparamref name="T"/> 수형으로 변환합니다. 실패 시에는 <paramref name="defaultValueFactory"/>의 값을 반환합니다.
        /// </summary>
        /// <typeparam name="T">추출할 값의 수형</typeparam>
        /// <param name="value">추출할 값</param>
        /// <param name="defaultValueFactory">추출 실패 시의 기본을 제공하는 Factory</param>
        /// <param name="useCurrentCulture">현재 AppDomain의 CultureInfo를 사용할 것인가.</param>
        public static T DefValue<T>(object value, Func<T> defaultValueFactory, bool useCurrentCulture = false) {
            defaultValueFactory.ShouldNotBeNull("defaultValueFactory");

            // value 가 null이면, 기본값 반환
            if(IsNullOrDbNull(value))
                return defaultValueFactory();

            // value가 빈 문자열로 대체된다면, 기본값으로 반환
            if(value.ToString().IsEmpty())
                return defaultValueFactory();

            // NOTE: TryParse가 가능한 수형은 따로 실행한다.
            T result;

            if(TryParse<T>(value, out result))
                return result;

            try {
                var converted = ConvertValue(value, typeof(T), true);
                return (T)converted;

                // BUG: Nullable 수형에 대해서는 항상 False를 반환해서 defaultValue 값이 반환된다.
                // return IsSameOrSubclassOf(converted.GetType(), typeof(T)) ? (T)converted : defaultValue;
            }
            catch(Exception ex) {
                var defaultValue = defaultValueFactory();

                if(log.IsWarnEnabled)
                    log.WarnException(string.Format(ConvertErrorMsgFormat, value, defaultValue, typeof(T)), ex);

                return defaultValue;
            }
        }

        /// <summary>
        /// <paramref name="value"/>를 <typeparamref name="T"/> 수형으로 변환을 시도합니다.
        /// </summary>
        /// <typeparam name="T">변환 대상 수형</typeparam>
        /// <param name="value">변환 하려는 값</param>
        /// <param name="parsedValue">변환 결과 값</param>
        /// <returns>변환 성공 여부</returns>
        public static bool TryParse<T>(object value, out T parsedValue) {
            parsedValue = default(T);
            object result;

            var isParsed = TryParse(value, typeof(T), out result);

            if(isParsed)
                parsedValue = (T)result;

            return isParsed;
        }

        /// <summary>
        /// <paramref name="value"/>를 <paramref name="targetType"/> 수형으로 변환을 시도합니다.
        /// </summary>
        /// <param name="value">변환 하려는 값</param>
        /// <param name="targetType">변환 대상 수형</param>
        /// <param name="parsedValue">변환 결과 값</param>
        /// <returns>변환 성공 여부</returns>
        public static bool TryParse(object value, Type targetType, out object parsedValue) {
            if(IsTraceEnabled)
                log.Trace("값을 파싱합니다. value=[{0}], targetType=[{1}]", value, targetType);

            bool isParsed = false;
            parsedValue = null;

            if(targetType == typeof(string)) {
                parsedValue = (value != null) ? value.ToString() : string.Empty;
                isParsed = true;
            }

            if(targetType == typeof(char)) {
                char result;
                isParsed = char.TryParse(value.ToString(), out result);
                if(isParsed)
                    parsedValue = result;
            }
            if(targetType == typeof(byte)) {
                byte result;
                isParsed = byte.TryParse(value.ToString(), out result);
                if(isParsed)
                    parsedValue = result;
            }

            if(targetType == typeof(int)) {
                int result;
                isParsed = int.TryParse(value.ToString(), out result);
                if(isParsed)
                    parsedValue = result;
            }
            else if(targetType == typeof(long)) {
                long result;
                isParsed = long.TryParse(value.ToString(), out result);
                if(isParsed)
                    parsedValue = result;
            }
            else if(targetType == typeof(short)) {
                short result;
                isParsed = short.TryParse(value.ToString(), out result);
                if(isParsed)
                    parsedValue = result;
            }
            else if(targetType == typeof(float)) {
                float result;
                isParsed = float.TryParse(value.ToString(), out result);
                if(isParsed)
                    parsedValue = result;
            }
            else if(targetType == typeof(double)) {
                double result;
                isParsed = double.TryParse(value.ToString(), out result);
                if(isParsed)
                    parsedValue = result;
            }
            else if(targetType == typeof(decimal)) {
                decimal result;
                isParsed = decimal.TryParse(value.ToString(), out result);
                if(isParsed)
                    parsedValue = result;
            }
            else if(targetType == typeof(DateTime)) {
                DateTime result;
                isParsed = DateTime.TryParse(value.ToString(), out result);
                if(isParsed)
                    parsedValue = result;
            }
            else if(targetType == typeof(TimeSpan)) {
                TimeSpan result;
                isParsed = TimeSpan.TryParse(value.ToString(), out result);
                if(isParsed)
                    parsedValue = result;
            }
            else if(targetType == typeof(Guid)) {
                Guid result;
                isParsed = Guid.TryParse(value.ToString(), out result);
                if(isParsed)
                    parsedValue = result;
            }

            return isParsed;
        }

        /// <summary>
        /// 개체를 지정된 형식으로 변환한다.
        /// </summary>
        /// <param name="value">변환될 값</param>
        /// <param name="targetType">변환할 형식</param>
        /// <param name="useCurrentCulture">현재의 CultureInfo를 사용할 것인가? InvariantCulture 정보를 이용할 것인가 여부</param>
        /// <returns></returns>
        public static object ConvertValue(object value, Type targetType, bool useCurrentCulture = false) {
            var culture = (useCurrentCulture)
                              ? CultureInfo.CurrentCulture
                              : CultureInfo.InvariantCulture;

            object converted;
            return TryConvertValue(value, targetType, culture, out converted) ? converted : value;
        }

        /// <summary>
        /// 개체를 지정된 형식으로 변환한다.
        /// </summary>
        /// <param name="value">변환될 값</param>
        /// <param name="targetType">변환할 형식</param>
        /// <param name="culture">변환시 사용할 CultureInfo 객체</param>
        /// <returns>실패시 null 반환</returns>
        public static object ConvertValue(object value, Type targetType, CultureInfo culture) {
            object converted;
            return TryConvertValue(value, targetType, culture, out converted) ? converted : value;
        }

        /// <summary>
        /// 개체를 지정된 형식으로 변환한다.
        /// </summary>
        /// <param name="value">변환될 값</param>
        /// <param name="targetType">변환할 형식</param>
        /// <param name="converted">변환 여부</param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static bool TryConvertValue(object value, Type targetType, out object converted) {
            return TryConvertValue(value, targetType, false, out converted);
        }

        /// <summary>
        /// 개체를 지정된 형식으로 변환한다.
        /// </summary>
        /// <param name="value">변환될 값</param>
        /// <param name="targetType">변환할 형식</param>
        /// <param name="useCurrentCulture">현재의 CultureInfo를 사용할 것인가? InvariantCulture 정보를 이용할 것인가 여부</param>
        /// <param name="converted">변환 여부</param>
        /// <returns></returns>
        public static bool TryConvertValue(object value, Type targetType, bool useCurrentCulture, out object converted) {
            var culture = (useCurrentCulture)
                              ? CultureInfo.CurrentCulture
                              : CultureInfo.InvariantCulture;

            return TryConvertValue(value, targetType, culture, out converted);
        }

        /// <summary>
        /// 개체를 지정된 형식으로 변환한다.
        /// </summary>
        /// <param name="value">변환될 값</param>
        /// <param name="targetType">변환할 형식</param>
        /// <param name="culture">변환시 사용할 CultureInfo 객체</param>
        /// <param name="converted">변환 여부</param>
        /// <returns>실패시 null 반환</returns>
        [CLSCompliant(false)]
        public static bool TryConvertValue(object value, Type targetType, CultureInfo culture, out object converted) {
            targetType.ShouldNotBeNull("targetType");

            if(IsTraceEnabled)
                log.Trace("값의 형식을 변환하려고합니다.. value=[{0}], targetType=[{1}], culture=[{2}]", value, targetType, culture);

            converted = value;

            if(IsNullOrDbNull(value))
                return false;

            var valueType = value.GetType();

            // 변환할 필요없다.
            if(valueType.Equals(targetType))
                return false;

            if(TryParse(value, targetType, out converted))
                return true;

            try {
                // var isConverted = true;

                var converter = TypeDescriptor.GetConverter(targetType);

                // 지정된 형식이 지정된 값을 변환 못할 때
                if(converter.CanConvertFrom(valueType) == false) {
                    var valueTypeConverter = TypeDescriptor.GetConverter(valueType);

                    // 지정된 값을 지정된 형식으로 변환 가능할 때
                    if(valueTypeConverter.CanConvertTo(targetType)) {
                        converted = valueTypeConverter.ConvertTo(null, culture, value, targetType);
                        return true;
                    }
                    return false;
                }

                converted = converter.ConvertFrom(null, culture, value);
                return true;
            }
            catch(Exception ex) {
                if(IsDebugEnabled) {
                    log.Debug("값을 원하는 형식으로 변환하는데 실패했습니다. 기존 값을 그대로 반환합니다. value=[{0}], targetType=[{1}]", value, targetType);
                    log.Debug(ex);
                }
                converted = value;
                return false;
            }
        }

        /// <summary>
        /// 지정된 값을 원하는 포맷형식으로 문자열을 만든다.
        /// </summary>
        /// <param name="value">구성할 값</param>
        /// <param name="format">포맷</param>
        /// <param name="provider">포맷 프로바이더</param>
        /// <returns>문자열</returns>
        public static string ToString(object value, string format, IFormatProvider provider) {
            if(IsNullOrDbNull(value))
                return null;

            provider = provider ?? CultureInfo.CurrentCulture;

            if(value is IFormattable)
                return ((IFormattable)value).ToString(format, provider);

            if(!(provider is CultureInfo))
                provider = CultureInfo.CurrentCulture;

            return ConvertValue(value, typeof(string), provider as CultureInfo) as string;
        }

        /// <summary>
        /// 지정된 값을 알맞는 enum 값으로 변환한다.
        /// 닷넷에서의 enum 값은 기본적으로 문자열로 저장가능하므로 다시 읽어서, 실제 값으로 변환할 때 사용한다.
        /// </summary>
        /// <param name="value">실제 enum 값</param>
        /// <param name="defaultValue">실패시의 기본 값</param>
        /// <returns>변환된 enumeration instance.</returns>
        public static Enum ConvertEnum(object value, Enum defaultValue) {
            return ConvertEnum(value, () => defaultValue);
        }

        /// <summary>
        /// 지정된 값을 알맞는 enum 값으로 변환한다.
        /// 닷넷에서의 enum 값은 기본적으로 문자열로 저장가능하므로 다시 읽어서, 실제 값으로 변환할 때 사용한다.
        /// </summary>
        /// <param name="value">실제 enum 값</param>
        /// <param name="defaultValueFactory">변환 실패시의 기본 값을 생성해주는 Factory</param>
        /// <returns>변환된 enumeration instance.</returns>
        public static Enum ConvertEnum(object value, Func<Enum> defaultValueFactory) {
            defaultValueFactory.ShouldNotBeNull("defaultValueFactory");

            if(IsNullOrDbNull(value))
                return defaultValueFactory();

            var defaultValue = defaultValueFactory();
            try {
                var valueType = value.GetType();
                var enumType = defaultValue.GetType();

                if(valueType.IsEnum && valueType != enumType)
                    return defaultValue;

                var converted = Equals(valueType, enumType) ? value : ConvertValue(value, enumType);

                // 변환된 형식이 지정된 형식에 정의되어 있으면 그 값을 반환한다.
                //
                if(Enum.IsDefined(enumType, converted))
                    return (Enum)Enum.ToObject(enumType, converted);

                // Enum 값이 Flags Attribute 속성을 가진다면, IsDefined에서 걸러내지 못하므로
                //
                var attr = enumType.GetCustomAttributes(typeof(FlagsAttribute), false);
                if(attr.Length > 0 && attr[0] is FlagsAttribute)
                    return (Enum)Enum.ToObject(enumType, converted);

                return defaultValue;
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled)
                    log.WarnException(string.Format(ConvertErrorMsgFormat, value, defaultValue, defaultValue.GetType()), ex);

                return defaultValue;
            }
        }

        /// <summary>
        /// 지정된 값을 알맞는 enum 값으로 변환한다.
        /// 닷넷에서의 enum 값은 기본적으로 문자열로 저장가능하므로 다시 읽어서, 실제 값으로 변환할 때 사용한다.
        /// </summary>
        /// <param name="value">실제 enum 값</param>
        /// <param name="defaultValue">실패시의 기본 값</param>
        /// <returns>변환된 enumeration instance.</returns>
        public static T ConvertEnum<T>(object value, T defaultValue) {
            return ConvertEnum<T>(value, () => defaultValue);
        }

        /// <summary>
        /// 지정된 값을 알맞는 enum 값으로 변환한다.
        /// 닷넷에서의 enum 값은 기본적으로 문자열로 저장가능하므로 다시 읽어서, 실제 값으로 변환할 때 사용한다.
        /// </summary>
        /// <param name="value">실제 enum 값</param>
        /// <param name="defaultValueFactory">변환 실패시의 기본 값을 생성해주는 Factory</param>
        /// <returns>변환된 enumeration instance.</returns>
        public static T ConvertEnum<T>(object value, Func<T> defaultValueFactory) {
            defaultValueFactory.ShouldNotBeNull("defaultValueFactory");

            if(IsDebugEnabled)
                log.Debug("객체를 enum 수형으로 변환하려고 합니다... value=[{0}], enum type=[{1}]", value, typeof(T).Name);

            if(IsNullOrDbNull(value))
                return defaultValueFactory();

            try {
                var valueType = value.GetType();
                var enumType = typeof(T);

                if(valueType.IsEnum && valueType != enumType)
                    return defaultValueFactory();

                var converted = Equals(valueType, enumType) ? value : ConvertValue(value, enumType, false);

                // 변환된 형식이 지정된 형식에 정의되어 있으면 그 값을 반환한다.
                //
                if(Enum.IsDefined(enumType, converted))
                    return (T)Enum.ToObject(enumType, converted);

                // Enum 값이 Flags Attribute 속성을 가진다면, IsDefined에서 걸러내지 못하므로
                //
                var attr = enumType.GetCustomAttributes(typeof(FlagsAttribute), false);
                if(attr.Length > 0 && attr[0] is FlagsAttribute)
                    return (T)Enum.ToObject(enumType, converted);

                return defaultValueFactory();
            }
            catch(Exception ex) {
                var defaultValue = defaultValueFactory();

                if(log.IsWarnEnabled)
                    log.WarnException(string.Format(ConvertErrorMsgFormat, value, defaultValue, typeof(T)), ex);

                return defaultValue;
            }
        }

        /// <summary>
        /// 지정된 객체가 null이거나 DBNull 이라면 True를 반환한다.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrDbNull(object value) {
            if(ReferenceEquals(value, null) || Equals(value, DBNull.Value))
                return true;

#if !SILVERLIGHT
            if(value is System.Data.SqlTypes.INullable && ((System.Data.SqlTypes.INullable)value).IsNull)
                return true;
#endif
            return false;
        }
    }
}