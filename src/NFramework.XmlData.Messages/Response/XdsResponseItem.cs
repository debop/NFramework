using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.XmlData.Messages {
    /// <summary>
    /// 하나의 요청(<see cref="XdsRequestItem"/>)에 대한 결과를 나타낸다.
    /// </summary>
    [Serializable]
    public class XdsResponseItem {
        #region << logger >>

        [NonSerialized] private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Initialize a new instance of <see cref="XdsResponseItem"/>
        /// </summary>
        public XdsResponseItem() {
            InitFields();
        }

        /// <summary>
        /// Initialize a new instance of <see cref="XdsResponseItem"/> with response type ...
        /// </summary>
        /// <param name="responseKind">Response type</param>
        /// <param name="requestId">request id</param>
        /// <param name="sequenceId">sequence id (요청에 여러개의 parameter set이 있는 경우)</param>
        public XdsResponseItem(XmlDataResponseKind responseKind, int requestId, int sequenceId) {
            InitFields();

            ResponseKind = responseKind;
            RequestId = requestId;
            SequenceId = sequenceId;
        }

        /// <summary>
        /// Initialize parameters.
        /// </summary>
        protected virtual void InitFields() {
            RequestId = MsgConsts.INVALID_ID;
            ResponseId = MsgConsts.INVALID_ID;
            SequenceId = MsgConsts.INVALID_ID;
            PageCount = MsgConsts.INVALID_ID;
            TotalRecordCount = MsgConsts.INVALID_ID;
            PageSize = MsgConsts.NO_PAGE_INDEX;
            PageNo = MsgConsts.NO_PAGE_INDEX;
            Sort = string.Empty;

            ResponseKind = XmlDataResponseKind.None;
        }

        /// <summary>
        /// 요청 Id
        /// </summary>
        [XmlAttribute("reqId")]
        public int RequestId { get; set; }

        /// <summary>
        /// 응답 Id
        /// </summary>
        [XmlAttribute("id")]
        public int ResponseId { get; set; }

        /// <summary>
        /// 요청에 대한 다중 결과 시의 시퀀스 Id
        /// </summary>
        [XmlAttribute("seqId")]
        public int SequenceId { get; set; }

        /// <summary>
        /// 결과 페이지 수
        /// </summary>
        [XmlAttribute("pageCount")]
        public int PageCount { get; set; }

        /// <summary>
        /// 전체 결과 레코드 수
        /// </summary>
        [XmlAttribute("totalCount")]
        public int TotalRecordCount { get; set; }

        /// <summary>
        /// 결과 페이징시의 페이지 크기
        /// </summary>
        [XmlAttribute("pageSize")]
        public int PageSize { get; set; }

        /// <summary>
        /// 결과 페이징시의 현재 페이지
        /// </summary>
        [XmlAttribute("pageNo")]
        public int PageNo { get; set; }

        /// <summary>
        /// 정렬 조건
        /// </summary>
        [XmlAttribute("sort")]
        public string Sort { get; set; }

        /// <summary>
        /// 응답 종류 <c>XmlDataResponseKind</c> (None(0), Dataset(1), Scalar(2), Xml(3))
        /// </summary>
        [XmlAttribute("rtype")]
        public int RType {
            get { return ResponseKind.ToString("D").AsInt(ResponseKind.GetHashCode()); }
            set { ResponseKind = value.AsEnum(XmlDataResponseKind.None); }
        }

        /// <summary>
        /// Response Type
        /// </summary>
        [XmlIgnore]
        public XmlDataResponseKind ResponseKind { get; set; }

        private XdsFieldCollection _fields = new XdsFieldCollection();

        /// <summary>
        /// 결과 SET의 Field 정보 (DB 스키마 정보)
        /// </summary>
        [XmlArray("FIELDS")]
        [XmlArrayItem("F", typeof(XdsField))]
        public XdsFieldCollection Fields {
            get { return _fields ?? (_fields = new XdsFieldCollection()); }
            set { _fields = value; }
        }

        private XdsRecordCollection _records = new XdsRecordCollection();

        /// <summary>
        /// 결과 SET 정보
        /// </summary>
        [XmlArray("RECORDS")]
        [XmlArrayItem("R", typeof(XdsRecord))]
        public XdsRecordCollection Records {
            get { return _records ?? (_records = new XdsRecordCollection()); }
            set { _records = value; }
        }

        /// <summary>
        /// 결과 셋의 하나의 값을 반환한다.
        /// </summary>
        /// <param name="row">row index</param>
        /// <param name="column">column index</param>
        /// <returns>Value of Resultset</returns>
        public object this[int row, int column] {
            get { return Records[row, column]; }
        }

        /// <summary>
        /// 결과 셋의 하나의 값을 반환한다.
        /// </summary>
        /// <param name="row">row index</param>
        /// <param name="fieldName">filed name</param>
        /// <returns>Value of Resultset</returns>
        public object this[int row, string fieldName] {
            get { return this[row, GetFieldOrder(fieldName)]; }
        }

        [NonSerialized] private readonly IDictionary<string, int> _fieldOrderCache = new Dictionary<string, int>();

        /// <summary>
        /// find order index of field which name is specified fieldname
        /// </summary>
        /// <param name="fieldName">field name</param>
        /// <returns>field(column) index. if not exist the field, return -1.</returns>
        protected virtual int GetFieldOrder(string fieldName) {
            fieldName.ShouldNotBeWhiteSpace("fieldName");

            lock(_fieldOrderCache) {
                if(_fieldOrderCache.ContainsKey(fieldName))
                    return _fieldOrderCache[fieldName];

                for(var i = 0; i < Fields.Count; i++) {
                    if(Fields[i].Name.EqualTo(fieldName)) {
                        if(IsDebugEnabled)
                            log.Debug("필드명 [{0}]의 Order [{1}]를 찾아서 Cache에 넣었습니다.", fieldName, i);

                        _fieldOrderCache.Add(Fields[i].Name, i);
                        return i;
                    }
                }
            }
            if(log.IsWarnEnabled)
                log.Warn("해당 필드 이름이 존재하지 않습니다. fieldname" + fieldName);

            return -1;
        }

        /// <summary>
        /// ResultSet 내용을 2차원 배열로 빌드하여 반환한다.
        /// </summary>
        /// <param name="withFieldName">Field name을 배열의 첫번째 행에 넣을 것인가?</param>
        /// <returns>ResultSet 내용을 담은 2차원 배열</returns>
        public object[,] ToArray(bool withFieldName) {
            int columnCount = Fields.Count;
            int rowCount = Records.Count;

            var result = new object[rowCount,columnCount];

            int offset = 0;

            if(withFieldName) {
                for(int i = 0; i < columnCount; i++)
                    result[offset, i] = Fields[i].Name;

                offset = 1;
            }

            for(int r = 0; r < rowCount; r++)
                for(int c = 0; c < columnCount; c++)
                    result[r + offset, c] = Records[r, c];

            return result;
        }

        /// <summary>
        /// Response 객체를 DataTable로 만든다.
        /// </summary>
        /// <param name="tableName">table name for <see cref="DataTable"/></param>
        /// <returns>instance of <see cref="DataTable"/></returns>
        public DataTable ToDataTable(string tableName) {
            if(IsDebugEnabled)
                log.Debug("Create new DataTable from XdsResponseItem. TableName:" + tableName);

            if(Fields.Count <= 0 || Records.Count <= 0)
                return null;

            tableName = tableName.Replace(" ", "_", true);

            var dt = (tableName.IsWhiteSpace()) ? new DataTable() : new DataTable(tableName);

            for(var f = 0; f < Fields.Count; f++) {
                var t = Type.GetType(Fields[f].TypeName);
                dt.Columns.Add(new DataColumn(Fields[f].Name, t ?? typeof(object)));
            }

            // record 정보를 채운다
            for(var r = 0; r < Records.Count; r++) {
                dt.Rows.Add(Records[r].GetValues());
            }

            return dt;
        }

        /// <summary>
        /// <see cref="DataSet"/> 으로부터 <see cref="XdsResponseItemCollection"/>을 빌드한다.
        /// </summary>
        /// <param name="ds">원본 <see cref="DataSet"/></param>
        /// <returns>Instance of <see cref="XdsResponseItemCollection"/></returns>
        public static XdsResponseItemCollection Create(DataSet ds) {
            ds.ShouldNotBeNull("ds");

            var responseItems = new XdsResponseItemCollection();

            foreach(DataTable table in ds.Tables)
                responseItems.AddResponseItem(Create(table.DefaultView));

            //for (int i = 0; i < ds.Tables.Count; i++)
            //    responseItems.AddResponseItem(Create(ds.Tables[i].DefaultView));

            return responseItems;
        }

        /// <summary>
        /// <see cref="IDataReader"/> 로부터 <see cref="XdsResponseItem"/>을 빌드한다.
        /// </summary>
        /// <param name="dr"></param>
        /// <returns>Instance of <see cref="XdsResponseItem"/></returns>
        public static XdsResponseItem Create(IDataReader dr) {
            dr.ShouldNotBeNull("dr");

            if(IsDebugEnabled)
                log.Debug("Create new XdsResponseItem from IDataReader");

            var responseItem = new XdsResponseItem();

            for(var i = 0; i < dr.FieldCount; i++)
                responseItem.Fields.AddField(dr.GetName(i), dr.GetDataTypeName(i), MsgConsts.INVALID_ID);

            do {
                while(dr.Read())
                    responseItem.Records.AddRecord(dr);
            } while(dr.NextResult());

            return responseItem;
        }

        /// <summary>
        /// <see cref="DataView"/>로부터 <see cref="XdsResponseItem"/>을 빌드한다.
        /// </summary>
        /// <param name="dv">원본 <see cref="DataView"/></param>
        /// <returns>Instance of <see cref="XdsResponseItem"/></returns>
        public static XdsResponseItem Create(DataView dv) {
            dv.ShouldNotBeNull("dv");

            var responseItem = new XdsResponseItem();

            for(var i = 0; i < dv.Table.Columns.Count; i++)
                responseItem.Fields.AddField(dv.Table.Columns[i]);

            for(var i = 0; i < dv.Count; i++)
                responseItem.Records.AddRecord(dv[i].Row);

            return responseItem;
        }
    }

    /// <summary>
    /// Collection of <see cref="XdsResponseItem"/>
    /// </summary>
    [Serializable]
    public class XdsResponseItemCollection : List<XdsResponseItem> {
        #region << logger >>

        [NonSerialized] private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Add new <see cref="XdsResponseItem"/>
        /// </summary>
        /// <param name="responseItem">Instance of <see cref="XdsResponseItem"/> to add.</param>
        /// <returns>index of collection, if <paramref name="responseItem"/> is null, return -1</returns>
        public int AddResponseItem(XdsResponseItem responseItem) {
            if(responseItem == null)
                return -1;

            base.Add(responseItem);
            responseItem.ResponseId = Count - 1;

            if(IsDebugEnabled)
                log.Debug("Add new XdsResponseItem is SUCCESS. ResponseId=[{0}]", responseItem.ResponseId);

            return responseItem.ResponseId;
        }

        /// <summary>
        /// Add new <see cref="XdsResponseItem"/>  with response type, request id, sequence id
        /// </summary>
        /// <param name="responseKind">response type</param>
        /// <param name="requestId">request id</param>
        /// <param name="sequenceId">sequece id of the specified request.</param>
        /// <returns>new instance of <see cref="XdsResponseItem"/></returns>
        public virtual XdsResponseItem AddResponseItem(XmlDataResponseKind responseKind, int requestId, int sequenceId) {
            if(IsDebugEnabled)
                log.Debug(@"Add new XdsResponseItem. responseKind=[{0}], requestId=[{1}], sequenceId=[{2}]",
                          responseKind, requestId, sequenceId);

            var item = new XdsResponseItem(responseKind, requestId, sequenceId);
            AddResponseItem(item);
            return item;
        }

        /// <summary>
        /// Add new <see cref="XdsResponseItem"/> with <see cref="IDataReader"/>, request id, sequence id
        /// </summary>
        /// <param name="dr">Instance of <see cref="IDataReader"/></param>
        /// <param name="requestId">request id</param>
        /// <param name="sequenceId">sequece id of the specified request.</param>
        /// <returns>index of collection</returns>
        public virtual XdsResponseItem AddResponseItem(IDataReader dr, int requestId, int sequenceId) {
            var item = XdsResponseItem.Create(dr);
            item.RequestId = requestId;
            item.SequenceId = sequenceId;

            AddResponseItem(item);
            return item;
        }

        /// <summary>
        /// Add new <see cref="XdsResponseItem"/> with <see cref="DataView"/>, request id, sequence id
        /// </summary>
        /// <param name="dv">Instance of <see cref="DataView"/></param>
        /// <param name="requestId">request id</param>
        /// <param name="sequenceId">sequece id of the specified request.</param>
        /// <returns>index of collection</returns>
        public virtual XdsResponseItem AddResponseItem(DataView dv, int requestId, int sequenceId) {
            var item = XdsResponseItem.Create(dv);
            item.RequestId = requestId;
            item.SequenceId = sequenceId;

            AddResponseItem(item);
            return item;
        }

        /// <summary>
        /// Add new collection of <see cref="XdsResponseItem"/> with <see cref="DataSet"/>, request id, sequence id
        /// </summary>
        /// <param name="ds">Instance of <see cref="DataSet"/></param>
        /// <param name="requestId">request id</param>
        /// <param name="sequenceId">sequece id of the specified request.</param>
        public void AddResponseItemArray(DataSet ds, int requestId, int sequenceId) {
            var items = XdsResponseItem.Create(ds);

            for(var i = 0; i < items.Count; i++) {
                items[i].RequestId = requestId;
                items[i].SequenceId = sequenceId;
            }

            AddRange(items);
        }
    }
}