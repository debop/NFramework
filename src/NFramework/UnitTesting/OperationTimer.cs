using System;
using System.Diagnostics;
using System.Text;

namespace NSoft.NFramework.UnitTesting {
    /// <summary>
    /// UnitTest시에 성능검사를 위한 Timer
    /// </summary>
    /// <example>
    /// <code>
    /// using(new OperationTimer("Performance Test"))
    /// {
    ///		// some test...
    /// }
    /// 
    /// // log 에 다음과 같이 출력된다. (INFO 레벨)
    /// ### Performace Test took xxxx msec
    /// </code>
    /// </example>
    [Serializable]
    public sealed class OperationTimer : IDisposable {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly string _message;
        private readonly Int32[] _startGenerations = new Int32[3];
        private readonly bool _clearGabage;

#if !SILVERLIGHT
        private readonly Stopwatch _stopwatch;
#else
		private readonly NSoft.NFramework.TimePeriods.TimeRange _period;
#endif

        /// <summary>
        /// 생성자
        /// </summary>
        public OperationTimer() : this("Performance Test") {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="message">이 객체가 소멸될 때 표시되는 메시지</param>
        /// <param name="clearGabage">테스트 전 후에 Gabage collection을 수행할 것인가?</param>
        public OperationTimer(string message, bool clearGabage = false) {
            if(clearGabage)
                GabageCollect();

            _message = message;
            _clearGabage = clearGabage;

#if !SILVERLIGHT
            if(_clearGabage)
                for(int i = 0; i < 3; i++)
                    _startGenerations[i] = GC.CollectionCount(i);

            _stopwatch = new Stopwatch();
            _stopwatch.Start();
#else
			_period = new NSoft.NFramework.TimePeriods.TimeRange(DateTime.Now, null);
#endif
        }

        private static void GabageCollect() {
            if(IsDebugEnabled)
                log.Debug("Gabage Collection을 수행합니다...");

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
#if !SILVERLIGHT
            GC.WaitForFullGCComplete();
#endif
        }

        #region << IDisposable >>

        private bool _isDisposed;

        ///<summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        public void Dispose() {
            if(_isDisposed)
                return;
#if !SILVERLIGHT
            _stopwatch.Stop();


            var totalGenerations = new Int32[3];
            if(_clearGabage) {
                for(var i = 0; i < 3; i++)
                    totalGenerations[i] = GC.CollectionCount(i) - _startGenerations[i];
            }

            var duration = _stopwatch.Elapsed;

            if(log.IsInfoEnabled) {
                log.Info("### {0} took {1} msecs", _message, duration.TotalMilliseconds);

                if(_clearGabage) {
                    var gens = new StringBuilder();

                    for(var i = 0; i < 3; i++) {
                        if(i > 0)
                            gens.Append(",");
                        gens.AppendFormat(" Gen {0}: {1}", i, totalGenerations[i]);
                    }

                    if(log.IsInfoEnabled)
                        log.Info(" <<< Gabage Generations: " + gens);
                }
            }
#else
			_period.End = DateTime.Now;

			if (log.IsInfoEnabled)
				log.Info("### {0} took {1} msecs", _message, _period.Duration.TotalMilliseconds);
#endif

            if(_clearGabage)
                GabageCollect();

            _isDisposed = true;
        }

        #endregion
    }
}