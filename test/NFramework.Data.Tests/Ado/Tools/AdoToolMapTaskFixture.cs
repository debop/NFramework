using System;
using System.Linq;
using NSoft.NFramework.Data.DataObjects.Northwind;
using NSoft.NFramework.Data.Persisters;
using NSoft.NFramework.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.Tools {
    [TestFixture]
    public class AdoToolMapTaskFixture : AdoFixtureBase {
        public static IAdoRepository NWRepository {
            get { return NorthwindAdoRepository; }
        }

        private static readonly DateTime CurrentTime = DateTime.Now;

        [Test]
        public void ExecuteReaderMapTask_NameMapper_Test() {
            var emps = NWRepository.ExecuteInstanceAsync<Employee>(TrimMapper, SQL_EMPLOYEE_SELECT).Result;

            emps.All(emp => emp.EmployeeID > 0).Should().Be.True();
            emps.All(emp => emp.LastName.IsNotWhiteSpace()).Should().Be.True();
            emps.All(emp => emp.FirstName.IsNotWhiteSpace()).Should().Be.True();
            emps.All(emp => emp.BirthDate.HasValue && emp.BirthDate.Value < CurrentTime).Should().Be.True();
            emps.All(emp => emp.Title.IsNotWhiteSpace()).Should().Be.True();
        }

        [Test]
        public void ExecuteReaderMapTask_NameMapper_AdditionalMapping_Test() {
            var emps =
                NWRepository.ExecuteInstanceAsync<Employee>(TrimMapper,
                                                            (reader, emp) => { emp.Title = string.Empty; },
                                                            SQL_EMPLOYEE_SELECT)
                    .Result;

            emps.All(emp => emp.EmployeeID > 0).Should().Be.True();
            emps.All(emp => emp.LastName.IsNotWhiteSpace()).Should().Be.True();
            emps.All(emp => emp.FirstName.IsNotWhiteSpace()).Should().Be.True();
            emps.All(emp => emp.BirthDate.HasValue && emp.BirthDate.Value < CurrentTime).Should().Be.True();

            emps.All(emp => emp.Title.IsWhiteSpace()).Should().Be.True();
        }

        [Test]
        public void ExecuteReaderMapTask_NameMap_Test() {
            var persister = new TrimReaderPersister<Employee>(ActivatorTool.CreateInstance<Employee>);

            var emps =
                NWRepository.ExecuteInstanceAsync<Employee>(persister, SQL_EMPLOYEE_SELECT)
                    .Result;

            emps.All(emp => emp.EmployeeID > 0).Should().Be.True();
            emps.All(emp => emp.LastName.IsNotWhiteSpace()).Should().Be.True();
            emps.All(emp => emp.FirstName.IsNotWhiteSpace()).Should().Be.True();
            emps.All(emp => emp.BirthDate.HasValue && emp.BirthDate.Value < CurrentTime).Should().Be.True();
            emps.All(emp => emp.Title.IsNotWhiteSpace()).Should().Be.True();
        }
    }
}