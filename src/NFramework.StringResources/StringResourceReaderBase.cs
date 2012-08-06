using System;
using System.Collections;
using System.Resources;

namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// <see cref="IResourceReader"/>의 Abstract class
    /// </summary>
    public abstract class StringResourceReaderBase : System.Resources.IResourceReader {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private Hashtable _resources;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="resources"></param>
        protected StringResourceReaderBase(Hashtable resources) {
            resources.ShouldNotBeNull("resources");
            _resources = resources;
        }

        /// <summary>
        /// 리소스 해제
        /// </summary>
        public void Close() {
            Dispose();
        }

        /// <summary>
        /// 리소스 열거자를 반환합니다.
        /// </summary>
        /// <returns></returns>
        public IDictionaryEnumerator GetEnumerator() {
            return _resources.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #region << IDisposable >>

        /// <summary>
        /// 리소스 해제 여부
        /// </summary>
        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// 소멸자
        /// </summary>
        ~StringResourceReaderBase() {
            Dispose(false);
        }

        /// <summary>
        /// 리소스 해제
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 리소스 해제
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if(IsDisposed)
                return;

            if(disposing) {
                if(_resources != null) {
                    _resources.Clear();
                    _resources = null;
                }

                if(IsDebugEnabled)
                    log.Debug("StringResourceReaderBase is disposed.");
            }
            IsDisposed = true;
        }

        #endregion
    }
}