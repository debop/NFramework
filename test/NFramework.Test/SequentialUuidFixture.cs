using System;
using NUnit.Framework;

namespace NSoft.NFramework.Tools {
    [TestFixture]
    public class SequentialUuidFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        [Test]
        public void Generate_SequenceUuid() {
            const int GuidCount = 100;

            var guids = new Guid[GuidCount];

            for(int i = 0; i < GuidCount; i++) {
                guids[i] = GuidTool.NewSequentialGuid();

                log.Debug("Guid{0} = {1}", i, guids[i]);

                if(i > 0)
                    Assert.IsTrue(guids[i].CompareTo(guids[i - 1]) > 0, "Error!!! not sequential guid.");
            }
        }
    }
}