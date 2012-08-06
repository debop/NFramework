using System;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Tools;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework {
    [Microsoft.Silverlight.Testing.Tag("Core")]
    [TestFixture]
    public class GuidCombFixture : AbstractFixture {
        [Test]
        public void GenerateTest() {
            Enumerable.Range(0, 100)
                .Select(i => GuidTool.NewComb())
                .RunEach(guid => {
                             Assert.IsInstanceOf<Guid>(guid);
                             Assert.AreNotEqual(Guid.Empty, guid);
                         });
        }

        [Test]
        public void GenerateParallel() {
            TestTool.RunTasks(100,
                              () => {
                                  var guid = GuidTool.NewComb();
                                  Assert.IsInstanceOf<Guid>(guid);
                                  Assert.AreNotEqual(Guid.Empty, guid);
                              });
        }
    }
}