using System;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.SessionState;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Web.HttpApplications;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.WebHost
{
	public class DefaultHttpApplication : WindsorAsyncHttpApplication
	{
		#region << logger >>

		private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
		private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
		private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

		#endregion

		protected override void OnBeginRequest(object sender, EventArgs e)
		{
			base.OnBeginRequest(sender, e);

			// 비동기 방식 처리를 위한 Task 내에서 정보가 유지되는 확인하기 위해서 
			HttpContext.Current.Items["ProductName"] = "NSoft.NFramework";

			if(IsDebugEnabled)
				log.Debug("ProductName=" + HttpContext.Current.Items["ProductName"]);
		}

		/// <summary>
		/// Application_Start 시에 실행할 비동기 작업의 본체입니다.
		/// </summary>
		protected override void ApplicationStartAfter(HttpContext context)
		{
			base.ApplicationStartAfter(context);

			var appPath = context.GetApplicationRootPath();

			// 여기에 비동기 작업을 정의하면 됩니다.
			// 메일을 보낸다던지 하더는 일...

			Task.Factory
				.StartNew(() =>
				          new SmtpClient("mail.realweb21.com")
				          	.Send("debop@realweb21.com",
				          	      "debop@realweb21.com",
				          	      "웹 응용프로그램 시작 시 메일보내기",
				          	      string.Concat(appPath, " 가 시작했습니다...  Machine=", Server.MachineName)))
				.ContinueWith(task =>
				              {
				              	if(task.IsFaulted)
				              		if(log.IsErrorEnabled)
				              			log.ErrorException("알림 메일 발송에 실패했습니다.", task.Exception);

				              	if(task.IsCompleted)
				              		if(IsDebugEnabled)
				              			log.Debug("알림 메일을 발송했습니다!!!");
				              },
				              TaskContinuationOptions.LongRunning);


			Task.Factory
				.StartNew(() =>
				          With.TryAction(() => AppDomain.CurrentDomain
				                               	.GetAssemblies()
				                               	.RunEach(asm => log.Debug("Assembly=[{0}]", asm))),
				          TaskCreationOptions.LongRunning);
		}

		public override void Application_Start(object sender, EventArgs e)
		{
			base.Application_Start(sender, e);

			var targetUriString = WebTool.ServerName + WebTool.AppPath;

			// 1분마다 자신의 Application을 호출하는 Background Thread를 생성합니다.
			//
			//Task.Factory
			//    .StartNew((uriString) =>
			//              {
			//                  while(true)
			//                  {
			//                      try
			//                      {
			//                          using(var webClient = new WebClient())
			//                          {
			//                              if(IsDebugEnabled)
			//                                  log.Debug("Self Activation을 수행을 시작합니다... uriString=" + uriString);

			//                              var task = webClient.DownloadStringTask((string)uriString);
			//                              task.WaitAsync(TimeSpan.FromSeconds(60));

			//                              if(IsDebugEnabled)
			//                                  log.Debug("Self Activation을 수행을 완료했습니다!!! uriString=" + uriString);
			//                          }
			//                      }
			//                      catch(Exception ex)
			//                      {
			//                          if(log.IsWarnEnabled) 
			//							{
			//                              log.Warn("Self Activation을 통한 프로그램 다운을 막는데 실패했습니다. uriString=[{0}]", uriString);
			//								log.Warn(ex);
			//							}
			//                      }

			//                      Thread.Sleep(60 * 1000);
			//                  }
			//              },
			//              targetUriString,
			//              TaskCreationOptions.LongRunning);
		}

		public override void Session_Start(object sender, EventArgs e)
		{
			base.Session_Start(sender, e);

			if(Session.Mode == SessionStateMode.Off)
				return;

			if(IsInfoEnabled)
				log.Info("세션이 시작되었습니다.  SessionID=[{0}]", Session.SessionID);

			Session["UserId"] = Guid.NewGuid().ToString();

			// 엄청 큰 데이타? (SharedCacheSessionStateStoreProvider 테스트용
			//Session["UserData"] = ArrayTool.GetRandomBytes(2048000);
		}

		public override void Session_End(object sender, EventArgs e)
		{
			if(Session.Mode == SessionStateMode.Off)
				return;

			var userId = (string) Session["UserId"];

			userId.ShouldNotBeWhiteSpace("userId");

			if(IsInfoEnabled)
				log.Info("세션이 종료되었습니다. SessionID=[{0}]", Session.SessionID);

			base.Session_End(sender, e);
		}

		/// <summary>
		/// Application_Error 시에 비동기적으로 실행할 작업입니다. (기본적으로는 로그에 쓰는 작업을 합니다)
		/// overriding 시 base method를 호출해 주셔야합니다.
		/// </summary>
		/// <param name="context">현재 Context 정보</param>
		/// <param name="exception">Web Applicatoin 예외 정보</param>
		protected override void ApplicationErrorAfter(HttpContext context, Exception exception)
		{
			base.ApplicationErrorAfter(context, exception);

			var appPath = context.GetApplicationRootPath();

			// 예외 발생 시 관리자에게 메일을 보냅니다.
			Task.Factory
				.StartNew(() =>
				          new SmtpClient("mail.realweb21.com")
				          	.Send("debop@realweb21.com",
				          	      "debop@realweb21.com",
				          	      appPath ?? HostingEnvironment.ApplicationVirtualPath + " 에서 예외가 발생했습니다.",
				          	      "예외 정보" + Environment.NewLine + exception))
				.ContinueWith(task =>
				              {
				              	if(task.IsFaulted)
				              		if(log.IsErrorEnabled)
				              		{
				              			log.Error("예외에 대한 메일 발송에 실패했습니다.");
				              			log.Error(task.Exception);
				              		}

				              	if(task.IsCompleted)
				              		if(IsDebugEnabled)
				              			log.Debug("예외에 대한 메일을 발송했습니다!!!");
				              },
				              TaskContinuationOptions.ExecuteSynchronously);
		}
	}
}