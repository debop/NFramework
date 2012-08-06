using System;

namespace NSoft.NFramework.FusionCharts.WebHost.Powers.MultiAxis {
    public partial class MultiAxisLineChartForm : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            mxLineChart.DataUrl = "MultiAxisLineChartDataProvider.ashx?Option=" + dataOptions.SelectedValue;
        }
    }
}