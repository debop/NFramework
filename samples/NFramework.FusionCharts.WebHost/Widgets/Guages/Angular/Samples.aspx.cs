using System;

namespace NSoft.NFramework.FusionCharts.WebHost.Widgets.Guages.Angular {
    public partial class Samples : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            angularGauge.DataUrl = "SampleData.ashx?Option=" + SampleList.SelectedValue;
        }
    }
}