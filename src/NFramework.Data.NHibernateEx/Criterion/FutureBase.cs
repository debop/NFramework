using System;

namespace NSoft.NFramework.Data.NHibernateEx.Criterion {
    /// <summary>
    /// Future Value (Lazy Loading Entity)를 구현하기 위한 기본 클래스입니다.
    /// Current Thread 하에서 여러 Entity들을 Future Value형식으로 Loading하기로 예약하고, 
    /// 실제 사용할 때 하나의 Batch Query 형식으로, 한꺼번에 가져온다.
    /// 이렇게 하면 Network Round trip이 줄어들고, 실제 성능이 향상된다.
    /// </summary>
    [Obsolete("NHibernate 3.x 의 Future를 사용하세요")]
    public class FutureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 캐시를 위한 Local 저장소 키
        /// </summary>
        public const string CacheKey = "NSoft.NFramework.Data.NHibernate.FutureValue.Cache.Key";

        /// <summary>
        /// indicateing this instance was loaded
        /// </summary>
        protected virtual bool IsLoaded { get; set; }

        /// <summary>
        /// <see cref="CriteriaBatch"/> instance in current thread context.
        /// </summary>
        protected static CriteriaBatch Batcher {
            get {
                object current;

                if(Local.Data.TryGetValue(CacheKey, out current) == false)
                    Local.Data[CacheKey] = current = new CriteriaBatch();

                return current as CriteriaBatch;
            }
        }

        /// <summary>
        /// Execute all the queries in the batch.
        /// </summary>
        protected virtual void ExecuteBatchQuery() {
            if(IsDebugEnabled)
                log.Debug("Execute Batch queries is starting...");

            Batcher.Execute(UnitOfWork.CurrentSession);
            IsLoaded = true;
            ClearBatcher();

            if(IsDebugEnabled)
                log.Debug("Execute Batch queries is success!!!");
        }

        /// <summary>
        /// Clears the batcher.
        /// </summary>
        private static void ClearBatcher() {
            Local.Data[CacheKey] = null;

            if(IsDebugEnabled)
                log.Debug("batcher is cleared.");
        }
    }
}