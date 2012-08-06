using System;
using System.Threading;

namespace NSoft.NFramework.Threading {
    /// <summary>
    /// <see cref="Thread"/> 진행정보를 담은 EventArgs
    /// </summary>
    [Serializable]
    public class WorkerProgressChangedEventArgs : WorkerThreadEventArgs {
        private readonly int _progressPercentage;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="processPercentage"></param>
        public WorkerProgressChangedEventArgs(Thread thread, int processPercentage = 0) : base(thread) {
            _progressPercentage = processPercentage;
        }

        /// <summary>
        /// 진행률 (0 ~ 100) 사이의 값을 가진다.
        /// </summary>
        public int ProgressPercentage {
            get { return _progressPercentage; }
        }
    }
}