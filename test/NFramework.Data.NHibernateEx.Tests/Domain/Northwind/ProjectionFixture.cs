using System;
using NHibernate.Criterion;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain.Northwind;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Tests.Domain.Northwind {
    [TestFixture]
    public class ProjectionFixture : NorthwindDbTestFixtureBase {
        [Test]
        public void RowCount_By_Criteria_Projection() {
            var criteria = DetachedCriteria.For<Customer>().SetProjection(Projections.RowCount());

            var result = UnitOfWork.CurrentSession.GetExecutableCriteria<Customer>(criteria).List();
            Console.WriteLine(result[0]);

            long count = result[0].AsInt(-1);
            Assert.AreEqual(count, Repository<Customer>.Count());
        }

        [Test]
        public void Projection_With_GroupBy() {
            var projections
                = MaxUnitPriceAndAvgQuantity
                    .Add(Projections.GroupProperty("Product").As("Product")); // group by Product

            var summaries
                = Repository<OrderDetail>.ReportAll<OrderDetailSummary>(projections,
                                                                        new[] { NHibernate.Criterion.Order.Desc("MaxUnitPrice") });

            Assert.IsNotNull(summaries);
            Assert.Greater(summaries.Count, 0);
            // Print(summaries);
        }

        [Test]
        public void Projection_With_Criteria() {
            var summaries
                = Repository<OrderDetail>.ReportAll<OrderDetailSummary>(MaxUnitPriceAndAvgQuantity,
                                                                        new[] { Restrictions.Gt("Quantity", 2) });

            Assert.IsNotNull(summaries);
            Assert.Greater(summaries.Count, 0);
            // Print(summaries);
        }

        private static ProjectionList MaxUnitPriceAndAvgQuantity {
            get {
                return Projections.ProjectionList()
                    .Add(Projections.Max("UnitPrice").As("MaxUnitPrice")) // MAX(UnitPrice)
                    .Add(Projections.Avg("Quantity").As("AvgQuantity")); // AVG(Quantity)
            }
        }
    }

    /// <summary>
    /// NHibernate Projection을 이용한 Reporting을 위한 함수.
    /// </summary>
    public class OrderDetailSummary {
        public OrderDetailSummary() {}

        public OrderDetailSummary(decimal maxUnitPrice, double avgQuantity) {
            MaxUnitPrice = maxUnitPrice;
            AvgQuantity = avgQuantity;
        }

        public OrderDetailSummary(decimal maxUnitPrice, double avgQuantity, Product product) {
            MaxUnitPrice = maxUnitPrice;
            AvgQuantity = avgQuantity;
            Product = product;
        }

        public virtual Product Product { get; set; }

        public virtual decimal MaxUnitPrice { get; set; }

        public virtual double AvgQuantity { get; set; }

        public override string ToString() {
            return string.Format("OrderDetailSummary# MaxUnitPrice=[{0}], AvgQuantity=[{1}], Product=[{2}]", MaxUnitPrice, AvgQuantity,
                                 Product);
        }
    }
}