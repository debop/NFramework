using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.LinqEx {
    /// <summary>
    /// Pivot Table을 빌드합니다.
    /// </summary>
    public static class PivotBuilder {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 제공되는 집계함수들입니다. ("AbsAvg", "AbsMax", "AbsMin", "AbsSum", "Avg", "Count", "LongCount", "Max", "Min", "Norm", "RootMeanSquare", "StDev", "Sum", "Variance")
        /// </summary>
        public static string[] AggregateFunctionNames = new string[]
                                                        {
                                                            "AbsAvg",
                                                            "AbsMax",
                                                            "AbsMin",
                                                            "AbsSum",
                                                            "Avg",
                                                            "Count",
                                                            "LongCount",
                                                            "Max",
                                                            "Min",
                                                            "Norm",
                                                            "RootMeanSquare",
                                                            "StDev",
                                                            "Sum",
                                                            "Variance"
                                                        };

        /// <summary>
        /// 지정한 시퀀스 정보로부터, Pivot Table형식의 통계 정보를 추출하도록 합니다.
        /// </summary>
        /// <typeparam name="T">원본 정보의 형식</typeparam>
        /// <typeparam name="TValue">원본 정보 중의 Pivot Table의 값으로 계산하기 위한 대상 컬럼의 수형</typeparam>
        /// <typeparam name="TResult">Pivot Table의 값의 Aggregate된 결과 값의 형식(SUM, COUNT 등은 컬럼 수형과는 다릅니다)</typeparam>
        /// <param name="sequence">데이타 시퀀스</param>
        /// <param name="rowExpr">행을 표현할 속성을 지정하는 Expression (ex. item=>item.EmployeeId)</param>
        /// <param name="colExpr">컬럼을 표현할 속성을 지정하는 Expression (ex. item=>item.OrderYear)</param>
        /// <param name="aggregateFunc">PIVOT의 값에 표현할 집계함수 (ex. 합:(s, n)=>s+n, Count:(s, n)=> s+1 )</param>
        /// <param name="valueExpr">집계를 위한 값을 나타내는 Expression (ex. item=>item.Price)</param>
        /// <returns>Pivoting된 정보를 담은 DataTable</returns>
        public static DataTable BuildPivotTable<T, TValue, TResult>(this IEnumerable<T> sequence,
                                                                    Expression<Func<T, object>> rowExpr,
                                                                    Expression<Func<T, object>> colExpr,
                                                                    Func<TResult, TValue, TResult> aggregateFunc,
                                                                    Expression<Func<T, TValue>> valueExpr) {
            return BuildPivotTableInternal(sequence,
                                           rowExpr,
                                           colExpr,
                                           colValues => colValues.Aggregate(default(TResult), aggregateFunc),
                                           valueExpr);
        }

        /// <summary>
        /// 지정한 시퀀스 정보로부터, Pivot Table형식의 통계 정보를 추출하도록 합니다.
        /// </summary>
        /// <typeparam name="T">원본 정보의 형식</typeparam>
        /// <typeparam name="TValue">원본 정보 중의 Pivot Table의 값으로 계산하기 위한 대상 컬럼의 수형</typeparam>
        /// <typeparam name="TResult">Pivot Table의 값의 Aggregate된 결과 값의 형식(SUM, COUNT 등은 컬럼 수형과는 다릅니다)</typeparam>
        /// <param name="sequence">데이타 시퀀스</param>
        /// <param name="rowExpr">행을 표현할 속성을 지정하는 Expression (ex. item=>item.EmployeeId)</param>
        /// <param name="colExpr">컬럼을 표현할 속성을 지정하는 Expression (ex. item=>item.OrderYear)</param>
        /// <param name="aggregateFuncName">PIVOT의 값에 표현할 집계함수명 (AbsAvg, AbsMax, AbsMin, AbsSum, Avg, Count, LongCount, Max, Min, Norm, RootMeanSquare, StDev, Sum, Variance) </param>
        /// <param name="valueExpr">집계를 위한 값을 나타내는 Expression (ex. item=>item.Price)</param>
        /// <returns>Pivoting된 정보를 담은 DataTable</returns>
        [CLSCompliant(false)]
        public static DataTable BuildPivotTable<T, TValue, TResult>(this IEnumerable<T> sequence,
                                                                    Expression<Func<T, object>> rowExpr,
                                                                    Expression<Func<T, object>> colExpr,
                                                                    string aggregateFuncName,
                                                                    Expression<Func<T, TValue>> valueExpr)
            where TResult : IConvertible {
            return BuildPivotTableInternal(sequence,
                                           rowExpr,
                                           colExpr,
                                           colValues => colValues.AggregateValues<TValue, TResult>(aggregateFuncName),
                                           valueExpr);
        }

        /// <summary>
        /// 지정한 시퀀스 정보로부터, Pivot Table형식의 통계 정보를 추출하도록 합니다.
        /// </summary>
        /// <typeparam name="T">원본 정보의 형식</typeparam>
        /// <typeparam name="TValue">원본 정보 중의 Pivot Table의 값으로 계산하기 위한 대상 컬럼의 수형</typeparam>
        /// <typeparam name="TResult">Pivot Table의 값의 Aggregate된 결과 값의 형식(SUM, COUNT 등은 컬럼 수형과는 다릅니다)</typeparam>
        /// <param name="sequence">데이타 시퀀스</param>
        /// <param name="rowPropertyName">행을 표현할 속성명(ex. "EmployeeId")</param>
        /// <param name="colPropertyName">컬럼을 표현할 속성명(ex. "OrderYear")</param>
        /// <param name="aggregateFuncName">PIVOT의 값에 표현할 집계함수명 (AbsAvg, AbsMax, AbsMin, AbsSum, Avg, Count, LongCount, Max, Min, Norm, RootMeanSquare, StDev, Sum, Variance) </param>
        /// <param name="valuePropertyName">집계를 위한 값을 나타내는 Expression (ex. item=>item.Price)</param>
        /// <returns>Pivoting된 정보를 담은 DataTable</returns>
        [CLSCompliant(false)]
        public static DataTable BuildPivotTable<T, TValue, TResult>(this IEnumerable<T> sequence,
                                                                    string rowPropertyName,
                                                                    string colPropertyName,
                                                                    string aggregateFuncName,
                                                                    string valuePropertyName)
            where TResult : IConvertible {
            if(IsDebugEnabled)
                log.Debug("시퀀스 정보로부터 Pivot Table을 생성합니다... " +
                          "rowPropertyName=[{0}], colPropertyName=[{1}], aggregateFuncName=[{2}], valuePropertyName=[{3}]",
                          rowPropertyName, colPropertyName, aggregateFuncName, valuePropertyName);

            var rowExpr = LinqEx.DynamicExpression.ParseLambda<T, object>(rowPropertyName);
            var colExpr = LinqEx.DynamicExpression.ParseLambda<T, object>(colPropertyName);
            var valueExpr = LinqEx.DynamicExpression.ParseLambda<T, TValue>(valuePropertyName);

            return BuildPivotTable<T, TValue, TResult>(sequence, rowExpr, colExpr, aggregateFuncName, valueExpr);
        }

        /// <summary>
        /// 지정한 시퀀스 정보로부터, Pivot Table형식의 통계 정보를 추출하도록 합니다.
        /// </summary>
        /// <typeparam name="T">원본 정보의 형식</typeparam>
        /// <typeparam name="TValue">원본 정보 중의 Pivot Table의 값으로 계산하기 위한 대상 컬럼의 수형</typeparam>
        /// <typeparam name="TResult">Pivot Table의 값의 Aggregate된 결과 값의 형식(SUM, COUNT 등은 컬럼 수형과는 다릅니다)</typeparam>
        /// <param name="sequence">데이타 시퀀스</param>
        /// <param name="rowPropertyName">행을 표현할 속성명(ex. "EmployeeId")</param>
        /// <param name="colPropertyName">컬럼을 표현할 속성명(ex. "OrderYear")</param>
        /// <param name="aggregateFunc">PIVOT의 값에 표현할 집계함수 (ex. 합:(s, n)=>s+n, Count:(s, n)=> s+1 )</param>
        /// <param name="valuePropertyName">집계를 위한 값을 나타내는 Expression (ex. item=>item.Price)</param>
        /// <returns>Pivoting된 정보를 담은 DataTable</returns>
        public static DataTable BuildPivotTable<T, TValue, TResult>(this IEnumerable<T> sequence,
                                                                    string rowPropertyName,
                                                                    string colPropertyName,
                                                                    Func<TResult, TValue, TResult> aggregateFunc,
                                                                    string valuePropertyName) {
            if(IsDebugEnabled)
                log.Debug("시퀀스 정보로부터 Pivot Table을 생성합니다... " +
                          "rowPropertyName=[{0}], colPropertyName=[{1}], aggregateFunc=[{2}], valuePropertyName=[{3}]",
                          rowPropertyName, colPropertyName, aggregateFunc, valuePropertyName);

            var rowExpr = LinqEx.DynamicExpression.ParseLambda<T, object>(rowPropertyName);
            var colExpr = LinqEx.DynamicExpression.ParseLambda<T, object>(colPropertyName);
            var valueExpr = LinqEx.DynamicExpression.ParseLambda<T, TValue>(valuePropertyName);

            return BuildPivotTable(sequence, rowExpr, colExpr, aggregateFunc, valueExpr);
        }

        /// <summary>
        /// 지정한 시퀀스 정보로부터, Pivot Table형식의 통계 정보를 추출하도록 합니다.
        /// </summary>
        /// <typeparam name="T">원본 정보의 형식</typeparam>
        /// <typeparam name="TValue">원본 정보 중의 Pivot Table의 값으로 계산하기 위한 대상 컬럼의 수형</typeparam>
        /// <typeparam name="TResult">Pivot Table의 값의 Aggregate된 결과 값의 형식(SUM, COUNT 등은 컬럼 수형과는 다릅니다)</typeparam>
        /// <param name="sequence">데이타 시퀀스</param>
        /// <param name="rowExpr">행을 표현할 속성을 지정하는 Expression (ex. item=>item.EmployeeId)</param>
        /// <param name="colExpr">컬럼을 표현할 속성을 지정하는 Expression (ex. item=>item.OrderYear)</param>
        /// <param name="aggregateColumnValueFunc">PIVOT의 값에 표현할 집계함수 (ex. 합:(s, n)=>s+n, Count:(s, n)=> s+1 )</param>
        /// <param name="valueExpr">집계를 위한 값을 나타내는 Expression (ex. item=>item.Price)</param>
        /// <returns>Pivoting된 정보를 담은 DataTable</returns>
        internal static DataTable BuildPivotTableInternal<T, TValue, TResult>(this IEnumerable<T> sequence,
                                                                              Expression<Func<T, object>> rowExpr,
                                                                              Expression<Func<T, object>> colExpr,
                                                                              Func<IList<TValue>, TResult> aggregateColumnValueFunc,
                                                                              Expression<Func<T, TValue>> valueExpr) {
            var entities = sequence.ToList();

            if(IsDebugEnabled)
                log.Debug("시퀀스로부터 Pivot Table을 빌드합니다... 시퀀스 Data 갯수=[{0}]", entities.Count);

            var rowFunc = rowExpr.Compile();
            var colFunc = colExpr.Compile();
            var valueFunc = valueExpr.Compile();

            var rows = entities.Select(rowFunc).Distinct().OrderBy(r => r).ToList();
            var cols = entities.Select(colFunc).Distinct().OrderBy(c => c).ToList();
            var colNames = cols.Select(c => c.ToString()).ToList();

            var rowPropertyName = LinqEx.LinqTool.FindMemberName(rowExpr.Body);

            var dataTable = CreatePivotTable(rowPropertyName,
                                             rowExpr.Body.Type,
                                             colNames,
                                             typeof(TResult));

            try {
                var initialTable = dataTable.Copy();

                if(IsDebugEnabled)
                    log.Debug("병렬 방식으로 PivotTalble을 빌드합니다...");

                Parallel.ForEach<object, DataTable>(
                    rows,
                    initialTable.Copy,
                    (row, loopState, rowTable) => {
                        var dataRow = rowTable.NewRow();

                        dataRow[0] = row;

                        var rowValues = entities.Where(item => Equals(rowFunc(item), row)).ToList();

                        var colIndex = 1;

                        foreach(var col in cols) {
                            var column = col;

                            // NOTE: ROW, COL에 해당하는 VALUE만을 추출하여, 집계합수로 집계를 구한다.
                            //
                            var colValues = rowValues.Where(item => Equals(colFunc(item), column)).Select(valueFunc).ToList();
                            var aggregateResult = aggregateColumnValueFunc(colValues);

                            if(IsDebugEnabled)
                                log.Debug("Row[{0}], Column[{1}] 의 집계 값 [{2}]을 계산했습니다.", row, column, aggregateResult);

                            dataRow[colIndex] = aggregateResult;
                            colIndex++;
                        }

                        rowTable.Rows.Add(dataRow);

                        return rowTable;
                    },
                    localTable => {
                        lock(dataTable)
                            dataTable.Merge(localTable, true);

                        localTable.Dispose();
                    });

                if(IsDebugEnabled)
                    log.Debug("병렬 방식으로 PivotTalble을 빌드했습니다!!!");

                // 병렬 수행에 따라 정렬이 되지 않습니다. 따로 정렬을 수행하여 반환합니다.
                //
                var dv = dataTable.DefaultView;
                dv.Sort = rowPropertyName;
                return dv.ToTable();
            }
            finally {
                dataTable.Dispose();
            }
        }

        /// <summary>
        /// 지정된 시퀀스에 대해 집계함수를 수행합니다.<br/>
        /// 지원하는 집계함수 : <see cref="AggregateFunctionNames"/> (AbsAvg, AbsMax, AbsMin, AbsSum, Avg, Count, LongCount, Max, Min, Norm, RootMeanSquare, StDev, Sum, Variance)
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="aggregateFunctionName"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static TResult AggregateValues<TValue, TResult>(this IEnumerable<TValue> sequence, string aggregateFunctionName)
            where TResult : IConvertible {
            TResult aggregateResult;

            if(IsDebugEnabled)
                log.Debug("시퀀스 내용을 지정된 함수[{0}]로 집계합니다...", aggregateFunctionName);

            var itemCount = sequence.Take(2).Count();

            if(itemCount == 0)
                return default(TResult);

            switch(aggregateFunctionName.ToLower()) {
                case "absavg":
                    aggregateResult = sequence.Select(x => Math.Abs(x.AsDouble())).Average().AsValue<TResult>();
                    break;

                case "absmax":
                    aggregateResult = sequence.Select(x => Math.Abs(x.AsDouble())).Max().AsValue<TResult>();
                    break;

                case "absmin":
                    aggregateResult = sequence.Select(x => Math.Abs(x.AsDouble())).Min().AsValue<TResult>();
                    break;

                case "abssum":
                    aggregateResult = sequence.Select(x => Math.Abs(x.AsDouble())).Sum().AsValue<TResult>();
                    break;

                case "avg":
                    aggregateResult = sequence.Select(x => x.AsDouble()).Average().AsValue<TResult>();
                    break;

                case "count":
                    aggregateResult = sequence.Count().AsValue<TResult>();
                    break;

                case "longcount":
                    aggregateResult = sequence.LongCount().AsValue<TResult>();
                    break;

                case "max":
                    aggregateResult = sequence.Max().AsValue<TResult>();
                    break;

                case "min":
                    aggregateResult = sequence.Min().AsValue<TResult>();
                    break;

                case "norm":
                    aggregateResult = sequence.Select(x => x.AsDouble()).AsNorm().AsValue<TResult>();
                    break;

                case "rootmeansquare":
                    aggregateResult = (itemCount < 2)
                                          ? default(TResult)
                                          : sequence.Select(x => x.AsDouble()).AsRootMeanSquare().AsValue<TResult>();
                    break;

                case "stdev":
                    aggregateResult = (itemCount < 2)
                                          ? default(TResult)
                                          : sequence.Select(x => x.AsDouble()).AsStDev().AsValue<TResult>();
                    break;

                case "sum":
                    aggregateResult = sequence.Select(x => x.AsDouble()).Sum().AsValue<TResult>();
                    break;

                case "variance":
                    aggregateResult = (itemCount < 2)
                                          ? default(TResult)
                                          : sequence.Select(x => x.AsDouble()).AsVariance().AsValue<TResult>();

                    break;

                default:
                    throw new NotSupportedException("지정된 집계함수는 지원되지 않습니다. 집계함수=" + aggregateFunctionName);
            }

            if(IsDebugEnabled)
                log.Debug("시퀀스 내용을 지정된 함수[{0}] 로 집계한 결과=[{1}]", aggregateFunctionName, aggregateResult);

            return aggregateResult;
        }

        /// <summary>
        /// 지정한 ROW, COLUMNS 들에 대한 PIVOT TABLE을 빌드합니다.
        /// </summary>
        /// <param name="rowName"></param>
        /// <param name="rowType"></param>
        /// <param name="columnNames"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        internal static DataTable CreatePivotTable(string rowName, Type rowType, IEnumerable<string> columnNames, Type valueType) {
            if(IsDebugEnabled)
                log.Debug("PIVOT TABLE을 표현하기 위해 DataTable을 빌드합니다..." +
                          "ROW Name=[{0}], Type=[{1}],  COLUMNS Names=[{2}], Type=[{3}]", rowName, rowType,
                          columnNames.CollectionToString(), valueType);

            var table = new DataTable { Locale = CultureInfo.InvariantCulture };

            table.BeginInit();
            try {
                var rowColumn = new DataColumn(rowName, rowType.IsNullableType()
                                                            ? Nullable.GetUnderlyingType(rowType)
                                                            : rowType);
                table.Columns.Add(rowColumn);

                table.Columns.AddRange(columnNames
                                           .Select(name => new DataColumn(name, valueType.IsNullableType()
                                                                                    ? Nullable.GetUnderlyingType(valueType)
                                                                                    : valueType))
                                           .ToArray());

                table.PrimaryKey = new[] { rowColumn };
            }
            finally {
                table.EndInit();
            }

            return table;
        }

        /// <summary>
        /// 지정한 DataTable의 내용을 문자열로 만듭니다
        /// </summary>
        /// <param name="dataTable">문자열로 표현할 데이타 테이블</param>
        /// <param name="columnDelimiter">컬럼간의 구분 값(",")</param>
        /// <param name="includeHeader">컬럼 헤더 정보를 포함할 것인자</param>
        /// <returns></returns>
        public static string AsText(this DataTable dataTable, string columnDelimiter = ",", bool includeHeader = true) {
            if(IsDebugEnabled)
                log.Debug("DataTable 내용을 Dump 하여 문자열로 반환합니다. columnDelimiter=[{0}], includeHeader=[{1}]", columnDelimiter, includeHeader);

            var sb = new StringBuilder(dataTable.Rows.Count * dataTable.Columns.Count * 4);

            columnDelimiter = columnDelimiter ?? ",";

            if(includeHeader) {
                foreach(DataColumn col in dataTable.Columns)
                    sb.Append(col.ColumnName).Append(columnDelimiter);

                sb.AppendLine();
            }

            foreach(DataRow row in dataTable.Rows) {
                foreach(DataColumn col in dataTable.Columns)
                    sb.Append(row[col]).Append(columnDelimiter);

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}