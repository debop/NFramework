using System;

namespace NSoft.NFramework.FusionCharts.WebHost.Widgets.RealTime {
    public partial class RandomData : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack)
                ChartNameList.Items[0].Selected = true;

            ChartName = ChartNameList.SelectedValue;
            Title = "RealTime Charts - " + ChartName;

            staticChart.FileName = ChartName + ".swf";
        }

        protected string ChartName { get; set; }
    }
}