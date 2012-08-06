namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// Clock에 대한 Proxy입니다. 기본적으로 <see cref="SystemClock"/>을 제공합니다.
    /// </summary>
    public static class ClockProxy {
        private static readonly object _syncLock = new object();
        private static IClock _clock;

        /// <summary>
        /// Clock
        /// </summary>
        public static IClock Clock {
            get {
                if(_clock == null)
                    lock(_syncLock)
                        if(_clock == null) {
                            var clock = new SystemClock();
                            System.Threading.Thread.MemoryBarrier();
                            _clock = clock;
                        }

                return _clock;
            }
            set {
                lock(_syncLock)
                    _clock = value;
            }
        }
    }
}