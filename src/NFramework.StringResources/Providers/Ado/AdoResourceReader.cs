using System.Collections;

namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// Resource Reader by ADO.NET
    /// </summary>
    public sealed class AdoResourceReader : ResourceReaderBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public AdoResourceReader(Hashtable resources) : base(resources) {}
    }
}