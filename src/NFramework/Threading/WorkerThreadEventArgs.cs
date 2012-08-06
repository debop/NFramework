using System;
using System.Threading;

namespace NSoft.NFramework.Threading {
    /// <summary>
    /// <see cref="AbstractThread"/>의 Event Handler에서 사용하는 기본 EventArgs
    /// </summary>
    [Serializable]
    public class WorkerThreadEventArgs : EventArgs<Thread> {
        public WorkerThreadEventArgs(Thread item) : base(item) {}
        public WorkerThreadEventArgs(Thread item, object data) : base(item, data) {}
    }
}