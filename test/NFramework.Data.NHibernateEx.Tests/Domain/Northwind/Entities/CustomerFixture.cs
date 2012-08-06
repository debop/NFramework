using System;
using NHibernate;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain.Northwind;
using NSoft.NFramework.Serializations;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Tests.Domain.Northwind {
    [TestFixture]
    public class CustomerFixture : NorthwindDbTestFixtureBase {
        public INHRepository<Customer> Repository {
            get { return GetRepository<Customer>(); }
        }

        [Test]
        public void TransientTest() {
            var transientCustomer = new Customer("A");
            Assert.IsTrue(transientCustomer.IsTransient); // only check identity value is null or default id
            Assert.IsTrue(Repository.IsTransient(transientCustomer));

            Assert.AreEqual(transientCustomer, new Customer("A")); // 둘다 아무 것도 바뀌지 않았으므로

            var customer2 = SerializerTool.DeepCopy(TestCustomer);
            customer2.CompanyName = base.TestCustomer.CompanyName;
            Assert.AreEqual(base.TestCustomer, customer2); // 둘다 Transient이고, 같은 속성값들을 가진다.

            var savedCustomer = Repository.Get(base.TestCustomer.Id, LockMode.None);

            Assert.AreEqual(savedCustomer.CompanyName, TestCustomer.CompanyName);
            Assert.IsFalse(savedCustomer.IsTransient);

            Assert.IsTrue(savedCustomer != transientCustomer);
            Assert.AreNotEqual(savedCustomer, transientCustomer); // Persistent object .vs. transient object

            Assert.AreEqual(savedCustomer, TestCustomer); // Persistent object .vs. transient object (but id is equal )
            Assert.IsTrue(savedCustomer.Equals(TestCustomer));

            Assert.IsFalse(savedCustomer.IsTransient);
            Assert.IsFalse(Repository.IsTransient(savedCustomer));

            // Assigned이지만 EntityStateInterceptor를 이용하면 AssignedId를 가지는 Entity도 Transient인지를 파악할 수 있다.
            //
            Assert.IsTrue(base.TestCustomer.IsTransient);
            Assert.IsTrue(Repository.IsTransient(TestCustomer));

            // Console.WriteLine("Test NorthwindCustomer: " + savedCustomer);
        }

        [Test]
        public void FindAll() {
            TestTool
                .RunTasks(4,
                          () => {
                              // ThreadedRepeat 있다면 항상 각 Thread 별로 UnitOfWork.Start() 를 호출해야 합니다.);
                              using(UnitOfWork.Start(UnitOfWorkNestingOptions.ReturnExistingOrCreateUnitOfWork)) {
                                  var customers = Repository<Customer>.FindAll();
                                  Assert.IsTrue(customers.Count > 0);

                                  //foreach (NorthwindCustomer customer in customers)
                                  //    Console.WriteLine("###NorthwindCustomer: " + customer.ToString(true));
                              }
                          });
        }

        [Test]
        public void FindAll_Paging() {
            var customers = Repository.FindAll(1, 10);
            var customers2 = Repository.FindAll(10, 5);

            Assert.AreEqual(10, customers.Count);
            Assert.AreEqual(5, customers2.Count);
        }

        [Test]
        public void Get() {
            var foundCustomer = Repository.Get(TestCustomer.Id, LockMode.None);
            Assert.AreEqual(TestCustomer, foundCustomer);
        }

        [Test]
        public void GetOrdersShippedTo() {
            //DateTime orderedDate = new DateTime(1998, 3, 10);
            //NorthwindCustomer customer = Repository.Get(base.TestCustomer.Id);
            //IList<NFramework.Data.NHibernateEx.Tests.Domain.Northwind.Order> orders = customer.GetOrdersOrderedOn(base.FactoryName, orderedDate);

            //Assert.AreEqual(1, orders.Count);

            //foreach (NFramework.Data.NHibernateEx.Tests.Domain.Northwind.Order order in orders)
            //{
            //    Assert.AreEqual(customer.Id, order.OrderedBy.Id);
            //    Assert.AreEqual(orderedDate, order.OrderDate);

            //    if (order.ShipVia != null)
            //        Console.WriteLine("ShipVia: " + order.ShipVia);

            //    if (order.Seller != null)
            //    {
            //        Console.WriteLine("Sell by " + order.Seller);

            //        foreach (Territory t in order.Seller.Territories)
            //            Console.WriteLine("Territory: " + t);
            //    }
            //}
        }

        /// <summary>
        /// Generic Repository가 아닌 상속한 Repository를 사용한다.
        /// </summary>
        [Test]
        public void Find_By_OrderedDate() {
            var customerRepository = (CustomerRepository)Repository;

            var customer = customerRepository.Get(TestCustomer.Id);

            // Windsor.Northwind.config에 OrderDate를 지정해두었다.
            DateTime? orderDate = null; // new DateTime(1998, 3, 10);  

            var orders
                = customerRepository.GetOrdersOrderedOn(customer, orderDate);

            Assert.AreEqual(orders.Count, 1);
        }

        [Test]
        public void NewCustomer() {
            var customer = new Customer("DEBOP", "RealWeb");

            try {
                // Identity가 assigned 라면, 먼저 있는지 검사한 후에 사용한다.
                //
                if(Repository.ExistsById(customer.Id))
                    Repository.Update(customer);
                else
                    Repository.Save(customer);
            }
            catch(Exception ex) {
                Console.WriteLine("Can't save : " + ex.Message, ex);
            }
        }

        [Test]
        public void MultiTest() {
            TestTool.RunTasks(4, () => NHWith.Transaction(delegate {
                                                              FindAll();
                                                              FindAll_Paging();
                                                              Get();
                                                          }));
        }
    }
}