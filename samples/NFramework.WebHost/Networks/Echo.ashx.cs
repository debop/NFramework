using System;
using System.Web;
using System.Web.Services;

namespace NSoft.NFramework.WebHost.Networks
{
	/// <summary>
	/// $codebehindclassname$의 요약 설명입니다.
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class Echo1 : IHttpAsyncHandler
	{
		#region << logger >>

		private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
		private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

		#endregion

		private delegate void AsyncProcessContextDelegate(HttpContext context);

		private AsyncProcessContextDelegate _delegate;

		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
		{
			if(IsDebugEnabled)
				log.Debug("비동기 에코 실행을 수행합니다...");

			_delegate = new AsyncProcessContextDelegate(RunEcho);

			return _delegate.BeginInvoke(context, cb, extraData);
		}

		public void EndProcessRequest(IAsyncResult result)
		{
			_delegate.EndInvoke(result);

			if(IsDebugEnabled)
				log.Debug("비동기 에코 실행을 완료했습니다!!!");
		}

		public void ProcessRequest(HttpContext context)
		{
			RunEcho(context);
		}

		public bool IsReusable
		{
			get { return false; }
		}

		private static void RunEcho(HttpContext context)
		{
			HttpRequest request = context.Request;
			HttpResponse response = context.Response;

			if(IsDebugEnabled)
				log.Debug("Echo.ashx Request type is " + request.RequestType);

			response.Clear();
			response.ContentType = "text/plain";
			// response.ContentEncoding = Encoding.UTF8;

			response.Write("RequestType is " + request.RequestType + "\r\n");
			response.Write("");

			switch(request.RequestType.ToUpper())
			{
				case System.Net.WebRequestMethods.Http.Get:
					foreach(string name in request.QueryString.AllKeys)
					{
						response.Write(string.Format("{0} = {1}\r\n", name, request.QueryString[name]));
					}
					response.Write("");
					response.Write("RawData : " + request.QueryString);
					break;

				case System.Net.WebRequestMethods.Http.Post:

					foreach(string name in request.Form.AllKeys)
					{
						response.Write(string.Format("{0} = {1}\r\n", name, request.Form[name]));
					}
					response.Write("---------- Start of RawData -------------");
					response.Write(string.Format("RawData : {0}", request.Form));
					response.Write("---------- End of RawData -------------");
					break;

				default:
					response.Write(request.InputStream);
					break;
			}

			response.Write("");
			response.Write("End Test.");

			if(IsDebugEnabled)
				log.Debug("Echo Process End.");
		}
	}
}