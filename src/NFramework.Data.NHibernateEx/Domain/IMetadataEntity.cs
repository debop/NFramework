using System.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// Metadata를 가지는 Entity를 나타냅니다.
    /// </summary>
    public interface IMetadataEntity : IStateEntity {
        /// <summary>
        /// Entity마다 많은 Indexer들이 있으므로 이를 방지하기 위해 Named Indexer를 이용한다.<br/>
        /// 예 : Employee.Metadata["NickName"] 
        /// </summary>
        NamedIndexer<IMetadataValue, string> Metadatas { get; }

        /// <summary>
        /// Metadata Key collection
        /// </summary>
        IList<string> MetadataKeys { get; }

        /// <summary>
        /// Add metadata
        /// </summary>
        /// <param name="key"></param>
        /// <param name="metadataValue"></param>
        void AddMetadata(string key, IMetadataValue metadataValue);

        /// <summary>
        /// Add metadata
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void AddMetadata(string key, object value);

        /// <summary>
        /// Remove metadata with key.
        /// </summary>
        /// <param name="key"></param>
        void RemoveMetadata(string key);
    }
}