using System;
using System.Globalization;
using System.Web.Compilation;
using NUnit.Framework;

namespace NSoft.NFramework.StringResources {
    [TestFixture]
    public class ContainerFixture : ResourceProviderTestFixtureBase {
        [Test]
        public void GetProviderFactory() {
            Assert.IsNotNull(ResourceProvider.ProviderFactory);
        }

        [Test]
        public void GetProvider() {
            Console.WriteLine("기본 Assembly 사용");
            IResourceProvider provider = ResourceProvider.ProviderFactory.CreateGlobalResourceProvider("Glossary");
            Assert.IsNotNull(provider);

            Assert.IsNotNull(provider.GetObject("HomePage", new CultureInfo("ko")));

            Console.WriteLine("기타 Assembly 사용");

            Assert.IsNotNull(ResourceProvider.ProviderFactory.CreateGlobalResourceProvider("Sample.ExtResources|Glossary"));
            Assert.IsNotNull(ResourceProvider.ProviderFactory.CreateGlobalResourceProvider("Sample.ExtResources|CommonTerms"));
        }
    }
}