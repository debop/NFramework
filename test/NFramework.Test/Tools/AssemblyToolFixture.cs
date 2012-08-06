using System.Reflection;
using NUnit.Framework;

namespace NSoft.NFramework.Tools {
    [TestFixture]
    public class AssemblyToolFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [Test]
        public void ExtractAssemblyInfo() {
            var asms = new[]
                       {
                           Assembly.GetExecutingAssembly(),
                           Assembly.GetCallingAssembly()
                       };

            foreach(var asm in asms) {
                if(asm == null)
                    continue;

                log.Debug("--------------------");
                log.Debug("Assembly Name=" + asm.FullName);

                log.Debug("Assembly Company=" + asm.GetCompany());
                log.Debug("Assembly Product=" + asm.GetProduct());
                log.Debug("Assembly Description=" + asm.GetDescription());
                log.Debug("Assembly Title=" + asm.GetTitle());
                log.Debug("Assembly Copyright=" + asm.GetCopyright());

                log.Debug("Assembly DefaultAlias=" + asm.GetDefaultAlias());
                log.Debug("Assembly Culture=" + asm.GetCulture());
                log.Debug("Assembly Configuration=" + asm.GetConfiguration());

                log.Debug("Assembly Version=" + asm.GetFileVersion());
                log.Debug("Assembly FileVersion=" + asm.GetFileVersion());
                log.Debug("Assembly Information Version=" + asm.GetInformationalVersion());
                log.Debug("--------------------");
            }
        }
    }
}