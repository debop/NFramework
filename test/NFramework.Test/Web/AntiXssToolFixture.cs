using System;
using NUnit.Framework;

namespace NSoft.NFramework.Web.Tools {
    [TestFixture]
    public class AntiXssToolFixture {
        [Test]
        public void EncodeJavascript() {
            var encoded = AntiXssTool.JavaScriptEncode("javascript:alert('abc');");
            Console.WriteLine(encoded);
        }
    }
}