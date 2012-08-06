using System;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Validations {
    [Microsoft.Silverlight.Testing.Tag("Validations")]
    [TestFixture]
    public class ValidatorToolFixture {
        [Test]
        public void GetValidatorTest() {
            var validator = ValidatorTool.GetValidator(typeof(Customer));
            validator.Should().Not.Be.Null();

            validator = ValidatorTool.GetValidator(typeof(User));
            validator.Should().Not.Be.Null();
        }

        [Test]
        public void GetValidatorOfTest() {
            var customer = new Customer();
            var validator = ValidatorTool.GetValidatorOf(customer);
            validator.Should().Not.Be.Null();

            var user = new User();
            validator = ValidatorTool.GetValidatorOf(user);
            validator.Should().Not.Be.Null();
        }

        [Test]
        public void CustomerValidatorTest() {
            var customer = new Customer()
                           {
                               Name = "배성혁",
                               Company = "리얼웹",
                               Discount = 12.5m,
                               ZipCode = "135-010"
                           };

            var result = ValidatorTool.Validate(customer);

            result.IsValid.Should().Be.True();
            result.Errors.Count.Should().Be(0);

            customer = new Customer()
                       {
                           Name = "",
                           Company = "",
                           Discount = 12.5m,
                           ZipCode = ""
                       };

            result = ValidatorTool.Validate(customer);

            result.IsValid.Should().Be.False();
            result.Errors.Count.Should().Be.GreaterThan(0);

            foreach(var failure in result.Errors) {
                Console.WriteLine(failure);
            }
        }

        [Test]
        public void UserValidatorTest() {
            var user = new User()
                       {
                           Id = "debop",
                           Password = "123456abc1234",
                           Name = "배성혁",
                           LastAccessTime = DateTime.Today
                       };

            var result = ValidatorTool.Validate(user);

            result.IsValid.Should().Be.True();
            result.Errors.Count.Should().Be(0);

            foreach(var failure in result.Errors) {
                Console.WriteLine(failure);
            }

            user = new User()
                   {
                       Id = "debop",
                       Password = "123",
                       Name = "배성혁",
                       LastAccessTime = DateTime.Today.AddYears(1)
                   };

            result = ValidatorTool.Validate(user);

            result.IsValid.Should().Be.False();
            result.Errors.Count.Should().Be.GreaterThan(0);

            foreach(var failure in result.Errors) {
                Console.WriteLine(failure);
            }
        }
    }
}