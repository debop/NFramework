using System;
using NSoft.NFramework.Data;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.DataServices.DataCommands;
using NSoft.NFramework.DataServices.Messages;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.DataServices.DataServices {
    public class DataCommandServiceImpl : DataServiceImpl {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public DataCommandServiceImpl() {}
        public DataCommandServiceImpl(IAdoRepository adoRepository) : base(adoRepository) {}
        public DataCommandServiceImpl(IAdoRepository adoRepository, INameMapper nameMapper) : base(adoRepository, nameMapper) {}

        protected override ResponseItem DoProcessRequestItem(RequestItem requestItem) {
            if(log.IsDebugEnabled)
                log.Debug("요청 Id=[{0}], Method=[{1}], Query=[{2}]에 대한 작업 처리를 시작합니다.",
                          requestItem.Id, requestItem.Method, requestItem.Query);

            var responseItem = new ResponseItem(requestItem);

            if(requestItem.Query.IsWhiteSpace())
                return responseItem;

            if(IsDebugEnabled)
                log.Debug("요청정보를 처리합니다. 메소드=[{0}], 인자=[{1}]",
                          requestItem.Method, requestItem.Parameters.CollectionToString());
            try {
                var command = DataCommandTool.GetCommand(requestItem.Method);

                Guard.Assert(command != null, "해당하는 메소드에 정의된 Command 가 없습니다. method=[{0}]", requestItem.Method);

                ExecuteQueries(requestItem.PreQueries);

                responseItem.ResultValue = command.Execute(AdoRepository, requestItem);

                if(IsDebugEnabled)
                    log.Debug("메소드를 실행했습니다. method=[{0}], resultValue=[{1}]", requestItem.Method, responseItem.ResultValue);

                ExecuteQueries(requestItem.PostQueries);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("쿼리문 수행에 실패했습니다. Query=" + requestItem.Query, ex);

                responseItem.Errors.Add(new ErrorMessage(ex));
            }

            if(log.IsDebugEnabled)
                log.Debug("요청 Id=[{0}], Method=[{1}], Query=[{2}]에 대한 작업 처리를 완료했습니다!!!",
                          requestItem.Id, requestItem.Method, requestItem.Query);

            return responseItem;
        }
    }
}