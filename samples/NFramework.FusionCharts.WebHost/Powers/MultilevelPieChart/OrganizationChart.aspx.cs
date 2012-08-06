using System;

namespace NSoft.NFramework.FusionCharts.WebHost.Powers.MultilevelPieChart {
    public partial class OrganizationChart : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            multiPieChart.DataUrl = "MultilevelPieChartDataProvider.ashx?Option=" + SampleList.SelectedValue;
        }
    }
}