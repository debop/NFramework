using System.Web;
using System.Web.Services;

namespace NSoft.NFramework.WebHost.Networks
{
	/// <summary>
	/// $codebehindclassname$의 요약 설명입니다.
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class Echo21 : IHttpHandler
	{
		#region << logger >>

		private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
		private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

		#endregion

		public void ProcessRequest(HttpContext context)
		{
			RunEcho(context);
		}

		public bool IsReusable
		{
			get { return false; }
		}

		private void RunEcho(HttpContext context)
		{
			HttpRequest request = context.Request;
			HttpResponse response = context.Response;

			if(IsDebugEnabled)
				log.Debug("Echo2.ashx Request type is " + request.RequestType);

			response.Clear();

			response.ContentType = "text/html";

			if(request.Form["postparam"] == null)
			{
				response.Write("postparam값이 없습니다.<p>");
			}
			else
			{
				response.Write("postparam값을 얻었습니다.<p>");
				string post = request.Form["postparam"];
				response.Write(post);
			}

			response.Write("<HR>Post값 출력</HR>");

			int cnt = 1;
			foreach(string name in request.Form)
			{
				response.Write(string.Format("{0}.{1}:{2}<br/>", cnt, name, request.Form[name]));
				cnt++;
			}

			response.End();

			if(IsDebugEnabled)
				log.Debug("Echo2.ashx Process End.");
		}
	}
}