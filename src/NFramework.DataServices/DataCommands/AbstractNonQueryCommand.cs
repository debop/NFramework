using System.Data.Common;
using NSoft.NFramework.Data;
using NSoft.NFramework.DataServices.Messages;
using NSoft.NFramework.Json;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.DataServices.DataCommands {
    /// <summary>
    /// <see cref="DbCommand.ExecuteNonQuery"/>를 수행하고, Output Parameter 등을 반환하는 Command 입니다.
    /// </summary>
    public abstract class AbstractNonQueryCommand : AbstractDataCommand {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Command를 수행하고, 결과를 XML 문자열로 반환합니다.
        /// </summary>
        /// <param name="repository">Repository</param>
        /// <param name="requestItem">요청 정보</param>
        /// <returns>Data 처리 결과의 XML 문자열</returns>
        public override string Execute(IAdoRepository repository, RequestItem requestItem) {
            Repository = repository;

            var method = requestItem.Method;
            var requestParameters = requestItem.Parameters;

            if(IsDebugEnabled)
                log.Debug("{0}를 수행합니다... method=[{1}], requestParameters=[{2}]",
                          GetType().FullName, method, requestParameters.CollectionToString());

            var query = repository.QueryProvider.GetQuery(method).AsText(method);
            string result;

            using(var cmd = repository.GetCommand(query, true)) {
                repository.ExecuteNonQuery(cmd, GetParameters(requestParameters));
                var outputs = AdoTool.GetOutputParameters(repository.Db, cmd);

                result = JsonTool.SerializeAsText(outputs);
            }

            if(IsDebugEnabled)
                log.Debug("{0}를 완료했습니다. method=[{1}], requestParameters=[{2}]",
                          GetType().FullName, method, requestParameters.CollectionToString());

            return result;
        }
    }
}