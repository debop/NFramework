using System;
using System.Net;

namespace NSoft.NFramework.WebHost.Networks
{
	public partial class Echo : System.Web.UI.Page
	{
		#region << logger >>

		private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
		private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if(IsDebugEnabled)
				log.Debug("Echo.aspx page loading... Request Type= " + Request.RequestType);

			Response.Clear();
			Response.ContentType = @"text/plain";

			Response.Write("RequestType is " + Request.RequestType + Environment.NewLine);

			switch(Request.RequestType.ToUpper())
			{
				case WebRequestMethods.Http.Get:

					foreach(var name in Request.QueryString.AllKeys)
					{
						Response.Write(name + " = " + Request.QueryString[name]);
						Response.Write(Environment.NewLine);
					}
					Response.Write("");
					Response.Write("RawData = " + Request.QueryString);
					break;

				case WebRequestMethods.Http.Post:

					foreach(var name in Request.Form.AllKeys)
					{
						Response.Write(string.Format("{0} = {1}\r\n", name, Request.Form[name]));
					}
					Response.Write("---------- Start of RawData -------------");
					Response.Write("RawData = " + Request.Form);
					Response.Write("---------- End of RawData -------------");
					break;

				default:

					Response.Write(Request.InputStream);
					break;
			}

			Response.Write(Environment.NewLine);
			Response.Write("End Test.");

			if(IsDebugEnabled)
				log.Debug("Echo.aspx processing FINISHED.");
		}
	}
}