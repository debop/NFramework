using System;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// Database처리를 위한 Query문장에서의 Parameter 정보를 나타낸다.
    /// </summary>
    public interface INamedParameter : IEquatable<INamedParameter> {
        /// <summary>
        /// Parameter name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Parameter value
        /// </summary>
        object Value { get; set; }
    }
}