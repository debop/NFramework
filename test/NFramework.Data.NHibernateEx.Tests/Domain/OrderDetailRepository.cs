namespace NSoft.NFramework.Data.NHibernateEx.Domain.Northwind {
    public class OrderDetailRepository : NHRepository<OrderDetail> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public OrderDetailRepository() {}

        public OrderDetailRepository(string message) {
            if(IsDebugEnabled)
                log.Debug("OrderDetail 전용 Repository를 생성했습니다. Message: " + message);
        }
    }
}