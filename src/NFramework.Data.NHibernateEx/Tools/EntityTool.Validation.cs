using System;
using Castle.Components.Validator;
using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.Data.NHibernateEx {
    public static partial class EntityTool {
        private static IValidatorRunner _validator;

        /// <summary>
        /// 유효성 검사기. 기본적으로 <see cref="ValidatorRunner"/>를 사용한다.
        /// </summary>
        public static IValidatorRunner Validator {
            get {
                if(_validator == null)
                    lock(_syncLock)
                        if(_validator == null) {
                            var validator = IoC.TryResolve<IValidatorRunner>(() => new ValidatorRunner(new CachedValidationRegistry()),
                                                                             true);
                            System.Threading.Thread.MemoryBarrier();
                            _validator = validator;
                        }

                return _validator;
            }
        }

        /// <summary>
        /// 지정된 엔티티의 유효성을 검사합니다.
        /// </summary>
        /// <param name="entity">유효성 검사를 수행할 엔티티</param>
        /// <exception cref="ValidationException">유효성 검사에 실패했을 때</exception>
        public static void ValidateEntity(this IDataObject entity) {
            entity.ShouldNotBeNull("entity");

            if(Validator.IsValid(entity) == false) {
                var summary = Validator.GetErrorSummary(entity);
                var msg = string.Join(Environment.NewLine, summary.ErrorMessages);

                if(log.IsWarnEnabled) {
                    log.Warn("엔티티 유효성 검사에 실패했습니다!!!");
                    log.Warn(msg);
                }

                throw new ValidationException(msg);
            }
        }

        /// <summary>
        /// 지정된 엔티티가 null이 아님을 검사한다.
        /// </summary>
        /// <param name="entity"></param>
        public static void AssertExists(this IDataObject entity) {
            Guard.Assert(entity != null, @"지정된 entity가 Database에 존재하지 않습니다.");
        }

        /// <summary>
        /// 지정된 엔티티가 null이 아님을 검사한다.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityName"></param>
        public static void AssertExists(this IDataObject entity, string entityName) {
            Guard.Assert(entity != null, @"지정된 Entity가 Database에 존재하지 않습니다. entity name=[{0}]", entityName);
        }
    }
}