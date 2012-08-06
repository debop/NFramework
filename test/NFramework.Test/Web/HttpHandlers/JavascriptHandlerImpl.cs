
#if EXPERIMENTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NSoft.NFramework.Core
{
	public class JavascriptHandlerImpl : AbstractHttpHandler
	{
		#region Overrides of AbstractHttpHandler

		/// <summary>
		/// 요청을 처리하는 기본 구현 함수
		/// </summary>
		/// <param name="context"></param>
		public override void OnProcessRequest(HttpContextBase context)
		{
			string content = GetContent();

			var response = context.Response;
			response.ContentType = "application/x-javascript";
			response.Write(content);
		}

		#endregion


		protected virtual string GetContent()
		{
			return "YOUR JAVA SCRIPT FILE CONTENT"; 
		}
	}
}
#endif