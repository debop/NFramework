using System;

namespace NSoft.NFramework.FusionCharts.WebHost.Charts.MultiSeries {
    public partial class MultiSeriesChart : System.Web.UI.Page {
        public virtual string ChartName { get; set; }

        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack)
                ChartNameList.Items[0].Selected = true;

            ChartName = ChartNameList.SelectedValue;
            Title = "Multi Series Charts - " + ChartName;

            staticChart.FileName = ChartName + ".swf";
            dynamicChart.FileName = ChartName + ".swf";
        }
    }
}