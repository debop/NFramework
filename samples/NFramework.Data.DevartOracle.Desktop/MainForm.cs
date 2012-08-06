using System;
using System.Windows.Forms;
using NSoft.NFramework.Data.DevartOracle.Desktop.Domains.Models;
using NSoft.NFramework.Data.NHibernateEx;

namespace NSoft.NFramework.Data.DevartOracle.Desktop {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }

        private void cmdLoadCompany_Click(object sender, EventArgs e) {
            var count = Repository<Company>.Count();
            lblMessage.Text = "Company Count = " + count;
        }
    }
}