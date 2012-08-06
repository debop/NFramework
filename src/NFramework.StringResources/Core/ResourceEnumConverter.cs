using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// Enum 값의 표현 문자열을 다국어 버전에 맞게 표시할 수 있도록 한다.
    /// <example>
    /// 
    /// <code>
    /// [TypeConverter(typeof(ResourceEnumConverter))]
    /// public enum Size { Small, Midium, Large }
    /// 
    /// // 다국어 리스소에 다음과 같이 정의하면 된다.
    /// // Size_Small = 작음
    /// // Size_Midium = 중간
    /// // Size_Large = 큼
    /// //
    /// // CurrentUICulture가 ko 기반이면 Size.Small.ToString() 은 "작음" 이라고 표현된다.
    /// </code>
    /// </example>
    /// </summary>
    public class ResourceEnumConverter : System.ComponentModel.EnumConverter {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Convert the given enum value to string using the registered type converter
        /// </summary>
        /// <param name="value">The enum value to convert to string</param>
        /// <returns>The localized string value for the enum</returns>
        public static string ConvertToString(Enum value) {
            var converter = TypeDescriptor.GetConverter(value.GetType());
            return converter.ConvertToString(value);
        }

        /// <summary>
        /// Return a list of the enum values and their associated display text for the given enum type
        /// </summary>
        /// <param name="enumType">The enum type to get the values for</param>
        /// <param name="culture">The culture to get the text for</param>
        /// <returns>
        /// A list of KeyValuePairs where the key is the enum value and the value is the text to display
        /// </returns>
        /// <remarks>
        /// This method can be used to provide localized binding to enums in ASP.NET applications.   Unlike 
        /// windows forms the standard ASP.NET controls do not use TypeConverters to convert from enum values
        /// to the displayed text.   You can bind an ASP.NET control to the list returned by this method by setting
        /// the DataValueField to "Key" and theDataTextField to "Value". 
        /// </remarks>
        public static IList<KeyValuePair<Enum, string>> GetValues(Type enumType, CultureInfo culture) {
            if(IsDebugEnabled)
                log.Debug(@"Get Enum Value and ValueText Pair. EnumType=[{0}], Culture=[{1}]", enumType, culture);

            var converter = TypeDescriptor.GetConverter(enumType);

            return
                Enum.GetValues(enumType)
                    .Cast<Enum>()
                    .Select(v => new KeyValuePair<Enum, string>(v, converter.ConvertToString(null, culture, v)))
                    .ToList();
        }

        /// <summary>
        /// Return a list of the enum values and their associated display text for the given enum type in the current UI Culture
        /// </summary>
        /// <param name="enumType">The enum type to get the values for</param>
        /// <returns>
        /// A list of KeyValuePairs where the key is the enum value and the value is the text to display
        /// </returns>
        /// <remarks>
        /// This method can be used to provide localized binding to enums in ASP.NET applications.   Unlike 
        /// windows forms the standard ASP.NET controls do not use TypeConverters to convert from enum values
        /// to the displayed text.   You can bind an ASP.NET control to the list returned by this method by setting
        /// the DataValueField to "Key" and theDataTextField to "Value". 
        /// </remarks>
        public static IList<KeyValuePair<Enum, string>> GetValues(Type enumType) {
            return GetValues(enumType, CultureInfo.CurrentUICulture);
        }

        private class LookupTable : Dictionary<string, object> {}

        private readonly IDictionary<CultureInfo, LookupTable> _lookupTables = new Dictionary<CultureInfo, LookupTable>();
        private readonly ResourceManager _rm;

        // flag enum
        private readonly bool _isFlags;
        private readonly Array _flagValues;

        private readonly object _sync = new object();

        /// <summary>
        /// 생성자
        /// </summary>
        public ResourceEnumConverter(Type type, ResourceManager rm)
            : base(type) {
            rm.ShouldNotBeNull("rm");

            _rm = rm;
            var flagAttr = type.GetCustomAttributes(typeof(FlagsAttribute), true);
            _isFlags = flagAttr.Length > 0;

            if(_isFlags)
                _flagValues = Enum.GetValues(type);
        }

        /// <summary>
        /// 지정된 <see cref="CultureInfo"/>에 해당하는 Key-Value 쌍의 Loopup Table을 반환한다. 없으면 새로 만든다.
        /// </summary>
        private LookupTable GetLookupTable(CultureInfo culture) {
            if(IsDebugEnabled)
                log.Debug("지정된 문화권의 Lookup Table을 로드합니다... culture=[{0}]", culture);

            LookupTable lookup = null;

            // if (culture == null)
            if(culture.IsNullCulture()) {
                culture = CultureInfo.CurrentCulture;

                if(IsDebugEnabled)
                    log.Debug("지정한 Culture가 null 이어서, 현재 Culture를 사용합니다. culture=[{0}]", culture);
            }

            lock(_sync) {
                if(_lookupTables.TryGetValue(culture, out lookup) == false) {
                    if(IsDebugEnabled)
                        log.Debug("lookup table for culture[{0}] doesn't exists, so create new lookup table.", culture);

                    lookup = new LookupTable();

                    var standardValues = GetStandardValues();
                    if(standardValues != null) {
                        foreach(var value in standardValues) {
                            var text = GetValueText(culture, value);

                            if(text != null)
                                lookup.Add(text, value);
                        }
                    }
                    _lookupTables.Add(culture, lookup);
                }
            }
            return lookup;
        }

        /// <summary>
        /// 지정된 <see cref="CultureInfo"/>에 해당하는 Enum 값의 표현 문자열을 구한다. 
        /// 만약 없다면 Enum 값을 표현 문자열로 반환한다.
        /// </summary>
        protected virtual string GetValueText(CultureInfo culture, object value) {
            var type = value.GetType();

            // Localization 값을 가지는 리소스의 키의 명칭 형식은 EnumType_EnumName 형식이다.
            // 예) Compression.GZip ==> Compression_GZip : GZip(영문), GZip 압축 (한글)

            var resourceKey = string.Format("{0}_{1}", type.Name, value);
            var resourceText = _rm.GetString(resourceKey, culture);

            if(IsDebugEnabled)
                log.Debug("Get Localized Enum Value. culture=[{0}], value=[{1}], resourceKey=[{2}], resourceText=[{3}]",
                          culture, value, resourceKey, resourceText);

            resourceText = resourceText ?? value.ToString();
            return resourceText;
        }

        /// <summary>
        /// 지정된 값이 하나의 Bit만을 표시하는 것인지 판단한다.
        /// </summary>
        private static bool IsSingleBitValue(ulong value) {
            switch(value) {
                case 0:
                    return false;
                case 1:
                    return true;
            }
            return ((value & (value - 1)) == 0);
        }

        /// <summary>
        /// 지정된 <see cref="CultureInfo"/>에 해당하는 Enum 값의 문자열을 반환한다.
        /// </summary>
        private string GetFlagValueText(CultureInfo culture, object value) {
            // 표준 값이라면 (중복 Flag 값이 아니라면) 그 값을 반환한다.
            //
            if(Enum.IsDefined(value.GetType(), value))
                return GetValueText(culture, value);

            // Flag 된 값이라면 Flag에 따라 나눈다.
            var lValue = Convert.ToUInt64(value);
            string result = null;

            // Enum의 각 Value들에 대해서 지정된 값과 Bit Flag를 비교하여 포함되면 표현 문자열에 넣는다.
            foreach(var flagValue in _flagValues) {
                var lFlagValue = Convert.ToUInt64(flagValue);

                if(IsSingleBitValue(lFlagValue)) {
                    if((lFlagValue & lValue) == lFlagValue) {
                        var valueText = GetValueText(culture, flagValue);

                        if(result == null)
                            result = valueText;
                        else
                            result += ", " + valueText;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// <seealso cref="GetValueText(CultureInfo, object)"/>와 반대로 Value의 지정된 Culture에 해당하는 표현문자열로 Enum 값을 Parsing한다.
        /// </summary>
        private object GetValue(CultureInfo culture, string text) {
            var lookup = GetLookupTable(culture);
            object value = null;
            lookup.TryGetValue(text, out value);

            return value;
        }

        /// <summary>
        /// <seealso cref="GetFlagValueText(CultureInfo, object)"/>와 반대로 Value의 지정된 Culture에 해당하는 표현문자열로 Enum 값을 Parsing한다.
        /// </summary>
        private object GetFlagValue(CultureInfo culture, string text) {
            var lookup = GetLookupTable(culture);

            var textParts = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            ulong result = 0;

            foreach(string textPart in textParts) {
                object value = null;
                if(!lookup.TryGetValue(textPart.Trim(), out value))
                    return null;

                result |= Convert.ToUInt64(value);
            }

            return Enum.ToObject(EnumType, result);
        }

        /// <summary>
        /// Parse string values to enum values
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if(value is string) {
                var result = (_isFlags)
                                 ? GetFlagValue(culture, (string)value)
                                 : GetValue(culture, (string)value);

                return result ?? base.ConvertFrom(context, culture, value);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Convert the enum value to localized text
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if(value != null && destinationType == typeof(string)) {
                object text = (_isFlags)
                                  ? GetFlagValueText(culture, value)
                                  : GetValueText(culture, value);
                return text;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}