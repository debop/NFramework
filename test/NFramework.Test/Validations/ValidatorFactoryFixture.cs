using System;
using System.Linq;
using System.Reflection;
using FluentValidation;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Validations {
    /// <summary>
    /// 참고 : http://docs.castleproject.org/Windsor.Referencing-types-in-XML.ashx
    /// </summary>
    [Microsoft.Silverlight.Testing.Tag("Validations")]
    [TestFixture]
    public class ValidatorFactoryFixture : AbstractFixture {
        protected override void OnFixtureSetUp() {
            base.OnFixtureSetUp();

            IoC.Initialize();
        }

        [Test]
        public void FindValidatorType() {
            var targetType = typeof(User);
            var validatorGenericType = typeof(IValidator<>).MakeGenericType(new[] { targetType });

            validatorGenericType.IsGenericType.Should().Be.True();

            var validatorConcreteType =
                Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .FirstOrDefault(type => TypeTool.IsSameOrSubclassOrImplementedOf(type, validatorGenericType));

            validatorConcreteType.Should().Not.Be.Null();

            validatorConcreteType.Should().Be(typeof(UserValidator));
        }

        [Test]
        public void FindValidatorTypeFromReferences() {
            var targetType = typeof(User);
            var validatorGenericType = typeof(IValidator<>).MakeGenericType(new[] { targetType });

            validatorGenericType.IsGenericType.Should().Be.True();

#if !SILVERLIGHT
            var validatorConcreteType =
                AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(asm => asm.GetTypes())
                    .FirstOrDefault(type => TypeTool.IsSameOrSubclassOrImplementedOf(type, validatorGenericType));
#else
			var validatorConcreteType =
				Assembly
					.GetExecutingAssembly()
					.GetTypes()
					.Where(type => RwType.IsSameOrSubclassOrImplementedOf(type, validatorGenericType))
					.FirstOrDefault();
#endif

            validatorConcreteType.Should().Not.Be.Null();

            validatorConcreteType.Should().Be(typeof(UserValidator));
        }

        [Test]
        public void GetValidatorTest() {
            var factory = new ValidatorFactory();
            var validator = factory.GetValidator(typeof(Customer));

            typeof(CustomerValidator).IsInstanceOfType(validator).Should().Be.True();
        }

        [Test]
        public void ValidCustomerTest() {
            var customer = new Customer()
                           {
                               Name = "배성혁",
                               Company = "리얼웹",
                               Discount = 12.5m,
                               ZipCode = "135-010"
                           };

            var factory = new ValidatorFactory();
            var customerValidator = factory.GetValidator(typeof(Customer));

            var result = customerValidator.Validate(customer);
            result.IsValid.Should().Be.True();
            result.Errors.Count.Should().Be(0);
        }

        [Test]
        public void InValidCustomerTest() {
            var customer = new Customer()
                           {
                               Name = "",
                               Company = "",
                               HasDiscount = true,
                           };

            var factory = new ValidatorFactory();
            var customerValidator = factory.GetValidator(typeof(Customer));

            var result = customerValidator.Validate(customer);
            result.IsValid.Should().Be.False();
            result.Errors.Count.Should().Be.GreaterThan(0);

            result.Errors.RunEach(err => Console.WriteLine(err.ErrorMessage));
        }
    }
}