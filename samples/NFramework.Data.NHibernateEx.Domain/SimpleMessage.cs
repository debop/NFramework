using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    [Serializable]
    public class SimpleMessage {
        #region << logger >>

        [NonSerialized] private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public string Sender { get; set; }

        public string Receiver { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime? SendTime { get; set; }

        public DateTime? ReceiveTime { get; set; }

        public bool? IsRead { get; set; }
    }
}