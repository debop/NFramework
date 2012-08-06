using System;
using System.IO;
using NSoft.NFramework.IO;
using NUnit.Framework;

namespace NSoft.NFramework.Data.QueryProviders {
    /// <summary>
    /// NOTE : 파일을 다루므로, Thread-Safe 하지 않습니다!!!
    /// </summary>
    [TestFixture]
    public class DynamicQueryProviderFixture {
        private static readonly string IniFilePath = FileTool.GetPhysicalPath(@".\QueryFiles\Pubs.ado.mssql.ini");

        [Test]
        public void DynamicReloadedTest() {
            var provider = new DynamicQueryProvider(IniFilePath);

            provider.QueryFileChanged += (sender, args) => {
                                             Assert.IsTrue(args.FullPath.Equals(provider.QueryFilePath));
                                             Assert.IsTrue(args.ChangeType == WatcherChangeTypes.Changed);
                                         };

            Assert.IsNotNull(provider);
            Assert.IsNotNull(provider.GetQueries());

            File.AppendAllText(IniFilePath, Environment.NewLine);

            Assert.IsNotNull(provider);
            Assert.IsNotNull(provider.GetQueries());
        }
    }
}