using System;
using NHibernate;
using NHibernate.Type;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain.Northwind;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Tests.Domain.Northwind {
    [TestFixture]
    public class ProductFixture : NorthwindDbTestFixtureBase {
        private static INHRepository<Product> Repository {
            get { return GetRepository<Product>(); }
        }

        private const int TestProductId = 41;

        [Test]
        public void Get() {
            var product = Repository.Get(TestProductId);
            Assert.IsNotNull(product);

            product = Repository.Get(42, LockMode.Upgrade);
            Assert.IsNotNull(product);

#pragma warning disable 0618

            var futureProduct = Repository.FutureGet(TestProductId);

#pragma warning restore 0618

            Assert.IsNotNull(futureProduct.Value);
        }

        [Test]
        public void FindAll_With_Paging() {
            var products = Repository.FindAll(4, 5);
            Assert.AreEqual(products.Count, 5);

            foreach(var product in products) {
                var p = Repository.Get(product.Id, LockMode.None);
                Assert.AreEqual(p, product);
            }
        }

        [Test]
        public void GetByCategory() {
            var products = Repository.FindAllByHql("from Product as p where p.Category.Name = :CategoryName",
                                                   new NHParameter("CategoryName", "Seafood", TypeFactory.GetStringType(255)));

            Assert.Greater(products.Count, 0);

            foreach(Product product in products) {
                Assert.IsNotNull(product);
                //Console.WriteLine("Product[{0}] in Category[{1}] is supplied by {2}",
                //                  product.Name, product.Category.Name, product.SuppliedBy.CompanyName);
            }

            // 첫번째 제품의 공급자가 공급하는 모든 제품
            var allProductBySupplier =
                Repository.FindAllByHql("from Product as p where p.SuppliedBy = :SuppliedBy",
                                        new NHParameter("SuppliedBy", products[0].SuppliedBy));

            foreach(Product product in allProductBySupplier) {
                Assert.IsNotNull(product);
                //Console.WriteLine("Product[{0}] in Category[{1}] is supplied by {2}",
                //                  product.Name, product.Category.Name, product.SuppliedBy.CompanyName);
            }
        }

        [Test]
        public void MultiTesting() {
            TestTool.RunTasks(4, () => NHWith.Transaction(delegate {
                                                              Get();
                                                              FindAll_With_Paging();
                                                              GetByCategory();
                                                          }));
        }

        /// <summary>
        /// Product 정보를 Procedure에서 얻을 때, association 된 Category, Supplier 정보는 기본 hbm의 association 특성에 따라 가져온다.
        /// </summary>
        /*
		CREATE PROCEDURE GetProductsByCategoryId
		@CategoryId int = 1
		AS
		BEGIN
				-- exec dbo.GetProductsByCategoryId 1
		set nocount on;

		select  P.ProductId 
			   ,P.ProductName 
			   ,P.SupplierId
			   ,P.CategoryId
			   ,P.QuantityPerUnit
			   ,P.UnitPrice
			   ,P.UnitsInStock
			   ,P.UnitsOnOrder
			   ,P.ReorderLevel
			   ,P.Discontinued
			   ,C.CategoryName 
			   ,C.Description
		  from Products P inner join Categories C on P.CategoryID = C.CategoryID
		 where P.CategoryID = @CategoryID  
		*/
        [Test]
        public void GetProductsByCategoryId() {
            //IQuery query = UnitOfWork.CurrentSession.GetNamedQuery("GetProductsByCategoryId");
            // query.SetInt32("CategoryId", 1);

            var query = UnitOfWork.CurrentSession.GetNamedQuery("GetProductsByCategoryId", new NHParameter("CategoryId", 1));

            var products = query.List<Product>();

            foreach(var product in products) {
                Console.WriteLine(ReflectionTool.ObjectToString(product, true));
                Console.WriteLine("\t Category: " + product.Category.Name);
                Console.WriteLine("\t SuppliedBy: " + product.SuppliedBy.ContactTitle);
            }
        }
    }
}