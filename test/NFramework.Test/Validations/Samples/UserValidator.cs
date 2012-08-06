using System;
using FluentValidation;

namespace NSoft.NFramework.Validations {
    /// <summary>
    /// <see cref="User"/>에 대한 Validator입니다.
    /// </summary>
    public class UserValidator : AbstractValidator<User> {
        public UserValidator() {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Password).NotEmpty().Length(6, 24);

            RuleFor(x => x.LastAccessTime).LessThanOrEqualTo(DateTime.Now);
        }
    }
}