using System;
using System.ComponentModel;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// Converter for <see cref="Vector2D"/>
    /// </summary>
    public class Vector2DConverter : ExpandableObjectConverter {
        /// <summary>
        /// 지정된 sourceType으로부터 <see cref="Vector2D"/> 형식으로 변환이 가능한가?
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            if(sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// <see cref="Vector2D"/> 형식을 지정한 destinationType으로 변환 가능한가?
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
        /// 지정된 값을 <see cref="Vector2D"/> 형식의 값으로 변환
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context,
                                           System.Globalization.CultureInfo culture,
                                           object value) {
            if(value.GetType() == typeof(string))
                return Vector2D.Parse((string)value);

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// 지정한 <see cref="Vector2D"/> 인스턴스 값을 지정한 수형으로 변환한다.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context,
                                         System.Globalization.CultureInfo culture,
                                         object value,
                                         Type destinationType) {
            if((destinationType == typeof(string)) && (value is Vector2D)) {
                var v = (Vector2D)value;
                return v.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// 지정된 컨텍스트를 사용하여, 이 개체가 목록에서 선택할 수 있는 표준 값 집합을 지원하는지 여부를 반환합니다.
        /// </summary>
        /// <returns>
        /// 개체가 지원하는 일반 값 집합을 찾기 위해 <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"/>를 호출해야 하는 경우 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        /// <param name="context">형식 컨텍스트를 제공하는 <see cref="T:System.ComponentModel.ITypeDescriptorContext"/>입니다. </param>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
            return true;
        }

        /// <summary>
        /// 지정된 수형의 기본값이 되는 것들을 등록해 둔다. (디자인시에 활용하기 위해)
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            return new StandardValuesCollection(new object[3] { Vector2D.Zero, Vector2D.XAxis, Vector2D.YAxis });
        }
    }
}