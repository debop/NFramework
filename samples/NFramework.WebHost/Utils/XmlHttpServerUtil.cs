using System.IO;
using System.Web;
using NSoft.NFramework.Networks;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Xml;

namespace NSoft.NFramework.WebHost.Utils
{
	/// <summary>
	/// 테스트용으로, Xml Http 로 요청이 온 것을 응답한다.
	/// </summary>
	public static class XmlHttpServerUtil
	{
		#region << logger >>

		private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

		private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public static void DoAction(HttpContext context)
		{
			var request = context.Request;
			var response = context.Response;

			// string action = ConvertTool.DefValue(request["Action"], XmlHttpMethods.GetXml);
			string action = request["Action"].AsText(XmlHttpMethods.GetXml);

			if(IsDebugEnabled)
				log.Debug("Action = " + action);

			response.Clear();

			switch(action.ToUpper())
			{
				case "GETXML":
					response.ContentType = "text/xml";
					response.Write("<GetXml><Action>GetXml</Action></GetXml>");
					break;

				case "POSTTEXT":
					response.ContentType = "text/plain";
					string payload = request.Form.ToString();
					response.Write("Action=" + XmlHttpMethods.PostText);
					response.Write("PostText with payload= " + payload);
					break;

				case "POSTXML":
					response.ContentType = "text/xml";
					using(var reader = new StreamReader(request.InputStream))
					{
						// string으로 변환시에는 null terminator를 제거한다.
						string xmlText = reader.ReadToEnd().TrimEnd('\0');

						if(IsDebugEnabled)
							log.Debug("PostXml action with form data=[{0}]", xmlText);

						if(xmlText.IsWhiteSpace())
							xmlText = "<PostXml>Input stream is empty.</PostXml>";

						XmlDoc doc = XmlTool.CreateXmlDocument(xmlText);

						if(IsDebugEnabled)
							log.Debug("Action:PostXml, returns: " + doc.Xml);

						//response.Clear();
						doc.Save(response.OutputStream);
						// response.Write(doc.Xml);
					}
					break;

				case "GETTEXT":
				default:
					response.ContentType = "text/plain";
					response.Write("Action = " + XmlHttpMethods.GetText);
					response.Write("; QueryParams = " + request.QueryString);
					break;
			}
		}
	}
}