using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// Data-biding 을 위한 Thread-safe한 Observable dictionary입니다.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [DebuggerDisplay("Count={Count}")]
    [Serializable]
    public class ObservableConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged,
                                                                INotifyPropertyChanged {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly SynchronizationContext _context;
        private readonly ConcurrentDictionary<TKey, TValue> _dictionary;

        public ObservableConcurrentDictionary() {
            _context = AsyncOperationManager.SynchronizationContext;
            _dictionary = new ConcurrentDictionary<TKey, TValue>();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyObserversOfChange() {
            var collectionHandler = CollectionChanged;
            var propertyHandler = PropertyChanged;

            if(collectionHandler != null || propertyHandler != null) {
                if(IsDebugEnabled)
                    log.Debug("Observer에게, 현재 인스턴스의 변화를 통지합니다.");

                _context.Post(state => {
                                  if(collectionHandler != null)
                                      collectionHandler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                                  if(propertyHandler != null) {
                                      propertyHandler(this, new PropertyChangedEventArgs("Count"));
                                      propertyHandler(this, new PropertyChangedEventArgs("Keys"));
                                      propertyHandler(this, new PropertyChangedEventArgs("Values"));
                                  }
                              },
                              null);
            }
        }

        private bool TryAddWithNotification(TKey key, TValue value) {
            bool result = _dictionary.TryAdd(key, value);

            if(result)
                NotifyObserversOfChange();

            return result;
        }

        private bool TryAddWithNotification(KeyValuePair<TKey, TValue> item) {
            return TryAddWithNotification(item.Key, item.Value);
        }

        private bool TryRemoveWithNotification(TKey key, out TValue value) {
            var result = _dictionary.TryRemove(key, out value);
            if(result)
                NotifyObserversOfChange();
            return result;
        }

        private void UpdateWithNotification(TKey key, TValue value) {
            _dictionary[key] = value;
            NotifyObserversOfChange();
        }

        #region Implementation of IEnumerable

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
            return _dictionary.GetEnumerator();
            // return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _dictionary.GetEnumerator();
            //return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<KeyValuePair<TKey,TValue>>

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) {
            TryAddWithNotification(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear() {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Clear();
            NotifyObserversOfChange();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) {
            return Remove(item.Key);
        }

        int ICollection<KeyValuePair<TKey, TValue>>.Count {
            get { return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Count; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly {
            get { return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).IsReadOnly; }
        }

        #endregion

        #region Implementation of IDictionary<TKey,TValue>

        public bool ContainsKey(TKey key) {
            return _dictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value) {
            TryAddWithNotification(key, value);
        }

        public bool Remove(TKey key) {
            TValue removed;
            return TryRemoveWithNotification(key, out removed);
        }

        public bool TryGetValue(TKey key, out TValue value) {
            return _dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key] {
            get { return _dictionary[key]; }
            set { UpdateWithNotification(key, value); }
        }

        public ICollection<TKey> Keys {
            get { return _dictionary.Keys; }
        }

        public ICollection<TValue> Values {
            get { return _dictionary.Values; }
        }

        #endregion
                                                                }
}