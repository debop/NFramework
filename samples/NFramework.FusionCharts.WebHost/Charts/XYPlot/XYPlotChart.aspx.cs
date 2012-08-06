using System;

namespace NSoft.NFramework.FusionCharts.WebHost.Charts.XYPlot {
    public partial class XYPlotChart : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack)
                ChartNameList.Items[0].Selected = true;

            ChartName = ChartNameList.SelectedValue;
            Title = "XY Flot Charts - " + ChartName;

            staticChart.FileName = ChartName + ".swf";
            staticChart.DataUrl = ChartName + ".xml";

            dynamicChart.FileName = ChartName + ".swf";
            dynamicChart.DataUrl = "~/Charts/DataHandlers/XYPlotChartDataHandler.ashx?FactoryId=2&ChartName=" + ChartName;
        }

        public string ChartName { get; set; }
    }
}