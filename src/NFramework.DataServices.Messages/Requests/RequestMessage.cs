using System;
using System.Collections.Generic;

namespace NSoft.NFramework.DataServices.Messages {
    /// <summary>
    /// 요청 메시지
    /// </summary>
    [Serializable]
    public class RequestMessage : MessageBase {
        public RequestMessage()
            : base(MessageDirection.Request) {
            Transactional = false;
            AsParallel = false;
        }

        /// <summary>
        /// Transaction 하에서 요청들을 처리할 것인가?
        /// </summary>
        public bool Transactional { get; set; }

        /// <summary>
        /// 병렬 방식으로 요청들을 처리할 것인가?
        /// </summary>
        public bool AsParallel { get; set; }

        private IList<RequestItem> _items;

        /// <summary>
        /// 요청 항목들
        /// </summary>
        public IList<RequestItem> Items {
            get { return _items ?? (_items = new List<RequestItem>()); }
            set { _items = value; }
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

        public RequestItem AddItem(string method) {
            return AddItem(method, ResponseFormatKind.None, null, null);
        }

        public RequestItem AddItem(string method, ResponseFormatKind responseFormat) {
            return AddItem(method, responseFormat, null, null);
        }

        public RequestItem AddItem(string method, ResponseFormatKind responseForm, int? firstResult, int? maxResults) {
            var requestItem = new RequestItem
                              {
                                  Method = method,
                                  ResponseFormat = responseForm,
                                  FirstResult = firstResult,
                                  MaxResults = maxResults,
                              };

            Items.Add(requestItem);
            return requestItem;
        }

        public override int GetHashCode() {
            return Hasher.Compute(MessageId);
        }
    }
}