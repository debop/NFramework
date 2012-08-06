using System.Linq;
using NSoft.NFramework.Data;
using NSoft.NFramework.Data.Mappers;

namespace NSoft.NFramework.DataServices.DataServices {
    public class AsyncDataCommandServiceImpl : DataCommandServiceImpl {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public AsyncDataCommandServiceImpl() {}
        public AsyncDataCommandServiceImpl(IAdoRepository repository) : base(repository) {}
        public AsyncDataCommandServiceImpl(IAdoRepository repository, INameMapper nameMapper) : base(repository, nameMapper) {}

        protected override void DoProcessRequestItems(Messages.RequestMessage requestMessage, Messages.ResponseMessage responseMessage) {
            // 요청작업이 병렬로 처리해서는 안될 경우를 대비
            //
            if(requestMessage.AsParallel == false) {
                base.DoProcessRequestItems(requestMessage, responseMessage);
                return;
            }

            if(IsDebugEnabled)
                log.Debug("요청항목들을 병렬로 처리합니다...");

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