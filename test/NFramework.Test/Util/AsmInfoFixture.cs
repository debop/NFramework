using System;
using NUnit.Framework;

namespace NSoft.NFramework {
    [TestFixture]
    public class AsmInfoFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [Test]
        public void ExtractAssemblyInfo() {
            foreach(var asm in AppDomain.CurrentDomain.GetAssemblies()) {
                var asmInfo = new AsmInfo(asm);

                log.Debug("---------------------------------------");
                log.Debug("Name=" + asmInfo.FullName);
                log.Debug("Version=" + asmInfo.Version);
                log.Debug("CodeBase=" + asmInfo.CodeBase);
                log.Debug("Company=" + asmInfo.Company);
                log.Debug("Configuration=" + asmInfo.Configuration);
                log.Debug("Copyright=" + asmInfo.Copyright);
                log.Debug("Culture=" + asmInfo.Culture);
                log.Debug("DefaultAlias=" + asmInfo.DefaultAlias);
                log.Debug("Description=" + asmInfo.Description);
                log.Debug("InfomationalVersion=" + asmInfo.InfomationalVersion);
                log.Debug("Product=" + asmInfo.Product);
                log.Debug("Title=" + asmInfo.Title);
                log.Debug("Trademark=" + asmInfo.Trademark);
                log.Debug("Win32FileVersion=" + asmInfo.Win32FileVersion);
                log.Debug("---------------------------------------");
            }
        }
    }
}