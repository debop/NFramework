using System;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.WebHost.Utils
{
	public partial class WebUtilsTestPage : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			lblUrl.Text = WebTool.GetScriptPath("~/Utils/WebUtilsTestPage.aspx");

			lblUrl.Text += "<BR/>ServerName=" + WebTool.ServerName;
		}
	}
}