using System.Web;
using NSoft.NFramework.Web.HttpHandlers;

namespace NSoft.NFramework.WebHost.Async
{
	/// <summary>
	/// AsyncEcho의 요약 설명입니다.
	/// </summary>
	public class AsyncEcho : AbstractHttpAsyncHandler
	{
		/// <summary>
		/// 비동기 방식의 HttpHandler의 Main 함수
		/// </summary>
		/// <param name="context"></param>
		protected override void DoProcessRequest(HttpContext context)
		{
			base.DoProcessRequest(context);

			WriteEcho(context);
		}

		/// <summary>
		/// 사용자 구현 함수
		/// </summary>
		/// <param name="context"></param>
		private static void WriteEcho(HttpContext context)
		{
			var response = context.Response;

			response.ContentType = "text/plain";

			response.Write("Hello World.");
			response.Write("Hello World.");
			response.Write("Hello World.");
			response.Write("Hello World.");
			response.Write("Hello World.");
			response.Write("Hello World.");
		}
	}
}