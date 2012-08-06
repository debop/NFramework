using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace NSoft.NFramework.LinqEx.Queries {
    /// <summary>
    /// 정적인 Predicate를 만들어서 사용하면 성능상의 효과가 크다.
    /// </summary>
    [TestFixture]
    public class PredicateBuilderFixture {
        private IList<Product> _products;

        [TestFixtureSetUp]
        public void ClassSetUp() {
            _products =
                new List<Product>
                {
                    new Product
                    {
                        Description = @"가전제품이고 집에서 쓰는 것이고, 삼성제품이고, 컴퓨터 부품입니다. 모니터예요.",
                        LastSale = DateTime.Today.AddDays(-40)
                    },
                    new Product
                    {
                        Description = @"가전제품이고 집에서 쓰는 것이고, LG제품이고, 컴퓨터 부품입니다. 본체예요.",
                        LastSale = DateTime.Today.AddDays(-10)
                    }
                };
        }

        [Test]
        public void ContainsInDescriptoinTest() {
            var monitor = Product.ContainsInDescriptionExpr("컴퓨터", "모니터");
            var computer = Product.ContainsInDescriptionExpr("컴퓨터", "본체").And(Product.IsSellingExpression);
            var predicate = monitor.Or(computer).Compile();

            var query = _products.Where(predicate);

            Assert.AreEqual(2, query.Count());

            foreach(Product p in query)
                Console.WriteLine(p.Description);
        }

        [Test]
        public void IsSellingExpression() {
            var query = _products.Where(p => Product.IsSellingExpression.Invoke(p));

            Assert.AreEqual(1, query.Count());
            Assert.IsTrue(query.First().LastSale.GetValueOrDefault(DateTime.Today) > DateTime.Today.AddDays(-30));

            foreach(Product p in query)
                Console.WriteLine(p.LastSale);
        }

        [Test]
        public void IsSellingFunction() {
            var query = _products.Where(Product.IsSellingFunction);

            Assert.AreEqual(1, query.Count());
            Assert.IsTrue(query.First().LastSale.GetValueOrDefault(DateTime.Today) > DateTime.Today.AddDays(-30));

            foreach(Product p in query)
                Console.WriteLine(p.LastSale);
        }

        public class Product {
            /// <summary>
            /// 제품 설명에 지정된 키워드가 있는지 알아보는 Predicate
            /// </summary>
            /// <param name="keywords"></param>
            /// <returns></returns>
            public static Expression<Func<Product, bool>> ContainsInDescriptionExpr(params string[] keywords) {
                var predicate = PredicateBuilder.False<Product>();

                foreach(string keyword in keywords) {
                    string temp = keyword;
                    predicate = predicate.Or(p => p.Description.Contains(temp));
                }
                return LinqTool.Expr(predicate);
            }

            /// <summary>
            /// 제품 판매 여부를 나타내는 Predicate
            /// </summary>
            /// <returns></returns>
            public static Expression<Func<Product, bool>> IsSellingExpression {
                get {
                    return p => !p.Discontinued &&
                                p.LastSale.HasValue &&
                                p.LastSale.Value > DateTime.Today.AddDays(-30);
                }
            }

            private static Func<Product, bool> _isSellingFunction;

            public static Func<Product, bool> IsSellingFunction {
                get { return _isSellingFunction = _isSellingFunction ?? IsSellingExpression.Compile(); }
            }

            public bool Discontinued { get; set; }
            public DateTime? LastSale { get; set; }
            public string Description { get; set; }
        }
    }
}