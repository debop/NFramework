using System;
using System.Xml.Serialization;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    [Serializable]
    [XmlRoot(ElementName = "SimpleMessage")]
    public class FSimpleMessageXml {
        #region << logger >>

        [NonSerialized] private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [XmlAttribute]
        public string Sender { get; set; }

        [XmlAttribute]
        public string Receiver { get; set; }

        [XmlElement]
        public string Title { get; set; }

        [XmlElement]
        public string Content { get; set; }

        [XmlAttribute]
        public DateTime SendTime { get; set; }

        [XmlAttribute]
        public DateTime ReceiveTime { get; set; }

        [XmlAttribute]
        public bool IsRead { get; set; }
    }
}