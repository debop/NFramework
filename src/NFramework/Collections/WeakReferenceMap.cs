using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Collections {
    /// <summary>
    /// 객체의 Weak Reference 를 보관하여, Gabage Collection에 의해 수거되는 것을 허용하는 Hashtable입니다.
    /// </summary>
    [Serializable]
    public class WeakReferenceMap : IDictionary {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly Dictionary<WeakReferenceWrapper, WeakReferenceWrapper> _innerMap
            = new Dictionary<WeakReferenceWrapper, WeakReferenceWrapper>();

        /// <summary>
        /// 이미 Gabage Collector에 의해 수거된 요소들을 제거합니다.
        /// </summary>
        public void Scavenge() {
            var deadKeys =
                _innerMap
                    .Where(pair => (pair.Key.IsAlive == false) || (pair.Value.IsAlive == false))
                    .Select(pair => pair.Key)
                    .ToArray();

            deadKeys.RunEach(k => _innerMap.Remove(k));
        }

        public bool Contains(object key) {
            return _innerMap.ContainsKey(WeakReferenceWrapper.Wrap(key));
        }

        public void Add(object key, object value) {
            Scavenge();
            _innerMap.AddValue(WeakReferenceWrapper.Wrap(key), WeakReferenceWrapper.Wrap(value));
        }

        public void Clear() {
            _innerMap.Clear();
        }

        public IEnumerator<KeyValuePair<object, object>> GetEnumerator() {
            return
                _innerMap
                    .Where(item => item.Key.IsAlive)
                    .ToDictionary(item => WeakReferenceWrapper.Unwrap(item.Key), item => WeakReferenceWrapper.Unwrap(item.Value))
                    .ToList()
                    .GetEnumerator();
        }

        public void Remove(object key) {
            _innerMap.Remove(WeakReferenceWrapper.Wrap(key));
        }

        public object this[object key] {
            get {
                if(Contains(key) == false)
                    return null;

                return WeakReferenceWrapper.Unwrap(_innerMap[WeakReferenceWrapper.Wrap(key)]);
            }
            set {
                Scavenge();
                _innerMap.AddValue(WeakReferenceWrapper.Wrap(key), WeakReferenceWrapper.Wrap(value));
            }
        }

        public ICollection Keys {
            get { return _innerMap.Keys.Select(k => WeakReferenceWrapper.Unwrap(k)).ToList(); }
        }

        public ICollection Values {
            get { return _innerMap.Values.Select(v => WeakReferenceWrapper.Unwrap(v)).ToList(); }
        }

        public bool IsReadOnly {
            get { return ((IDictionary)_innerMap).IsReadOnly; }
        }

        public bool IsFixedSize {
            get { return ((IDictionary)_innerMap).IsFixedSize; }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return (IEnumerator)GetEnumerator();
        }

        public void CopyTo(Array array, int index) {
            var weakArray =
                _innerMap.ToDictionary(pair => WeakReferenceWrapper.Unwrap(pair.Key),
                                       pair => WeakReferenceWrapper.Unwrap(pair.Value))
                    .ToArray();

            Array.Copy(weakArray, 0, array, index, weakArray.Length);
        }

        /// <summary>
        /// 단순히 컬렉션의 요소를 반환합니다. 실제 GC에 의해 소멸된 것도 갯수에 포함될 수 있습니다.
        /// </summary>
        public int Count {
            get { return _innerMap.Count; }
        }

        public object SyncRoot {
            get { return ((ICollection)_innerMap).SyncRoot; }
        }

        public bool IsSynchronized {
            get { return ((ICollection)_innerMap).IsSynchronized; }
        }

        IDictionaryEnumerator IDictionary.GetEnumerator() {
            return (IDictionaryEnumerator)GetEnumerator();
        }
    }
}