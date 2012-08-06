using System;
using NSoft.NFramework.Web.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Core.Web {
    [TestFixture]
    public class WebToolFixture {
        [Test]
        [TestCase("<div class=\"main\"><p>안녕하세요</p></div>")]
        [TestCase("<div class=\"main\"><p>안녕하세요. <b>정말</b> 좋아보입니다.</p></div>")]
        public void CanRemoveHtml(string html) {
            var plainText = WebTool.RemoveHtml(html);
            Assert.AreNotEqual(html, plainText);
            Console.WriteLine("{0}=>{1}", html, plainText);
        }
    }
}