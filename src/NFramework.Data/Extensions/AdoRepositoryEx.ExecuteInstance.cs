using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Data.Persisters;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

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
        public static IList<T> ExecuteInstance<T>(this IAdoRepository repository,
                                                  INameMapper nameMapper,
                                                  DbCommand cmd,
                                                  params IAdoParameter[] parameters) where T : class {
            nameMapper.ShouldNotBeNull("nameMapper");
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("DataReader를 INameMapper를 통해 지정된 Class의 인스턴스들을 생성합니다. " +
                          "persistent=[{0}], nameMapper=[{1}], commandText=[{2}], parameters=[{3}]",
                          typeof(T).FullName, nameMapper, cmd.CommandText, parameters.CollectionToString());

            using(var reader = repository.ExecuteReader(cmd, parameters))
                return reader.Map<T>(ActivatorTool.CreateInstance<T>, reader.NameMapping(nameMapper));
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
        public static IList<T> ExecuteInstance<T>(this IAdoRepository repository,
                                                  INameMapper nameMapper,
                                                  Action<IDataReader, T> additionalMapping,
                                                  DbCommand cmd,
                                                  params IAdoParameter[] parameters) where T : class {
            nameMapper.ShouldNotBeNull("nameMapper");
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("DataReader를 INameMapper를 통해 지정된 Class의 인스턴스들을 생성합니다. " +
                          "persistent=[{0}], nameMapper=[{1}], commandText=[{2}], parameters=[{3}]",
                          typeof(T).FullName, nameMapper, cmd.CommandText, parameters.CollectionToString());

            using(var reader = repository.ExecuteReader(cmd, parameters))
                return reader.Map<T>(ActivatorTool.CreateInstance<T>, reader.NameMapping(nameMapper), 0, 0, additionalMapping);
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
        public static IList<T> ExecuteInstance<T>(this IAdoRepository repository,
                                                  INameMap nameMap,
                                                  DbCommand cmd,
                                                  params IAdoParameter[] parameters) where T : class {
            nameMap.ShouldNotBeNull("nameMap");
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("DataReader를 INameMap를 통해 지정된 Class의 인스턴스들을 생성합니다. " +
                          "persistent=[{0}], nameMap=[{1}], commandText=[{2}], parameters=[{3}]",
                          typeof(T).FullName, nameMap.DictionaryToString(), cmd.CommandText, parameters.CollectionToString());

            using(var reader = repository.ExecuteReader(cmd, parameters))
                return reader.Map<T>(ActivatorTool.CreateInstance<T>, nameMap);
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
        public static IList<T> ExecuteInstance<T>(this IAdoRepository repository,
                                                  INameMap nameMap,
                                                  Action<IDataReader, T> additionalMapping,
                                                  DbCommand cmd,
                                                  params IAdoParameter[] parameters) where T : class {
            nameMap.ShouldNotBeNull("nameMap");
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("DataReader를 INameMap를 통해 지정된 Class의 인스턴스들을 생성합니다. " +
                          "persistent=[{0}], nameMap=[{1}], commandText=[{2}], parameters=[{3}]",
                          typeof(T).FullName, nameMap.DictionaryToString(), cmd.CommandText, parameters.CollectionToString());

            using(var reader = repository.ExecuteReader(cmd, parameters))
                return reader.Map<T>(ActivatorTool.CreateInstance<T>, nameMap, 0, 0, additionalMapping);
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
        public static IList<T> ExecuteInstance<T>(this IAdoRepository repository,
                                                  IReaderPersister<T> persister,
                                                  DbCommand cmd,
                                                  params IAdoParameter[] parameters) where T : class {
            persister.ShouldNotBeNull("persister");
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("DataReader를 IReaderPersister를 통해 지정된 Class의 인스턴스들을 생성합니다. " +
                          "persistent=[{0}], persister=[{1}], commandText=[{2}], parameters=[{3}]",
                          typeof(T).FullName, persister, cmd.CommandText, parameters.CollectionToString());

            using(var reader = repository.ExecuteReader(cmd, parameters))
                return reader.Map<T>(persister);
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
        public static IList<T> ExecuteInstance<T>(this IAdoRepository repository,
                                                  Func<IDataReader, T> mapFunc,
                                                  DbCommand cmd,
                                                  params IAdoParameter[] parameters) {
            mapFunc.ShouldNotBeNull("mapFunc");
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("DataReader를 Mapping function 를 통해 지정된 Class의 인스턴스들을 생성합니다. " +
                          "persistent=[{0}], mapFunc=[{1}], commandText=[{2}], parameters=[{3}]",
                          typeof(T).FullName, mapFunc, cmd.CommandText, parameters.CollectionToString());

            using(var reader = repository.ExecuteReader(cmd, parameters))
                return reader.Map<T>(mapFunc);
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
        public static IPagingList<T> ExecuteInstance<T>(this IAdoRepository repository,
                                                        INameMapper nameMapper,
                                                        DbCommand cmd,
                                                        int pageIndex,
                                                        int pageSize,
                                                        params IAdoParameter[] parameters) where T : class {
            nameMapper.ShouldNotBeNull("nameMapper");
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug(
                    "Build Persistent Objects from CommandText=[{0}], nameMapper=[{1}], pageIndex=[{2}], pageSize=[{3}], parameters=[{4}]",
                    cmd.CommandText, nameMapper, pageIndex, pageSize, parameters.CollectionToString());


            var query = cmd.CommandText;
            var firstResult = pageIndex * pageSize;
            var maxResults = pageSize;

            var totalItemCountTask = repository.CountAsync(query, parameters);

            IList<T> instances;

            using(var dr = repository.ExecuteReader(cmd, parameters))
                instances = dr.Map<T>(ActivatorTool.CreateInstance<T>, nameMapper, firstResult, maxResults);

            var pagingList = new PagingList<T>(instances, pageIndex, pageSize, totalItemCountTask.Result);

            if(IsDebugEnabled)
                log.Debug("ExecuteReader를 수행해 엔티티로 매핑했습니다. pageIndex=[{0}], pageSize=[{1}], totalItemCount=[{2}]",
                          pageIndex, pageSize, totalItemCountTask.Result);

            return pagingList;
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
        public static IPagingList<T> ExecuteInstance<T>(this IAdoRepository repository,
                                                        INameMapper nameMapper,
                                                        Action<IDataReader, T> additionalMapping,
                                                        DbCommand cmd,
                                                        int pageIndex,
                                                        int pageSize,
                                                        params IAdoParameter[] parameters) where T : class {
            nameMapper.ShouldNotBeNull("nameMapper");
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug(
                    "IDataReader로부터 PersistentObject를 생성합니다... CommandText=[{0}], NameMapper=[{1}], pageIndex=[{2}], pageSize=[{3}], parameters=[{4}]",
                    cmd.CommandText, nameMapper, pageIndex, pageSize, parameters.CollectionToString());


            var query = cmd.CommandText;
            var firstResult = pageIndex * pageSize;
            var maxResults = pageSize;


            var totalItemCountTask = repository.CountAsync(query, parameters);

            IList<T> instances;
            using(var reader = repository.ExecuteReader(cmd, parameters))
                instances = reader.Map<T>(ActivatorTool.CreateInstance<T>, nameMapper, firstResult, maxResults, additionalMapping);

            var pagingList = new PagingList<T>(instances, pageIndex, pageSize, totalItemCountTask.Result);

            if(IsDebugEnabled)
                log.Debug("ExecuteReader를 수행해 엔티티로 매핑했습니다. pageIndex=[{0}], pageSize=[{1}], totalItemCount=[{2}]",
                          pageIndex, pageSize, totalItemCountTask.Result);

            return pagingList;
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
        public static IPagingList<T> ExecuteInstance<T>(this IAdoRepository repository,
                                                        INameMap nameMap,
                                                        DbCommand cmd,
                                                        int pageIndex,
                                                        int pageSize,
                                                        params IAdoParameter[] parameters) where T : class {
            nameMap.ShouldNotBeNull("nameMap");
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("IDataReader로부터 PersistentObject를 생성합니다... " +
                          "CommandText=[{0}], NameMapping=[{1}], pageIndex=[{2}], pageSize=[{3}], parameters=[{4}]",
                          cmd.CommandText, nameMap, pageIndex, pageSize, parameters.CollectionToString());


            var query = cmd.CommandText;

            var firstResult = pageIndex * pageSize;
            var maxResults = pageSize;


            var totalItemCountTask = repository.CountAsync(query, parameters);

            IList<T> instances;
            using(var dr = repository.ExecuteReader(cmd, parameters))
                instances = dr.Map<T>(ActivatorTool.CreateInstance<T>, nameMap, firstResult, maxResults);

            var pagingList = new PagingList<T>(instances, pageIndex, pageSize, totalItemCountTask.Result);

            if(IsDebugEnabled)
                log.Debug("ExecuteReader를 수행해 엔티티로 매핑했습니다!!! pageIndex=[{0}], pageSize=[{1}], totalItemCount=[{2}]",
                          pageIndex, pageSize, totalItemCountTask.Result);

            return pagingList;
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
        public static IPagingList<T> ExecuteInstance<T>(this IAdoRepository repository,
                                                        INameMap nameMap,
                                                        Action<IDataReader, T> additionalMapping,
                                                        DbCommand cmd,
                                                        int pageIndex,
                                                        int pageSize,
                                                        params IAdoParameter[] parameters) where T : class {
            nameMap.ShouldNotBeNull("nameMap");
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("ExecuteReader를 수행해 PersistentObject로 매핑합니다... " +
                          "CommandText=[{0}], NameMapping=[{1}], pageIndex=[{2}], pageSize=[{3}], parameters=[{4}]",
                          cmd.CommandText, nameMap, pageIndex, pageSize, parameters.CollectionToString());


            var query = cmd.CommandText;
            var firstResult = pageIndex * pageSize;
            var maxResults = pageSize;

            var totalItemCountTask = repository.CountAsync(query, parameters);

            IList<T> instances;
            using(var dr = repository.ExecuteReader(cmd, parameters))
                instances = dr.Map<T>(ActivatorTool.CreateInstance<T>, nameMap, firstResult, maxResults, additionalMapping);


            var pagingList = new PagingList<T>(instances, pageIndex, pageSize, totalItemCountTask.Result);

            if(IsDebugEnabled)
                log.Debug("ExecuteReader를 수행해 PersistentObject로 매핑했습니다!!! pageIndex=[{0}], pageSize=[{1}], totalItemCount=[{2}]",
                          pageIndex, pageSize, totalItemCountTask.Result);

            return pagingList;
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
        public static IPagingList<T> ExecuteInstance<T>(this IAdoRepository repository,
                                                        IReaderPersister<T> persister,
                                                        DbCommand cmd,
                                                        int pageIndex,
                                                        int pageSize,
                                                        params IAdoParameter[] parameters) {
            return repository.ExecuteInstance<T>(persister.Persist, cmd, pageIndex, pageSize, parameters);
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
        public static IPagingList<T> ExecuteInstance<T>(this IAdoRepository repository,
                                                        Func<IDataReader, T> @mapFunc,
                                                        DbCommand cmd,
                                                        int pageIndex,
                                                        int pageSize,
                                                        params IAdoParameter[] parameters) {
            @mapFunc.ShouldNotBeNull("@mapFunc");
            cmd.ShouldNotBeNull("cmd");

            pageIndex.ShouldBePositiveOrZero("pageIndex");
            pageSize.ShouldBePositive("pageSize");

            if(IsDebugEnabled)
                log.Debug("ExecuteReader를 수행해 엔티티로 매핑합니다... " +
                          "CommandText=[{0}], @mapFunc=[{1}], pageIndex=[{2}], pageSize=[{3}], parameters=[{4}]",
                          cmd.CommandText, @mapFunc, pageIndex, pageSize, parameters.CollectionToString());


            var query = cmd.CommandText;
            var firstResult = pageIndex * pageSize;
            var maxResults = pageSize;

            var totalItemCountTask = repository.CountAsync(query, parameters);

            IList<T> instances;
            using(var dr = repository.ExecuteReader(cmd, parameters))
                instances = dr.Map<T>(@mapFunc, firstResult, maxResults);

            var pagingList = new PagingList<T>(instances, pageIndex, pageSize, totalItemCountTask.Result);

            if(IsDebugEnabled)
                log.Debug("ExecuteReader를 수행해 엔티티로 매핑했습니다!!! pageIndex=[{0}], pageSize=[{1}], totalItemCount=[{2}]",
                          pageIndex, pageSize, totalItemCountTask.Result);

            return pagingList;
        }
    }
}