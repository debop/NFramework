using System;

namespace NSoft.NFramework.XmlData.WebHost {
    public partial class _Default : System.Web.UI.Page {
        protected override void OnPreLoad(EventArgs e) {
            base.OnPreLoad(e);

            if(Request.TotalBytes == 0)
                Server.Transfer("Usage.htm", true);
            else
                Server.Transfer("~/XmlDataService.aspx", true);
        }
    }
}