using System;
using System.Web.UI;
using NSoft.NFramework.Caching;

namespace NSoft.NFramework.Web.PageStatePersisters {
    /// <summary>
    /// <see cref="ConcurrentCacheRepository"/>를 이용한 PageStatePersister 입니다.
    /// </summary>
    [Serializable]
    public class ConcurrentPageStatePersister : PageStatePersister<ConcurrentCacheRepository> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public ConcurrentPageStatePersister(Page page) : base(page) {}
    }
}