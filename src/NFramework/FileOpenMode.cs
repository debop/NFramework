using System;

namespace NSoft.NFramework {
    /// <summary>
    /// File Open Mode
    /// </summary>
    [Flags]
    public enum FileOpenMode {
        /// <summary>
        /// open file for read
        /// </summary>
        Read = 0x01,

        /// <summary>
        /// open file for write
        /// </summary>
        Write = 0x02,

        /// <summary>
        /// Open file for read or write
        /// </summary>
        ReadWrite = Read | Write
    }
}