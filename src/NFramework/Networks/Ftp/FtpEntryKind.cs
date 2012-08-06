using System;

namespace NSoft.NFramework.Networks {
    /// <summary>
    /// FTP 엔트리의 종류
    /// </summary>
    [Serializable]
    public enum FtpEntryKind {
        /// <summary>
        /// File
        /// </summary>
        File,

        /// <summary>
        /// Directory
        /// </summary>
        Directory
    }
}