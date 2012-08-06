using System.Linq;
using NSoft.NFramework.Data;
using NSoft.NFramework.DataServices.Messages;

namespace NSoft.NFramework.DataServices {
    /// <summary>
    /// 비동기 방식으로 DataService를 제공하는 클래스입니다.
    /// </summary>
    public class AsyncDataServiceImpl : DataServiceImpl {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public AsyncDataServiceImpl() {}

        public AsyncDataServiceImpl(IAdoRepository repository) : base(repository) {}

        protected override void DoProcessRequestItems(RequestMessage requestMessage, ResponseMessage responseMessage) {
            // 요청작업이 병렬로 처리해서는 안될 경우를 대비
            //
            if(requestMessage.AsParallel == false) {
                base.DoProcessRequestItems(requestMessage, responseMessage);
                return;
            }

            if(IsDebugEnabled)
                log.Debug("요청 항목들을 병렬로 처리합니다...");

            // 병렬로 작업을 처리하고, 응답메시지에 추가합니다.
            requestMessage.Items
                .AsParallel()
                .AsOrdered()
                .ForAll(requestItem => {
                            var responseItem = DoProcessRequestItem(requestItem);
                            responseMessage.Items.Add(responseItem);
                        });
        }
    }
}