using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Validations {
    [Microsoft.Silverlight.Testing.Tag("Validations")]
    [TestFixture]
    public class FluentValidationFixture {
        [Test]
        public void ValidCustomerTest() {
            var customer = new Customer()
                           {
                               Name = "배성혁",
                               Company = "리얼웹",
                               Discount = 12.5m,
                               ZipCode = "135-010"
                           };

            var customerValidator = new CustomerValidator();

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

            var customerValidator = new CustomerValidator();

            var result = customerValidator.Validate(customer);
            result.IsValid.Should().Be.False();
            result.Errors.Count.Should().Be.GreaterThan(0);
        }
    }
}