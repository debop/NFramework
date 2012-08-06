using System;
using System.Linq;
using System.Reflection;
using FluentValidation;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Validations {
    /// <summary>
    /// FluentValidation 의 Validator를 생성합니다.<br/>
    /// </summary>
    /// <example>
    /// <code>
    /// var customer = new Customer();
    /// var factory = new ValidatorFactory()
    /// 
    /// var validator = factory.GetValidator(customer.GetType());
    /// 
    /// var result = validator.Validate(customer);
    /// 
    /// result.IsValid.Should().Be.True();	   
    /// result.Errors.Count.Should().Be(0);
    /// </code>
    /// </example>
    public class ValidatorFactory : ValidatorFactoryBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// IValidator<T> 형식을 구현한 Validator의 인스턴스를 제공합니다.
        /// </summary>
        /// <param name="validatorType"></param>
        /// <returns></returns>
        public override IValidator CreateInstance(Type validatorType) {
            // NOTE: http://docs.castleproject.org/Windsor.Referencing-types-in-XML.ashx

            if(IoC.IsInitialized && IoC.Container.Kernel.HasComponent(validatorType)) {
                if(IsDebugEnabled)
                    log.Debug("Validator 수형 [{0}] 을 구현한 Concrete Class를 Windsor Container로부터 인스턴싱합니다...", validatorType);

                var result = IoC.Resolve(validatorType);

                if(result != null)
                    return result as IValidator;
            }

            if(IsDebugEnabled)
                log.Debug("Validator 수형 [{0}] 을 구현한 Concrete Class를 Assembly에서 찾아서 인스턴싱합니다...", validatorType);

            var validatorConcreteType =
                Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .FirstOrDefault(type => TypeTool.IsSameOrSubclassOrImplementedOf(type, validatorType));

            if(validatorConcreteType != null)
                return CreateValidatorInstance(validatorConcreteType);

#if !SILVERLIGHT
            validatorConcreteType =
                AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(asm => asm.GetTypes())
                    .FirstOrDefault(type => TypeTool.IsSameOrSubclassOrImplementedOf(type, validatorType));
#endif
            return CreateValidatorInstance(validatorConcreteType);
        }

        private static IValidator CreateValidatorInstance(Type concreteType) {
            if(concreteType != null)
                return ActivatorTool.CreateInstance(concreteType) as IValidator;

            return null;
        }
    }
}