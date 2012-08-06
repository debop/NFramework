using System;
using System.Xml;
using NSoft.NFramework.UnitTesting;
using NSoft.NFramework.Xml;
using NUnit.Framework;

namespace NSoft.NFramework.Networks {
    /// <summary>
    /// XmlHttpClient 테스트 (현재 웹 응용프로그램이 NET 3.5에서만 제대로 작동합니다)
    /// </summary>
    [TestFixture]
    public class XmlHttpClientFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private const string HttpHost = @"http://localhost:3500";
        private const string ScriptPath = "/NFramework/Networks/XmlHttp/XmlHttpServer";
        private const string ScriptParam = "?Action=";

        private const string TestHandler = HttpHost + ScriptPath + ".ashx" + ScriptParam;
        private const string TestPage = HttpHost + ScriptPath + ".aspx" + ScriptParam;

        private static readonly string[] testUrls = new[] { TestHandler, TestPage };

        [Test]
        public void GetText() {
            foreach(string url in testUrls) {
                string text = XmlHttpClient.GetText(url + XmlHttpMethods.GetText, true);

                Assert.IsNotEmpty(text, "url=" + url);

                Console.WriteLine("GetText() called. url=" + url);
                Console.WriteLine("returns = " + text);
            }
        }

        [Test]
        public void GetXml() {
            foreach(string url in testUrls) {
                var doc = XmlHttpClient.GetXml(url + XmlHttpMethods.GetXml, true);

                Assert.IsNotNull(doc, "url=" + url);
                Assert.IsTrue(doc.IsValidDocument(), "url=" + url);

                Console.WriteLine("GetText() called. url=" + url);
                Console.WriteLine("returns = " + doc.OuterXml);
            }
        }

        [Test]
        public void PostText() {
            foreach(string url in testUrls) {
                string text = XmlHttpClient.PostText(url + XmlHttpMethods.PostText, "A=가&B=나", true);

                Assert.IsNotEmpty(text, "url=" + url);

                Console.WriteLine("url=" + url);
                Console.WriteLine("retunrs=" + text);
            }
        }

        [Test]
        public void PostXml() {
            // NOTE : 이 테스트가 실패한다면, 웹 서버 Page의 ValidateRequest 옵션이 False 인지를 확인해라.
            //
            foreach(string url in testUrls) {
                try {
                    XmlDocument postDoc = XmlTool.CreateXmlDocument("<PostXml>Data for PostXml</PostXml>");
                    var doc = XmlHttpClient.PostXml(url + XmlHttpMethods.PostXml, postDoc, true);

                    Assert.IsNotNull(doc, "url=" + url);
                    Assert.IsTrue(doc.IsValidDocument(), "url=" + url);

                    Console.WriteLine("url=" + url);
                    Console.WriteLine("returns=" + doc.OuterXml);
                }
                catch(Exception ex) {
                    if(log.IsErrorEnabled)
                        log.ErrorException("웹 서버 Page의 ValidateRequest=False 이어야 합니다.", ex);
                }
            }
        }

        [Test]
        public void ThreadTest() {
            TestTool.RunTasks(5,
                              delegate {
                                  GetText();
                                  GetXml();
                                  PostText();
                                  PostXml();
                              });
        }
    }
}