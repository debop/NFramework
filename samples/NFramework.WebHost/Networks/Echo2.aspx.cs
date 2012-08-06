using System;

namespace NSoft.NFramework.WebHost.Networks
{
	public partial class Echo2 : System.Web.UI.Page
	{
		#region << logger >>

		private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
		private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if(IsDebugEnabled)
				log.Debug("Echo2.aspx Request type is " + Request.RequestType);

			Response.Clear();

			Response.ContentType = "text/html";

			if(Request.Form["postparam"] == null)
			{
				Response.Write("postparam값이 없습니다.<p>");
			}
			else
			{
				Response.Write("postparam값을 얻었습니다.<p>");
				string post = Request.Form["postparam"];
				Response.Write(post);
			}

			Response.Write("<HR>Post값 출력</HR>");

			int cnt = 1;
			foreach(string name in Request.Form)
			{
				Response.Write(string.Format("{0}.{1}:{2}<br/>", cnt, name, Request.Form[name]));
				cnt++;
			}

			Response.Write("End Test.");

			if(IsDebugEnabled)
				log.Debug("Echo2.aspx Process End.");
		}
	}
}