using System;
using System.Collections.Generic;

namespace NSoft.NFramework.DataServices.Messages {
    /// <summary>
    /// 응답 결과 항목
    /// </summary>
    [Serializable]
    public class ResponseItem : MessageObjectBase {
        public ResponseItem() {
            Id = Guid.NewGuid();
            ResponseFormat = ResponseFormatKind.None;
        }

        public ResponseItem(RequestItem requestItem) : this() {
            RequestItem = requestItem;
        }

        /// <summary>
        /// 응답 항목 Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 결과 Format
        /// </summary>
        public ResponseFormatKind ResponseFormat { get; set; }

        /// <summary>
        /// 요청 정보
        /// </summary>
        public RequestItem RequestItem { get; set; }

        /// <summary>
        /// 값이 하나인 경우, ResultValue로 값을 제공합니다.
        /// </summary>
        public object ResultValue { get; set; }

        private ResultSet _resultSet;

        /// <summary>
        /// 결과 정보
        /// </summary>
        public ResultSet ResultSet {
            get { return _resultSet ?? (_resultSet = new ResultSet()); }
            set { _resultSet = value; }
        }

        public ResultRow this[int index] {
            get { return ResultSet[index]; }
        }

        public object this[int rowIndex, string fieldName] {
            get { return ResultSet[rowIndex, fieldName]; }
        }

        /// <summary>
        /// 요청 처리 시간 (속도를 알면 나중에 좋을 듯!!!)
        /// </summary>
        public TimeSpan? ExecutionTime { get; set; }

        private IList<ErrorMessage> _errors;

        public IList<ErrorMessage> Errors {
            get { return _errors ?? (_errors = new List<ErrorMessage>()); }
            set { _errors = value; }
        }

        public bool HasError {
            get { return (_errors != null && _errors.Count > 0); }
        }

        public override int GetHashCode() {
            return Hasher.Compute(Id, ResponseFormat);
        }
    }
}