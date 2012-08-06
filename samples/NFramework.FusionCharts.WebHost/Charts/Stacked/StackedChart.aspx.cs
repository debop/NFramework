using System;

namespace NSoft.NFramework.FusionCharts.WebHost.Charts.Stacked {
    public partial class StackedChart : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack)
                ChartNameList.Items[0].Selected = true;

            ChartName = ChartNameList.SelectedValue;
            Title = "StackedCharts - " + ChartName;

            staticChart.FileName = ChartName + ".swf";
            dynamicChart.FileName = ChartName + ".swf";
        }

        public string ChartName { get; set; }
    }
}