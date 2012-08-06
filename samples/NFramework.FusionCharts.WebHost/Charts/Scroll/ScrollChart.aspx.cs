using System;

namespace NSoft.NFramework.FusionCharts.WebHost.Charts.Scroll {
    public partial class ScrollChart : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack)
                ChartNameList.Items[0].Selected = true;

            ChartName = ChartNameList.SelectedValue;
            Title = "Scroll Charts - " + ChartName;

            staticChart.FileName = ChartName + ".swf";
            dynamicChart.FileName = ChartName + ".swf";

            staticChart.DataUrl = "Data.xml";
            dynamicChart.DataUrl = "~/Charts/DataHandlers/CombinationChartDataHandler.ashx?FactoryId=2";

            if(ChartName.Contains("LineDY"))
                staticChart.DataUrl = "LineDual.xml";

            if(ChartName == "ScrollCombiDY2D") {
                staticChart.DataUrl = "MultiSeriesStackedData.xml";
                dynamicChart.DataUrl = "~/Charts/DataHandlers/MultiSeriesStackedChartDataHandler.ashx?FactoryId=2&LineDual=1";
            }
        }

        protected string ChartName { get; set; }
    }
}