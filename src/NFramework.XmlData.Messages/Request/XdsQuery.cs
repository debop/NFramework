using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NSoft.NFramework.XmlData.Messages {
    /// <summary>
    /// 단순 쿼리문을 나타내는 객체
    /// </summary>
    [Serializable]
    public class XdsQuery {
        /// <summary>
        /// Constructor
        /// </summary>
        public XdsQuery() : this(string.Empty, XmlDataRequestKind.Query) {}

        /// <summary>
        /// Constructor with the specified query string, request type
        /// </summary>
        /// <param name="query">simple query string to execute</param>
        /// <param name="requestKind">request type</param>
        public XdsQuery(string query, XmlDataRequestKind requestKind) {
            Query = query;
            RequestKind = requestKind;
        }

        /// <summary>
        /// Query String to execute
        /// </summary>
        [XmlText]
        public string Query { get; set; }

        /// <summary>
        /// Int Number of Request Type
        /// </summary>
        [XmlAttribute("qtype")]
        public int QType {
            get { return RequestKind.ToString("D").AsInt(RequestKind.GetHashCode); }
            set { RequestKind = value.AsEnum(XmlDataRequestKind.Query); }
        }

        /// <summary>
        /// Request Type
        /// </summary>
        [XmlIgnore]
        public XmlDataRequestKind RequestKind { get; set; }
    }

    /// <summary>
    /// Collection of <see cref="XdsQuery"/>
    /// </summary>
    [Serializable]
    public class XdsQueryCollection : List<XdsQuery> {
        /// <summary>
        /// Add new instance of <see cref="XdsQuery"/> that has the specified query string.
        /// </summary>
        /// <param name="query">query string to execute</param>
        /// <returns>Return added <see cref="XdsQuery"/></returns>
        public XdsQuery AddQuery(string query) {
            return AddQuery(query, XmlDataRequestKind.Query);
        }

        /// <summary>
        /// Add new instance of <see cref="XdsQuery"/> that has the specified query string and request type
        /// </summary>
        /// <param name="query">query string to execute</param>
        /// <param name="requestKind">request type</param>
        /// <returns>Return added <see cref="XdsQuery"/></returns>
        public virtual XdsQuery AddQuery(string query, XmlDataRequestKind requestKind) {
            var xq = new XdsQuery(query, requestKind);
            base.Add(xq);
            return xq;
        }
    }
}