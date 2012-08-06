using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using Castle.Core;
using NSoft.NFramework.Data;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.DataServices.Messages;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.DataServices {
    /// <summary>
    /// 요청 정보 (<see cref="RequestMessage"/>)를 받아, 처리하고, 응답 정보 (<see cref="ResponseMessage"/>)를 빌드하는 기본 DataService 입니다.
    /// </summary>
    public class DataServiceImpl : IDataService {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private IAdoRepository _adoRepository;
        private INameMapper _nameMapper;

        public DataServiceImpl() {}

        public DataServiceImpl(IAdoRepository adoRepository) {
            adoRepository.ShouldNotBeNull("adoRepository");
            AdoRepository = adoRepository;
        }

        public DataServiceImpl(IAdoRepository adoRepository, INameMapper nameMapper) {
            adoRepository.ShouldNotBeNull("adoRepository");
            nameMapper.ShouldNotBeNull("nameMapper");

            AdoRepository = adoRepository;
            NameMapper = nameMapper;
        }

        /// <summary>
        /// Data 처리 시에 사용할 <see cref="IAdoRepository"/> 인스턴스
        /// </summary>
        public virtual IAdoRepository AdoRepository {
            get { return _adoRepository ?? (_adoRepository = IoC.TryResolve<IAdoRepository, AdoRepositoryImpl>(LifestyleType.Transient)); }
            set { _adoRepository = value; }
        }

        /// <summary>
        /// <see cref="ResultSet"/>의 컬럼명을 Class 속성으로 
        /// </summary>
        public virtual INameMapper NameMapper {
            get { return _nameMapper ?? (_nameMapper = IoC.TryResolve<INameMapper, TrimNameMapper>(LifestyleType.Transient)); }
            set { _nameMapper = value; }
        }

        /// <summary>
        /// DATA 처리를 위한 요청정보를 처리해서, 응답정보를 빌드해서 반환합니다.
        /// </summary>
        /// <param name="requestMessage">요청 메시지</param>
        /// <returns>응답 메시지</returns>
        public virtual ResponseMessage Execute(RequestMessage requestMessage) {
            var responseMsg = new ResponseMessage();

            if(IsDebugEnabled)
                log.Debug("요청정보에 따른 작업을 시작합니다...");

            if(requestMessage == null) {
                if(log.IsWarnEnabled)
                    log.Warn("요청 정보가 NULL입니다.");
                responseMsg.Errors.Add(new ErrorMessage { Code = 0, Message = DataServiceTool.NoRequestProvided });
                return responseMsg;
            }

            TransactionScope txScope = null;

            try {
                // 1. 사전 작업
                //
                responseMsg.MessageId = requestMessage.MessageId;

                if(requestMessage.Transactional) {
                    if(IsDebugEnabled)
                        log.Debug("요청작업이 Transaction을 필요로 하여, Transaction을 시작합니다...");

                    txScope = AdoTool.CreateTransactionScope(TransactionScopeOption.Required, IsolationLevel.ReadCommitted);
                }

                //
                // 2. 본 작업
                //
                ExecuteInternal(requestMessage, responseMsg);


                // 3. 사후 작업
                //
                if(txScope != null) {
                    txScope.Complete();

                    if(IsDebugEnabled)
                        log.Debug("요청작업의 Transaction을 Commit 했습니다.");
                }
            }
            catch(Exception ex) {
                responseMsg.AddError(ex);

                if(log.IsErrorEnabled)
                    log.Error("요청정보에 따른 작업처리 중 예외가 발생했습니다.", ex);

                if(txScope != null)
                    txScope.Dispose();
            }

            if(IsDebugEnabled)
                log.Debug("요청정보에 따른 작업을 완료했습니다!!! Request MessageId=[{0}], Response MessageId=[{1}]", requestMessage.MessageId,
                          responseMsg.MessageId);

            return responseMsg;
        }

        protected virtual void ExecuteInternal(RequestMessage requestMessage, ResponseMessage responseMessage) {
            if(IsDebugEnabled)
                log.Debug("요청 항목 순서대로 작업을 처리합니다.");

            // 1. 요청 메소드의 실제 Query 문을 매핑합니다.
            DataServiceTool.LoadQueryStatements(this, requestMessage);

            // 2. 요청 메시지 전체에 대한 사전 작업 수행
            ExecuteQueries(requestMessage.PreQueries);

            // 3. 요청 항목별로 작업 처리
            DoProcessRequestItems(requestMessage, responseMessage);

            // 4. 요청 메시지 전체에 대한 사후 작업 수행
            ExecuteQueries(requestMessage.PostQueries);
        }

        protected virtual void DoProcessRequestItems(RequestMessage requestMessage, ResponseMessage responseMessage) {
            // 3. 요청 항목별로 작업 처리
            foreach(var requestItem in requestMessage.Items) {
                var responseItem = DoProcessRequestItem(requestItem);
                responseMessage.Items.Add(responseItem);
            }
        }

        protected virtual ResponseItem DoProcessRequestItem(RequestItem requestItem) {
            if(log.IsDebugEnabled)
                log.Debug("요청 Id=[{0}], Method=[{1}], Query=[{2}]에 대한 작업 처리를 시작합니다.",
                          requestItem.Id, requestItem.Method, requestItem.Query);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var responseItem = new ResponseItem(requestItem);

            if(requestItem.Query.IsWhiteSpace())
                return responseItem;

            try {
                ExecuteQueries(requestItem.PreQueries);

                var adoParams = requestItem.BuildAdoParameters().ToArray();

                if(requestItem.ResponseFormat == ResponseFormatKind.ResultSet)
                    OpenQuery(requestItem, responseItem, adoParams);
                else
                    ExecuteQuery(requestItem, responseItem, adoParams);

                ExecuteQueries(requestItem.PostQueries);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("쿼리문 수행에 실패했습니다. Query=" + requestItem.Query, ex);

                responseItem.Errors.Add(new ErrorMessage(ex));
            }

            stopwatch.Stop();
            responseItem.ExecutionTime = stopwatch.Elapsed;

            if(log.IsDebugEnabled)
                log.Debug("요청 Id=[{0}], Method=[{1}], Query=[{2}]에 대한 작업 처리를 완료했습니다!!! 수행시간=[{3}]",
                          requestItem.Id, requestItem.Method, requestItem.Query, responseItem.ExecutionTime);

            return responseItem;
        }

        /// <summary>
        /// 요청 쿼리 문을 수행하여, <see cref="ResultSet"/> 으로 빌드하여, 응답 결과를 적용합니다.
        /// </summary>
        protected virtual void OpenQuery(RequestItem requestItem, ResponseItem responseItem, IAdoParameter[] adoParameters) {
            if(IsDebugEnabled)
                log.Debug("쿼리문을 수행하여 결과셋을 설정합니다...");

            using(var cmd = AdoRepository.GetCommand(requestItem.Query)) {
                var reader = AdoRepository.ExecuteReader(cmd, adoParameters);
                var resultSet = reader.CreateResultSet(NameMapper,
                                                       requestItem.FirstResult.GetValueOrDefault(),
                                                       requestItem.MaxResults.GetValueOrDefault(int.MaxValue));

                responseItem.ResultSet = resultSet;
            }
        }

        /// <summary>
        /// 요청 쿼리 문을 수행하는데, 
        /// <see cref="IAdoRepository.ExecuteNonQuery(string,IAdoParameter[])" /> 나 
        /// <see cref="IAdoRepository.ExecuteScalar(string,IAdoParameter[])"/> 를 실행합니다.
        /// </summary>
        /// <param name="requestItem"></param>
        /// <param name="responseItem"></param>
        /// <param name="adoParameters"></param>
        protected virtual void ExecuteQuery(RequestItem requestItem, ResponseItem responseItem, IAdoParameter[] adoParameters) {
            // Scalar 값 구하기
            if(requestItem.ResponseFormat == ResponseFormatKind.Scalar) {
                if(IsDebugEnabled)
                    log.Debug("ExecuteNonScalar 를 수행하여, ResultValue 값을 설정합니다...");

                responseItem.ResultValue = AdoRepository.ExecuteScalar(requestItem.Query, adoParameters);
                return;
            }

            // 쿼리문이라면, 실행
            var query = requestItem.Query;
            if(AdoTool.IsSqlString(query)) {
                if(IsDebugEnabled)
                    log.Debug("ExecuteNonQuery 를 수행하고, 영향받은 행의 수를 ResultValue에 설정합니다...");

                responseItem.ResultValue = AdoRepository.ExecuteNonQuery(requestItem.Query, adoParameters);
                return;
            }


            if(IsDebugEnabled)
                log.Debug("ExecuteProcedure를 수행하고, Parameter 정보들을 ResultSet에 설정합니다.");

            //  Procedure 라면, Output Parameter 등을 ResultSet으로 반환함.
            //! 단 Procedure가 ResultSet을 반환해야 하는 경우는 OpenQuery를 수행할 수 있도록 ResponseFormat 을 ResponseFormatKind.ResultSet 으로 할 것
            //
            var parameters = AdoRepository.ExecuteProcedure(query, adoParameters);

            if(parameters != null) {
                responseItem.ResultValue = parameters.GetReturnValue();

                var row = new ResultRow(parameters.ToDictionary(p => p.Name, p => p.Value));
                responseItem.ResultSet.Add(row);
            }
        }

        /// <summary>
        /// 단순 쿼리문에 대해서, ExecuteNonQuery를 수행합니다.
        /// </summary>
        /// <param name="queries"></param>
        protected virtual void ExecuteQueries(IEnumerable<string> queries) {
            foreach(var query in queries.Where(q => q.IsNotWhiteSpace()))
                AdoRepository.ExecuteNonQuery(query);
        }
    }
}