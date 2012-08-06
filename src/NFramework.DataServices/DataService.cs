using System.Threading;
using NSoft.NFramework.Data;
using NSoft.NFramework.DataServices.Messages;

namespace NSoft.NFramework.DataServices {
    /// <summary>
    /// Data Service 의 Static Class 입니다. 실제 IDataService 구현체를 생성하지 않고, IoC로부터 인스턴스를 Resolve 해서 사용합니다.
    /// </summary>
    public static class DataService {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static IDataService _instance;
        private static readonly object _syncLock = new object();

        /// <summary>
        /// JSON Data Service 를 수행할 실제 인스턴스. IoC를 통해 제공받습니다.
        /// </summary>
        internal static IDataService Instance {
            get {
                if(_instance == null)
                    lock(_syncLock)
                        if(_instance == null) {
                            var dataService = DataServiceTool.ResolveDataService();
                            Thread.MemoryBarrier();
                            _instance = dataService;

                            if(IsDebugEnabled)
                                log.Debug("IDataService의 인스턴스를 생성했습니다. Instance=[{0}]", dataService);
                        }

                return _instance;
            }
            set { _instance = value; }
        }

        /// <summary>
        /// Data 처리 시에 사용할 <see cref="IAdoRepository"/> 인스턴스
        /// </summary>
        public static IAdoRepository AdoRepository {
            get { return Instance.AdoRepository; }
        }

        /// <summary>
        /// DATA 처리를 위한 요청정보를 처리해서, 응답정보를 빌드해서 반환합니다.
        /// </summary>
        /// <param name="requestMessage">요청 메시지</param>
        /// <returns>응답 메시지</returns>
        public static ResponseMessage Execute(RequestMessage requestMessage) {
            return Instance.Execute(requestMessage);
        }
    }
}