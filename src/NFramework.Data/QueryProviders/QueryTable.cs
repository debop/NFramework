using System;
using System.Collections.Generic;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// QueryName:Query String 을 갖는 Dictionary입니다.
    /// </summary>
    /// <remarks>
    /// QueryKey = QueryString 을 갖는 Dictionary입니다. QueryKey는 [Section,]QueryName 형식입니다.
    /// </remarks>
    [Serializable]
    public sealed class QueryTable : Dictionary<string, string> {
        /// <summary>
        /// Return string using Set format.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return ToString(false);
        }

        /// <summary>
        /// Return query string table information by dictionary style.
        /// </summary>
        /// <param name="showDetails">Indicate detail view.</param>
        /// <returns>query string table.</returns>
        public string ToString(bool showDetails) {
            return this.DictionaryToString<string, string>(showDetails);
        }
    }
}