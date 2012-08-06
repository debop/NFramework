using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using NSoft.NFramework.Parallelism.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.Extensions.EAP {
    [TestFixture]
    public class SmtpClientExtensionsTestCase : ParallelismFixtureBase {
        private const int TestCount = 10;

        private const string SmtpHost = "mail.realweb21.com";
        private const string FromAddress = "debop@realweb21.com";
        private const string Recipients = "debop@realweb21.com";

        [Test]
        public void SendMailAsync() {
            var tasks = new List<Task>();

            for(int i = 0; i < TestCount; i++) {
                var task =
                    new SmtpClient(SmtpHost)
                        .SendTask(FromAddress,
                                  Recipients,
                                  "제목: 비동기 메일 발송",
                                  "비동기 메일 발송 본문입니다.",
                                  null);
                tasks.Add(task);
            }

            log.Debug("메일 발송 요청 후 대기 중입니다.");

            var mailTasks = tasks.ToArray();

            Task.Factory
                .ContinueWhenAll(mailTasks, _ => log.Debug("메일 발송 완료!!!"))
                .Wait();
        }

        [Test]
        public void SendMailAsync_By_MailMessage() {
            var tasks = new List<Task>();

            var fromAddr = new MailAddress(FromAddress, FromAddress, Encoding.UTF8);
            var toAddr = new MailAddress(Recipients, Recipients, Encoding.UTF8);

            var mailMessage = new MailMessage(fromAddr, toAddr)
                              {
                                  Subject = @"비동기 메일 발송 BY MailMessage",
                                  SubjectEncoding = Encoding.UTF8,
                                  Body = @"비동기 메일 발송 본문입니다.",
                                  BodyEncoding = Encoding.UTF8
                              };

            for(int i = 0; i < TestCount; i++) {
                var task = new SmtpClient(SmtpHost).SendTask(mailMessage, null);
                tasks.Add(task);
            }

            log.Debug("메일 발송 요청 후 대기 중입니다.");

            var mailTasks = tasks.ToArray();

            Task.Factory.ContinueWhenAll(mailTasks, _ => log.Debug("메일 발송 완료!!!")).Wait();
        }
    }
}