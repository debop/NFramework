using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Data.Persisters;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data {
    public static partial class AdoTool {
        /// <summary>
        /// <paramref name="table"/>의 정보를 읽어들여, <see cref="TrimNameMapper"/> 매핑정보를 기반으로 T 수형의 인스턴스를 생성하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">생성할 Persistent object의 수형</typeparam>
        /// <param name="table">읽을 DataTable</param>
        /// <param name="propertyNamesToExclude">매핑에서 제외할 속성명</param>
        /// <returns>Persistent object의 열거자</returns>
        public static IList<T> Map<T>(this DataTable table, string[] propertyNamesToExclude) where T : class {
            return Map(table.CreateDataReader(), ActivatorTool.CreateInstance<T>, propertyNamesToExclude);
        }

        /// <summary>
        /// <paramref name="table"/>의 정보를 읽어들여, <see cref="TrimNameMapper"/> 매핑정보를 기반으로 T 수형의 인스턴스를 생성하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">생성할 Persistent object의 수형</typeparam>
        /// <param name="table">읽을 DataTable</param>
        /// <param name="firstResult">Persistent object를 만들 첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">Persistent object를 만들 최대 레코드 수 (0이거나 null이면 DataReader 끝까지)</param>
        /// <param name="propertyNamesToExclude">매핑에서 제외할 속성명</param>
        /// <returns>Persistent object의 열거자</returns>
        public static IList<T> Map<T>(this DataTable table, int firstResult = 0, int maxResults = 0,
                                      string[] propertyNamesToExclude = null) where T : class {
            return Map(table.CreateDataReader(), ActivatorTool.CreateInstance<T>, firstResult, maxResults, propertyNamesToExclude);
        }

        /// <summary>
        /// <paramref name="table"/>의 정보를 읽어들여, <see cref="TrimNameMapper"/> 매핑정보를 기반으로 T 수형의 인스턴스를 생성하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">생성할 Persistent object의 수형</typeparam>
        /// <param name="table">읽을 DataTable</param>
        /// <param name="propertyNamesToExclude">매핑에서 제외할 속성명</param>
        /// <param name="targetFactory">Persistent object 생성 함수</param>
        /// <returns>Persistent object의 열거자</returns>
        public static IList<T> Map<T>(this DataTable table, Func<T> targetFactory, string[] propertyNamesToExclude) {
            return Map(table.CreateDataReader(), targetFactory, propertyNamesToExclude);
        }

        /// <summary>
        /// <paramref name="table"/>의 정보를 읽어들여, <see cref="TrimNameMapper"/> 매핑정보를 기반으로 T 수형의 인스턴스를 생성하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">생성할 Persistent object의 수형</typeparam>
        /// <param name="table">읽을 DataTable</param>
        /// <param name="firstResult">Persistent object를 만들 첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">Persistent object를 만들 최대 레코드 수 (0이거나 null이면 DataReader 끝까지)</param>
        /// <param name="targetFactory">Persistent object 생성 함수</param>
        /// <param name="propertyNamesToExclude">매핑에서 제외할 속성명</param>
        /// <returns>Persistent object의 열거자</returns>
        public static IList<T> Map<T>(this DataTable table, Func<T> targetFactory, int firstResult = 0, int maxResults = 0,
                                      string[] propertyNamesToExclude = null) {
            return Map(table.CreateDataReader(), targetFactory, firstResult, maxResults, propertyNamesToExclude);
        }

        /// <summary>
        /// <paramref name="table"/>의 정보를 읽어들여, <see cref="TrimNameMapper"/> 매핑정보를 기반으로 T 수형의 인스턴스를 생성하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">생성할 Persistent object의 수형</typeparam>
        /// <param name="table">읽을 DataTable</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 속성명</param>
        /// <returns>Persistent object의 열거자</returns>
        public static IList<T> Map<T>(this DataTable table, params Expression<Func<T, object>>[] propertyExprsToExclude) where T : class {
            return Map(table.CreateDataReader(), ActivatorTool.CreateInstance<T>, propertyExprsToExclude);
        }

        /// <summary>
        /// <paramref name="table"/>의 정보를 읽어들여, <see cref="TrimNameMapper"/> 매핑정보를 기반으로 T 수형의 인스턴스를 생성하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">생성할 Persistent object의 수형</typeparam>
        /// <param name="table">읽을 DataTable</param>
        /// <param name="firstResult">Persistent object를 만들 첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">Persistent object를 만들 최대 레코드 수 (0이거나 null이면 DataReader 끝까지)</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>Persistent object의 열거자</returns>
        public static IList<T> Map<T>(this DataTable table, int firstResult, int maxResults,
                                      params Expression<Func<T, object>>[] propertyExprsToExclude) where T : class {
            return Map(table.CreateDataReader(), ActivatorTool.CreateInstance<T>, firstResult, maxResults, propertyExprsToExclude);
        }

        /// <summary>
        /// <paramref name="table"/>의 정보를 읽어들여, <see cref="TrimNameMapper"/> 매핑정보를 기반으로 T 수형의 인스턴스를 생성하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">생성할 Persistent object의 수형</typeparam>
        /// <param name="table">읽을 DataTable</param>
        /// <param name="targetFactory">Persistent object 생성 함수</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>Persistent object의 열거자</returns>
        public static IList<T> Map<T>(this DataTable table, Func<T> targetFactory,
                                      params Expression<Func<T, object>>[] propertyExprsToExclude) {
            return Map(table.CreateDataReader(), targetFactory, propertyExprsToExclude);
        }

        /// <summary>
        /// <paramref name="table"/>의 정보를 읽어들여, <see cref="TrimNameMapper"/> 매핑정보를 기반으로 T 수형의 인스턴스를 생성하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">생성할 Persistent object의 수형</typeparam>
        /// <param name="table">읽을 DataTable</param>
        /// <param name="targetFactory">Persistent object 생성 함수</param>
        /// <param name="firstResult">Persistent object를 만들 첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">Persistent object를 만들 최대 레코드 수 (0이거나 null이면 DataReader 끝까지)</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>Persistent object의 열거자</returns>
        public static IList<T> Map<T>(this DataTable table, Func<T> targetFactory, int firstResult, int maxResults,
                                      params Expression<Func<T, object>>[] propertyExprsToExclude) {
            return Map(table.CreateDataReader(), targetFactory, firstResult, maxResults, propertyExprsToExclude);
        }

        /// <summary>
        /// <paramref name="table"/>의 정보를 Persistent object로 변환하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">Persistent object 의 수형</typeparam>
        /// <param name="table"><see cref="DataTable"/>원본 데이타 테이블</param>
        /// <param name="nameMapper">컬럼명-속성명 매퍼</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>Persistent object의 컬렉션</returns>
        public static IList<T> Map<T>(this DataTable table, INameMapper nameMapper,
                                      params Expression<Func<T, object>>[] propertyExprsToExclude) where T : class {
            var propertyNamesToExclude = propertyExprsToExclude.Select(expr => expr.Body.FindMemberName()).ToArray();
            return Map(table.CreateDataReader(), ActivatorTool.CreateInstance<T>, table.NameMapping(nameMapper, propertyNamesToExclude));
        }

        /// <summary>
        /// <paramref name="table"/>의 정보를 Persistent object로 변환하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">Persistent object 의 수형</typeparam>
        /// <param name="table"><see cref="DataTable"/>원본 데이타 테이블</param>
        /// <param name="nameMapper">컬럼명-속성명 매퍼</param>
        /// <param name="firstResult">Persistent object를 만들 첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">Persistent object를 만들 최대 레코드 수 (0이거나 null이면 DataReader 끝까지)</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>Persistent object의 컬렉션</returns>
        public static IList<T> Map<T>(this DataTable table,
                                      INameMapper nameMapper,
                                      int firstResult,
                                      int maxResults,
                                      params Expression<Func<T, object>>[] propertyExprsToExclude) where T : class {
            var propertyNamesToExclude = propertyExprsToExclude.Select(expr => expr.Body.FindMemberName()).ToArray();
            return Map(table.CreateDataReader(),
                       ActivatorTool.CreateInstance<T>,
                       table.NameMapping(nameMapper, propertyNamesToExclude),
                       firstResult,
                       maxResults);
        }

        /// <summary>
        /// <paramref name="table"/>의 정보를 Persistent object로 변환하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">Persistent object 의 수형</typeparam>
        /// <param name="table"><see cref="DataTable"/>원본 데이타 테이블</param>
        /// <param name="targetFactory">Persistent object 생성 함수</param>
        /// <param name="nameMapper">컬럼명-속성명 매퍼</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>Persistent object의 컬렉션</returns>
        public static IList<T> Map<T>(this DataTable table,
                                      Func<T> targetFactory,
                                      INameMapper nameMapper,
                                      params Expression<Func<T, object>>[] propertyExprsToExclude) {
            var propertyNamesToExclude = propertyExprsToExclude.Select(expr => expr.Body.FindMemberName()).ToArray();
            return Map(table.CreateDataReader(),
                       targetFactory,
                       table.NameMapping(nameMapper, propertyNamesToExclude));
        }

        /// <summary>
        /// <paramref name="table"/>의 정보를 Persistent object로 변환하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">Persistent object 의 수형</typeparam>
        /// <param name="table"><see cref="DataTable"/>원본 데이타 테이블</param>
        /// <param name="targetFactory">Persistent object 생성 함수</param>
        /// <param name="nameMapper">컬럼명-속성명 매퍼</param>
        /// <param name="firstResult">Persistent object를 만들 첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">Persistent object를 만들 최대 레코드 수 (0이거나 null이면 DataReader 끝까지)</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성</param>
        /// <returns>Persistent object의 컬렉션</returns>
        public static IList<T> Map<T>(this DataTable table, Func<T> targetFactory,
                                      INameMapper nameMapper,
                                      int firstResult,
                                      int maxResults,
                                      params Expression<Func<T, object>>[] propertyExprsToExclude) {
            var propertyNamesToExclude = propertyExprsToExclude.Select(expr => expr.Body.FindMemberName()).ToArray();
            return Map(table.CreateDataReader(),
                       targetFactory,
                       table.NameMapping(nameMapper, propertyNamesToExclude),
                       firstResult,
                       maxResults);
        }

        /// <summary>
        /// <paramref name="table"/>의 정보를 Persistent object로 변환하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">Persistent object 의 수형</typeparam>
        /// <param name="table"><see cref="DataTable"/></param>
        /// <param name="nameMap">컬럼명-속성명 매핑정보</param>
        /// <param name="firstResult">Persistent object를 만들 첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">Persistent object를 만들 최대 레코드 수 (0이거나 null이면 DataReader 끝까지)</param>
        /// <returns>Persistent object의 컬렉션</returns>
        public static IList<T> Map<T>(this DataTable table, INameMap nameMap, int firstResult = 0, int maxResults = 0) where T : class {
            return Map(table.CreateDataReader(), ActivatorTool.CreateInstance<T>, nameMap, firstResult, maxResults);
        }

        /// <summary>
        /// <paramref name="table"/>의 정보를 Persistent object로 변환하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">Persistent object 의 수형</typeparam>
        /// <param name="table"><see cref="DataTable"/></param>
        /// <param name="nameMap">컬럼명-속성명 매핑정보</param>
        /// <param name="firstResult">Persistent object를 만들 첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">Persistent object를 만들 최대 레코드 수 (0이거나 null이면 DataReader 끝까지)</param>
        /// <param name="targetFactory">Persistent object 생성 함수</param>
        /// <returns>Persistent object의 컬렉션</returns>
        public static IList<T> Map<T>(this DataTable table, Func<T> targetFactory, INameMap nameMap, int firstResult = 0,
                                      int maxResults = 0) {
            return Map(table.CreateDataReader(), targetFactory, nameMap, firstResult, maxResults);
        }

        /// <summary>
        /// <paramref name="table"/>의 정보를 Persistent object로 변환하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">Persistent object 의 수형</typeparam>
        /// <param name="table"><see cref="DataTable"/></param>
        /// <param name="rowPersister">Persister</param>
        /// <param name="firstResult">Persistent object를 만들 첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">Persistent object를 만들 최대 레코드 수 (0이거나 null이면 DataReader 끝까지)</param>
        /// <returns>Persistent object의 컬렉션</returns>
        public static IList<T> Map<T>(this DataTable table, IRowPersister<T> rowPersister, int firstResult = 0, int maxResults = 0) {
            return Map(table, rowPersister.Persist, firstResult, maxResults);
        }

        /// <summary>
        /// <paramref name="table"/>의 정보를 Persistent object로 변환하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">Persistent object 의 수형</typeparam>
        /// <param name="table"><see cref="DataTable"/></param>
        /// <param name="rowMapper">매핑 함수</param>
        /// <param name="firstResult">Persistent object를 만들 첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">Persistent object를 만들 최대 레코드 수 (0이거나 null이면 DataReader 끝까지)</param>
        /// <returns>Persistent object의 컬렉션</returns>
        public static IList<T> Map<T>(this DataTable table, Func<DataRow, T> rowMapper, int firstResult = 0, int maxResults = 0) {
            table.ShouldNotBeNull("table");
            rowMapper.ShouldNotBeNull("rowMapper");

            if(IsDebugEnabled)
                log.Debug("Mapping Function를 이용하여 DataTable로부터 [{0}] 수형의 객체를 생성합니다. firstResult=[{1}], maxResults=[{2}]",
                          typeof(T).Name, firstResult, maxResults);

            var firstCount = Math.Max(0, firstResult);
            var maxCount = maxResults;

            if(maxCount <= 0)
                maxCount = Int32.MaxValue;

            return
                table.Rows
                    .Cast<DataRow>()
                    .Skip(firstCount)
                    .Take(maxCount)
                    .Select(rowMapper)
                    .ToList();
        }

        /// <summary>
        /// <paramref name="table"/>의 레코드 중 시작 ~ 끝 범위 조건 (<paramref name="startPredicate"/>, <paramref name="endPredicate"/>)에 해당하는 DataRow만 매핑함수를 이용하여 매핑합니다.
        /// </summary>
        /// <example>
        /// <code>
        ///     // 모든 레코드
        ///		Map{User}(table, row=>new User(row), row=>true, row=>false);
        ///     
        ///		// 특정 컬럼 값이 있으면 시작하여 끝까지
        ///     Map{User}(table, row=>new User(row), row=>row.IsNull("UserNo")==false, row=>false);
        /// </code>
        /// </example>
        public static IEnumerable<T> Map<T>(this DataTable table, Func<DataRow, T> rowMapper, Func<DataRow, bool> startPredicate,
                                            Func<DataRow, bool> endPredicate = null) {
            rowMapper.ShouldNotBeNull("rowMapper");
            startPredicate.ShouldNotBeNull("startPredicate");
            // endPredicate.ShouldNotBeNull("endPredicate");

            endPredicate = endPredicate ?? (r => false);

            return
                table.Rows
                    .Cast<DataRow>()
                    .SkipWhile(row => !startPredicate(row))
                    .TakeWhile(row => !endPredicate(row))
                    .Select(rowMapper);
        }

        /// <summary>
        /// <paramref name="table"/>의 레코드 중 <paramref name="filterFunc"/>으로 필터링된 DataRow만 매핑을 수행합니다.
        /// </summary>
        public static IEnumerable<T> MapIf<T>(this DataTable table, Func<DataRow, T> rowMapper, Func<DataRow, bool> filterFunc) {
            table.ShouldNotBeNull("table");
            rowMapper.ShouldNotBeNull("rowMapper");
            filterFunc.ShouldNotBeNull("filterFunc");

            return
                table.Rows
                    .Cast<DataRow>()
                    .Where(filterFunc)
                    .Select(rowMapper);
        }

        /// <summary>
        /// <paramref name="continuationCondition"/>이 만족할 때까지만, Mapping을 수행합니다. 
        /// </summary>
        public static IEnumerable<T> MapWhile<T>(this DataTable table, Func<DataRow, T> rowMapper,
                                                 Func<DataRow, bool> continuationCondition) {
            table.ShouldNotBeNull("table");
            rowMapper.ShouldNotBeNull("rowMapper");
            continuationCondition.ShouldNotBeNull("continuationCondition");

            return
                table.Rows
                    .Cast<DataRow>()
                    .TakeWhile(continuationCondition)
                    .Select(rowMapper);
        }
    }
}