using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NSoft.NFramework.XmlData.Messages {
    /// <summary>
    /// Request 객체 (<REQUEST/>) - 단위 요청 정보
    /// </summary>
    [Serializable]
    public class XdsRequestItem {
        #region << logger >>

        [NonSerialized] private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public XdsRequestItem() {
            Query = string.Empty;
            Id = MsgConsts.INVALID_ID;
            Sort = string.Empty;

            RequestKind = XmlDataRequestKind.Query;
            ResponseKind = XmlDataResponseKind.None;

            PageSize = MsgConsts.NO_PAGE_INDEX;
            PageNo = MsgConsts.NO_PAGE_INDEX;

            Parameters = new XdsParameterCollection();
            Values = new XdsValueCollection();

            PreQueries = new XdsQueryCollection();
            PostQueries = new XdsQueryCollection();
        }

        /// <summary>
        /// 수행할 SQL Query 문
        /// </summary>
        [XmlElement("QUERY")]
        public string Query { get; set; }

        /// <summary>
        /// 요청 순서를 나타내는 Index
        /// </summary>
        [XmlAttribute("id")]
        public int Id { get; set; }

        /// <summary>
        /// 요청 종류 ( 0 : StoredProcedure, 1 : Query ) <seealso cref="XmlDataRequestKind"/>의 서수형.
        /// </summary>
        [XmlAttribute("qtype")]
        public int QType {
            get { return RequestKind.ToString("D").AsInt(RequestKind.GetHashCode); }
            set { RequestKind = value.AsEnum(XmlDataRequestKind.Query); }
        }

        /// <summary>
        /// 요청 종류 ( 0 : Query, 1 : StoredProcedure )
        /// </summary>
        [XmlIgnore]
        public XmlDataRequestKind RequestKind { get; set; }

        /// <summary>
        /// 응답 종류 (None(0), Dataset(1), Scalar(2), Xml(3)) <seealso cref="XmlDataResponseKind"/>
        /// </summary>
        [XmlAttribute("rtype")]
        public int RType {
            get { return ResponseKind.ToString("D").AsInt(ResponseKind.GetHashCode); }
            set { ResponseKind = value.AsEnum(XmlDataResponseKind.None); }
        }

        /// <summary>
        /// 응답 종류 (None(0), Dataset(1), Scalar(2), Xml(3)) <seealso cref="XmlDataResponseKind"/>
        /// </summary>
        [XmlIgnore]
        public XmlDataResponseKind ResponseKind { get; set; }

        /// <summary>
        /// 페이징 기능시의 단위 페이지 크기
        /// </summary>
        [XmlAttribute("pageSize")]
        public int PageSize { get; set; }

        /// <summary>
        /// 페이징 기능 적용시의 결과로 받고자하는 페이지 번호 (0부터 시작)
        /// </summary>
        [XmlAttribute("pageNo")]
        public int PageNo { get; set; }

        /// <summary>
        /// 정렬 적용 방법 (이제는 사용하지 않는다.)
        /// </summary>
        [XmlAttribute("sort")]
        public string Sort { get; set; }

        /// <summary>
        /// Stored Procedure의 인자 정보 Collection
        /// </summary>
        [XmlArray("PARAMS")]
        [XmlArrayItem("P", typeof(XdsParameter))]
        public XdsParameterCollection Parameters { get; set; }

        /// <summary>
        /// Parameters 대한 입력 값
        /// </summary>
        [XmlArray("VALUES")]
        [XmlArrayItem("V", typeof(XdsValue))]
        public XdsValueCollection Values { get; set; }

        /// <summary>
        /// 실제 요청 작업 전에 수행할 선행 작업 정보
        /// </summary>
        [XmlArray("PREQUERIES")]
        [XmlArrayItem("Q", typeof(XdsQuery))]
        public XdsQueryCollection PreQueries { get; set; }

        /// <summary>
        /// 실제 요청 작업 후에 수행할 후행 작업 정보
        /// </summary>
        [XmlArray("POSTQUERIES")]
        [XmlArrayItem("Q", typeof(XdsQuery))]
        public XdsQueryCollection PostQueries { get; set; }

        /// <summary>
        /// Parameter를 추가한다.
        /// </summary>
        /// <param name="requestParamNames"></param>
        public virtual void AddParamArray(params string[] requestParamNames) {
            if(requestParamNames == null || requestParamNames.Length == 0)
                return;

            foreach(string paramName in requestParamNames)
                Parameters.AddParameter(paramName);
        }

        /// <summary>
        /// 이차원 배열의 Arguement를 넣는다.
        /// 새로운 인자집합을 가진 Value 객체를 만들고 그것을 ValueCollection객체에 넣는다.
        /// </summary>
        /// <param paramName="args">argument set</param>
        public virtual void AddValueArray(params object[][] args) {
            if(args == null || !args.GetType().IsArray)
                return;

            int rank = args.Rank;
            int dims = args.GetLength(rank - 1);

            for(int i = 0; i < dims; i++)
                Values.AddValue(args[i]);
        }

        /// <summary>
        /// Parameter의 값을 설정한다.
        /// </summary>
        /// <param name="args"></param>
        public virtual void AddValue(params object[] args) {
            if(args != null && args.Length > 0)
                Values.AddValue(args);
        }
    }

    /// <summary>
    /// Collection of <see cref="XdsRequestItem"/>
    /// </summary>
    [Serializable]
    public class XdsRequestItemCollection : List<XdsRequestItem> {
        #region << logger >>

        [NonSerialized] private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 서버에서 수행할 쿼리문을 요청문서에 등록한다.
        /// </summary>
        /// <param name="query">simple query string to execute</param>
        /// <param name="requestKind">request type</param>
        /// <param name="responseKind">response type</param>
        /// <param name="pageSize">page size</param>
        /// <param name="pageNo">page number (start with 1)</param>
        /// <returns>index of added <see cref="XdsRequestItem"/></returns>
        public virtual int AddRequestItem(string query, XmlDataRequestKind requestKind, XmlDataResponseKind responseKind,
                                          int pageSize, int pageNo) {
            if(IsDebugEnabled)
                log.Debug(@"Add new Request Item. query=[{0}], requestKind=[{1}], responseKind=[{2}], pageSize=[{3}], pageNo=[{4}]",
                          query, requestKind, responseKind, pageSize, pageNo);

            var request = new XdsRequestItem
                          {
                              Query = query,
                              RequestKind = requestKind,
                              ResponseKind = responseKind,
                              PageSize = pageSize,
                              PageNo = pageNo
                          };

            lock(this) {
                request.Id = Count;
                Add(request);
            }

            return request.Id;
        }

        /// <summary>
        /// 서버에서 수행할 쿼리문을 요청문서에 등록한다.
        /// </summary>
        /// <param name="query">simple query string to execute</param>
        /// <param name="requestKind">request type</param>
        /// <param name="responseKind">response type</param>
        /// <returns>index of added <see cref="XdsRequestItem"/></returns>
        public int AddRequestItem(string query, XmlDataRequestKind requestKind, XmlDataResponseKind responseKind) {
            return AddRequestItem(query, requestKind, responseKind, MsgConsts.NO_PAGE_INDEX, MsgConsts.NO_PAGE_INDEX);
        }

        /// <summary>
        /// 서버에서 수행할 쿼리문을 요청문서에 등록한다.
        /// </summary>
        /// <param name="query">simple query string to execute</param>
        /// <param name="pageSize">page size</param>
        /// <param name="pageNo">page number (start with 1)</param>
        /// <returns>index of added <see cref="XdsRequestItem"/></returns>
        public int AddRequestItem(string query, int pageSize, int pageNo) {
            return AddRequestItem(query, XmlDataRequestKind.Query, XmlDataResponseKind.DataSet, pageSize, pageNo);
        }

        /// <summary>
        /// 서버에서 수행할 쿼리문을 요청문서에 등록한다.
        /// </summary>
        /// <param name="query">simple query string to execute</param>
        /// <returns>index of added <see cref="XdsRequestItem"/></returns>
        public int AddRequestItem(string query) {
            return AddRequestItem(query, MsgConsts.NO_PAGE_INDEX, MsgConsts.NO_PAGE_INDEX);
        }

        /// <summary>
        /// 서버에서 수행할 쿼리문을 요청문서에 등록한다.
        /// </summary>
        /// <param name="query">simple query string to execute</param>
        /// <param name="responseKind">response type</param>
        /// <param name="pageSize">page size</param>
        /// <param name="pageNo">page number (start with 1)</param>
        /// <returns>index of added <see cref="XdsRequestItem"/></returns>
        public int AddQuery(string query, XmlDataResponseKind responseKind, int pageSize, int pageNo) {
            return AddRequestItem(query, XmlDataRequestKind.Query, responseKind, pageSize, pageNo);
        }

        /// <summary>
        /// 서버에서 수행할 쿼리문을 요청문서에 등록한다.
        /// </summary>
        /// <param name="query">simple query string to execute</param>
        /// <param name="responseKind">response type</param>
        /// <returns>index of added <see cref="XdsRequestItem"/></returns>
        public virtual int AddQuery(string query, XmlDataResponseKind responseKind) {
            return AddQuery(query, responseKind, MsgConsts.NO_PAGE_INDEX, MsgConsts.NO_PAGE_INDEX);
        }

        /// <summary>
        /// 서버에서 수행할 쿼리문을 요청문서에 등록한다.
        /// </summary>
        /// <param name="query">simple query string to execute</param>
        /// <param name="pageSize">page size</param>
        /// <param name="pageNo">page number (start with 1)</param>
        /// <returns>index of added <see cref="XdsRequestItem"/></returns>
        public virtual int AddQuery(string query, int pageSize, int pageNo) {
            return AddQuery(query, XmlDataResponseKind.DataSet, pageSize, pageNo);
        }

        /// <summary>
        /// 서버에서 수행할 쿼리문을 요청문서에 등록한다.
        /// </summary>
        /// <param name="query">simple query string to execute</param>
        /// <returns>index of added <see cref="XdsRequestItem"/></returns>
        public virtual int AddQuery(string query) {
            return AddQuery(query, XmlDataResponseKind.None);
        }

        /// <summary>
        /// 서버에서 수행할 Procudure Name 을 요청문서에 등록한다.
        /// </summary>
        /// <param name="spName">Procedure name</param>
        /// <param name="responseKind">response type</param>
        /// <param name="pageSize">page size</param>
        /// <param name="pageNo">page number (start with 1)</param>
        /// <returns>index of added <see cref="XdsRequestItem"/></returns>
        public int AddStoredProc(string spName, XmlDataResponseKind responseKind, int pageSize, int pageNo) {
            return AddRequestItem(spName, XmlDataRequestKind.StoredProc, responseKind, pageSize, pageNo);
        }

        /// <summary>
        /// 서버에서 수행할 Procudure Name 을 요청문서에 등록한다.
        /// </summary>
        /// <param name="spName">Procedure name</param>
        /// <param name="responseKind">response type</param>
        /// <returns>index of added <see cref="XdsRequestItem"/></returns>
        public virtual int AddStoredProc(string spName, XmlDataResponseKind responseKind) {
            return AddStoredProc(spName, responseKind, MsgConsts.NO_PAGE_INDEX, MsgConsts.NO_PAGE_INDEX);
        }

        /// <summary>
        /// 서버에서 수행할 Procudure Name 을 요청문서에 등록한다.
        /// </summary>
        /// <param name="spName">Procedure name</param>
        /// <param name="pageSize">page size</param>
        /// <param name="pageNo">page number (start with 1)</param>
        /// <returns>index of added <see cref="XdsRequestItem"/></returns>
        public virtual int AddStoredProc(string spName, int pageSize, int pageNo) {
            return AddStoredProc(spName, XmlDataResponseKind.DataSet, pageSize, pageNo);
        }

        /// <summary>
        /// 서버에서 수행할 Procudure Name 을 요청문서에 등록한다.
        /// </summary>
        /// <param name="spName">Procedure name</param>
        /// <returns>index of added <see cref="XdsRequestItem"/></returns>
        public virtual int AddStoredProc(string spName) {
            return AddStoredProc(spName, XmlDataResponseKind.None);
        }

        /// <summary>
        /// 서버에서 수행할 Method name 을 요청문서에 등록한다.
        /// </summary>
        /// <param name="method">method name to execute</param>
        /// <param name="responseKind">response type</param>
        /// <param name="pageSize">page size</param>
        /// <param name="pageNo">page number (start with 1)</param>
        /// <returns>index of added <see cref="XdsRequestItem"/></returns>
        public virtual int AddMethod(string method, XmlDataResponseKind responseKind, int pageSize, int pageNo) {
            return AddRequestItem(method, XmlDataRequestKind.Method, responseKind, pageSize, pageNo);
        }

        /// <summary>
        /// 서버에서 수행할 Method name 을 요청문서에 등록한다.
        /// </summary>
        /// <param name="method">method name to execute</param>
        /// <param name="responseKind">response type</param>
        /// <returns>index of added <see cref="XdsRequestItem"/></returns>
        public virtual int AddMethod(string method, XmlDataResponseKind responseKind) {
            return AddMethod(method, responseKind, MsgConsts.NO_PAGE_INDEX, MsgConsts.NO_PAGE_INDEX);
        }

        /// <summary>
        /// 서버에서 수행할 Method name 을 요청문서에 등록한다.
        /// </summary>
        /// <param name="method">method name to execute</param>
        /// <param name="pageSize">page size</param>
        /// <param name="pageNo">page number (start with 1)</param>
        /// <returns>index of added <see cref="XdsRequestItem"/></returns>
        public virtual int AddMethod(string method, int pageSize, int pageNo) {
            return AddMethod(method, XmlDataResponseKind.DataSet, pageSize, pageNo);
        }

        /// <summary>
        /// 서버에서 수행할 Method name 을 요청문서에 등록한다.
        /// </summary>
        /// <param name="method">method name to execute</param>
        /// <returns>index of added <see cref="XdsRequestItem"/></returns>
        public virtual int AddMethod(string method) {
            return AddMethod(method, XmlDataResponseKind.None);
        }
    }
}