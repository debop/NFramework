
#if EXPERIMENTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace NSoft.NFramework.Web.HttpModules
{
	/// <summary>
	/// 웹 Application을 만들지 않고도,  Mocking을 통해 HttpModule을 테스트 할 수 있음을 보여주는 예제용 HttpModule.
	/// </summary>
	public class RemoveW3HttpModuleImpl : NSoft.NFramework.Web.HttpModules.AbstractHttpModule
	{
		private const string Prefix = "http://www.";

		public override void OnBeginRequest(System.Web.HttpContextBase context)
		{
			string url = context.Request.Url.ToString();
			bool startsWithW3 = url.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase);

			if (startsWithW3)
			{
				string newUrl = "http://" + url.Substring(Prefix.Length);

				var response = context.Response;

				response.StatusCode = (int)HttpStatusCode.MovedPermanently;
				response.Status = "301 Moved Permanently";
				response.RedirectLocation = newUrl;
				response.SuppressContent = true;
				response.End();
			}
		}
	}
}
#endif