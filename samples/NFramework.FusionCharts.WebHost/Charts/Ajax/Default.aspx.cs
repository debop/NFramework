using System;
using System.Linq;
using System.Web.UI.WebControls;
using NSoft.NFramework.FusionCharts.WebHost.Domain.Services;

namespace NSoft.NFramework.FusionCharts.WebHost.Charts.Ajax {
    public partial class Default : System.Web.UI.Page {
        public int FactoryId {
            get { return ViewState["FactoryId"].AsInt(1); }
            set { ViewState["FactoryId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {
                FactoryId = Request["FactoryId"].AsInt(1);

                BuildFactoryMasterButtonList(RadioButtonList1);
                RadioButtonList1.SelectedIndex = FactoryId - 1;

                UpdateChart();
            }
        }

        protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateChart();
        }

        protected void UpdateChart() {
            // ViewState가 유지되는지
            lblFactory.Text = "Previous Factory = Factory " + FactoryId.AsText();

            FactoryId = RadioButtonList1.SelectedValue.AsInt();

            var outputs = FactoryRepository.FindAllFactoryOutputByFactoryId(FactoryId);

            GridView1.DataSource = outputs.OrderBy(output => output.DatePro);
            GridView1.DataBind();

            factoryOutputChart.DataUrl = "FactoryDataHandler.ashx?FactoryId=" + FactoryId;
        }

        private static void BuildFactoryMasterButtonList(ListControl buttonList) {
            var masters = FactoryRepository.FindAllFactoryMaster();

            buttonList.Items.Clear();
            foreach(var master in masters)
                buttonList.Items.Add(new ListItem(master.Name, master.Id.ToString()));

            if(buttonList.Items.Count > 0)
                buttonList.Items[0].Selected = true;
        }
    }
}