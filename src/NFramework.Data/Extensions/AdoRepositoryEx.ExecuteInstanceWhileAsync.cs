using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Data.Persisters;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// ADO.NET DataReader, DataTable 을 Persistent object로 매핑하는 작업을 비동기적으로 수행할 수 있도록 하는 Extension Method
    /// </summary>
    public static partial class AdoRepositoryEx {
        /// <summary>
        /// <see cref="IAdoRepository.ExecuteInstance{T}(System.Func{System.Data.IDataReader,T},System.Data.Common.DbCommand,NFramework.Data.IAdoParameter[])"/> 를 비동기적으로 수행하여, 
        /// <paramref name="continuationCondition"/> 조건이 만족할 동안만, DataReader로부터 T 수형의 인스턴스를 매핑한다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="targetFactory">대상 객체 생성용 Factory</param>
        /// <param name="nameMapper">DB 컬럼명- 클래스 속성명 매퍼</param>
        /// <param name="continuationCondition">매핑을 계속할 조건인지 판단하는 델리게이트</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IEnumerable<T>> ExecuteInstanceWhileAsync<T>(this IAdoRepository repository,
                                                                        Func<T> targetFactory,
                                                                        INameMapper nameMapper,
                                                                        Func<IDataReader, bool> continuationCondition,
                                                                        string query,
                                                                        params IAdoParameter[] parameters) where T : class {
            nameMapper.ShouldNotBeNull("nameMapper");
            continuationCondition.ShouldNotBeNull("continuationCondition");

            return Task.Factory.StartNew(() => {
                                             using(var reader = repository.ExecuteReader(query, parameters))
                                                 return reader.MapWhile<T>(ActivatorTool.CreateInstance<T>,
                                                                           nameMapper,
                                                                           continuationCondition,
                                                                           null);
                                         });
        }

        /// <summary>
        /// <see cref="IAdoRepository.ExecuteInstance{T}(System.Func{System.Data.IDataReader,T},System.Data.Common.DbCommand,NFramework.Data.IAdoParameter[])"/> 를 비동기적으로 수행하여, 
        /// <paramref name="continuationCondition"/> 조건이 만족할 동안만, DataReader로부터 T 수형의 인스턴스를 매핑한다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="targetFactory">대상 객체 생성용 Factory</param>
        /// <param name="nameMap">DB 컬럼명- 클래스 속성명 매핑 정보</param>
        /// <param name="continuationCondition">매핑을 계속할 조건인지 판단하는 델리게이트</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IEnumerable<T>> ExecuteInstanceWhileAsync<T>(this IAdoRepository repository,
                                                                        Func<T> targetFactory,
                                                                        INameMap nameMap,
                                                                        Func<IDataReader, bool> continuationCondition,
                                                                        string query,
                                                                        params IAdoParameter[] parameters) where T : class {
            nameMap.ShouldNotBeNull("nameMap");
            continuationCondition.ShouldNotBeNull("continuationCondition");

            return Task.Factory.StartNew(() => {
                                             using(var reader = repository.ExecuteReader(query, parameters))
                                                 return reader.MapWhile<T>(ActivatorTool.CreateInstance<T>,
                                                                           nameMap,
                                                                           continuationCondition,
                                                                           null);
                                         });
        }

        /// <summary>
        /// <see cref="IAdoRepository.ExecuteInstance{T}(System.Func{System.Data.IDataReader,T},System.Data.Common.DbCommand,NFramework.Data.IAdoParameter[])"/> 를 비동기적으로 수행하여, 
        /// <paramref name="continuationCondition"/> 조건이 만족할 동안만, DataReader로부터 T 수형의 인스턴스를 매핑한다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="persister">대상 객체를 빌드하는 Persister</param>
        /// <param name="continuationCondition">매핑을 계속할 조건인지 판단하는 델리게이트</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IEnumerable<T>> ExecuteInstanceWhileAsync<T>(this IAdoRepository repository,
                                                                        IReaderPersister<T> persister,
                                                                        Func<IDataReader, bool> continuationCondition,
                                                                        string query,
                                                                        params IAdoParameter[] parameters) where T : class {
            persister.ShouldNotBeNull("persister");
            continuationCondition.ShouldNotBeNull("continuationCondition");

            return Task.Factory.StartNew(() => {
                                             using(var reader = repository.ExecuteReader(query, parameters))
                                                 return reader.MapWhile<T>(persister, continuationCondition, null);
                                         });
        }

        /// <summary>
        /// <see cref="IAdoRepository.ExecuteInstance{T}(System.Func{System.Data.IDataReader,T},System.Data.Common.DbCommand,NFramework.Data.IAdoParameter[])"/> 를 비동기적으로 수행하여, 
        /// <paramref name="continuationCondition"/> 조건이 만족할 동안만, DataReader로부터 T 수형의 인스턴스를 매핑한다.
        /// </summary>
        /// <typeparam name="T">결과 셋으로 표현할 엔티티의 수형</typeparam>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="mapFunc">매핑 함수</param>
        /// <param name="continuationCondition">매핑을 계속할 조건인지 판단하는 델리게이트</param>
        /// <param name="query">실행할 SQL문 또는 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>매핑한 엔티티 컬렉션을 결과값으로 가지는 Task</returns>
        public static Task<IEnumerable<T>> ExecuteInstanceWhileAsync<T>(this IAdoRepository repository,
                                                                        Func<IDataReader, T> mapFunc,
                                                                        Func<IDataReader, bool> continuationCondition,
                                                                        string query,
                                                                        params IAdoParameter[] parameters) where T : class {
            mapFunc.ShouldNotBeNull("mapFunc");
            continuationCondition.ShouldNotBeNull("continuationCondition");

            return Task.Factory.StartNew(() => {
                                             using(var reader = repository.ExecuteReader(query, parameters))
                                                 return reader.MapWhile<T>(mapFunc, continuationCondition);
                                         });
        }
    }
}