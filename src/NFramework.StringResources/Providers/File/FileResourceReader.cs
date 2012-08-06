using System.Collections;
using System.Resources;

namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// FileResourceProvider가 제공하는 <see cref="IResourceReader"/>
    /// </summary>
    public sealed class FileResourceReader : ResourceReaderBase {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resources"></param>
        public FileResourceReader(Hashtable resources) : base(resources) {}
    }
}