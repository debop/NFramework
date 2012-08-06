using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Networks {
    /// <summary>
    /// 메일 발송 예제
    /// </summary>
    /// <remarks>
    /// 우리 서버를 통해서 보낼 때는 첨부파일명이 제대로 가는데, 두산엔진메일서버를 거칠 때는 파일명이 'Unknown_file_2' 으로 보인다???
    /// 
    /// 두산엔진은 메일 메시지 전체 용량이 30M이므로, 메일 실패시에는 용량을 검사해야 한다.
    /// </remarks>
    [TestFixture]
    public class SmtpClientFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        // 두산엔진 이정아 과장
        //
        // 
        //private const string SmtpServer = "wiseshield.doosanengine.com"; // 10.100.100.5
        //private const string UserName = "7310122821519";
        //private const string Password = "xf0416!";
        //private const string FromAddress = "gillian@doosanengine.com";

        private const string SmtpServer = "mail.realweb21.com";
        private const string UserName = "debop@realweb21.com";
        private const string Password = "baekwon9";
        private const string FromAddress = "debop@realweb21.com";

        // private const string ToAddress = "debop@realweb21.com,gillian@doosanengine.com";
        private const string ToAddress = "debop@realweb21.com,realrnjs@realweb21.com";

        private const string AttachFileName = @"UnitTest_Files\bookmark.htm";
        private const string AttachFileName2 = @"UnitTest_Files\bookmark2.htm";

        private static string GetTesterInformation() {
            var hostName = Dns.GetHostName();
            var ipAddress = Dns.GetHostAddresses(hostName).LastOrDefault();

            return string.Format(":NFramework 테스트 실행 컴퓨터={0}, IPAddress={1}", hostName, ipAddress);
        }

        [Test]
        public void DirectMailTest() {
            var mailClient = new SmtpClient(SmtpServer);

            mailClient.Send(FromAddress,
                            ToAddress.Replace(';', ','),
                            "Direct Mail using SmtpClient.Send()",
                            "SmtpClient 인스턴스를 이용한 메일 본문입니다");
        }

        [Test]
        public void SimpleTest() {
            using(var message = new MailMessage()) {
                message.From = new MailAddress(FromAddress);

                var toAddrs = ToAddress.Split(';', ',');
                foreach(string toAddr in toAddrs)
                    message.To.Add(toAddr);

                message.Subject = "메일 제목" + GetTesterInformation();
                message.Body = "메일 본문" + Environment.NewLine + GetTesterInformation();


                var mailClient = new SmtpClient(SmtpServer);

                // 로그인 정보를 입력한다.
                //
                mailClient.Credentials = new NetworkCredential(UserName, Password);

                mailClient.Send(message);
            }
        }

        [Test]
        public void HtmlMailTest() {
            using(var message = new MailMessage()) {
                message.From = new MailAddress(FromAddress);

                var toAddrs = ToAddress.Split(';', ',');
                foreach(string toAddr in toAddrs)
                    message.To.Add(toAddr);

                message.Subject = "Html 메일입니다." + GetTesterInformation();
                message.SubjectEncoding = Encoding.UTF8;

                message.IsBodyHtml = true;
                // message.Body = "<html><body><table><tr><td>안녕하세요</td></tr></table></body></html>";
                message.Body = ReadFile(AttachFileName, Encoding.UTF8);
                message.BodyEncoding = Encoding.UTF8;

                var mailClient = new SmtpClient(SmtpServer);

                // 로그인 정보를 입력한다.
                //
                mailClient.Credentials = new NetworkCredential(UserName, Password);

                mailClient.Send(message);
            }
        }

        [Test]
        public void AttachFileFromLocalTest() {
            using(var message = new MailMessage()) {
                message.From = new MailAddress(FromAddress);

                var toAddrs = ToAddress.Split(';', ',');
                foreach(string toAddr in toAddrs)
                    message.To.Add(toAddr);

                // 제목
                //
                message.Subject = "첨부파일 메일입니다." + GetTesterInformation();
                message.SubjectEncoding = Encoding.UTF8;

                // 본문
                //
                message.IsBodyHtml = true; // Html 인지 설정
                // message.Body = "<html><body><table><tr><td>안녕하세요</td></tr></table></body></html>";
                message.Body = ReadFile(AttachFileName, Encoding.UTF8);
                message.BodyEncoding = Encoding.UTF8;

                // 첨부파일
                message.Attachments.Add(new Attachment(AttachFileName));
                message.Attachments.Add(new Attachment(AttachFileName2));

                // 로그인 정보를 입력하고, 메일을 보낸다.
                //
                var mailClient = new SmtpClient(SmtpServer);
                mailClient.Credentials = new NetworkCredential(UserName, Password);
                mailClient.Send(message);
            }
        }

        /// <summary>
        /// 첨부파일이 FTP에 있을 경우
        /// </summary>
        [Test]
        public void AttachFileFromFtpTest() {
            using(var message = new MailMessage()) {
                message.From = new MailAddress(FromAddress, "메일테스터");

                var toAddrs = ToAddress.Split(';', ',');
                foreach(string toAddr in toAddrs)
                    message.To.Add(toAddr);

                // 제목
                //
                message.Subject = "FTP 첨부파일 메일입니다." + GetTesterInformation();
                // message.SubjectEncoding = Encoding.UTF8;

                // 본문
                //
                message.IsBodyHtml = true; // Html 인지 설정
                // message.Body = "<html><body><table><tr><td>안녕하세요</td></tr></table></body></html>";
                message.Body = ReadFile(AttachFileName, Encoding.UTF8);
                // message.BodyEncoding = Encoding.UTF8;

                // FTP 첨부파일
                //
                var attachment = new Attachment(FtpDownload("/ATTACH/2007-04/PDF.pdf"), "PDF.pdf");
                attachment.NameEncoding = Encoding.UTF8;
                message.Attachments.Add(attachment);

                //for( int i = 1; i < 5; i++ )
                //{
                //   string remoteFile = "/첨부/Abba" + i.ToString() + ".mp3";
                //   string filename = "Abba" + i.ToString() + ".mp3";
                //   attachment = new Attachment(FtpDownload(remoteFile), filename );

                //   Console.WriteLine("Attachment Filename: " + attachment.Name);

                //   attachment.Name = filename;
                //   //attachment.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                //   //attachment.NameEncoding = Encoding.UTF8;

                //   message.Attachments.Add(attachment);
                //}

                // 첨부파일 크기가 너무 커서 안된다.
                attachment = new Attachment(FtpDownload("/ATTACH/2007-04/CM.ppt"), "CM.ppt");
                message.Attachments.Add(attachment);

                // 로그인 정보를 입력하고, 메일을 보낸다.
                //
                var mailClient = new SmtpClient(SmtpServer);
                mailClient.Credentials = new NetworkCredential(UserName, Password);
                mailClient.Send(message);
            }
        }

        [Test]
        public void HttpGetToMailTest() {
            using(var message = new MailMessage()) {
                message.From = new MailAddress(FromAddress);

                var toAddrs = ToAddress.Split(';', ',');
                foreach(string toAddr in toAddrs)
                    message.To.Add(toAddr);

                // 제목
                //
                message.Subject = "HttpGet을 이용한 본문 메시지입니다." + GetTesterInformation();
                message.SubjectEncoding = Encoding.UTF8;

                // 본문
                //
                message.IsBodyHtml = true; // Html 인지 설정

                // Html Encoding관련 문제가 있다.
                //
                // HttpClient http = new HttpClient("http://www.codeproject.com/");
                var http = new HttpClient("http://www.naver.com/");
                message.Body = http.Get();
                // message.BodyEncoding = Encoding.GetEncoding("euc-kr");
                message.IsBodyHtml = true;

                // 첨부파일
                message.Attachments.Add(new Attachment(AttachFileName));
                message.Attachments.Add(new Attachment(AttachFileName2));

                // 로그인 정보를 입력하고, 메일을 보낸다.
                //
                var mailClient = new SmtpClient(SmtpServer);
                mailClient.Credentials = new NetworkCredential(UserName, Password);
                mailClient.Send(message);
            }
        }

        private static string ReadFile(string filename) {
            return ReadFile(filename, Encoding.Default);
        }

        private static string ReadFile(string filename, Encoding enc) {
            using(var reader = new StreamReader(filename, enc)) {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// FTP 의 원격파일을 다운로드한다.
        /// </summary>
        /// <param name="remoteFile"></param>
        /// <returns></returns>
        private static Stream FtpDownload(string remoteFile) {
            using(new OperationTimer("FTP Download elapsed ")) {
                var stream = new MemoryStream();

                var ftp = new FtpClient("localhost", "anonymous", "");
                ftp.Download(remoteFile, stream);

                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
        }
    }
}