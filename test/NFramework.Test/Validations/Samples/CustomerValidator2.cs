using FluentValidation;

namespace NSoft.NFramework.Validations {
    /// <summary>
    /// <see cref="Customer"/>에 대한 Validator입니다.
    /// </summary>
    public class CustomerValidator2 : AbstractValidator<Customer> {
        public CustomerValidator2() {
            RuleFor(c => c.Name).NotEmpty().Length(2, 32);

            RuleFor(c => c.ZipCode).NotEmpty();
            RuleFor(c => c.Discount).Must(discount => discount >= 0).When(c => c.HasDiscount.GetValueOrDefault(true),
                                                                          ApplyConditionTo.AllValidators);
        }
    }
}