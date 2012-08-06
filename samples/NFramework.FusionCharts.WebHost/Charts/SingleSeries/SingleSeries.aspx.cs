using System;

namespace NSoft.NFramework.FusionCharts.WebHost.Charts.SingleSeries {
    public partial class SingleSeries : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack)
                ChartNameList.Items[0].Selected = true;

            ChartName = ChartNameList.SelectedValue;
            Title = "Single Series Charts - " + ChartName;

            staticChart.FileName = ChartName + ".swf";
            dynamicChart.FileName = ChartName + ".swf";
        }

        public string ChartName { get; set; }
    }
}