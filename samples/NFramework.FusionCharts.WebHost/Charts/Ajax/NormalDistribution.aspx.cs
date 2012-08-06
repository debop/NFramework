using System;
using System.Text;

namespace NSoft.NFramework.FusionCharts.WebHost.Charts.Ajax {
    public partial class NormalDistribution : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack)
                DrawNormalDistribution(-5, 5, 0.2, new[] { 0.0 }, new[] { 1.0 });
        }

        protected void btnCalc_Click(object sender, EventArgs e) {
            double avg = edAvg.Text.AsDouble(0.0);
            double stDev = edStDev.Text.AsDouble(1.0);

            double min = avg + -3.0 * stDev;
            double max = avg + 3.0 * stDev;
            double step = (max - min) / 100;

            DrawNormalDistribution(min, max, step, new[] { avg, avg / 2.0 + 0.1 * stDev }, new[] { stDev, stDev * 0.95 });
        }

        protected void DrawNormalDistribution(double min, double max, double step, double[] avgs, double[] stDevs) {
            var nds = new StringBuilder();
            for(int i = 0; i < avgs.Length; i++)
                nds.AppendFormat("&ND{0}={1},{2}", i, avgs[i], stDevs[i]);

            normalDistributionChart.DataUrl = string.Format("NormalDistributionHandler.ashx?Min={0}&Max={1}&Step={2}{3}", min, max, step,
                                                            nds);
        }
    }
}