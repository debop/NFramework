using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Serializations.Serializers;
using NUnit.Framework;

namespace NSoft.NFramework.XmlData {
    public abstract class AbstractXmlDataFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        [TestFixtureSetUp]
        public void ClassSetUp() {
            IoC.Initialize();
        }

        [TestFixtureTearDown]
        public void ClassTearDown() {
            IoC.Reset();
        }

        internal static ISerializer GetSerializer(bool? compress, bool? security) {
            ISerializer result = new CloneSerializer();

            if(compress.GetValueOrDefault(false))
                result = new CompressSerializer(result);

            if(security.GetValueOrDefault(false))
                result = new EncryptSerializer(result);

            return result;
        }
    }
}