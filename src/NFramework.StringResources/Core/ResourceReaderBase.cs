using System;
using System.Collections;
using System.Resources;

namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// Base Resource Reader class
    /// </summary>
    public abstract class ResourceReaderBase : IResourceReader {
        private Hashtable _resources;

        /// <summary>
        /// constructor
        /// </summary>
        protected ResourceReaderBase(Hashtable resources) {
            resources.ShouldNotBeNull("resources");
            _resources = resources;
        }

        ///<summary>
        /// Closes the resource reader after releasing any resources associated with it.
        ///</summary>
        public void Close() {
            Dispose();
        }

        ///<summary>
        /// Returns an <see cref="T:System.Collections.IDictionaryEnumerator" /> of the resources for this reader.
        ///</summary>
        ///<returns>
        /// A dictionary enumerator for the resources for this reader.
        ///</returns>
        public IDictionaryEnumerator GetEnumerator() {
            return _resources.GetEnumerator();
        }

        ///<summary>
        /// Returns an <see cref="T:System.Collections.IDictionaryEnumerator" /> of the resources for this reader.
        ///</summary>
        ///<returns>
        /// A dictionary enumerator for the resources for this reader.
        ///</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #region << IDisposable >>

        /// <summary>
        /// Indicate current instance is disposed.
        /// </summary>
        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~ResourceReaderBase() {
            Dispose(false);
        }

        ///<summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Release unmanaged resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if(IsDisposed)
                return;

            if(disposing) {
                if(_resources != null)
                    _resources.Clear();
                _resources = null;
            }
            IsDisposed = true;
        }

        #endregion
    }
}