using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Data.Persisters;

namespace NSoft.NFramework.Data {
    public static partial class AdoRepositoryEx {
        ///<summary>
        /// DbCommand를 실행해 얻은 DataReader를 통해 지정된 형식의 인스턴스를 만든다.
        ///</summary>
        /// <typeparam name="T">Type of Persistent object</typeparam>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="nameMapper">컬럼명과 속성명 Mapper</param>
        /// <param name="cmd">실행할 DbCommand</param>
        /// <param name="parameters">Procedure 인자</param>
        /// <returns>Collection of Persistent object</returns>
        public static Task<IList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                             INameMapper nameMapper,
                                                             DbCommand cmd,
                                                             params IAdoParameter[] parameters) where T : class {
            return Task.Factory.StartNew(() => repository.ExecuteInstance<T>(nameMapper, cmd, parameters));
        }

        ///<summary>
        /// DbCommand를 실행해 얻은 DataReader를 통해 지정된 형식의 인스턴스를 만든다.
        ///</summary>
        /// <typeparam name="T">Type of Persistent object</typeparam>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="nameMapper">컬럼명과 속성명 Mapper</param>
        /// <param name="additionalMapping">추가적인 매핑 함수</param>
        /// <param name="cmd">실행할 DbCommand</param>
        /// <param name="parameters">Procedure 인자</param>
        /// <returns>Collection of Persistet object</returns>
        public static Task<IList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                             INameMapper nameMapper,
                                                             Action<IDataReader, T> additionalMapping,
                                                             DbCommand cmd,
                                                             params IAdoParameter[] parameters) where T : class {
            return Task.Factory.StartNew(() => repository.ExecuteInstance<T>(nameMapper, additionalMapping, cmd, parameters));
        }

        /// <summary>
        /// ExecuteInstance() 를 비동기적으로 수행하여, 
        /// DataReader로부터 T 수형의 인스턴스를 매핑한다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="nameMapper">DataReader 컬럼과 엔티티의 속성명의 Name 매퍼</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                             INameMapper nameMapper,
                                                             string query,
                                                             params IAdoParameter[] parameters) where T : class {
            return ExecuteInstanceAsync(repository, nameMapper, (Action<IDataReader, T>)null, query, parameters);
        }

        /// <summary>
        /// <see cref="ExecuteInstance{T}(IAdoRepository, INameMapper,DbCommand,IAdoParameter[])"/> 를 비동기적으로 수행하여, 
        /// DataReader로부터 T 수형의 인스턴스를 매핑한다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="nameMapper">DataReader 컬럼과 엔티티의 속성명의 Name 매퍼</param>
        /// <param name="additionalMapping">추가적인 매핑 함수</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                             INameMapper nameMapper,
                                                             Action<IDataReader, T> additionalMapping,
                                                             string query,
                                                             params IAdoParameter[] parameters) where T : class {
            nameMapper.ShouldNotBeNull("nameMapper");
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기적으로 ExecuteReader<{0}>()을 수행하고, 결과를 IList<{0}> 으로 반환합니다... query=[{1}]", typeof(T).FullName, query);

            return
                Task.Factory.StartNew(() => {
                                          using(var cmd = repository.GetCommand(query))
                                              return repository.ExecuteInstance<T>(nameMapper,
                                                                                   additionalMapping,
                                                                                   cmd,
                                                                                   parameters);
                                      },
                                      TaskCreationOptions.PreferFairness);
        }

        /// <summary>
        /// Execute DbCommand, Build instance of specified type by mapping DataReader Column Value to Instance Property Value
        /// </summary>
        /// <typeparam name="T">Type of Persistent object</typeparam>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="nameMap">Key = ColumnName of DataReader, Value = Property Name of Specifieid Type</param>
        /// <param name="cmd">Instance of DbCommand to executed</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>Collection of Persistent object</returns>
        public static Task<IList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                             INameMap nameMap,
                                                             DbCommand cmd,
                                                             params IAdoParameter[] parameters) where T : class {
            return Task.Factory.StartNew(() => repository.ExecuteInstance<T>(nameMap, cmd, parameters));
        }

        /// <summary>
        /// Execute DbCommand, Build instance of specified type by mapping DataReader Column Value to Instance Property Value
        /// </summary>
        /// <typeparam name="T">Type of Persistent object</typeparam>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="nameMap">Key = ColumnName of DataReader, Value = Property Name of Specifieid Type</param>
        /// <param name="additionalMapping">추가적인 매핑 함수</param>
        /// <param name="cmd">Instance of DbCommand to executed</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>Collection of Persistet object</returns>
        public static Task<IList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                             INameMap nameMap,
                                                             Action<IDataReader, T> additionalMapping,
                                                             DbCommand cmd,
                                                             params IAdoParameter[] parameters) where T : class {
            return Task.Factory.StartNew(() => repository.ExecuteInstance<T>(nameMap, additionalMapping, cmd, parameters));
        }

        /// <summary>
        /// <see cref="ExecuteInstance{T}(IAdoRepository, System.Func{IDataReader,T},DbCommand,int,int,IAdoParameter[])"/>을 비동기적으로 수행합니다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="nameMap">DataReader 컬럼명 - Class 속성명의 매핑 정보</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                             INameMap nameMap,
                                                             string query,
                                                             params IAdoParameter[] parameters) where T : class {
            return ExecuteInstanceAsync(repository, nameMap, (Action<IDataReader, T>)null, query, parameters);
        }

        /// <summary>
        /// <see cref="ExecuteInstance{T}(IAdoRepository, System.Func{System.Data.IDataReader,T},System.Data.Common.DbCommand,int,int,NFramework.Data.IAdoParameter[])"/>을 비동기적으로 수행합니다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="nameMap">DataReader 컬럼명 - Class 속성명의 매핑 정보</param>
        /// <param name="additionalMapping">추가적인 매핑 함수</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                             INameMap nameMap,
                                                             Action<IDataReader, T> additionalMapping,
                                                             string query,
                                                             params IAdoParameter[] parameters) where T : class {
            nameMap.ShouldNotBeNull("nameMap");
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기적으로 ExecuteReader<{0}>()을 수행하고, 결과를 IList<{0}>으로 매핑합니다... query=[{1}]", typeof(T).FullName, query);

            return Task.Factory.StartNew(() => {
                                             using(var cmd = repository.GetCommand(query))
                                                 return repository.ExecuteInstance<T>(nameMap,
                                                                                      additionalMapping,
                                                                                      cmd,
                                                                                      parameters);
                                         });
        }

        /// <summary>
        /// Execute DbCommand, Build instance of specified type from IDataReader using Persister
        /// </summary>
        /// <typeparam name="T">Type of persistent object</typeparam>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="persister">Persister from IDataReader</param>
        /// <param name="cmd">Instance of DbCommand</param>
        /// <param name="parameters">Parameters for DbCommand to execute</param>
        /// <returns>List of Persistent Object</returns>
        public static Task<IList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                             IReaderPersister<T> persister,
                                                             DbCommand cmd,
                                                             params IAdoParameter[] parameters) where T : class {
            return Task.Factory.StartNew(() => repository.ExecuteInstance<T>(persister, cmd, parameters));
        }

        /// <summary>
        /// <see cref="ExecuteInstance{T}(IAdoRepository,NFramework.Data.Persisters.IReaderPersister{T},System.Data.Common.DbCommand,NFramework.Data.IAdoParameter[])"/> 를 비동기적으로 수행하여, 
        /// DataReader로부터 T 수형의 인스턴스를 매핑한다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="persister">DataReader로부터 {T} 수형의 인스턴스를 생성하는 생성자</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                             IReaderPersister<T> persister,
                                                             string query,
                                                             params IAdoParameter[] parameters) where T : class {
            persister.ShouldNotBeNull("persister");
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기적으로 ExecuteReader<{0}>()을 수행하고, 결과를 IList<{0}>으로 매핑합니다... query=[{1}]", typeof(T).FullName, query);

            return Task.Factory.StartNew(() => {
                                             using(var cmd = repository.GetCommand(query))
                                                 return repository.ExecuteInstance<T>(persister,
                                                                                      cmd,
                                                                                      parameters);
                                         });
        }

        /// <summary>
        /// DbCommand를 실행해 얻은 DataReader를 Converter를 통해 지정된 형식의 인스턴스를 만든다.
        /// </summary>
        /// <typeparam name="T">Type of persistent object</typeparam>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="mapFunc">IDataReader의 한 레코드인 IDataRecord 정보를 가지고, Persistent Object를 만들 Converter</param>
        /// <param name="cmd">Instance of DbCommand to executed</param>
        /// <param name="parameters">Parameters for DbCommand to execute</param>
        /// <returns>Collection of Persistent Object</returns>
        public static Task<IList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                             Func<IDataReader, T> mapFunc,
                                                             DbCommand cmd,
                                                             params IAdoParameter[] parameters) {
            return Task.Factory.StartNew(() => repository.ExecuteInstance<T>(mapFunc, cmd, parameters));
        }

        /// <summary>
        /// <see cref="ExecuteInstance{T}(IAdoRepository,System.Func{System.Data.IDataReader,T},System.Data.Common.DbCommand,NFramework.Data.IAdoParameter[])"/> 를 비동기적으로 수행하여, 
        /// DataReader로부터 T 수형의 인스턴스를 매핑한다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="mapFunc">DataReader로부터 {T} 수형의 인스턴스로 변환하는 메소드</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                             Func<IDataReader, T> mapFunc,
                                                             string query,
                                                             params IAdoParameter[] parameters) {
            mapFunc.ShouldNotBeNull("mapFunc");
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 ExecuteReader<{0}>()을 수행하고, 결과를 IList<{0}>으로 매핑합니다... query=[{1}]", typeof(T).FullName, query);

            return Task.Factory.StartNew(() => {
                                             using(var cmd = repository.GetCommand(query))
                                                 return repository.ExecuteInstance<T>(mapFunc, cmd, parameters);
                                         });
        }

        //! ===============================================================================================

        /// <summary>
        /// 지정된 Command를 실행하여, 결과 셋을 Paging하여, 지정된 Page에 해당하는 정보만 Persistent Object로 빌드하여 반환한다.
        /// </summary>
        /// <typeparam name="T">Persistent Object의 수형</typeparam>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="nameMapper">컬럼명:속성명의 매핑정보를 가진 Mapper</param>
        /// <param name="cmd">실행할 DbCommand</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size (0이면 페이징 안함)</param>
        /// <param name="parameters">DbCommand 실행시의 Parameter 정보</param>
        /// <returns>Paging된 Persistent Object의 List</returns>
        public static Task<IPagingList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                                   INameMapper nameMapper,
                                                                   DbCommand cmd,
                                                                   int pageIndex,
                                                                   int pageSize,
                                                                   params IAdoParameter[] parameters) where T : class {
            return Task.Factory.StartNew(() => repository.ExecuteInstance<T>(nameMapper, cmd, pageIndex, pageSize, parameters));
        }

        /// <summary>
        /// 지정된 Command를 실행하여, 결과 셋을 Paging하여, 지정된 Page에 해당하는 정보만 Persistent Object로 빌드하여 반환한다.
        /// </summary>
        /// <typeparam name="T">Persistent Object의 수형</typeparam>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="nameMapper">컬럼명:속성명의 매핑정보를 가진 Mapper</param>
        /// <param name="additionalMapping">추가적인 매핑 함수</param>
        /// <param name="cmd">실행할 DbCommand</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">DbCommand 실행시의 Parameter 정보</param>
        /// <returns>Paging된 Persistent Object의 List</returns>
        public static Task<IPagingList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                                   INameMapper nameMapper,
                                                                   Action<IDataReader, T> additionalMapping,
                                                                   DbCommand cmd,
                                                                   int pageIndex,
                                                                   int pageSize,
                                                                   params IAdoParameter[] parameters) where T : class {
            return
                Task.Factory.StartNew(
                    () => repository.ExecuteInstance<T>(nameMapper, additionalMapping, cmd, pageIndex, pageSize, parameters));
        }

        /// <summary>
        /// <see cref="ExecuteInstance{T}(IAdoRepository, INameMapper,DbCommand,IAdoParameter[])"/> 를 비동기적으로 수행하여, 
        /// DataReader로부터 T 수형의 인스턴스를 매핑한다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="nameMapper">DataReader 컬럼과 엔티티의 속성명의 Name 매퍼</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IPagingList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                                   INameMapper nameMapper,
                                                                   string query,
                                                                   int pageIndex,
                                                                   int pageSize,
                                                                   params IAdoParameter[] parameters) where T : class {
            return ExecuteInstanceAsync(repository, nameMapper, (Action<IDataReader, T>)null, query, pageIndex, pageSize, parameters);
        }

        /// <summary>
        /// <see cref="ExecuteInstance{T}(IAdoRepository,INameMapper,DbCommand,IAdoParameter[])"/> 를 비동기적으로 수행하여, 
        /// DataReader로부터 T 수형의 인스턴스를 매핑한다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="nameMapper">DataReader 컬럼과 엔티티의 속성명의 Name 매퍼</param>
        /// <param name="additionalMapping">추가적인 매핑 함수</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IPagingList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                                   INameMapper nameMapper,
                                                                   Action<IDataReader, T> additionalMapping,
                                                                   string query,
                                                                   int pageIndex,
                                                                   int pageSize,
                                                                   params IAdoParameter[] parameters) where T : class {
            nameMapper.ShouldNotBeNull("nameMapper");
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기적으로 ExecuteReader<{0}>()을 수행하고, 결과를 IList<{0}> 으로 반환합니다... query=[{1}]", typeof(T).FullName, query);

            return
                Task.Factory.StartNew(() => {
                                          using(var cmd = repository.GetCommand(query))
                                              return repository.ExecuteInstance<T>(nameMapper,
                                                                                   additionalMapping,
                                                                                   cmd,
                                                                                   pageIndex,
                                                                                   pageSize,
                                                                                   parameters);
                                      });
        }

        /// <summary>
        /// 지정된 Command를 실행하여, 결과 셋을 Paging하여, 지정된 Page에 해당하는 정보만 Persistent Object로 빌드하여 반환한다.
        /// </summary>
        /// <typeparam name="T">Persistent Object의 수형</typeparam>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="nameMap">컬럼명:속성명의 매핑정보</param>
        /// <param name="cmd">실행할 DbCommand</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">DbCommand 실행시의 Parameter 정보</param>
        /// <returns>Paging된 Persistent Object의 List</returns>
        public static Task<IPagingList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                                   INameMap nameMap,
                                                                   DbCommand cmd,
                                                                   int pageIndex,
                                                                   int pageSize,
                                                                   params IAdoParameter[] parameters) where T : class {
            return Task.Factory.StartNew(() => repository.ExecuteInstance<T>(nameMap, cmd, pageIndex, pageSize, parameters));
        }

        /// <summary>
        /// 지정된 Command를 실행하여, 결과 셋을 Paging하여, 지정된 Page에 해당하는 정보만 Persistent Object로 빌드하여 반환한다.
        /// </summary>
        /// <typeparam name="T">Persistent Object의 수형</typeparam>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="nameMap">컬럼명:속성명의 매핑정보</param>
        /// <param name="additionalMapping">추가적인 매핑 함수</param>
        /// <param name="cmd">실행할 DbCommand</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">DbCommand 실행시의 Parameter 정보</param>
        /// <returns>Paging된 Persistent Object의 List</returns>
        public static Task<IPagingList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                                   INameMap nameMap,
                                                                   Action<IDataReader, T> additionalMapping,
                                                                   DbCommand cmd,
                                                                   int pageIndex,
                                                                   int pageSize,
                                                                   params IAdoParameter[] parameters) where T : class {
            return
                Task.Factory.StartNew(
                    () => repository.ExecuteInstance<T>(nameMap, additionalMapping, cmd, pageIndex, pageSize, parameters));
        }

        /// <summary>
        /// <see cref="ExecuteInstance{T}(IAdoRepository,System.Func{IDataReader,T},DbCommand,int,int,IAdoParameter[])"/>을 비동기적으로 수행합니다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="nameMap">DataReader 컬럼명 - Class 속성명의 매핑 정보</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IPagingList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                                   INameMap nameMap,
                                                                   string query,
                                                                   int pageIndex,
                                                                   int pageSize,
                                                                   params IAdoParameter[] parameters) where T : class {
            return ExecuteInstanceAsync(repository, nameMap, (Action<IDataReader, T>)null, query, pageIndex, pageSize, parameters);
        }

        /// <summary>
        /// <see cref="ExecuteInstance{T}(IAdoRepository,System.Func{System.Data.IDataReader,T},DbCommand,int,int,IAdoParameter[])"/>을 비동기적으로 수행합니다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="nameMap">DataReader 컬럼명 - Class 속성명의 매핑 정보</param>
        /// <param name="additionalMapping">추가적인 매핑 함수</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IPagingList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                                   INameMap nameMap,
                                                                   Action<IDataReader, T> additionalMapping,
                                                                   string query,
                                                                   int pageIndex,
                                                                   int pageSize,
                                                                   params IAdoParameter[] parameters) where T : class {
            nameMap.ShouldNotBeNull("nameMap");
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기적으로 ExecuteReader<{0}>()을 수행하고, 결과를 IList<{0}>으로 매핑합니다... query=[{1}]", typeof(T).FullName, query);

            return Task.Factory.StartNew(() => {
                                             using(var cmd = repository.GetCommand(query))
                                                 return repository.ExecuteInstance<T>(nameMap,
                                                                                      additionalMapping,
                                                                                      cmd,
                                                                                      pageIndex,
                                                                                      pageSize,
                                                                                      parameters);
                                         });
        }

        /// <summary>
        /// 지정된 Command를 실행하여, 결과 셋을 Paging하여, 지정된 Page에 해당하는 정보만 Persistent Object로 빌드하여 반환한다.
        /// </summary>
        /// <typeparam name="T">Persistent Object의 수형</typeparam>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="persister">IDataReader로부터 Persistent Object를 빌드하는 Persister</param>
        /// <param name="cmd">실행할 DbCommand</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">DbCommand 실행시의 Parameter 정보</param>
        /// <returns>Paging된 Persistent Object의 List</returns>
        public static Task<IPagingList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                                   IReaderPersister<T> persister,
                                                                   DbCommand cmd,
                                                                   int pageIndex,
                                                                   int pageSize,
                                                                   params IAdoParameter[] parameters) {
            return Task.Factory.StartNew(() => repository.ExecuteInstance<T>(persister, cmd, pageIndex, pageSize, parameters));
        }

        /// <summary>
        /// <see cref="ExecuteInstance{T}(IAdoRepository,IReaderPersister{T},DbCommand,IAdoParameter[])"/> 를 비동기적으로 수행하여, 
        /// DataReader로부터 T 수형의 인스턴스를 매핑한다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="persister">DataReader로부터 {T} 수형의 인스턴스를 생성하는 생성자</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IPagingList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                                   IReaderPersister<T> persister,
                                                                   string query,
                                                                   int pageIndex,
                                                                   int pageSize,
                                                                   params IAdoParameter[] parameters) where T : class {
            persister.ShouldNotBeNull("persister");
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기적으로 ExecuteReader<{0}>()을 수행하고, 결과를 IList<{0}>으로 매핑합니다... query=[{1}]", typeof(T).FullName, query);

            return Task.Factory.StartNew(() => {
                                             using(var cmd = repository.GetCommand(query))
                                                 return repository.ExecuteInstance<T>(persister,
                                                                                      cmd,
                                                                                      pageIndex,
                                                                                      pageSize,
                                                                                      parameters);
                                         });
        }

        /// <summary>
        /// 지정된 Command를 실행하여, 결과 셋을 Paging하여, 지정된 Page에 해당하는 정보만 Persistent Object로 빌드하여 반환한다.
        /// </summary>
        /// <typeparam name="T">Persistent Object의 수형</typeparam>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="mapFunc">IDataReader로부터 Persistent Object를 빌드하는 Mapping function</param>
        /// <param name="cmd">실행할 DbCommand</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">DbCommand 실행시의 Parameter 정보</param>
        /// <returns>Paging된 Persistent Object의 List</returns>
        public static Task<IPagingList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                                   Func<IDataReader, T> @mapFunc,
                                                                   DbCommand cmd,
                                                                   int pageIndex,
                                                                   int pageSize,
                                                                   params IAdoParameter[] parameters) {
            return Task.Factory.StartNew(() => repository.ExecuteInstance<T>(@mapFunc, cmd, pageIndex, pageSize, parameters));
        }

        /// <summary>
        /// <see cref="ExecuteInstance{T}(IAdoRepository,System.Func{System.Data.IDataReader,T},DbCommand,IAdoParameter[])"/> 를 비동기적으로 수행하여, 
        /// DataReader로부터 T 수형의 인스턴스를 매핑한다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="mapFunc">DataReader로부터 {T} 수형의 인스턴스로 변환하는 메소드</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IPagingList<T>> ExecuteInstanceAsync<T>(this IAdoRepository repository,
                                                                   Func<IDataReader, T> mapFunc,
                                                                   string query,
                                                                   int pageIndex,
                                                                   int pageSize,
                                                                   params IAdoParameter[] parameters) where T : class {
            mapFunc.ShouldNotBeNull("converter");
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기적으로 ExecuteReader<{0}>()을 수행하고, 결과를 IList<{0}>으로 매핑합니다... query=[{1}]", typeof(T).FullName, query);

            return Task.Factory.StartNew(() => {
                                             using(var cmd = repository.GetCommand(query))
                                                 return repository.ExecuteInstance<T>(mapFunc,
                                                                                      cmd,
                                                                                      pageIndex,
                                                                                      pageSize,
                                                                                      parameters);
                                         });
        }
    }
}