using FluentValidation;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.FluentMappings.Validations {
    public class CustomerValidator2 : AbstractValidator<FluentCustomer> {
        public CustomerValidator2() {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Credit).NotEmpty().WithMessage("Credit 번호가 있어야 합니다.");
        }
    }
}