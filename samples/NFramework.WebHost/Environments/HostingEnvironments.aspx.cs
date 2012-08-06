using System;
using System.Text;
using System.Web.Hosting;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.WebHost.Enviroments
{
	public partial class HostingEnvironments : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			var sb = new StringBuilder();

			sb.AppendLine("HostingEnvironment Properties").AppendLine("<hr/>");
			sb.AppendLine("ApplicationHost= " + HostingEnvironment.ApplicationHost);
			sb.AppendFormat("\t SiteName=[{0}], VirtualPath=[{1}]",
			                HostingEnvironment.ApplicationHost.GetSiteName(),
			                HostingEnvironment.ApplicationHost.GetVirtualPath())
				.AppendLine();

			sb.AppendLine("ApplicationPhysicalPath= " + HostingEnvironment.ApplicationPhysicalPath);
			sb.AppendLine("ApplicationVirtualPath= " + HostingEnvironment.ApplicationVirtualPath);
			sb.AppendLine("IsHosted= " + HostingEnvironment.IsHosted);
			sb.AppendLine("ShutdownReason= " + HostingEnvironment.ShutdownReason);
			sb.AppendLine("SiteName= " + HostingEnvironment.SiteName);
			sb.AppendLine("VirtualPathProvider= " + HostingEnvironment.VirtualPathProvider);
			sb.AppendLine("HostName= " + WebTool.HostName);

			txtHostingEnvironment.Text =
				sb
					.Replace(Environment.NewLine, "<BR/>")
					.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;")
					.ToString();
		}
	}
}