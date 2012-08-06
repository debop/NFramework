using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.AdoPoco {
    public abstract partial class AdoProviderBase {
        /// <summary>
        ///  지정한 조회용 쿼리문을 수행하여, 결과를 {T} 수형의 컬렉션으로 반환합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="firstResult"></param>
        /// <param name="maxResults"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IList<T> Query<T>(string sql, int firstResult = 0, int maxResults = 0, IAdoParameter[] parameters = null) {
            sql.ShouldNotBeWhiteSpace("sql");

            if(IsDebugEnabled)
                log.Debug("지정된 쿼리문을 실행하여, 수형[{0}]의 컬렉션을 반환합니다. " +
                          "sql=[{1}], firstResult=[{2}], maxResults=[{3}], parameters=[{4}]",
                          typeof(T).FullName, sql, firstResult, maxResults, parameters.CollectionToString());

            OpenSharedConnection();

            try {
                using(var cmd = CreateCommand(Connection, sql, parameters)) {
                    OnExecutingCommand(cmd);
                    using(var reader = cmd.ExecuteReader()) {
                        OnExecutedCommand(cmd);
                        return reader.Map(ActivatorTool.CreateInstance<T>, NameMapper, firstResult, maxResults);
                    }
                }
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.Error("Command.ExecuteReader 수행 시에 예외가 발생했습니다...", ex);

                OnException(ex);
                throw;
            }
            finally {
                CloseSharedConnection();
            }
        }

        /// <summary>
        ///  지정한 조회용 쿼리문을 수행하여, 결과를 {T} 수형의 컬렉션으로 반환합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="firstResult"></param>
        /// <param name="maxResults"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual Task<IList<T>> QueryAsync<T>(string sql, int firstResult = 0, int maxResults = 0,
                                                    IAdoParameter[] parameters = null) {
            sql.ShouldNotBeWhiteSpace("sql");

            if(IsDebugEnabled)
                log.Debug("지정된 쿼리문을 비동기 방식으로 실행하여, 수형[{0}]의 컬렉션을 반환합니다. " +
                          "sql=[{1}], firstResult=[{2}], maxResults=[{3}], parameters=[{4}]",
                          typeof(T).FullName, sql, firstResult, maxResults, parameters.CollectionToString());

            OpenSharedConnection();
            try {
                using(var cmd = CreateCommand(Connection, sql, parameters)) {
                    OnExecutingCommand(cmd);
                    using(var reader = Task.Factory.StartNew(() => cmd.ExecuteReader()).Result) {
                        OnExecutedCommand(cmd);
                        var result = reader.MapAsParallel(ActivatorTool.CreateInstance<T>, NameMapper, firstResult, maxResults);
                        return Task.Factory.FromResult(result);
                    }
                }
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.Error("Command.ExecuteReader 수행 시에 예외가 발생했습니다...", ex);

                OnException(ex);
                throw;
            }
            finally {
                CloseSharedConnection();
            }
        }
    }
}