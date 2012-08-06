using System;

namespace NSoft.NFramework.FusionCharts.WebHost.Powers.Radar {
    public partial class RadarChartForm : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            radarChart.DataUrl = "RadarChartDataProvider.ashx?Option=" + dataOptions.SelectedValue;
        }
    }
}