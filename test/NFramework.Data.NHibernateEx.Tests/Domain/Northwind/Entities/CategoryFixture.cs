using System.Data;
using System.Linq;
using NHibernate.Linq;
using NSoft.NFramework.Data.Tests.Domain.Northwind;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Northwind {
    [TestFixture]
    public class CategoryFixture : NorthwindDbTestFixtureBase {
        public IQueryable<Category> Categories {
            get { return UnitOfWork.CurrentSession.Query<Category>(); }
        }

        [Test]
        public void FindAll() {
            var categories = Categories.ToList();

            Assert.IsNotNull(categories);
            Assert.Greater(categories.Count, 0);

            UnitOfWork.CurrentSession.Clear();
            // for second level cache test. 
            UnitOfWork.CurrentSessionFactory.Evict(typeof(Category));

            categories = Categories.ToList();
            Assert.IsNotNull(categories);
            Assert.Greater(categories.Count, 0);
        }

        [Test]
        public void Get() {
            var category = Repository<Category>.Get(1);
            Assert.IsNotNull(category);
            Assert.AreEqual(category.Id, 1);
            Assert.IsTrue(category.IsSaved);
        }

        [Test]
        public void LazyLoad() {
            // 첫번째 레코드가 (0,1) 이므로 Category의 두번째 레코드
            //
            var categories = Categories.Skip(1).Take(1);

            foreach(var category in categories) {
                foreach(var product in category.Products.Take(5)) {
                    foreach(var orderDetail in product.OrderDetails.Take(5)) {
                        // NOTE : OrderDetail.Order 를 lazy=proxy로 해야 제대로 된다....
                        Assert.AreEqual(orderDetail.Product.Id, product.Id);
                    }
                }
            }
        }

        [Test]
        public void MultiTest() {
            TestTool.RunTasks(4, () => NHWith.Transaction(IsolationLevel.ReadCommitted,
                                                          delegate {
                                                              FindAll();
                                                              Get();
                                                              LazyLoad();
                                                          },
                                                          UnitOfWorkNestingOptions.CreateNewOrNestUnitOfWork));
        }
    }
}