using System;
using System.Collections.Specialized;

namespace NSoft.NFramework.JobScheduler.Managers {
    /// <summary>
    /// 표준 서비스 작업 관리자
    /// </summary>
    [CLSCompliant(false)]
    public class StdServiceJobManager : AbstractServiceJobManager {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public StdServiceJobManager() {
            if(IsDebugEnabled)
                log.Debug("StdServiceJobManager가 생성되었습니다...");
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="props">속성 정보</param>
        public StdServiceJobManager(NameValueCollection props) : base(props) {
            if(IsDebugEnabled)
                log.Debug("StdServiceJobManager가 생성되었습니다...");
        }
    }
}