using System;
using System.Linq;
using NSoft.NFramework.Data.DataObjects.Northwind;
using NSoft.NFramework.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.Tools {
    [TestFixture]
    public class AdoToolMapAsParallelFixture : AdoFixtureBase {
        public static IAdoRepository NWRepository {
            get { return NorthwindAdoRepository; }
        }

        private static readonly DateTime CurrentTime = DateTime.Now;

        [Test]
        public void ExecuteReaderMapMapAsParallelTask_NameMapper_Test() {
            var emps =
                NWRepository.ExecuteInstanceAsParallelAsync<Employee>(TrimMapper, ActivatorTool.CreateInstance<Employee>,
                                                                      SQL_EMPLOYEE_SELECT).Result;

            emps.All(emp => emp.EmployeeID > 0).Should().Be.True();
            emps.All(emp => emp.LastName.IsNotWhiteSpace()).Should().Be.True();
            emps.All(emp => emp.FirstName.IsNotWhiteSpace()).Should().Be.True();
            emps.All(emp => emp.BirthDate.HasValue && emp.BirthDate.Value < CurrentTime).Should().Be.True();
            emps.All(emp => emp.Title.IsNotWhiteSpace()).Should().Be.True();
        }

        [Test]
        public void ExecuteReaderMapMapAsParallelTask_NameMapper_AdditionalMapping_Test() {
            var emps =
                NWRepository
                    .ExecuteInstanceAsParallelAsync<Employee>(TrimMapper,
                                                              ActivatorTool.CreateInstance<Employee>,
                                                              (row, emp) => { emp.Title = string.Empty; },
                                                              SQL_EMPLOYEE_SELECT)
                    .Result;

            emps.All(emp => emp.EmployeeID > 0).Should().Be.True();
            emps.All(emp => emp.LastName.IsNotWhiteSpace()).Should().Be.True();
            emps.All(emp => emp.FirstName.IsNotWhiteSpace()).Should().Be.True();
            emps.All(emp => emp.BirthDate.HasValue && emp.BirthDate.Value < CurrentTime).Should().Be.True();

            emps.All(emp => emp.Title.IsWhiteSpace()).Should().Be.True();
        }

        [Test]
        public void ExecuteReaderMapMapAsParallelTask_MapFunc() {
            Func<AdoResultRow, Employee> @mapFunc =
                row => new Employee
                       {
                           EmployeeID = row["EmployeeID"].AsInt(),
                           LastName = row["LastName"].AsText(),
                           FirstName = row["FirstName"].AsText(),
                           BirthDate = row["BirthDate"].AsDateTimeNullable(),
                           Title = row["Title"].AsText()
                       };

            var emps =
                NWRepository
                    .ExecuteInstanceAsParallelAsync(@mapFunc, SQL_EMPLOYEE_SELECT)
                    .Result;

            emps.All(emp => emp.EmployeeID > 0).Should("EmployeeID").Be.True();
            emps.All(emp => emp.LastName.IsNotWhiteSpace()).Should("LastName").Be.True();
            emps.All(emp => emp.FirstName.IsNotWhiteSpace()).Should("FirstName").Be.True();
            emps.All(emp => emp.BirthDate.HasValue && emp.BirthDate.Value < CurrentTime).Should("BirthDate").Be.True();

            emps.All(emp => emp.Title.IsNotWhiteSpace()).Should("Title").Be.True();
        }
    }
}