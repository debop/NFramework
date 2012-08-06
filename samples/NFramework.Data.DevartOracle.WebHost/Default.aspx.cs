using System;
using Devart.Data.Oracle;
using NSoft.NFramework.Data.DevartOracle.WebHost.Domains.Models;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.DevartOracle.WebHost {
    public partial class Default : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            int companyCount = 0;

            using(var conn = new OracleConnection(ConfigTool.GetConnectionString("LOCAL_XE")))
            using(var cmd = new OracleCommand("SELECT COUNT(*) FROM NH_COMPANY", conn)) {
                conn.Open();
                companyCount = cmd.ExecuteScalar().AsInt();
            }

            lblMessage.Text = "CompanyCount=" + companyCount;


            lblMessage2.Text = "CompanyCount=" + Repository<Company>.Count();
        }
    }
}