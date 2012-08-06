namespace NSoft.NFramework.Data.AdoPoco {
    /// <summary>
    /// Transaction 클래스
    /// </summary>
    public class AdoTransaction : IAdoTransaction {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private IAdoProvider _db;

        public AdoTransaction(IAdoProvider db) {
            db.ShouldNotBeNull("db");
            _db = db;
        }

        /// <summary>
        /// Transaction 을 완료합니다.
        /// </summary>
        public virtual void Complete() {
            Guard.Assert(_db != null, "Transaction을 생성한 IAdoProvider 인스턴스가 이미 메모리에서 해제되었습니다.");

            With.TryAction(_db.CompleteTransaction);
            _db = null;
        }

        /// <summary>
        /// 관리되지 않는 리소스의 확보, 해제 또는 다시 설정과 관련된 응용 프로그램 정의 작업을 수행합니다.
        /// </summary>
        public void Dispose() {
            if(_db != null) {
                With.TryAction(_db.AbortTransaction);
            }
        }
    }
}