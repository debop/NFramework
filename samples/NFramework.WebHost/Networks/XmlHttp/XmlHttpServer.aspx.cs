using System;
using System.Threading.Tasks;
using System.Web;
using NSoft.NFramework.WebHost.Utils;

namespace NSoft.NFramework.WebHost.Networks.XmlHttp
{
	/// <summary>
	/// NOTE: XML 정보를 POST 방식으로 제공받을 때에는 Page의 ValidateRequest=false로 지정하여야 한다.
	/// </summary>
	public partial class XmlHttpServer : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			// XmlHttpServerUtil.DoAction(HttpContext.Current);

			// NOTE: 비동기 방식으로 수행하려면, Thread가 달라져서, 일치할 수 없는 정보들은 따로 복사해서 보내야한다.
			var context = HttpContext.Current;

			With.TryAction(() => Task.Factory.StartNew(() => XmlHttpServerUtil.DoAction(context)).Wait());
		}
	}
}