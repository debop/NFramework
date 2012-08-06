using System;
using System.Threading;

namespace NSoft.NFramework.Parallelism.Tools {
    /// <summary>
    /// <see cref="CancellationTokenSource"/>에 대한 Extension Method를 제공합니다.
    /// </summary>
    public static class CancellationTokenTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <paramref name="source"/>를 취소하고, <see cref="OperationCanceledException"/>을 발생시키도록 합니다.
        /// </summary>
        /// <param name="source">The source to be canceled.</param>
        public static void CancelAndThrow(this CancellationTokenSource source) {
            if(IsDebugEnabled)
                log.Debug("취소 작업을 수행하고, 예외를 발생시킵니다.");

            source.Cancel();
            source.Token.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// <paramref name="token"/>이 취소 요청을 받았을 때, 같이 취소되게끔 연결된(Linked) <see cref="CancellationTokenSource"/>를 생성합니다.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The created CancellationTokenSource.</returns>
        public static CancellationTokenSource CreateLinkedSource(this CancellationToken token) {
            if(IsDebugEnabled)
                log.Debug("연결된 CancellationTokenSource를 생성합니다.");

            return CancellationTokenSource.CreateLinkedTokenSource(token, new CancellationToken());
        }
    }
}