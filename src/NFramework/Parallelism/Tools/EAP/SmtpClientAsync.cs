using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Parallelism.Tools {
    /// <summary>
    /// <see cref="SmtpClient"/>를 EAP (이벤트 기반 비동기 패턴)으로 수행하는 확장 메소드를 제공합니다.
    /// </summary>
    /// <remarks>
    /// 참고사이트:
    /// <list>
    ///		<item>http://msdn.microsoft.com/ko-kr/library/wewwczdw.aspx</item>
    ///		<item>http://msdn.microsoft.com/ko-kr/library/dd997423.aspx</item>
    /// </list>
    /// </remarks>
    public static class SmtpClientAsync {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 메일을 비동기적으로 전송하는 Task{bool}을 빌드합니다.
        /// </summary>
        /// <param name="client">smtp client</param>
        /// <param name="from">메일 발송자</param>
        /// <param name="recipients">메일 수신자들, 구분을 세미콜론(';') 또는 콤마(',')로 구분합니다.</param>
        /// <param name="subject">메일 제목</param>
        /// <param name="body">메일 본문</param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public static Task<bool> SendTask(this SmtpClient client, string from, string recipients, string subject, string body,
                                          object userToken = null) {
            return SendTask(client, new CancellationToken(), from, recipients, subject, body, userToken);
        }

        /// <summary>
        /// 메일을 비동기적으로 전송하는 Task{bool}을 빌드합니다.
        /// </summary>
        /// <param name="client">smtp client</param>
        /// <param name="token">작업 취소용 Token</param>
        /// <param name="from">메일 발송자</param>
        /// <param name="recipients">메일 수신자들, 구분을 세미콜론(';') 또는 콤마(',')로 구분합니다.</param>
        /// <param name="subject">메일 제목</param>
        /// <param name="body">메일 본문</param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public static Task<bool> SendTask(this SmtpClient client, CancellationToken token, string from, string recipients,
                                          string subject, string body, object userToken = null) {
            if(IsDebugEnabled)
                log.Debug("메일을 발송합니다... from=[{0}], recipients=[{1}], subject=[{2}], body=[{3}]",
                          from, recipients, subject, body.EllipsisChar(80));

            return SendTaskInternal(client, token, userToken, tcs => client.SendAsync(from, recipients, subject, body, tcs));
        }

        /// <summary>
        /// 메일을 비동기적으로 전송하는 Task{bool}을 빌드합니다.
        /// </summary>
        /// <param name="client">smtp client</param>
        /// <param name="message">mail message</param>
        /// <param name="userToken"></param>
        /// <returns>비동기 작업을 수행하는 Task{bool}</returns>
        public static Task<bool> SendTask(this SmtpClient client, MailMessage message, object userToken = null) {
            return SendTask(client, new CancellationToken(), message, userToken);
        }

        /// <summary>
        /// 메일을 비동기적으로 전송하는 Task{bool}을 빌드합니다.
        /// </summary>
        /// <param name="client">smtp client</param>
        /// <param name="token">작업 취소용 Token</param>
        /// <param name="message">mail message</param>
        /// <param name="userToken"></param>
        /// <returns>비동기 작업을 수행하는 Task{bool}</returns>
        public static Task<bool> SendTask(this SmtpClient client, CancellationToken token, MailMessage message, object userToken = null) {
            message.ShouldNotBeNull("message");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 메일을 발송합니다... from=[{0}], recipients=[{1}], subject=[{2}], body=[{3}]",
                          message.From, message.To.CollectionToString(), message.Subject, message.Body.EllipsisChar(80));

            return SendTaskInternal(client, token, userToken, tcs => client.SendAsync(message, tcs));
        }

        private static Task<bool> SendTaskInternal(SmtpClient client, CancellationToken token, object userToken,
                                                   Action<TaskCompletionSource<bool>> sendAsyncAction) {
            client.ShouldNotBeNull("client");
            sendAsyncAction.ShouldNotBeNull("sendAsyncAction");

            if(IsDebugEnabled)
                log.Debug("SmtpClient를 이용하여 비동기방식으로 메일을 발송합니다... SmtpHost=[{0}]", client.Host);

            var tcs = new TaskCompletionSource<bool>(userToken);
            token.Register(client.SendAsyncCancel);

            SendCompletedEventHandler handler = null;
            handler = (s, e) => EventAsyncPattern.HandleCompletion(tcs,
                                                                   e,
                                                                   () => (e.Cancelled == false && e.Error != null),
                                                                   () => client.SendCompleted -= handler);
            client.SendCompleted += handler;

            try {
                sendAsyncAction(tcs);
            }
            catch(Exception ex) {
                client.SendCompleted -= handler;
                tcs.TrySetException(ex);
            }

            return tcs.Task;
        }
    }
}