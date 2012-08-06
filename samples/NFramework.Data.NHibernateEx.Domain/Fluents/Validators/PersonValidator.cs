using FluentValidation;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.Validators {
    public class PersonValidator : AbstractValidator<Person> {
        public PersonValidator() {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name은 빈 문자열이면 안됩니다.");
        }
    }
}