using System;

namespace NSoft.NFramework.FusionCharts.WebHost.Charts.Combinations {
    public partial class CombinationChart : System.Web.UI.Page {
        public string ChartName {
            get { return ViewState["ChartName"].AsText(); }
            set { ViewState["ChartName"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack)
                ChartNameList.Items[0].Selected = true;

            lblChartName.Text = "Previous Chart Name = " + ChartName;

            ChartName = ChartNameList.SelectedValue;
            Title = "Multi Series Charts - " + ChartName;

            staticChart.FileName = ChartName + ".swf";
            dynamicChart.FileName = ChartName + ".swf";

            staticChart.DataUrl = "Data.xml";
            dynamicChart.DataUrl = "~/Charts/DataHandlers/CombinationChartDataHandler.ashx?FactoryId=2";

            if(ChartName.Contains("LineDY")) {
                staticChart.DataUrl = "LineDual.xml";
                dynamicChart.DataUrl = "~/Charts/DataHandlers/CombinationChartDataHandler.ashx?FactoryId=2&LineDual=0";
            }
            if(ChartName == "MSStackedColumn2DLineDY") {
                staticChart.DataUrl = "MultiSeriesStackedData.xml";
                dynamicChart.DataUrl = "~/Charts/DataHandlers/MultiSeriesStackedChartDataHandler.ashx?FactoryId=2&LineDual=1";
            }
        }
    }
}