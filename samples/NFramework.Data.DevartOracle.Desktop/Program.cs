using System;
using System.Windows.Forms;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.Data.DevartOracle.Desktop {
    internal static class Program {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        private static void Main() {
            if(IoC.IsNotInitialized)
                IoC.Initialize();

            if(UnitOfWork.IsNotStarted)
                UnitOfWork.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}