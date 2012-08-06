using System;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Xml {
    [TestFixture]
    public class XStreamReaderFixture {
        [Test]
        public void LoadUriTest() {
            // Read Rss
            var rss =
                XStreamReader.Load("http://services.social.microsoft.com/feeds/feed/CSharpHeadlines")
                    .Descendants("item")
                    .Select(x => new
                                 {
                                     Title = x.Element("title").GetElementValue(),
                                     Description = x.Element("description").GetElementValue(),
                                     PubDate = x.Element("pubDate").GetElementValue().AsDateTime()
                                 })
                    .ToList();

            rss.Should().Not.Be.Null();
            rss.RunEach(x => Console.WriteLine("Title=[{0}], Description=[{1}], PubDate=[{2}]", x.Title, x.Description, x.PubDate));
        }
    }
}