using System;
using System.Data;
using System.Linq;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Data.Persisters;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data {
    public static partial class AdoTool {
        /// <summary>
        /// DataTable 정보를 병렬로 Map을 수행합니다.
        /// </summary>
        /// <typeparam name="T">Persistent object 의 수형</typeparam>
        /// <param name="table"><see cref="DataTable"/></param>
        /// <param name="propertyNameToExclude">값 설정을 제외할 속성 명</param>
        /// <returns>Persistent object의 컬렉션</returns>
        public static ParallelQuery<T> MapAsParallel<T>(this DataTable table, params string[] propertyNameToExclude) where T : class {
            return MapAsParallel<T>(table, 0, 0, propertyNameToExclude);
        }

        /// <summary>
        /// DataTable 정보를 병렬로 Map을 수행합니다.
        /// </summary>
        /// <typeparam name="T">Persistent object 의 수형</typeparam>
        /// <param name="table"><see cref="DataTable"/></param>
        /// <param name="firstResult">Persistent object를 만들 첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">Persistent object를 만들 최대 레코드 수 (0이거나 null이면 DataReader 끝까지)</param>
        /// <param name="propertyNameToExclude">값 설정을 제외할 속성 명</param>
        /// <returns>Persistent object의 컬렉션</returns>
        public static ParallelQuery<T> MapAsParallel<T>(this DataTable table, int firstResult, int maxResults,
                                                        params string[] propertyNameToExclude) where T : class {
            table.ShouldNotBeNull("table");

            var nameMap = table.NameMapping(new TrimNameMapper(), propertyNameToExclude);

            return MapAsParallel(table, ActivatorTool.CreateInstance<T>, nameMap, firstResult, maxResults);
        }

        /// <summary>
        /// DataTable 정보를 병렬로 Map을 수행합니다.
        /// </summary>
        /// <typeparam name="T">Persistent object 의 수형</typeparam>
        /// <param name="table"><see cref="DataTable"/></param>
        /// <param name="nameMap">Name Map</param>
        /// <param name="targetFactory">Persistent object factory</param>
        /// <param name="firstResult">Persistent object를 만들 첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">Persistent object를 만들 최대 레코드 수 (0이거나 null이면 DataReader 끝까지)</param>
        /// <returns>Persistent object의 컬렉션</returns>
        public static ParallelQuery<T> MapAsParallel<T>(this DataTable table, Func<T> targetFactory, INameMap nameMap,
                                                        int firstResult = 0, int maxResults = 0) {
            var persister = new RowPersister<T>(new NameMapMapper(nameMap), targetFactory);
            return MapAsParallel(table, persister.Persist, firstResult, maxResults);
        }

        /// <summary>
        /// DataTable 정보를 병렬로 Map을 수행합니다.
        /// </summary>
        /// <typeparam name="T">Persistent object 의 수형</typeparam>
        /// <param name="table"><see cref="DataTable"/></param>
        /// <param name="nameMapper">Name Mapper</param>
        /// <param name="targetFactory">Persistent object factory</param>
        /// <param name="firstResult">Persistent object를 만들 첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">Persistent object를 만들 최대 레코드 수 (0이거나 null이면 DataReader 끝까지)</param>
        /// <returns>Persistent object의 컬렉션</returns>
        public static ParallelQuery<T> MapAsParallel<T>(this DataTable table, Func<T> targetFactory, INameMapper nameMapper,
                                                        int firstResult = 0, int maxResults = 0) {
            var persister = new RowPersister<T>(nameMapper, targetFactory);
            return MapAsParallel(table, persister.Persist, firstResult, maxResults);
        }

        /// <summary>
        /// DataTable 정보를 병렬로 Map을 수행합니다.
        /// </summary>
        /// <typeparam name="T">Persistent object 의 수형</typeparam>
        /// <param name="table"><see cref="DataTable"/></param>
        /// <param name="rowPersister"><see cref="DataRow"/>로부터 Persistent object를 빌드하는 Persister</param>
        /// <param name="firstResult">Persistent object를 만들 첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">Persistent object를 만들 최대 레코드 수 (0이거나 null이면 DataReader 끝까지)</param>
        /// <returns>Persistent object의 컬렉션</returns>
        public static ParallelQuery<T> MapAsParallel<T>(this DataTable table, IRowPersister<T> rowPersister, int firstResult = 0,
                                                        int maxResults = 0) {
            return MapAsParallel(table, rowPersister.Persist, firstResult, maxResults);
        }

        /// <summary>
        /// <paramref name="table"/>의 정보를 Persistent object로 병렬로 변환하여 제공합니다.
        /// </summary>
        /// <typeparam name="T">Persistent object 의 수형</typeparam>
        /// <param name="table"><see cref="DataTable"/></param>
        /// <param name="rowMapper">매핑 함수</param>
        /// <param name="firstResult">Persistent object를 만들 첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">Persistent object를 만들 최대 레코드 수 (0이거나 null이면 DataReader 끝까지)</param>
        /// <returns>Persistent object의 컬렉션</returns>
        public static ParallelQuery<T> MapAsParallel<T>(this DataTable table, Func<DataRow, T> rowMapper, int firstResult = 0,
                                                        int maxResults = 0) {
            table.ShouldNotBeNull("table");
            rowMapper.ShouldNotBeNull("rowMapper");

            if(IsDebugEnabled)
                log.Debug("Mapping Function를 이용하여 DataTable로부터 [{0}] 수형의 객체를 병렬로 생성합니다. firstResult=[{1}], maxResults=[{2}]",
                          typeof(T).FullName, firstResult, maxResults);

            var firstCount = Math.Max(0, firstResult);
            var maxCount = maxResults;

            if(maxCount <= 0)
                maxCount = table.Rows.Count;

            return
                table.Rows
                    .Cast<DataRow>()
                    .AsParallel()
                    .AsOrdered()
                    .Skip(firstCount)
                    .Take(maxCount)
                    .Select(rowMapper);
        }

        /// <summary>
        /// DataTable 정보를 기준으로 병렬로 Mapping을 수행해 T 수형의 인스턴스를 빌드합니다.
        /// </summary>
        /// <typeparam name="T">Mapping될 인스턴스의 수형</typeparam>
        /// <param name="table">DataTable</param>
        /// <param name="rowMapper">매핑 함수</param>
        /// <param name="dataRowFilter">매핑할 DataRow인지 판단하는 Predicate</param>
        /// <returns>매핑된 인스턴스의 컬렉션</returns>
        public static ParallelQuery<T> MapAsParallel<T>(this DataTable table, Func<DataRow, T> rowMapper,
                                                        Func<DataRow, bool> dataRowFilter) {
            table.ShouldNotBeNull("table");
            rowMapper.ShouldNotBeNull("rowMapper");
            dataRowFilter.ShouldNotBeNull("dataRowFilter");

            return
                table.Rows
                    .Cast<DataRow>()
                    .AsParallel()
                    .AsOrdered()
                    .Where(dataRowFilter)
                    .Select(rowMapper);
        }

        /// <summary>
        /// 계속 조건(<paramref name="continuationCondition"/>)이 참을 반환할 동안은 병렬로 매핑을 수행하여 반환합니다.
        /// </summary>
        public static ParallelQuery<T> ParallelMapWhile<T>(this DataTable table, Func<DataRow, T> rowMapper,
                                                           Func<DataRow, bool> continuationCondition) {
            table.ShouldNotBeNull("table");
            rowMapper.ShouldNotBeNull("rowMapper");
            continuationCondition.ShouldNotBeNull("continuationCondition");

            return
                table.Rows
                    .Cast<DataRow>()
                    .AsParallel()
                    .AsOrdered()
                    .TakeWhile(continuationCondition)
                    .Select(rowMapper);
        }
    }
}