using System;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;

namespace NSoft.NFramework.LinqEx.PivotTables {
    [TestFixture]
    public class PivotBuilderFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static string[] FunctionNames = PivotBuilder.AggregateFunctionNames;

        private static IList<OrderByYear> GetOrderByYears(int? count) {
            const int DefaultCount = 99;

            var entityCount = count ?? DefaultCount;
            var maxEmployeeId = Environment.ProcessorCount * 2 + 1;

            var list = new List<OrderByYear>(entityCount);


            for(int i = 0; i < entityCount; i++) {
                list.Add(new OrderByYear
                         {
                             OrderId = i,
                             EmployeeId = Rnd.Next(1, maxEmployeeId),
                             OrderYear = Rnd.Next(2000, 2010),
                             Price = (Rnd.NextDouble() * Rnd.Next(10000)).AsDecimalNullable()
                         });
            }
            return list;
        }

        private static readonly IList<OrderByYear> orders = GetOrderByYears(null);

        [Test]
        public void Pivot_Build_With_AggregateFunction_Sum() {
            var pivotTable =
                PivotBuilder
                    .BuildPivotTable<OrderByYear, decimal?, decimal>(orders,
                                                                     o => o.EmployeeId,
                                                                     o => o.OrderYear,
                                                                     (s, v) => s + v.GetValueOrDefault(0m),
                                                                     o => o.Price);

            VerifyPivotTable(pivotTable);
        }

        [Test]
        public void Pivot_Build_With_AggregateFunction_Count() {
            var pivotTable =
                PivotBuilder
                    .BuildPivotTable<OrderByYear, decimal?, int>(orders,
                                                                 o => o.EmployeeId,
                                                                 o => o.OrderYear,
                                                                 (s, v) => s + 1,
                                                                 o => o.Price);
            VerifyPivotTable(pivotTable);
        }

        [Test]
        public void Pivot_Build_With_AggregateFunction_Avg() {
            var pivotTable =
                PivotBuilder
                    .BuildPivotTable<OrderByYear, decimal?, decimal>(orders,
                                                                     o => o.EmployeeId,
                                                                     o => o.OrderYear,
                                                                     (s, v) => ((s + v) / 2.0m).AsDecimal(),
                                                                     o => o.Price);
            VerifyPivotTable(pivotTable);
        }

        [Test, TestCaseSource("FunctionNames")]
        public void PivotBuild_By_AggregateFunctionFuntionName_Property_Expression(string aggFunctionName) {
            var pivotTable =
                PivotBuilder
                    .BuildPivotTable<OrderByYear, decimal?, decimal>(orders,
                                                                     o => o.EmployeeId,
                                                                     o => o.OrderYear,
                                                                     aggFunctionName,
                                                                     o => o.Price);

            VerifyPivotTable(pivotTable);
        }

        [Test, TestCaseSource("FunctionNames")]
        public void PivotBuild_By_AggregateFunctionFuntionName_Property_Expression_ViceVersa(string aggFunctionName) {
            var pivotTable =
                PivotBuilder
                    .BuildPivotTable<OrderByYear, decimal?, decimal>(orders,
                                                                     o => o.OrderYear,
                                                                     o => o.EmployeeId,
                                                                     aggFunctionName,
                                                                     o => o.Price);

            VerifyPivotTable(pivotTable);
        }

        [Test, TestCaseSource("FunctionNames")]
        public void PivotBuild_By_AggregateFunctionFuntionName_Property_Name(string aggFunctionName) {
            var pivotTable =
                PivotBuilder
                    .BuildPivotTable<OrderByYear, decimal?, decimal>(orders,
                                                                     "EmployeeId",
                                                                     "OrderYear",
                                                                     aggFunctionName,
                                                                     "Price");

            VerifyPivotTable(pivotTable);
        }

#if !SILVERIGHT
        protected void VerifyPivotTable(DataTable pivotTable) {
            Assert.IsFalse(pivotTable.HasErrors);
            Assert.Greater(pivotTable.Rows.Count, 0);
            Assert.Greater(pivotTable.Columns.Count, 0);


            if(IsDebugEnabled)
                log.Debug(pivotTable.AsText());
        }
#endif

        [Serializable]
        public class OrderByYear {
            public int OrderId { get; set; }
            public int EmployeeId { get; set; }
            public int OrderYear { get; set; }
            public decimal? Price { get; set; }
        }
    }
}