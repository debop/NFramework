using System;
using System.Data;
using System.Xml.Serialization;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.XmlData.Messages {
    /// <summary>
    /// Response Document for Xml Data Service 
    /// </summary>
    [XmlRoot("RWXMLDS")]
    [Serializable]
    public class XdsResponseDocument : XdsDocumentBase {
        #region << logger >>

        [NonSerialized] private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Initialize a new instance of XdsResponseDocument.
        /// </summary>
        public XdsResponseDocument()
            : base(XmlDataDirectionKind.Response) {
            if(IsDebugEnabled)
                log.Debug("Initialize a new instance of XdsResponseDocument.");
        }

        private XdsResponseItemCollection _responses = new XdsResponseItemCollection();

        /// <summary>
        /// Collection of <see cref="XdsResponseItem"/>
        /// </summary>
        [XmlElement("RESPONSE")]
        public XdsResponseItemCollection Responses {
            get { return _responses ?? (_responses = new XdsResponseItemCollection()); }
            set { _responses = value; }
        }

        /// <summary>
        /// <see cref="XdsResponseItem"/>를 반환한다.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public XdsResponseItem this[int index] {
            get { return Responses[index]; }
        }

        /// <summary>
        /// 요청사항 실행 결과 예외가 발생했는지를 나타낸다.
        /// </summary>
        [XmlIgnore]
        public bool HasError {
            get { return (Errors.Count > 0); }
        }

        /// <summary>
        /// Add new <see cref="XdsResponseItem"/>  with response type, request id, sequence id
        /// </summary>
        /// <param name="responseKind">response type</param>
        /// <param name="requestId">request id</param>
        /// <param name="sequenceId">sequece id of the specified request.</param>
        /// <returns>new instance of <see cref="XdsResponseItem"/></returns>
        public XdsResponseItem AddResponseItem(XmlDataResponseKind responseKind, int requestId, int sequenceId) {
            return Responses.AddResponseItem(responseKind, requestId, sequenceId);
        }

        /// <summary>
        /// Add new <see cref="XdsResponseItem"/>
        /// </summary>
        /// <param name="responseItem">Instance of <see cref="XdsResponseItem"/> to add.</param>
        /// <returns>index of collection, if <paramref name="responseItem"/> is null, return -1</returns>
        public int AddResponseItem(XdsResponseItem responseItem) {
            return Responses.AddResponseItem(responseItem);
        }

        /// <summary>
        /// Add new <see cref="XdsResponseItem"/> with <see cref="IDataReader"/>, request id, sequence id
        /// </summary>
        /// <param name="dr">Instance of <see cref="IDataReader"/></param>
        /// <param name="requestId">request id</param>
        /// <param name="sequenceId">sequece id of the specified request.</param>
        /// <returns>index of collection</returns>
        public XdsResponseItem AddResponseItem(IDataReader dr, int requestId, int sequenceId) {
            return Responses.AddResponseItem(dr, requestId, sequenceId);
        }

        /// <summary>
        /// Add new <see cref="XdsResponseItem"/> with <see cref="DataView"/>, request id, sequence id
        /// </summary>
        /// <param name="dv">Instance of <see cref="DataView"/></param>
        /// <param name="requestId">request id</param>
        /// <param name="sequenceId">sequece id of the specified request.</param>
        /// <returns>index of collection</returns>
        public XdsResponseItem AddResponseItem(DataView dv, int requestId, int sequenceId) {
            return Responses.AddResponseItem(dv, requestId, sequenceId);
        }

        /// <summary>
        /// Add new collection of <see cref="XdsResponseItem"/> with <see cref="DataSet"/>, request id, sequence id
        /// </summary>
        /// <param name="ds">Instance of <see cref="DataSet"/></param>
        /// <param name="requestId">request id</param>
        /// <param name="sequenceId">sequece id of the specified request.</param>
        public void AddResponseItemArray(DataSet ds, int requestId, int sequenceId) {
            Responses.AddResponseItemArray(ds, requestId, sequenceId);
        }

        /// <summary>
        /// Convert current object to <see cref="DataSet"/>
        /// </summary>
        /// <returns>new instance of <see cref="DataSet"/></returns>
        public DataSet ToDataSet() {
            return ToDataSet(string.Empty);
        }

        /// <summary>
        /// Convert current object to <see cref="DataSet"/> with the specified name
        /// </summary>
        /// <param name="dataSetName">name of dataset</param>
        /// <returns>new instance of <see cref="DataSet"/></returns>
        public virtual DataSet ToDataSet(string dataSetName) {
            if(IsDebugEnabled)
                log.Debug("Create new instance of DataSet from XdsResponseDocument. dataSetName:" + dataSetName);

            var ds = (dataSetName.IsWhiteSpace()) ? new DataSet() : new DataSet(dataSetName);

            for(int i = 0; i < Responses.Count; i++) {
                var dt = Responses[i].ToDataTable("Response_" + i);
                if(dt != null)
                    ds.Tables.Add(dt);
            }

            return ds;
        }
    }
}