using System;
using NHibernate.Criterion;
using NHibernate.Type;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain.Northwind;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Tests.Domain.Northwind {
    [TestFixture]
    public class EmployeeFixture : NorthwindDbTestFixtureBase {
        private static INHRepository<Employee> EmployeeRepository {
            get { return GetRepository<Employee>(); }
        }

        [Test]
        public void FindAll() {
            var employees = EmployeeRepository.FindAll();
            Assert.Greater(employees.Count, 1);

            //foreach (Employee emp in employees)
            //    Console.WriteLine(emp);
        }

        [Test]
        public void SearchReportsTo() {
            var employees = EmployeeRepository.FindAll();

            foreach(var emp in employees) {
                var tmpEmp = emp;

                if(tmpEmp.ReportsTo == null) {
                    Console.WriteLine("{0} 는 관리자", tmpEmp.FullName);
                }
                else {
                    do {
                        Console.WriteLine("{0} 는 {1}에게 보고합니다.", tmpEmp.FullName, tmpEmp.ReportsTo.FullName);
                        tmpEmp = tmpEmp.ReportsTo;
                    } while(tmpEmp.ReportsTo != null);
                }
            }
        }

        [Test]
        public void FindFirstByHql() {
            var emp = EmployeeRepository.FindFirstByHql("from Employee as e where e.FullName.LastName = :LastName",
                                                        new NHParameter("LastName", "Suyama", TypeFactory.GetStringType(255)));

            while(emp != null) {
                Console.WriteLine("Employee Fullname: " + emp.FullName);
                emp = emp.ReportsTo;
            }
        }

        [Test]
        public void SearchReportsTo2() {
            var criteria = EmployeeRepository.CreateDetachedCriteria(); // DetachedCriteria.For<Employee>();
            criteria.Add(Restrictions.Eq("FullName.LastName", "Suyama"));

            //Employee emp = EmployeeRepository.("from Employee as e where e.FullName.LastName=:LastName", 
            //		                                            new NHParameter("LastName", "Suyama"));
            var emp = EmployeeRepository.FindOne(criteria);

            while(emp != null) {
                Console.WriteLine(emp.FullName);
                emp = emp.ReportsTo;
            }
        }

        [Test]
        public void GetPages() {
            var employees = EmployeeRepository.FindAll(3, 4);

            Assert.AreEqual(employees.Count, 4);

            //foreach (Employee emp in employees)
            //    Console.WriteLine(emp);
        }

        [Test]
        public void GetPagesByOrder() {
            var orders = new[]
                         {
                             NHibernate.Criterion.Order.Asc("HireDate"),
                             NHibernate.Criterion.Order.Desc("FullName.LastName")
                         };

            var employees = EmployeeRepository.FindAll(orders, 3, 4);

            Assert.AreEqual(employees.Count, 4);

            //foreach (Employee emp in employees)
            //    Console.WriteLine("HireDate: {0}, LastName:{1}", emp.HireDate, emp.FullName.LastName);
        }
    }
}