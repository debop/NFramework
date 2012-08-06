using System;
using NUnit.Framework;
using SharedCache.WinServiceCommon.Provider.Cache;

namespace NSoft.NFramework.Caching.SharedCache {
    [TestFixture]
    public class SharedCacheToolFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public IndexusProviderBase Cache {
            get { return IndexusDistributionCache.SharedCache; }
        }

        [Test]
        public void Statistics() {
            Console.WriteLine("Item Count =" + Cache.ItemCount());
            Console.WriteLine("ServiceTotalSize=" + Cache.TotalSize());

            foreach(var pair in Cache.ItemUsageList())
                Console.WriteLine("{0}-{1}", pair.Key, pair.Value);
        }
    }
}