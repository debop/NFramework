using System;

namespace NSoft.NFramework.FusionCharts.WebHost.Powers.DragNodes {
    public partial class DragNodeChartForm : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            dragNodeChart.DataUrl = "DragNodeDataProvider.ashx?Option=" + dataOptions.SelectedValue;
        }
    }
}