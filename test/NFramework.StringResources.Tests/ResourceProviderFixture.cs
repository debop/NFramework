using System;
using System.Globalization;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.InversionOfControl;
using NUnit.Framework;

namespace NSoft.NFramework.StringResources {
    [TestFixture]
    public class ResourceProviderFixture {
        private DefaultResourceProviderFactory _factory;

        static ResourceProviderFixture() {
            IoC.Initialize();
        }

        [TestFixtureSetUp]
        public void ClassSetUp() {
            _factory = IoC.Resolve<IResourceProviderFactory>() as DefaultResourceProviderFactory;
            Assert.IsNotNull(_factory);
        }

        [Test]
        public void ResourceProviderByConfiguration() {
            if(_factory.GlobalResourceProviderName.Contains("External"))
                ExternalResourceTest();
            else if(_factory.GlobalResourceProviderName.Contains("File"))
                FileResourceTest();
            else if(_factory.GlobalResourceProviderName.Contains("NH")) {
                NHResourceTest();
            }
        }

        [Test]
        public void ExternalResourceTest() {
            _factory.GlobalResourceProviderName = "ExternalResourceProvider";

            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|Glossary", "HomePage"));
            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|Glossary", "HomePage", new CultureInfo("ko")));
            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|Glossary", "HomePage", new CultureInfo("ko-KR")));
            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|Glossary", "HomePage", new CultureInfo("en")));
            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|Glossary", "HomePage", new CultureInfo("en-US")));
            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|Glossary", "HomePage", new CultureInfo("en-CA")));

            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|Glossary", "Welcome"));
            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|Glossary", "Welcome", new CultureInfo("ko")));
            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|Glossary", "Welcome", new CultureInfo("ko-KR")));
            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|Glossary", "Welcome", new CultureInfo("en")));
            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|Glossary", "Welcome", new CultureInfo("en-US")));
            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|Glossary", "Welcome", new CultureInfo("en-CA")));

            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|CommonTerms", "Hello"));
            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|CommonTerms", "Hello", new CultureInfo("ko")));
            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|CommonTerms", "Hello", new CultureInfo("ko-KR")));
            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|CommonTerms", "Hello", new CultureInfo("en")));
            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|CommonTerms", "Hello", new CultureInfo("en-US")));
            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|CommonTerms", "Hello", new CultureInfo("en-CA")));
        }

        [Test]
        public void ExternalResourceTest2() {
            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|CommonTerms", "Hello"));
            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|CommonTerms", "Hello", new CultureInfo("ko")));
            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|CommonTerms", "Hello", new CultureInfo("ko-KR")));
            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|CommonTerms", "Hello", new CultureInfo("en")));
            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|CommonTerms", "Hello", new CultureInfo("en-US")));
            Console.WriteLine(ResourceProvider.GetString("Sample.ExtResources|CommonTerms", "Hello", new CultureInfo("en-CA")));
        }

        [Test]
        public void FileResourceTest() {
            _factory.GlobalResourceProviderName = "FileResourceProvider";

            string classKey = "Glossary"; // "DEFAULT|Glossary" 와 같다.

            Console.WriteLine(ResourceProvider.GetString(classKey, "HomePage"));
            Console.WriteLine(ResourceProvider.GetString(classKey, "HomePage", new CultureInfo("ko")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "HomePage", new CultureInfo("ko-KR")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "HomePage", new CultureInfo("en")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "HomePage", new CultureInfo("en-US")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "HomePage", new CultureInfo("en-CA")));

            Assert.IsNotNull(ResourceProvider.GetString(classKey, "HomePage", new CultureInfo("en-CA")));

            Console.WriteLine(ResourceProvider.GetString(classKey, "Welcome"));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Welcome", new CultureInfo("ko")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Welcome", new CultureInfo("ko-KR")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Welcome", new CultureInfo("en")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Welcome", new CultureInfo("en-US")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Welcome", new CultureInfo("en-CA")));

            Assert.IsNotNull(ResourceProvider.GetString(classKey, "Welcome", new CultureInfo("en-CA")));

            classKey = "CommonTerms";

            Console.WriteLine(ResourceProvider.GetString(classKey, "Hello"));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Hello", new CultureInfo("ko")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Hello", new CultureInfo("ko-KR")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Hello", new CultureInfo("en")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Hello", new CultureInfo("en-US")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Hello", new CultureInfo("en-CA")));

            Assert.IsNotEmpty(ResourceProvider.GetString(classKey, "Hello", new CultureInfo("en-CA")));
        }

