using System;
using System.Collections.Generic;

namespace NSoft.NFramework.DataServices.Messages {
    /// <summary>
    /// ResultSet의 한 레코드를 표현합니다.
    /// </summary>
    [Serializable]
    public class ResultRow : Dictionary<string, object> {
        public ResultRow() {}
        public ResultRow(int capacity) : base(capacity) {}
        public ResultRow(IDictionary<string, object> dictionary) : base(dictionary) {}
#if !SILVERLIGHT
        protected ResultRow(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {}
#endif

        /// <summary>
        /// <see cref="ResultSet"/>의 필드명 (컬럼명)
        /// </summary>
        public ICollection<string> FieldNames {
            get { return Keys; }
        }
    }
}