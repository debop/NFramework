using System;

namespace NSoft.NFramework.FusionCharts.WebHost.Powers.Kagi {
    public partial class KagiChartForm : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            kagiChart.DataUrl = "KagiChartDataProvider.ashx?Option=" + dataOptions.SelectedValue;
        }
    }
}