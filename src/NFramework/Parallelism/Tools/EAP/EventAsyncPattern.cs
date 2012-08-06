using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.Tools {
    /// <summary>
    /// EAP (Event-based Asynchronous Pattern : 이벤트기반 비동기 패턴) 작업에서, 작업 완료에 따른 후속조치를 정의하는 함수를 제공합니다.
    /// </summary>
    /// <remarks>
    /// 참고사이트:
    /// <list>
    ///		<item>http://msdn.microsoft.com/ko-kr/library/wewwczdw.aspx</item>
    ///		<item>http://msdn.microsoft.com/ko-kr/library/dd997423.aspx</item>
    /// </list>
    /// </remarks>
    public static class EventAsyncPattern {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// EAP (Event-based Asynchronous Pattern : 이벤트기반 비동기 패턴) 작업에서, 작업 완료시의 후속조치를 정의합니다.
        /// </summary>
        /// <typeparam name="T">비동기 작업 결과물의 수형</typeparam>
        /// <param name="tcs">비동기 작업을 표현하는 delegate</param>
        /// <param name="e">비동기 작업완료 이벤트 인자</param>
        /// <param name="getResult">작업완료 시에 결과 반환 메소드</param>
        /// <param name="unregisterHandler">작업완료 이벤트 핸들러를 등록취소하는 Action</param>
        public static void HandleCompletion<T>(TaskCompletionSource<T> tcs,
                                               AsyncCompletedEventArgs e,
                                               Func<T> getResult,
                                               Action unregisterHandler) {
            tcs.ShouldNotBeNull("tcs");
            e.ShouldNotBeNull("e");

            if(e.UserState != tcs)
                return;

            getResult.ShouldNotBeNull("getResult");

            if(e.Cancelled)
                tcs.TrySetCanceled();

            else if(e.Error != null)
                tcs.TrySetException(e.Error);

            else
                tcs.TrySetResult(getResult());

            if(unregisterHandler != null)
                unregisterHandler();
        }
    }
}