using System;
using System.Collections.Concurrent;
using FluentValidation;
using FluentValidation.Results;

namespace NSoft.NFramework.Validations {
    /// <summary>
    /// FluentValidation 어셈블리를 이용히여, 정보에 대한 유효성 검증을 수행하는 Validator 및 메소드들을 제공합니다.
    /// 참고 : http://fluentvalidation.codeplex.com/
    /// </summary>
    public static class ValidatorTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly ConcurrentDictionary<Type, IValidator> _validators = new ConcurrentDictionary<Type, IValidator>();
        private static readonly ValidatorFactory _validatorFactory = new ValidatorFactory();

        /// <summary>
        /// <paramref name="targetType"/>에 해당하는 <see cref="IValidator"/>를 반환합니다.
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static IValidator GetValidator(this Type targetType) {
            targetType.ShouldNotBeNull("targetType");
            return _validators.GetOrAdd(targetType, type => _validatorFactory.GetValidator(type));
        }

        /// <summary>
        /// <typeparamref name="T"/> 수형의 인스턴스에 대한 유효성 검증을 수행할 <see cref="IValidator{T}"/>를 반환합니다.
        /// </summary>
        /// <typeparam name="T">유효성 검증 대상 엔티티의 수형</typeparam>
        /// <returns></returns>
        public static IValidator<T> GetValidator<T>() {
            return (IValidator<T>)GetValidator(typeof(T));
        }

        /// <summary>
        /// <paramref name="target"/>의 유효성 검증을 수행할 <see cref="IValidator"/>의 인스턴스를 반환합니다.
        /// </summary>
        /// <param name="target">유효성 검증 대상 객체</param>
        /// <returns></returns>
        public static IValidator GetValidatorOf(object target) {
            target.ShouldNotBeNull("target");

            return GetValidator(target.GetType());
        }

        /// <summary>
        /// <paramref name="target"/>에 해당하는 수형에 대한 Validator를 이용하여, Validation을 수행한 후 결과를 반환합니다.
        /// </summary>
        /// <param name="target">유효성 검증 대상 객체</param>
        /// <returns></returns>
        public static ValidationResult Validate(object target) {
            target.ShouldNotBeNull("target");

            var validator = GetValidatorOf(target);

            if(validator != null)
                return validator.Validate(target);

            var message = string.Format("수형[{0}]의 Validator가 없습니다.", target.GetType());
            return new ValidationResult(new[] { new ValidationFailure("Validator", message) });
        }
    }
}