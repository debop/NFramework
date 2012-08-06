using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using NSoft.NFramework.IO;
using NUnit.Framework;

namespace NSoft.NFramework.Networks {
    [TestFixture]
    public class MailSenderFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private const string SmtpHost = "mail.realweb21.com";

        private const string UserName = "debop@realweb21.com";
        private const string Password = "baekwon9";
        private const string FromAddress = "debop@realweb21.com";

        private const string ToAddress = "debop@realweb21.com,realrnjs@realweb21.com";

        private static readonly string[] AttachFileNames = new[]
                                                           {
                                                               @"UnitTest_Files\bookmark.htm",
                                                               @"UnitTest_Files\bookmark2.htm"
                                                           };

        private static string GetTesterInformation() {
            var hostName = Dns.GetHostName();
            var ipAddress = Dns.GetHostAddresses(hostName).LastOrDefault();

            return string.Format(":NFramework 테스트 실행 컴퓨터={0}, IPAddress={1}", hostName, ipAddress);
        }

        [Test]
        public void SimpleSend() {
            var mailer = new SmtpClient(SmtpHost);
            Assert.IsNotNull(mailer);

            mailer.Send(FromAddress,
                        ToAddress.Replace(";", ","),
                        "간단한 메일" + GetTesterInformation(),
                        "메일 본문입니다." + Environment.NewLine + GetTesterInformation());
        }

        [Test]
        public void SimpleMessage() {
            using(var message = new MailMessage(FromAddress, ToAddress.Replace(";", ","))) {
                Assert.IsNotNull(message);
                message.Subject = "NSoft.NFramework.Networks.Smtp.MailSender 발송 메일" + GetTesterInformation();
                message.Body = "메일 본문입니다" + Environment.NewLine + GetTesterInformation();

                MailSender.SendMessage(SmtpHost, message);
            }
        }

        [Test]
        public void HtmlMessage() {
            using(var message = new MailMessage(FromAddress, ToAddress.Replace(";", ","))) {
                Assert.IsNotNull(message);

                message.Subject = "NSoft.NFramework.Networks.Smtp.MailSender 발송 메일" + GetTesterInformation();
                message.BuildHtmlMessageBody(new Uri("http://www.naver.com"));

                MailSender.SendMessage(SmtpHost, message);
            }
        }

        [Test]
        public void AttachFiles() {
            using(var message = new MailMessage(FromAddress, ToAddress.Replace(";", ","))) {
                Assert.IsNotNull(message);

                message.Subject = "NSoft.NFramework.Networks.Smtp.MailSender 발송 메일" + GetTesterInformation();
                message.BuildHtmlMessageBody(new Uri("http://www.naver.com"));

                message.BuildAttachments(AttachFileNames);
                MailSender.SendMessage(SmtpHost, message);
            }
        }

        [Test]
        public void AttachFileStreams() {
            var attachFiles = new Dictionary<string, Stream>();

            using(var message = new MailMessage(FromAddress, ToAddress.Replace(";", ","))) {
                try {
                    Assert.IsNotNull(message);

                    message.Subject = "NSoft.NFramework.Networks.Smtp.MailSender 발송 메일" + GetTesterInformation();
                    message.BuildHtmlMessageBody(new Uri("http://www.naver.com"));

                    attachFiles.Clear();
                    foreach(string filename in AttachFileNames) {
                        if(filename.FileExists())
                            attachFiles.Add(filename, new FileStream(filename, FileMode.Open));
                    }

                    message.BuildAttachments(attachFiles);
                    MailSender.SendMessage(SmtpHost, message);
                }
                finally {
                    foreach(Stream fs in attachFiles.Values.Where(fs => fs != null)) {
                        fs.Close();
                    }
                }
            }
        }
    }
}