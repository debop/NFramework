using System;

namespace NSoft.NFramework.Reflections {
    /// <summary>
    /// 대상 수형에 맞게, 설정 값의 수형을 변환시켜, 설정할 수 있는 DynamicAccessor입니다.
    /// </summary>
    /// <typeparam name="T">접근할 대상 수형</typeparam>
    public class TypeConvertableDynamicAccessor<T> : DynamicAccessor<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly Type StringType = typeof(string);

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public TypeConvertableDynamicAccessor() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="suppressError">객체 접근 수행시의 예외 발생을 시킬 것인가 여부. true이면 예외발생을 하지 않는다.</param>
        public TypeConvertableDynamicAccessor(bool suppressError) : base(suppressError) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="suppressError">객체 접근 수행시의 예외 발생을 시킬 것인가 여부. true이면 예외 발생을 하지 않는다.</param>
        /// <param name="ignoreCase">속성명, 필드명의 대소문자를 구분할 것인가?</param>
        public TypeConvertableDynamicAccessor(bool suppressError, bool ignoreCase) : base(suppressError, ignoreCase) {}

        /// <summary>
        /// 지정된 인스턴스의 필드에 지정된 값을 필드 수형에 맞게 변환하여 설정합니다. 기존 <see cref="DynamicAccessor"/>는 수형이 정확히 일치하지 않으면 예외를 발생시킵니다.
        /// </summary>
        /// <param name="target">대상 객체</param>
        /// <param name="fieldName">필드 변수 명</param>
        /// <param name="fieldValue">설정할 필드 값</param>
        public override void SetFieldValue(T target, string fieldName, object fieldValue) {
            if(IsDebugEnabled)
                log.Debug("대상 인스턴스[{0}] 필드명[{1}]에 값[{2}]을 설정하려고합니다.", TargetType.FullName, fieldName, fieldValue);

            target.ShouldNotBeNull("target");

            var @exceptionAction =
                SuppressError
                    ? (Action<Exception>)(ex => { })
                    : ex => {
                          if(log.IsWarnEnabled) {
                              log.Warn("필드[{0}]에 값[{1}] 설정에 실패했습니다.", fieldName, fieldValue);
                              log.Warn(ex);
                          }
                      };

            var fName = GetFieldName(fieldName);
            var fieldSetter = FieldSetterMap.GetOrAddFieldSetter(TargetType, fName);

            if(fieldSetter == null) {
                if(log.IsWarnEnabled)
                    log.Warn("대상 인스턴스[{0}]에 필드명[{1}]에 대한 Setter가 정의되어 있지 않습니다. 대소문자를 구분합니다.", TargetType.FullName, fName);
                return;
            }

            var fieldType = GetFieldType(fName);
            var needConverting = fieldValue != null &&
                                 fieldType != null &&
                                 fieldType.IsSameOrSubclassOf(fieldValue.GetType()) == false &&
                                 fieldType != StringType;

            object valueToSetting = null;

            if(needConverting) {
                var convertedValue = fieldValue.AsValue(fieldType);

                if(IsDebugEnabled)
                    log.Debug("대상 인스턴스[{0}] 필드명[{1}][Type={2}]에 원본 값[{3}]을 변환한 값[{4}]으로 설정합니다.",
                              TargetType.FullName, fName, fieldType, fieldValue, convertedValue);

                valueToSetting = convertedValue;
            }
            else {
                if(IsDebugEnabled)
                    log.Debug("대상 인스턴스[{0}] 필드명[{1}]에 값[{2}]을 설정합니다.", TargetType.FullName, fName, fieldValue);

                valueToSetting = ConvertTool.IsNullOrDbNull(fieldValue) ? null : fieldValue;
            }
            With.TryAction(() => fieldSetter(target, valueToSetting), @exceptionAction);
        }

        /// <summary>
        /// 지정된 인스턴스의 속성에 지정된 값을 속성의 수형에 맞게 변환하여 설정합니다. 기존 <see cref="DynamicAccessor"/>는 수형이 정확히 일치하지 않으면 예외를 발생시킵니다.
        /// </summary>
        /// <param name="target">대상 객체</param>
        /// <param name="propertyName">속성 명</param>
        /// <param name="propertyValue">속성 값</param>
        public override void SetPropertyValue(T target, string propertyName, object propertyValue) {
            if(IsDebugEnabled)
                log.Debug("대상 인스턴스[{0}] 속성명[{1}]에 값[{2}]을 설정하려고합니다...", TargetType.FullName, propertyName, propertyValue);

            target.ShouldNotBeNull("target");

            var @exceptionAction =
                (SuppressError)
                    ? (Action<Exception>)(ex => { })
                    : ex => {
                          if(log.IsWarnEnabled) {
                              log.Warn("속성[{0}]에 값[{1}] 설정 작업이 실패했습니다.", propertyName, propertyValue);
                              log.Warn(ex);
                          }
                      };

            var pName = GetPropertyName(propertyName);
            var propertySetter = PropertySetterMap.GetOrAddPropertySetter(TargetType, pName, @exceptionAction);

            if(propertySetter == null) {
                if(log.IsWarnEnabled)
                    log.Warn("대상 인스턴스[{0}]에 속성명[{1}]에 대한 Setter가 정의되어 있지 않습니다. 대소문자를 구분합니다.", TargetType.FullName, pName);

                return;
            }

            var propertyType = GetPropertyType(pName);

            var needConverting = propertyValue != null &&
                                 propertyType != null &&
                                 propertyType.IsSameOrSubclassOf(propertyValue.GetType()) == false &&
                                 propertyType != StringType;

            object valueToSetting = null;

            if(needConverting) {
                // NOTE: Fasterflect.Probing.TypeConverter.Get() 메소드가 편하지만, Nullable을 지원하지 않습니다.
                //
                var convertedValue = propertyValue.AsValue(propertyType);

                if(IsDebugEnabled)
                    log.Debug("대상 인스턴스[{0}] 속성명[{1}][Type={2}]에 원본 값[{3}]을 변환한 값[{4}]으로 설정합니다.",
                              TargetType.FullName, pName, propertyType, propertyValue, convertedValue);

                valueToSetting = convertedValue;
            }
            else {
                if(IsDebugEnabled)
                    log.Debug("대상 인스턴스[{0}] 속성명[{1}]에 값[{2}]을 설정합니다.", TargetType.FullName, pName, propertyValue);

                valueToSetting = ConvertTool.IsNullOrDbNull(propertyValue) ? null : propertyValue;
            }

            With.TryAction(() => propertySetter(target, valueToSetting), @exceptionAction);
        }
    }
}