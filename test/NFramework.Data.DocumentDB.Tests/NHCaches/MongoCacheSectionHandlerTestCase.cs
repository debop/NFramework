using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NUnit.Framework;

namespace NSoft.NFramework.Data.MongoDB.NHCaches {
    [TestFixture]
    public class MongoCacheSectionHandlerTestCase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        public static XmlNode GetConfigurationSection(string xmlText) {
            var doc = new XmlDocument();
            doc.LoadXml(xmlText);
            return doc.DocumentElement;
        }

        public static MongoCacheClient[] CreateCache(MongoCacheConfig[] configs) {
            var result = new List<MongoCacheClient>(configs.Length);
            result.AddRange(configs.Select(cfg => new MongoCacheClient(cfg.Region, cfg.Properties)));
            return result.ToArray();
        }

        [Test]
        public void Parse_From_Null_Section() {
            var handler = new MongoCacheSectionHandler();
            var section = new XmlDocument();

            var result = handler.Create(null, null, section);

            Assert.IsNotNull(result);
            Assert.IsTrue(result is MongoCacheConfig[]);

            var cacheConfigs = result as MongoCacheConfig[];
            Assert.AreEqual(0, cacheConfigs.Length);
        }

        [Test]
        public void Parse_From_Config_Section() {
            const string xmlText =
                @"<mongoCache><cache region='NFramework' connectionString='server=localhost;database=NHCaches;safe=true;' expiration='00:00:05' compressThreshold='1024' /></mongoCache>";

            var handler = new MongoCacheSectionHandler();
            var section = GetConfigurationSection(xmlText);

            var cache = CreateCache(handler.Create(null, null, section) as MongoCacheConfig[]);

            Assert.AreEqual("NSoft.NFramework", cache[0].Region);
            Assert.AreEqual(5, cache[0].Expiry.TotalSeconds);
            Assert.AreEqual(1024, cache[0].CompressThreshold);
        }
    }
}