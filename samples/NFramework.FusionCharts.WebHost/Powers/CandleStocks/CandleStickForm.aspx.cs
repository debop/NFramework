using System;

namespace NSoft.NFramework.FusionCharts.WebHost.Powers.CandleStocks {
    public partial class CandleStickForm : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            candleStick.DataUrl = "CandleStickDataProvider.ashx?Option=" + dataOptions.SelectedValue;
        }
    }
}