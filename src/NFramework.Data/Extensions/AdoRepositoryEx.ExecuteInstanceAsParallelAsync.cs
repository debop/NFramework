using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Parallelism.Tools;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// ADO.NET DataReader, DataTable 을 Persistent object로 매핑하는 작업을 비동기적으로 수행할 수 있도록 하는 Extension Method
    /// </summary>
    public static partial class AdoRepositoryEx {
        /// <summary>
        /// <see cref="ExecuteInstance{T}(IAdoRepository,INameMapper,System.Data.Common.DbCommand,IAdoParameter[])"/> 를 비동기적으로 수행하여, 
        /// DataReader로부터 T 수형의 인스턴스를 매핑한다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="nameMapper">DataReader 컬럼과 엔티티의 속성명의 Name 매퍼</param>
        /// <param name="targetFactory">대상 객체 생성 함수</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IList<T>> ExecuteInstanceAsParallelAsync<T>(this IAdoRepository repository,
                                                                       INameMapper nameMapper,
                                                                       Func<T> targetFactory,
                                                                       string query,
                                                                       params IAdoParameter[] parameters) {
            return ExecuteInstanceAsParallelAsync(repository, nameMapper, targetFactory, 0, 0, (Action<AdoResultRow, T>)null, query,
                                                  parameters);
        }

        /// <summary>
        /// <see cref="ExecuteInstance{T}(IAdoRepository, INameMapper,System.Data.Common.DbCommand,IAdoParameter[])"/> 를 비동기적으로 수행하여, 
        /// DataReader로부터 T 수형의 인스턴스를 매핑한다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="nameMapper">DataReader 컬럼과 엔티티의 속성명의 Name 매퍼</param>
        /// <param name="targetFactory">대상 객체 생성 함수</param>
        /// <param name="additionalMapping">추가적인 매핑 함수</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IList<T>> ExecuteInstanceAsParallelAsync<T>(this IAdoRepository repository,
                                                                       INameMapper nameMapper,
                                                                       Func<T> targetFactory,
                                                                       Action<AdoResultRow, T> additionalMapping,
                                                                       string query,
                                                                       params IAdoParameter[] parameters) {
            return ExecuteInstanceAsParallelAsync(repository, nameMapper, targetFactory, 0, 0, additionalMapping, query, parameters);
        }

        /// <summary>
        /// <see cref="ExecuteInstance{T}(IAdoRepository,INameMapper,System.Data.Common.DbCommand,IAdoParameter[])"/> 를 비동기적으로 수행하여, 
        /// DataReader로부터 T 수형의 인스턴스를 매핑한다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="nameMapper">DataReader 컬럼과 엔티티의 속성명의 Name 매퍼</param>
        /// <param name="targetFactory">대상 객체 생성 함수</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 갯수(0이면 끝까지)</param>
        /// <param name="additionalMapping">추가적인 매핑 함수</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IList<T>> ExecuteInstanceAsParallelAsync<T>(this IAdoRepository repository,
                                                                       INameMapper nameMapper,
                                                                       Func<T> targetFactory,
                                                                       int firstResult,
                                                                       int maxResults,
                                                                       Action<AdoResultRow, T> additionalMapping,
                                                                       string query,
                                                                       params IAdoParameter[] parameters) {
            nameMapper.ShouldNotBeNull("nameMapper");
            targetFactory.ShouldNotBeNull("targetFactory");
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기적으로 ExecuteReader()을 수행하고, 병렬로 매핑하여 결과를 IList<{0}>으로 매핑합니다... " +
                          "query=[{1}], firstResult=[{2}], maxResults=[{3}]", typeof(T).Name, query, firstResult, maxResults);

            return
                Task.Factory
                    .StartNew(() => {
                                  using(var reader = repository.ExecuteReader(query, parameters))
                                      return reader.MapAsParallel<T>(targetFactory, nameMapper, firstResult, maxResults,
                                                                     additionalMapping);
                              });
        }

        /// <summary>
        /// <see cref="ExecuteInstance{T}(IAdoRepository,System.Func{System.Data.IDataReader,T},DbCommand,int,int,IAdoParameter[])"/>을 비동기적으로 수행합니다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="nameMap">DataReader 컬럼명 - Class 속성명의 매핑 정보</param>
        /// <param name="targetFactory">대상 객체 생성 함수</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IList<T>> ExecuteInstanceAsParallelAsync<T>(this IAdoRepository repository,
                                                                       INameMap nameMap,
                                                                       Func<T> targetFactory,
                                                                       string query,
                                                                       params IAdoParameter[] parameters) {
            return ExecuteInstanceAsParallelAsync(repository, nameMap, targetFactory, 0, 0, (Action<AdoResultRow, T>)null, query,
                                                  parameters);
        }

        /// <summary>
        /// 쿼리를 수행하여, 결과 셋을 대상 객체 컬렉션으로 병렬로 매핑을 수행하여 반환하는 Task를 빌드합니다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="nameMap">DataReader 컬럼명 - Class 속성명의 매핑 정보</param>
        /// <param name="targetFactory">대상 객체 생성 함수</param>
        /// <param name="additionalMapping">추가적인 매핑 함수</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IList<T>> ExecuteInstanceAsParallelAsync<T>(this IAdoRepository repository,
                                                                       INameMap nameMap,
                                                                       Func<T> targetFactory,
                                                                       Action<AdoResultRow, T> additionalMapping,
                                                                       string query,
                                                                       params IAdoParameter[] parameters) {
            return ExecuteInstanceAsParallelAsync(repository, nameMap, targetFactory, 0, 0, additionalMapping, query, parameters);
        }

        /// <summary>
        /// 쿼리를 수행하여, 결과 셋을 대상 객체 컬렉션으로 병렬로 매핑을 수행하여 반환하는 Task를 빌드합니다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="nameMap">DataReader 컬럼명 - Class 속성명의 매핑 정보</param>
        /// <param name="targetFactory">대상 객체 생성 함수</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 갯수(0이면 끝까지)</param>
        /// <param name="additionalMapping">추가적인 매핑 함수</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IList<T>> ExecuteInstanceAsParallelAsync<T>(this IAdoRepository repository,
                                                                       INameMap nameMap,
                                                                       Func<T> targetFactory,
                                                                       int firstResult,
                                                                       int maxResults,
                                                                       Action<AdoResultRow, T> additionalMapping,
                                                                       string query,
                                                                       params IAdoParameter[] parameters) {
            nameMap.ShouldNotBeNull("nameMap");
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기적으로 ExecuteReader()을 수행하고, 병렬로 매핑하여 결과를 IList<{0}>으로 매핑합니다... " +
                          "query=[{1}], firstResult=[{2}], maxResults=[{3}]", typeof(T).FullName, query, firstResult, maxResults);

            return
                Task.Factory
                    .StartNew(() => {
                                  using(var reader = repository.ExecuteReader(query, parameters))
                                      return reader.MapAsParallel<T>(targetFactory, nameMap, firstResult, maxResults, additionalMapping);
                              });
        }

        /// <summary>
        /// 쿼리를 수행하여, 결과 셋을 대상 객체 컬렉션으로 병렬로 매핑을 수행하여 반환하는 Task를 빌드합니다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="rowMapFunc">AdoResultRow로부터 {T} 수형의 인스턴스로 변환하는 메소드</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IList<T>> ExecuteInstanceAsParallelAsync<T>(this IAdoRepository repository,
                                                                       Func<AdoResultRow, T> rowMapFunc,
                                                                       string query,
                                                                       params IAdoParameter[] parameters) {
            return ExecuteInstanceAsParallelAsync(repository,
                                                  rowMapFunc,
                                                  0,
                                                  0,
                                                  query,
                                                  parameters);
        }

        /// <summary>
        /// 쿼리를 수행하여, 결과 셋을 대상 객체 컬렉션으로 병렬로 매핑을 수행하여 반환하는 Task를 빌드합니다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="rowMapFunc">AdoResultRow로부터 {T} 수형의 인스턴스로 변환하는 메소드</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 갯수(0이면 끝까지)</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IList<T>> ExecuteInstanceAsParallelAsync<T>(this IAdoRepository repository,
                                                                       Func<AdoResultRow, T> rowMapFunc,
                                                                       int firstResult,
                                                                       int maxResults,
                                                                       string query,
                                                                       params IAdoParameter[] parameters) {
            rowMapFunc.ShouldNotBeNull("rowMapFunc");
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기적으로 ExecuteReader()을 수행하고, 병렬로 매핑하여 결과를 IList<{0}>으로 매핑합니다... " +
                          "query=[{1}], firstResult=[{2}], maxResults=[{3}]", typeof(T).Name, query, firstResult, maxResults);

            return Task.Factory.StartNew(() => {
                                             using(var reader = repository.ExecuteReader(query, parameters))
                                                 return reader.MapAsParallel<T>(rowMapFunc, firstResult, maxResults);
                                         });
        }

        /// <summary>
        /// <see cref="ExecuteInstance{T}(IAdoRepository,Mappers.INameMapper,System.Data.Common.DbCommand,IAdoParameter[])"/> 를 비동기적으로 수행하여, 
        /// DataReader로부터 T 수형의 인스턴스를 매핑한다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="nameMapper">DataReader 컬럼과 엔티티의 속성명의 Name 매퍼</param>
        /// <param name="targetFactory">대상 객체 생성 Factory</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IPagingList<T>> ExecuteInstanceAsParallelAsync<T>(this IAdoRepository repository,
                                                                             INameMapper nameMapper,
                                                                             Func<T> targetFactory,
                                                                             string query,
                                                                             int pageIndex,
                                                                             int pageSize,
                                                                             params IAdoParameter[] parameters) {
            return repository.ExecuteInstanceAsParallelAsync(nameMapper, targetFactory, (Action<AdoResultRow, T>)null, query, pageIndex,
                                                             pageSize, parameters);
        }

        /// <summary>
        /// <see cref="ExecuteInstance{T}(IAdoRepository,INameMapper,DbCommand,IAdoParameter[])"/> 를 비동기적으로 수행하여, 
        /// DataReader로부터 T 수형의 인스턴스를 매핑한다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="nameMapper">DataReader 컬럼과 엔티티의 속성명의 Name 매퍼</param>
        /// <param name="targetFactory">대상 객체 생성 Factory</param>
        /// <param name="additionalMapping">추가적인 매핑 함수</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IPagingList<T>> ExecuteInstanceAsParallelAsync<T>(this IAdoRepository repository,
                                                                             INameMapper nameMapper,
                                                                             Func<T> targetFactory,
                                                                             Action<AdoResultRow, T> additionalMapping,
                                                                             string query,
                                                                             int pageIndex,
                                                                             int pageSize,
                                                                             params IAdoParameter[] parameters) {
            nameMapper.ShouldNotBeNull("nameMapper");
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기적으로 ExecuteReader()을 수행하고, 병렬 방식으로 매핑하여 결과를 IPageList<{0}>으로 반환하는 Task를 빌드합니다... query=[{1}]",
                          typeof(T).Name, query);

            var itemsTask = Task.Factory.StartNew(() => {
                                                      using(var reader = repository.ExecuteReader(query, parameters))
                                                          return reader.MapAsParallel<T>(targetFactory,
                                                                                         nameMapper,
                                                                                         pageIndex * pageSize,
                                                                                         pageSize,
                                                                                         additionalMapping);
                                                  });

            var totalItemCountTask = repository.CountAsync(query, parameters);


            var result = new PagingList<T>(itemsTask.Result, pageIndex, pageSize, totalItemCountTask.Result);
            return Task.Factory.FromResult((IPagingList<T>)result);
        }

        /// <summary>
        /// 비동기적으로 ExecuteReader()을 수행하고, 병렬 방식으로 매핑하여 결과를 IPageList{T} 으로 반환하는 Task를 빌드합니다
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="nameMap">DataReader 컬럼명 - Class 속성명의 매핑 정보</param>
        /// <param name="targetFactory">대상 객체 생성 Factory</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IPagingList<T>> ExecuteInstanceAsParallelAsync<T>(this IAdoRepository repository,
                                                                             INameMap nameMap,
                                                                             Func<T> targetFactory,
                                                                             string query,
                                                                             int pageIndex,
                                                                             int pageSize,
                                                                             params IAdoParameter[] parameters) {
            return ExecuteInstanceAsParallelAsync(repository, nameMap, targetFactory, null, query, pageIndex, pageSize, parameters);
        }

        /// <summary>
        /// 비동기적으로 ExecuteReader()을 수행하고, 병렬 방식으로 매핑하여 결과를 IPageList{T} 으로 반환하는 Task를 빌드합니다
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="nameMap">DataReader 컬럼명 - Class 속성명의 매핑 정보</param>
        /// <param name="targetFactory">대상 객체 생성 Factory</param>
        /// <param name="additionalMapping">추가적인 매핑 함수</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IPagingList<T>> ExecuteInstanceAsParallelAsync<T>(this IAdoRepository repository,
                                                                             INameMap nameMap,
                                                                             Func<T> targetFactory,
                                                                             Action<AdoResultRow, T> additionalMapping,
                                                                             string query,
                                                                             int pageIndex,
                                                                             int pageSize,
                                                                             params IAdoParameter[] parameters) {
            nameMap.ShouldNotBeNull("nameMap");
            targetFactory.ShouldNotBeNull("targetFactory");
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기적으로 ExecuteReader()을 수행하고, 병렬 방식으로 매핑하여 결과를 IPageList<{0}>으로 반환하는 Task를 빌드합니다... " +
                          @"query=[{1}]", typeof(T).Name, query);

            var itemsTask = Task.Factory.StartNew(() => {
                                                      using(var reader = repository.ExecuteReader(query, parameters))
                                                          return reader.MapAsParallel<T>(targetFactory,
                                                                                         nameMap,
                                                                                         pageIndex * pageSize,
                                                                                         pageSize,
                                                                                         additionalMapping);
                                                  });

            var totalItemCountTask = repository.CountAsync(query, parameters);

            var result = new PagingList<T>(itemsTask.Result, pageIndex, pageSize, totalItemCountTask.Result);
            return Task.Factory.FromResult((IPagingList<T>)result);
        }

        /// <summary>
        /// 비동기적으로 ExecuteReader()을 수행하고, 병렬 방식으로 매핑하여 결과를 IPageList{T} 으로 반환하는 Task를 빌드합니다
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="rowMapFunc">DataReader로부터 {T} 수형의 인스턴스로 변환하는 메소드</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IPagingList<T>> ExecuteInstanceAsParallelAsync<T>(this IAdoRepository repository,
                                                                             Func<AdoResultRow, T> rowMapFunc,
                                                                             string query,
                                                                             int pageIndex,
                                                                             int pageSize,
                                                                             params IAdoParameter[] parameters) {
            rowMapFunc.ShouldNotBeNull("converter");
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기적으로 ExecuteReader()을 수행하고, 병렬 방식으로 매핑하여 결과를 IPageList<{0}>으로 반환하는 Task를 빌드합니다... " +
                          @"query=[{1}]", typeof(T).Name, query);

            var itemsTask = Task.Factory.StartNew(() => {
                                                      using(var reader = repository.ExecuteReader(query, parameters))
                                                          return reader.MapAsParallel<T>(rowMapFunc, pageIndex * pageSize, pageSize);
                                                  });

            var totalItemCountTask = repository.CountAsync(query, parameters);

            var result = new PagingList<T>(itemsTask.Result, pageIndex, pageSize, totalItemCountTask.Result);
            return Task.Factory.FromResult((IPagingList<T>)result);
        }
    }
}