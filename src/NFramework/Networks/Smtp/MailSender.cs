using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Networks {
    /// <summary>
    /// <see cref="SmtpClient"/> 와 <see cref="MailMessage"/>를 이용한 메일 전송용 Utility Class
    /// </summary>
    public static class MailSender {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Mail 보내기
        /// </summary>
        /// <param name="host">SMTP Host address.</param>
        /// <param name="message">mail message</param>
        public static void SendMessage(string host, MailMessage message) {
            host.ShouldNotBeWhiteSpace("host");
            message.ShouldNotBeNull("message");

            if(IsDebugEnabled)
                log.Debug("email을 전송합니다... host=[{0}], message.Subject=[{1}]", host, message.Subject);

            var mailClient = new SmtpClient(host);
            mailClient.Send(message);
        }

        /// <summary>
        /// Mail 보내기
        /// </summary>
        /// <param name="host">SMTP Host address.</param>
        /// <param name="port">SMTP port</param>
        /// <param name="message">mail message</param>
        public static void SendMessage(string host, int port, MailMessage message) {
            host.ShouldNotBeWhiteSpace("host");
            message.ShouldNotBeNull("message");

            if(IsDebugEnabled)
                log.Debug("email을 전송합니다... host=[{0}], port=[{1}], message.Subject=[{2}]", host, port, message.Subject);

            var mailClient = new SmtpClient(host, port);
            mailClient.Send(message);
        }

        /// <summary>
        /// <see cref="MailMessage"/> 의 본문을 지정된 <see cref="Uri.AbsoluteUri"/>의 컨텐츠로 설정한다.
        /// </summary>
        /// <param name="message">메일 메시지 인스턴스</param>
        /// <param name="uri">본문으로 설정할 내용이 있는 <see cref="Uri"/></param>
        /// <param name="bodyEncoding">본문의 인코딩 방식</param>
        public static void BuildHtmlMessageBody(this MailMessage message, Uri uri, Encoding bodyEncoding = null) {
            message.ShouldNotBeNull("message");
            uri.ShouldNotBeNull("uri");

            if(IsDebugEnabled)
                log.Debug("HTML 형식의 메시지 본문을 빌드합니다... uri=[{0}], bodyEncoding=[{1}]", uri, bodyEncoding);

            try {
                message.IsBodyHtml = true;
                message.Body = HttpTool.GetString(uri.AbsoluteUri, bodyEncoding);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("HTML 형식의 메시지 본문을 빌드하는데 실패했습니다!!!");
                    log.Error(ex);
                }

                throw;
            }
        }

        /// <summary>
        /// 첨부파일 정보를 메일 본문에 추가한다.
        /// </summary>
        /// <param name="message">Mail message</param>
        /// <param name="filenames">array of attached file names.</param>
        public static void BuildAttachments(this MailMessage message, params string[] filenames) {
            if(filenames == null || filenames.Length == 0)
                return;

            message.ShouldNotBeNull("message");

            if(IsDebugEnabled)
                log.Debug("메일에 첨부파일을 추가합니다... filenames=[{0}]", filenames.CollectionToString());

            foreach(var filename in filenames)
                message.Attachments.Add(new Attachment(filename));
        }

        /// <summary>
        /// 첨부파일 정보를 메일 본문에 추가한다.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="files">파일명, FileStream 의 Dictionary</param>
        public static void BuildAttachments(this MailMessage message, IDictionary<string, Stream> files) {
            if(files == null || files.Count == 0)
                return;

            message.ShouldNotBeNull("message");

            foreach(var file in files.Where(f => f.Value != null)) {
                message.Attachments.Add(new Attachment(file.Value, file.Key));
            }
        }
    }
}