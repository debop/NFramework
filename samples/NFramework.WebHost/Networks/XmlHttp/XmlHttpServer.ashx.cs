using System.Web;
using System.Web.Services;
using NSoft.NFramework.Web.HttpHandlers;
using NSoft.NFramework.WebHost.Utils;

namespace NSoft.NFramework.WebHost.Networks.XmlHttp
{
	/// <summary>
	/// XmlHttp 통신의 서버 Daemon 역할을 담당하는 Handler입니다.
	/// </summary>
	[WebService(Namespace = "http://ws.realweb21.com/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class XmlHttpServer1 : AbstractHttpAsyncHandler
	{
		protected override void DoProcessRequest(HttpContext context)
		{
			XmlHttpServerUtil.DoAction(context);
		}
	}
}