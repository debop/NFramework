using System;

namespace NSoft.NFramework.Web {
    /// <summary>
    /// Web Content에 대한 사용가능한 압축알고리즘의 종류
    /// </summary>
    [Serializable]
    public enum WebCompressionKind {
        /// <summary>
        /// None
        /// </summary>
        None,

        /// <summary>
        /// GZip compression
        /// </summary>
        GZip,

        /// <summary>
        /// Deflate compression
        /// </summary>
        Deflate
    }
}