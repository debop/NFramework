using System;
using System.Net;
using NSoft.NFramework.Parallelism.Tools;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// 특정 웹 주소의 컨텐츠 내용을 캐시합니다.
    /// </summary>
    [Serializable]
    public class FutureWebCache : FutureCache<Uri, string> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly Func<Uri, string> @downloadStringTask =
            uri => With.TryFunctionAsync(() => new WebClient().DownloadStringTask(uri).Result,
                                         ageAction:
                                             age => {
                                                 if(log.IsWarnEnabled) {
                                                     log.Warn("웹 컨텐츠를 다운로드하는데 실패했습니다.  uri=[{0}]", uri);
                                                     log.Warn(age);
                                                 }
                                             });

        /// <summary>
        /// 생성자
        /// </summary>
        public FutureWebCache() : base(@downloadStringTask) {}

        public override string GetValue(Uri key) {
            if(IsDebugEnabled)
                log.Debug("웹 컨텐츠를 받기 작업을 시작합니다... key=[{0}]", key);

            var webContent = base.GetValue(key);

            if(webContent != null) {
                return webContent;
            }

            if(IsDebugEnabled)
                log.Debug("웹 컨텐츠를 받지 못했습니다. 다시 받기 위해, 해당 키의 캐시를 제거합니다. key=[{0}]", key);

            Remove(key);
            return null;
        }
    }
}