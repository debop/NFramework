using System.Data;
using NHibernate.Criterion;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain.Northwind;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Tests.Domain.Northwind {
    [TestFixture]
    public class RepositoryFixture : NorthwindDbTestFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [Test]
        public void GetMetaData() {
            var metaData = Repository<Customer>.GetClassMetadata();

            var propertyNames = metaData.PropertyNames;
            var propertyTypes = metaData.PropertyTypes;

            for(int i = 0; i < propertyNames.Length; i++)
                if(propertyTypes[i].IsEntityType == false && propertyTypes[i].IsCollectionType == false)
                    if(IsDebugEnabled)
                        log.Debug("Value Type Property# Name=[{0}], Type=[{1}]", propertyNames[i], propertyTypes[i]);
        }

        [Test]
        public void FindAll_By_DetachedCriteria() {
            var criteria = Repository<Customer>.CreateDetachedCriteria();

            criteria.Add(Restrictions.Like("CompanyName", "F", MatchMode.Start)); // "F%"
            criteria.CreateCriteria("Orders").Add(Restrictions.Like("ShipName", "S", MatchMode.Anywhere)); // %S%

            var customers = Repository<Customer>.FindAll(criteria);
            Assert.Greater(customers.Count, 0);

            var count = Repository<Customer>.Count(criteria);
            Assert.AreEqual(count, customers.Count);

            Print(customers);
        }

        [Test]
        public void FindAll_By_Example() {
            var customers = Repository<Customer>.FindAll(base.TestCustomer);
            Assert.AreEqual(customers.Count, 1);
            Print(customers);
        }

        [Test]
        public void FindAll_By_Example_With_Criterion() {
            var customer = new Customer("A")
                           {
                               CompanyName = base.TestCustomer.CompanyName.ToLower().Substring(0, 3),
                               ContactName = null
                           };

            var customers = Repository<Customer>.FindAll(new[]
                                                         {
                                                             Example.Create(customer)
                                                                 .EnableLike(MatchMode.Start)
                                                                 .IgnoreCase()
                                                                 .ExcludeProperty("Id")
                                                                 .ExcludeNulls()
                                                         });
            Assert.Greater(customers.Count, 0);
            // Assert.AreEqual(1, customers.Count);
            Print(customers);
        }

        [Test]
        public void ThreadSafeTest() {
            TestTool.RunTasks(4, () => NHWith.Transaction(IsolationLevel.ReadCommitted,
                                                          delegate {
                                                              FindAll_By_DetachedCriteria();
                                                              FindAll_By_Example_With_Criterion();
                                                              FindAll_By_Example();
                                                          },
                                                          UnitOfWorkNestingOptions.CreateNewOrNestUnitOfWork));
        }
    }
}