        [Test]
        public void FileResourceTest2() {
            _factory.GlobalResourceProviderName = "FileResourceProvider";

            string classKey = @"OtherResources|Glossary";

            Console.WriteLine(ResourceProvider.GetString(classKey, "HomePage"));
            Console.WriteLine(ResourceProvider.GetString(classKey, "HomePage", new CultureInfo("ko")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "HomePage", new CultureInfo("ko-KR")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "HomePage", new CultureInfo("en")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "HomePage", new CultureInfo("en-US")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "HomePage", new CultureInfo("en-CA")));

            Assert.IsNotNull(ResourceProvider.GetString(classKey, "HomePage", new CultureInfo("en-CA")));

            Console.WriteLine(ResourceProvider.GetString(classKey, "Welcome"));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Welcome", new CultureInfo("ko")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Welcome", new CultureInfo("ko-KR")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Welcome", new CultureInfo("en")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Welcome", new CultureInfo("en-US")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Welcome", new CultureInfo("en-CA")));

            Assert.IsNotEmpty(ResourceProvider.GetString(classKey, "Welcome", new CultureInfo("en-CA")));

            classKey = @"OtherResources|CommonTerms";

            Console.WriteLine(ResourceProvider.GetString(classKey, "Hello"));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Hello", new CultureInfo("ko")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Hello", new CultureInfo("ko-KR")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Hello", new CultureInfo("en")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Hello", new CultureInfo("en-US")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Hello", new CultureInfo("en-CA")));

            Assert.IsNotEmpty(ResourceProvider.GetString(classKey, "Hello", new CultureInfo("en-CA")));
        }

        [Test]
        public void NHResourceTest() {
            if(UnitOfWork.IsStarted == false)
                UnitOfWork.Start();

            _factory.GlobalResourceProviderName = "NHResourceProvider";

            string classKey = @"OtherResources|Glossary";

            Console.WriteLine(ResourceProvider.GetString(classKey, "HomePage"));
            Console.WriteLine(ResourceProvider.GetString(classKey, "HomePage", new CultureInfo("ko")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "HomePage", new CultureInfo("ko-KR")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "HomePage", new CultureInfo("en")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "HomePage", new CultureInfo("en-US")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "HomePage", new CultureInfo("en-CA")));

            Assert.IsNotNull(ResourceProvider.GetString(classKey, "HomePage", new CultureInfo("en-CA")));

            Console.WriteLine(ResourceProvider.GetString(classKey, "Welcome"));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Welcome", new CultureInfo("ko")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Welcome", new CultureInfo("ko-KR")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Welcome", new CultureInfo("en")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Welcome", new CultureInfo("en-US")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Welcome", new CultureInfo("en-CA")));

            Assert.IsNotNull(ResourceProvider.GetString(classKey, "Welcome", new CultureInfo("en-CA")));

            classKey = @"OtherResources|CommonTerms";

            Console.WriteLine(ResourceProvider.GetString(classKey, "Hello"));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Hello", new CultureInfo("ko")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Hello", new CultureInfo("ko-KR")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Hello", new CultureInfo("en")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Hello", new CultureInfo("en-US")));
            Console.WriteLine(ResourceProvider.GetString(classKey, "Hello", new CultureInfo("en-CA")));

            Assert.IsNotNull(ResourceProvider.GetString(classKey, "Hello", new CultureInfo("en-CA")));
        }
    }
}