using System;
using System.Collections.Generic;

namespace NSoft.NFramework.DataServices.Messages {
    /// <summary>
    /// 요청 항목
    /// </summary>
    [Serializable]
    public class RequestItem : MessageObjectBase {
        public RequestItem() : this(string.Empty, ResponseFormatKind.None) {}
        public RequestItem(string method) : this(method, ResponseFormatKind.None) {}

        public RequestItem(string method, ResponseFormatKind responseFormat) {
            Id = Guid.NewGuid();
            Method = method;
            RequestMethod = RequestMethodKind.Method;
            ResponseFormat = responseFormat;
        }

        /// <summary>
        /// Request Item Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 요청 정보 본문
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 요청 방법 (SqlString, Procedure, Method - 기본은  Method 임)
        /// </summary>
        public RequestMethodKind RequestMethod { get; set; }

        /// <summary>
        /// 응답 형식 (None, Scalar, DataSet, Xml 등)
        /// </summary>
        public ResponseFormatKind ResponseFormat { get; set; }

        public int? FirstResult { get; set; }

        public int? MaxResults { get; set; }

        private IList<RequestParameter> _parameters;

        public IList<RequestParameter> Parameters {
            get { return _parameters ?? (_parameters = new List<RequestParameter>()); }
            set { _parameters = value; }
        }

        private IList<string> _prepareStatements;

        /// <summary>
        /// 요청 작업 사전에 수행할 명령문들
        /// </summary>
        public IList<string> PrepareStatements {
            get { return _prepareStatements ?? (_prepareStatements = new List<string>()); }
            set { _prepareStatements = value; }
        }

        private IList<string> _postscriptStatements;

        /// <summary>
        /// 요청 작업 사후의 정리를 위한 명령문들
        /// </summary>
        public IList<string> PostscriptStatements {
            get { return _postscriptStatements ?? (_postscriptStatements = new List<string>()); }
            set { _postscriptStatements = value; }
        }

#if !SILVERLIGHT
        /// <summary>
        /// <see cref="Method"/> 에 대한 실제 SQL 문장. ( IAdoRepository.QueryProvider 를 통해 Method에 해당하는 SQL Statements 를 구합니다.)
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public string Query { get; set; }

        private IList<string> _preQueries;

        [Newtonsoft.Json.JsonIgnore]
        public IList<string> PreQueries {
            get { return _preQueries ?? (_preQueries = new List<string>()); }
        }

        private IList<string> _postQueries;

        [Newtonsoft.Json.JsonIgnore]
        public IList<string> PostQueries {
            get { return _postQueries ?? (_postQueries = new List<string>()); }
        }
#endif

        public override int GetHashCode() {
            return Hasher.Compute(Id, Method);
        }

        public override string ToString() {
            return
                string.Format(
                    "RequestItem# Id=[{0}], Body=[{1}], RequestMetohd=[{2}], ResponseFormat=[{3}], FirstResult=[{4}], MaxResults=[{5}]",
                    Id, Method, RequestMethod, ResponseFormat, FirstResult, MaxResults);
        }
    }
}