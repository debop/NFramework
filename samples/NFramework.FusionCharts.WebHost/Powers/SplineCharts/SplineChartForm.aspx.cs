using System;

namespace NSoft.NFramework.FusionCharts.WebHost.Powers.SplineCharts {
    public partial class SplineChartForm : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            chartSpline.FileName = ChartNameList.SelectedValue + ".swf";

            if(chartSpline.FileName.StartsWith("MS"))
                chartSpline.DataUrl = "MultiSeriesDataProvider.ashx";
            else
                chartSpline.DataUrl = "SingleSeriesDataProvider.ashx";
        }
    }
}