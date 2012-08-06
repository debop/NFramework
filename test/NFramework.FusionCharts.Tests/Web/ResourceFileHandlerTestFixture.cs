using System;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.FusionCharts.Web {
    /// <summary>
    /// 참고 : http://www.attilan.com/2006/08/accessing_embedded_resources_u.php
    /// </summary>
    [TestFixture]
    public class ResourceFileHandlerTestFixture : ChartTestFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [Test]
        public void ReadFusionChartJavascript() {
            var asm = typeof(ChartBase).Assembly;
            Assert.IsNotNull(asm);

            foreach(var name in asm.GetManifestResourceNames()) {
                Console.WriteLine("Resourcename = " + name);
                var stream = asm.GetManifestResourceStream(name);
                Assert.IsNotNull(stream);
                if(IsDebugEnabled) {
                    log.Debug("Resource name=" + name);
                    log.Debug("Contents= " + stream.ToText());
                }
            }
        }

        [Test]
        public void ReadFusionChartFile() {
            var asm = ReflectionTool.LoadAssembly(typeof(ChartBase).Assembly.FullName);
            Assert.IsNotNull(asm);

            foreach(var name in asm.GetManifestResourceNames()) {
                Console.WriteLine("ResourceName = " + name);
                var fileStream = asm.GetManifestResourceStream(name);
                Assert.IsNotNull(fileStream, "ResourceName=" + name);
            }
        }

        [Test]
        public void ReadEmbeddedResourceFile() {
            var asm = ReflectionTool.LoadAssembly(typeof(ChartBase).Assembly.FullName);
            var swf = asm.GetEmbeddedResourceFile("line.swf");
            Assert.IsNotNull(swf);
        }
    }
}