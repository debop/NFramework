using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Castle.Core;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data {
    public static partial class AdoTool {
        /// <summary>
        /// DataReader 정보를 대상 수형으로 병렬 방식으로 매핑합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="propertyExprsToExclude"></param>
        /// <returns></returns>
        public static IList<T> MapAsParallel<T>(this IDataReader reader, params Expression<Func<T, object>>[] propertyExprsToExclude)
            where T : class {
            return MapAsParallel(reader, ActivatorTool.CreateInstance<T>, 0, 0, (Action<AdoResultRow, T>)null, propertyExprsToExclude);
        }

        /// <summary>
        /// DataReader 정보를 대상 수형으로 병렬 방식으로 매핑합니다.
        /// </summary>
        /// <typeparam name="T">매핑 대상 수형</typeparam>
        /// <param name="reader">원본 DataReader</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 인덱스 (1 이상 설정해야 적용됨)</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성들</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> MapAsParallel<T>(this IDataReader reader,
                                                int firstResult,
                                                int maxResults,
                                                params Expression<Func<T, object>>[] propertyExprsToExclude) where T : class {
            return MapAsParallel(reader, ActivatorTool.CreateInstance<T>, firstResult, maxResults, (Action<AdoResultRow, T>)null,
                                 propertyExprsToExclude);
        }

        /// <summary>
        /// DataReader 정보를 대상 수형으로 병렬 방식으로 매핑합니다.
        /// </summary>
        /// <typeparam name="T">매핑 대상 수형</typeparam>
        /// <param name="reader">원본 DataReader</param>
        /// <param name="additionalMapping">추가적인 매핑 작업</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성들</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> MapAsParallel<T>(this IDataReader reader,
                                                Action<AdoResultRow, T> additionalMapping,
                                                params Expression<Func<T, object>>[] propertyExprsToExclude) where T : class {
            return MapAsParallel(reader, ActivatorTool.CreateInstance<T>, 0, 0, additionalMapping, propertyExprsToExclude);
        }

        /// <summary>
        /// DataReader 정보를 대상 수형으로 병렬 방식으로 매핑합니다.
        /// </summary>
        /// <typeparam name="T">매핑 대상 수형</typeparam>
        /// <param name="reader">원본 DataReader</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 인덱스 (1 이상 설정해야 적용됨)</param>
        /// <param name="additionalMapping">추가적인 매핑 작업</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성들</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> MapAsParallel<T>(this IDataReader reader,
                                                int firstResult,
                                                int maxResults,
                                                Action<AdoResultRow, T> additionalMapping,
                                                params Expression<Func<T, object>>[] propertyExprsToExclude) where T : class {
            var propertyNamesToExclude = propertyExprsToExclude.Select(expr => expr.Body.FindMemberName()).ToArray();
            var nameMapper = IoC.TryResolve<INameMapper, TrimNameMapper>(LifestyleType.Thread);
            var nameMap = reader.NameMapping(nameMapper, propertyNamesToExclude);

            return MapAsParallel(reader, ActivatorTool.CreateInstance<T>, nameMap, firstResult, maxResults, additionalMapping);
        }

        /// <summary>
        /// DataReader 정보를 대상 수형으로 병렬 방식으로 매핑합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="targetFactory"></param>
        /// <param name="propertyExprsToExclude"></param>
        /// <returns></returns>
        public static IList<T> MapAsParallel<T>(this IDataReader reader,
                                                Func<T> targetFactory,
                                                params Expression<Func<T, object>>[] propertyExprsToExclude) {
            return MapAsParallel(reader, targetFactory, 0, 0, (Action<AdoResultRow, T>)null, propertyExprsToExclude);
        }

        /// <summary>
        /// DataReader 정보를 대상 수형으로 병렬 방식으로 매핑합니다.
        /// </summary>
        /// <typeparam name="T">매핑 대상 수형</typeparam>
        /// <param name="reader">원본 DataReader</param>
        /// <param name="targetFactory">대상 인스턴스 생성 함수</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 인덱스 (1 이상 설정해야 적용됨)</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성들</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> MapAsParallel<T>(this IDataReader reader,
                                                Func<T> targetFactory,
                                                int firstResult,
                                                int maxResults,
                                                params Expression<Func<T, object>>[] propertyExprsToExclude) {
            return MapAsParallel(reader, targetFactory, firstResult, maxResults, (Action<AdoResultRow, T>)null, propertyExprsToExclude);
        }

        /// <summary>
        /// DataReader 정보를 대상 수형으로 병렬 방식으로 매핑합니다.
        /// </summary>
        /// <typeparam name="T">매핑 대상 수형</typeparam>
        /// <param name="reader">원본 DataReader</param>
        /// <param name="targetFactory">대상 인스턴스 생성 함수</param>
        /// <param name="additionalMapping">추가적인 매핑 작업</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성들</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> MapAsParallel<T>(this IDataReader reader,
                                                Func<T> targetFactory,
                                                Action<AdoResultRow, T> additionalMapping,
                                                params Expression<Func<T, object>>[] propertyExprsToExclude) {
            return MapAsParallel(reader, targetFactory, 0, 0, additionalMapping, propertyExprsToExclude);
        }

        /// <summary>
        /// DataReader 정보를 대상 수형으로 병렬 방식으로 매핑합니다.
        /// </summary>
        /// <typeparam name="T">매핑 대상 수형</typeparam>
        /// <param name="reader">원본 DataReader</param>
        /// <param name="targetFactory">대상 인스턴스 생성 함수</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 인덱스 (1 이상 설정해야 적용됨)</param>
        /// <param name="additionalMapping">추가적인 매핑 작업</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 대상 객체의 속성들</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> MapAsParallel<T>(this IDataReader reader,
                                                Func<T> targetFactory,
                                                int firstResult,
                                                int maxResults,
                                                Action<AdoResultRow, T> additionalMapping,
                                                params Expression<Func<T, object>>[] propertyExprsToExclude) {
            var propertyNamesToExclude = propertyExprsToExclude.Select(expr => expr.Body.FindMemberName()).ToArray();
            var nameMapper = IoC.TryResolve<INameMapper, TrimNameMapper>(LifestyleType.Thread);
            var nameMap = reader.NameMapping(nameMapper, propertyNamesToExclude);

            return MapAsParallel(reader, targetFactory, nameMap, firstResult, maxResults, additionalMapping);
        }

        /// <summary>
        /// IDataReader 정보를 병렬 방식으로 매핑을 수행하여, 컬렉션을 반환합니다.
        /// </summary>
        /// <typeparam name="T">매핑 대상 수형</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="nameMapper">컬럼명-속성명 Mapper</param>
        /// <param name="targetFactory">대상 수형 Factory 함수</param>
        /// <param name="propertyExprsToExclude">매핑 작업에서 제외할 속성 표현식 (예: user=>user.Password)</param>
        /// <returns>매핑된 객체의 컬렉션</returns>
        public static IList<T> MapAsParallel<T>(this IDataReader reader,
                                                Func<T> targetFactory,
                                                INameMapper nameMapper,
                                                params Expression<Func<T, object>>[] propertyExprsToExclude) {
            return MapAsParallel(reader, targetFactory, nameMapper, 0, 0, (Action<AdoResultRow, T>)null, propertyExprsToExclude);
        }

        /// <summary>
        /// IDataReader 정보를 병렬 방식으로 매핑을 수행하여, 컬렉션을 반환합니다.
        /// </summary>
        /// <typeparam name="T">매핑 대상 수형</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="nameMapper">컬럼명-속성명 Mapper</param>
        /// <param name="targetFactory">대상 수형 Factory 함수</param>
        /// <param name="additionalMapping">추가 매핑 작업을 수행할 델리게이트</param>
        /// <param name="propertyExprsToExclude">매핑 작업에서 제외할 속성 표현식 (예: user=>user.Password)</param>
        /// <returns>매핑된 객체의 컬렉션</returns>
        public static IList<T> MapAsParallel<T>(this IDataReader reader,
                                                Func<T> targetFactory,
                                                INameMapper nameMapper,
                                                Action<AdoResultRow, T> additionalMapping,
                                                params Expression<Func<T, object>>[] propertyExprsToExclude) {
            return MapAsParallel(reader, targetFactory, nameMapper, 0, 0, additionalMapping, propertyExprsToExclude);
        }

        /// <summary>
        /// IDataReader 정보를 병렬 방식으로 매핑을 수행하여, 컬렉션을 반환합니다.
        /// </summary>
        /// <typeparam name="T">매핑 대상 수형</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="nameMapper">컬럼명-속성명 Mapper</param>
        /// <param name="targetFactory">대상 수형 Factory 함수</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작합니다. null이면 0으로 간주)</param>
        /// <param name="maxResults">매핑할 최대 레코드 수 (0이면, IDataReader의 끝까지 매핑합니다)</param>
        /// <param name="propertyExprsToExclude">매핑 작업에서 제외할 속성 표현식 (예: user=>user.Password)</param>
        /// <returns>매핑된 객체의 컬렉션</returns>
        public static IList<T> MapAsParallel<T>(this IDataReader reader,
                                                Func<T> targetFactory,
                                                INameMapper nameMapper,
                                                int firstResult,
                                                int maxResults,
                                                params Expression<Func<T, object>>[] propertyExprsToExclude) {
            return MapAsParallel(reader, targetFactory, nameMapper, firstResult, maxResults, (Action<AdoResultRow, T>)null,
                                 propertyExprsToExclude);
        }

        /// <summary>
        /// IDataReader 정보를 병렬 방식으로 매핑을 수행하여, 컬렉션을 반환합니다.
        /// </summary>
        /// <typeparam name="T">매핑 대상 수형</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="nameMapper">컬럼명-속성명 Mapper</param>
        /// <param name="targetFactory">대상 수형 Factory 함수</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작합니다. null이면 0으로 간주)</param>
        /// <param name="maxResults">매핑할 최대 레코드 수 (0이면, IDataReader의 끝까지 매핑합니다)</param>
        /// <param name="additionalMapping">추가 매핑 작업을 수행할 델리게이트</param>
        /// <param name="propertyExprsToExclude">매핑 작업에서 제외할 속성 표현식 (예: user=>user.Password)</param>
        /// <returns>매핑된 객체의 컬렉션</returns>
        public static IList<T> MapAsParallel<T>(this IDataReader reader,
                                                Func<T> targetFactory,
                                                INameMapper nameMapper,
                                                int firstResult,
                                                int maxResults,
                                                Action<AdoResultRow, T> additionalMapping,
                                                params Expression<Func<T, object>>[] propertyExprsToExclude) {
            reader.ShouldNotBeNull("reader");

            if(nameMapper == null)
                nameMapper = IoC.TryResolve<INameMapper, TrimNameMapper>(LifestyleType.Thread);

            var propertyNamesToExclude = propertyExprsToExclude.Select(expr => expr.Body.FindMemberName()).ToArray();
            var nameMap = reader.NameMapping(nameMapper, propertyNamesToExclude);

            return MapAsParallel(reader, targetFactory, nameMap, firstResult, maxResults, additionalMapping);
        }

        /// <summary>
        /// IDataReader 정보를 병렬 방식으로 매핑을 수행하여, 컬렉션을 반환합니다.
        /// </summary>
        /// <typeparam name="T">매핑 대상 수형</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="nameMap">컬럼명-속성명 매핑 정보</param>
        /// <param name="targetFactory">대상 수형 Factory 함수</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작합니다. null이면 0으로 간주)</param>
        /// <param name="maxResults">매핑할 최대 레코드 수 (0이면, IDataReader의 끝까지 매핑합니다)</param>
        /// <param name="additionalMapping">추가 매핑 작업을 수행할 델리게이트</param>
        /// <returns>매핑된 객체의 컬렉션</returns>
        public static IList<T> MapAsParallel<T>(this IDataReader reader,
                                                Func<T> targetFactory,
                                                INameMap nameMap,
                                                int firstResult = 0,
                                                int maxResults = 0,
                                                Action<AdoResultRow, T> additionalMapping = null) {
            reader.ShouldNotBeNull("reader");
            nameMap.ShouldNotBeNull("nameMap");
            targetFactory.ShouldNotBeNull("targetFactory");

            Guard.Assert(reader.IsClosed == false, "IDataReader가 이미 닫혀있습니다!!!");

            if(IsDebugEnabled)
                log.Debug("IDataReader를 읽어 수형[{0}]으로 병렬로 매핑하여, 컬렉션으로 반환합니다...", typeof(T).Name);

            if(nameMap.Count == 0)
                return new List<T>();

            var resultSet = new AdoResultSet(reader, firstResult, maxResults);

            if(resultSet.Count == 0)
                return new List<T>();

            var targetAccessor = GetDynamicAccessor<T>();
            var fieldNames = resultSet.FieldNames.Where(n => nameMap.ContainsKey(n)).ToArray();

            return
                resultSet.Values
                    .AsParallel()
                    .AsOrdered()
                    .Select(row => {
                                var target = targetFactory();

                                fieldNames.RunEach(fn => targetAccessor.SetPropertyValue(target, nameMap[fn], row[fn]));

                                if(additionalMapping != null)
                                    additionalMapping(row, target);

                                return target;
                            })
                    .ToList();
        }

        /// <summary>
        /// IDataReader 정보를 <paramref name="rowMapFunc"/>가 병렬 방식으로 처리하여 대상 객체로 빌드합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="rowMapFunc">대상 객체를 빌드하는 함수</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작합니다. null이면 0으로 간주)</param>
        /// <param name="maxResults">매핑할 최대 레코드 수 (0이면, IDataReader의 끝까지 매핑합니다)</param>
        /// <returns>매핑된 대상 객체의 컬렉션</returns>
        public static IList<T> MapAsParallel<T>(this IDataReader reader, Func<AdoResultRow, T> rowMapFunc, int firstResult = 0,
                                                int maxResults = 0) {
            reader.ShouldNotBeNull("reader");
            rowMapFunc.ShouldNotBeNull("rowMapFunc");

            Guard.Assert(reader.IsClosed == false, "IDataReader가 이미 닫혀있습니다!!!");

            if(IsDebugEnabled)
                log.Debug("IDataReader를 읽어 수형[{0}]으로 매핑하여, 컬렉션으로 반환합니다...", typeof(T).Name);

            var resultSet = new AdoResultSet(reader, firstResult, maxResults);

            if(resultSet.Count == 0)
                return new List<T>();

            return
                resultSet.Values
                    .AsParallel()
                    .AsOrdered()
                    .Select(rowMapFunc)
                    .ToList();
        }

        /// <summary>
        /// <paramref name="reader"/>의 레코드 정보를 필터링 (<paramref name="rowFilter"/>)애 통과한 Row만 <paramref name="rowMapFunc"/>를 통해 병렬 방식으로 매핑을 수행합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체</typeparam>
        /// <param name="reader">IDataReader 객체</param>
        /// <param name="rowMapFunc">매핑 함수</param>
        /// <param name="rowFilter">매핑할 Row를 선별할 필터 함수</param>
        /// <returns>대상 객체 컬렉션</returns>
        public static IList<T> MapIfAsParallel<T>(this IDataReader reader, Func<AdoResultRow, T> rowMapFunc,
                                                  Func<AdoResultRow, bool> rowFilter) {
            reader.ShouldNotBeNull("reader");
            rowMapFunc.ShouldNotBeNull("rowMapFunc");
            rowFilter.ShouldNotBeNull("rowFilter");

            Guard.Assert(reader.IsClosed == false, "IDataReader가 이미 닫혀있습니다!!!");

            var resultSet = new AdoResultSet(reader);

            if(resultSet.Count == 0)
                return new List<T>();

            return
                resultSet.Values
                    .AsParallel()
                    .AsOrdered()
                    .Where(rowFilter)
                    .Select(rowMapFunc)
                    .ToList();
        }

        /// <summary>
        /// <paramref name="reader"/>의 레코드 정보를 <paramref name="rowMapFunc"/>를 통해 매핑을 병렬 방식으로 수행하는데, 계속조건 (<paramref name="continuationCondition"/>) 이 만족할때까지만 수행한다.
        /// </summary>
        /// <typeparam name="T">대상 객체</typeparam>
        /// <param name="reader">IDataReader 객체</param>
        /// <param name="rowMapFunc">매핑 함수</param>
        /// <param name="continuationCondition">진행 조건 (False가 나올 때까지 진행합니다)</param>
        /// <returns>대상 객체 컬렉션</returns>
        public static IList<T> MapWhileAsParallel<T>(this IDataReader reader, Func<AdoResultRow, T> rowMapFunc,
                                                     Func<AdoResultRow, bool> continuationCondition) {
            reader.ShouldNotBeNull("reader");
            rowMapFunc.ShouldNotBeNull("rowMapFunc");
            continuationCondition.ShouldNotBeNull("rowMapFunc");

            Guard.Assert(reader.IsClosed == false, "IDataReader가 이미 닫혀있습니다!!!");

            var resultSet = new AdoResultSet(reader);

            if(resultSet.Count == 0)
                return new List<T>();

            return
                resultSet.Values
                    .AsParallel()
                    .AsOrdered()
                    .TakeWhile(continuationCondition)
                    .Select(rowMapFunc)
                    .ToList();
        }
    }
}