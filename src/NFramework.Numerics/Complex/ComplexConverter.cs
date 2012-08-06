using System;
using System.ComponentModel;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 복소수 변환자
    /// </summary>
    public sealed class ComplexConverter : ExpandableObjectConverter {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        ///<summary>
        /// 지정된 원본 형식으로부터 변환이 가능한지 알아본다.
        ///</summary>
        ///<returns>
        ///true if this converter can perform the conversion; otherwise, false.
        ///</returns>
        ///<param name="context">
        /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context. 
        /// </param>
        ///<param name="sourceType">
        /// A <see cref="T:System.Type" /> that represents the type you want to convert from. 
        /// </param>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            if(sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// <see cref="Complex"/>가 원하는 형식으로 변환이 가능하지 검사한다.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            if(destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// 지정된 값을 <see cref="Complex"/>로 변환가능한지 검사한다.
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
            if(ReferenceEquals(value, null) == false && value.GetType() == typeof(string))
                return Complex.Parse((string)value);

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// 지정된 <paramref name="value"/>의 수형이 <see cref="Complex"/>이면, <paramref name="destinationType"/>으로 변환이 가능한지 검사한다.
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value,
                                         Type destinationType) {
            if((destinationType == typeof(string)) && (value is Complex))
                return ((Complex)value).ToString();

            return base.ConvertTo(context, culture, value, destinationType);
        }

        ///<summary>
        /// Returns whether this object supports a standard set of values that can be picked from a list, 
        /// using the specified context.
        ///</summary>
        ///<returns>
        ///true if <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues" /> 
        /// should be called to find a common set of values the object supports; otherwise, false.
        ///</returns>
        ///<param name="context">
        /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context. 
        /// </param>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
            return true;
        }

        ///<summary>
        /// Returns a collection of standard values for the data type 
        /// this type converter is designed for when provided with a format context.
        ///</summary>
        ///<returns>
        /// A <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection" /> that 
        /// holds a standard set of valid values, or null if the data type does not support a standard set of values.
        ///</returns>
        ///<param name="context">
        /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context 
        /// that can be used to extract additional information about the environment from which this converter is invoked. 
        /// This parameter or properties of this parameter can be null. 
        ///</param>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            return new StandardValuesCollection(new object[]
                                                {
                                                    Complex.Zero,
                                                    Complex.One,
                                                    Complex.I,
                                                    Complex.MaxValue,
                                                    Complex.MinValue
                                                });
        }
    }
}