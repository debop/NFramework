using System;

namespace NSoft.NFramework.DataServices.WebHost {
    public partial class Default : System.Web.UI.Page {
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            if(Request.TotalBytes == 0)
                Server.Transfer("~/Help/Usage.htm", true);
            else
                Server.Transfer("~/HttpHandlers/JsonDataService.aspx", true);
        }
    }
}