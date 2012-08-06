using System;

namespace NSoft.NFramework.FusionCharts.WebHost.Widgets.Gantt {
    public partial class GanttSample : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            gantt.DataUrl = "GanttDataProvider.ashx?Option=" + SampleList.SelectedValue;
        }

        protected string ChartName { get; set; }
        protected int ProjectId { get; set; }
    }
}