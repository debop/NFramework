using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using NSoft.NFramework.Collections;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.LinqEx {
    /// <summary>
    /// LINQ 사용시 시퀀스 조작 속도를 높히기 위해 사용하는 컬렉션
    /// </summary>
    /// <remarks>
    /// http://www.codeplex.com/i4o 를 참조할 것.
    /// 속도를 더 높히기 위해 Refactoring을 수행했습니다.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class IndexableCollection<T> : Collection<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly IDynamicAccessor<T> Accessor;
        private static readonly PropertyInfo[] PropertyInfos; // Public Property만 가져온다.
        private static readonly IList<string> PropertyNames;

        static IndexableCollection() {
            Accessor = DynamicAccessorFactory.CreateDynamicAccessor<T>(MapPropertyOptions.Safety);
            PropertyInfos = Accessor.PropertyMap.Values.ToArray();
            PropertyNames = Accessor.GetPropertyNames();
        }

        /// <summary>
        /// Index Table
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        public class IndexTable<TEntity> : Dictionary<string, MultiMap<int, TEntity>> {}

        private readonly IndexTable<T> _indexes = new IndexTable<T>();

        /// <summary>
        /// 생성자
        /// </summary>
        public IndexableCollection() {
            BuildIndexes();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="list">초기 요소</param>
        public IndexableCollection(IList<T> list)
            : base(list) {
            BuildIndexes();
        }

        /// <summary>
        /// 컬렉션의 요소의 Indexing 해야 할 속성에 대해, 인덱스를 생성합니다.
        /// </summary>
        private void BuildIndexes() {
            if(IsDebugEnabled)
                log.Debug("컬렉션의 요소들에 대해 IndexableAttribute를 조사하고, Indexable이라면 요소의 속성값을 인덱싱합니다.");

            foreach(var pi in PropertyInfos) {
                var attributes = pi.GetCustomAttributes(true);
                var indexables = attributes.Where(attr => attr is IndexableAttribute);

                if(indexables.ItemExistsAtLeast(1))
                    AddAllToIndex(pi.Name);
            }
        }

        /// <summary>
        /// 지정된 속성을 인덱스로 추가합니다.
        /// </summary>
        /// <param name="propertyName"></param>
        private void AddAllToIndex(string propertyName) {
            if(IsDebugEnabled)
                log.Debug("컬렉션의 모든 요소의 속성[{0}] 의 값을 인덱싱합니다.", propertyName);

            if(PropertyHasIndex(propertyName) == false)
                _indexes.Add(propertyName, new MultiMap<int, T>());

            foreach(T item in this)
                AddToIndex(item, propertyName, _indexes[propertyName]);
        }

        /// <summary>
        /// 요소의 속성 값을 인덱싱합니다.
        /// </summary>
        private static void AddToIndex(T item, string propertyName, MultiMap<int, T> index) {
            if(Equals(item, null))
                return;

            object propertyValue = Accessor.GetPropertyValue(item, propertyName);

            if(IsDebugEnabled)
                log.Debug("해당 요소[{0}] 의 속성 [{1}] 의 값은 [{2}] 입니다.", item, propertyName, propertyValue);

            if(propertyValue != null) {
                var hash = propertyValue.GetHashCode();
                if(index.ContainsKey(hash))
                    index[hash].Add(item);
                else {
                    //List<T> newList = new List<T> {item};
                    //index.Add(hash, newList);
                    index.Add(hash, item);
                }
            }
        }

        /// <summary>
        /// 동적으로 지정된 속성에 대해 인덱스를 생성한다.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool AddIndex(string propertyName) {
            if(PropertyNames.Contains(propertyName) == false)
                return false;

            AddAllToIndex(propertyName);
            return true;
        }

        /// <summary>
        /// 지정된 속성명의 인덱스를 제거합니다.
        /// </summary>
        /// <param name="propertyName">인덱스로 등록된 속성명</param>
        /// <returns>제거 여부. 속성명이 인덱스에 포함되어 있지 않다면 false를 반환한다.</returns>
        public bool RemoveIndex(string propertyName) {
            if(_indexes.ContainsKey(propertyName))
                return _indexes.Remove(propertyName);
            return false;
        }

        /// <summary>
        /// 속성명이 인덱스로 등록되어 있는지 검사
        /// </summary>
        /// <param name="propertyName">검사할 인덱스 명</param>
        /// <returns>인덱스로 등록되었는지 여부</returns>
        public bool PropertyHasIndex(string propertyName) {
            return _indexes.ContainsKey(propertyName);
        }

        /// <summary>
        /// 속성명에 따른 인덱스 정보 반환
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public MultiMap<int, T> GetIndexByProperty(string propertyName) {
            if(PropertyHasIndex(propertyName))
                return _indexes[propertyName];

            return new MultiMap<int, T>();
        }

        /// <summary>
        /// 요소 추가
        /// </summary>
        /// <param name="itemToAdd">추가할 요소</param>
        public new void Add(T itemToAdd) {
            foreach(string propertyName in _indexes.Keys) {
                var propertyValue = Accessor.GetPropertyValue(itemToAdd, propertyName);
                if(propertyValue != null)
                    AddToIndex(itemToAdd, propertyName, _indexes[propertyName]);
            }
            base.Add(itemToAdd);
        }

        /// <summary>
        /// 요소 제거
        /// </summary>
        /// <param name="itemToRemove">제거할 요소</param>
        /// <returns>제거 여부, 제거할 요소가 없으면 False</returns>
        public new bool Remove(T itemToRemove) {
            foreach(string propertyName in _indexes.Keys) {
                var propertyValue = Accessor.GetPropertyValue(itemToRemove, propertyName);
                if(propertyValue != null) {
                    var hashCode = propertyValue.GetHashCode();
                    var index = GetIndexByProperty(propertyName);

                    if(index.ContainsKey(hashCode))
                        index[hashCode].Remove(itemToRemove);
                }
            }
            return base.Remove(itemToRemove);
        }
    }
}