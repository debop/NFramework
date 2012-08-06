using System.Linq;
using NSoft.NFramework.Data;
using NSoft.NFramework.XmlData.Messages;

namespace NSoft.NFramework.XmlData {
    /// <summary>
    /// Client의 DB 처리 요청 정보(<see cref="XdsRequestDocument"/>)를 파싱하여 비동기적으로 쿼리를 수행한 후,
    /// 결과를 <see cref="XdsResponseDocument"/> 객체에 담아 반환하는 기능을 수행한다.
    /// </summary>
    // NOTE: 요청 쿼리문을 TPL을 이용하여, 병렬로 수행합니다. 
    // NOTE: MS SQL의 경우에는 EntLib 5를 이용하여 I/O 작업을 비동기적으로 실행하면, 성능을 더 높힐 수 있다.
    // TODO: 현재 이 클래스는 두가지를 해결해야 한다.
    //       1. 병렬 수행에 따라, ResponseDocument를 순서대로 넣기
    //       2. ResponseDocument를 조작할 때 동기화 하기 
    public class XmlDataAsyncManager : XmlDataManager, IXmlDataManager {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public XmlDataAsyncManager() {}
        public XmlDataAsyncManager(string dbName) : base(dbName) {}
        public XmlDataAsyncManager(IAdoRepository ado) : base(ado) {}

        /// <summary>
        /// 요청 정보를 병렬로 모두 수행합니다.
        /// </summary>
        protected override void ExecuteRequestCore(XdsRequestDocument requestDocument, XdsResponseDocument responseDocument) {
            if(requestDocument.IsParallelToolecute == false) {
                base.ExecuteRequestCore(requestDocument, responseDocument);
                return;
            }

            if(IsDebugEnabled)
                log.Debug(@"요청을 병렬방식으로 수행합니다...");

            // 요청을 병렬로 처리합니다.
            requestDocument.Requests
                .AsParallel()
                .AsOrdered()
                .ForAll(request => DoProcessRequestItem(request, responseDocument));

            if(IsDebugEnabled)
                log.Info(@"모든 요청을 병렬 수행으로 처리했습니다.");
        }
    }
}