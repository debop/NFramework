using System;
using System.Windows.Forms;
using Castle.Windsor;
using Castle.Windsor.Installer;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.DataServices.Clients.WindsorInstallers;
using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.DataServices.WinForm {
    internal static class Program {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        private static void Main() {
            IoC.Initialize(InstallComponents());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static IWindsorContainer InstallComponents() {
            var container = new WindsorContainer();

            container.Install(FromAssembly.This(),
                              FromAssembly.Containing<MessageSerializerWindsorInstaller>(),
                              FromAssembly.Containing<ICompressor>());
            return container;
        }
    }
}