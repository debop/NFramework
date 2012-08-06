using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// <see cref="AdoResultSet"/>의 Row를 표현합니다. (필드명-필드값) 매핑 정보를 가집니다.
    /// </summary>
    [Serializable]
    public class AdoResultRow : Dictionary<string, object> {
        public AdoResultRow() {}
        public AdoResultRow(IDictionary<string, object> dictionary) : base(dictionary) {}
        public AdoResultRow(int capacity) : base(capacity) {}
        public AdoResultRow(SerializationInfo info, StreamingContext context) : base(info, context) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="fieldNames"></param>
        public AdoResultRow(IDataReader reader, IList<string> fieldNames) {
            fieldNames = fieldNames ?? reader.GetFieldNames();

            foreach(var fieldName in fieldNames)
                Add(fieldName, reader.AsValue(fieldName));
        }

        /// <summary>
        /// <see cref="AdoResultSet"/>의 필드명 (컬럼명)
        /// </summary>
        public ICollection<string> FieldNames {
            get { return Keys; }
        }
    }
}