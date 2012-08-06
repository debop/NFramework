using System;
using System.Collections.Generic;
using System.Threading;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// IMetadataValue 정보 (<see cref="IMetadataValue"/>) 를 가지는 Entity의 기본 Class
    /// </summary>
    /// <typeparam name="TId">Id의 수형</typeparam>
    [Serializable]
    public abstract class MetadataEntityBase<TId> : DataEntityBase<TId>, IMetadataEntity {
        private readonly object _syncLock = new object();

        private IDictionary<string, IMetadataValue> _metadataMap;

        /// <summary>
        /// Metadata Map
        /// </summary>
        public virtual IDictionary<string, IMetadataValue> MetadataMap {
            get {
                if(_metadataMap == null)
                    lock(_syncLock)
                        if(_metadataMap == null) {
                            var map = new Dictionary<string, IMetadataValue>();
                            Thread.MemoryBarrier();
                            _metadataMap = map;
                        }
                return _metadataMap;
            }
            set { _metadataMap = value; }
        }

        /// <summary>
        /// 해당 키의 메타데이타 정보를 조회합니다. 없으면, 기본 메타데이타 값을 반환합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual IMetadataValue GetMetadataOrDefault(string key) {
            key.ShouldNotBeNull("key");

            lock(_syncLock) {
                IMetadataValue result;
                return MetadataMap.TryGetValue(key, out result) ? result : MetadataValue.Empty;
            }
        }

        private NamedIndexer<IMetadataValue, string> _metadataIndexer;

        /// <summary>
        /// Entity마다 많은 Indexer들이 있으므로 이를 방지하기 위해 Named Indexer를 이용한다.<br/>
        /// 예 : Employee.IMetadataValue["NickName"] 
        /// </summary>
        public virtual NamedIndexer<IMetadataValue, string> Metadatas {
            get {
                if(_metadataIndexer == null)
                    lock(_syncLock)
                        if(_metadataIndexer == null) {
                            var indexer = new NamedIndexer<IMetadataValue, string>(GetMetadataOrDefault, AddMetadata);
                            Thread.MemoryBarrier();
                            _metadataIndexer = indexer;
                        }

                return _metadataIndexer;
            }
        }

        /// <summary>
        /// IMetadataValue Key collection
        /// </summary>
        public virtual IList<string> MetadataKeys {
            get { return new List<string>(MetadataMap.Keys); }
        }

        /// <summary>
        /// Add IMetadataValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="metadataValue"></param>
        public virtual void AddMetadata(string key, IMetadataValue metadataValue) {
            lock(_syncLock)
                MetadataMap.AddValue(key, metadataValue);
            //MetadataMap[key] = metadataValue;
        }

        /// <summary>
        /// Add IMetadataValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void AddMetadata(string key, object value) {
            lock(_syncLock)
                MetadataMap.AddValue(key, new MetadataValue(value.AsText()));
            //MetadataMap[key] = new MetadataValue(value.AsText());
        }

        /// <summary>
        /// Remove IMetadataValue with key.
        /// </summary>
        /// <param name="key"></param>
        public virtual void RemoveMetadata(string key) {
            lock(_syncLock)
                MetadataMap.Remove(key);
        }
    }
}