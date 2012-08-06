using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace NSoft.NFramework.Web.HttpModules {
    /// <summary>
    /// 비동기적으로, 웹 응용프로그램에 대한 요청 정보를 로그합니다.
    /// </summary>
    [Serializable]
    public class AsyncAccessLogModule : IHttpModule {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private const string AsyncAccessLogModuleKey = "NSoft.NFramework.Web.HttpModules.AsyncAccessLogModule.Key";

        /// <summary>
        /// 모듈 초기화
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context) {
            if(log.IsInfoEnabled)
                log.Info("AsyncAccessLogModule 모듈을 초기화합니다. 단 logger의 LEVEL이 DEBUG가 가능할 때만 합니다. Application=[{0}]",
                         HostingEnvironment.ApplicationVirtualPath);

            if(IsDebugEnabled) {
                context.BeginRequest += OnBeginRequest;
                context.EndRequest += OnEndRequest;
            }
        }

        public void Dispose() {
            if(log.IsInfoEnabled)
                log.Info("AsyncAccessLogModule 을 메모리에서 제거합니다. Application=[{0}]", HostingEnvironment.ApplicationVirtualPath);
        }

        /// <summary>
        /// Log message format that used Before Request Executing
        /// </summary>
        private const string BeginRequestLogFormat = "웹 요청을 받았습니다. UserAddress=[{0}], Method=[{1}], ScriptPath=[{2}]";

        /// <summary>
        /// Log message format that used After Request Executing
        /// </summary>
        private const string EndRequestLogFormat =
            "웹 요청을 처리했습니다. UserAddress=[{0}], Method=[{1}], ScriptPath=[{2}], Response Status=[{3}]";

        private static void OnBeginRequest(object sender, EventArgs e) {
            var context = (HttpApplication)sender;

            // TaskCreationOptions.PreferFairness 를 지정해야, StartNew() 메소드를 바로 시작한다.
            var stopwatchTask
                = new Task<Lazy<double>>(() => {
                                             var sw = new Stopwatch();
                                             sw.Start();

                                             if(IsDebugEnabled) {
                                                 var request = context.Request;
                                                 log.Debug(BeginRequestLogFormat,
                                                           request.UserHostAddress,
                                                           request.RequestType,
                                                           request.CurrentExecutionFilePath);
                                             }

                                             // Lazy 값을 처음 호출할 때, stop watch가 끝나고, 경과 값을 반환한다.
                                             return new Lazy<double>(() => {
                                                                         sw.Stop();
                                                                         return sw.ElapsedMilliseconds;
                                                                     });
                                         });
            stopwatchTask.Start();

            Local.Data[AsyncAccessLogModuleKey] = stopwatchTask;
        }

        private static void OnEndRequest(object sender, EventArgs e) {
            var task = Local.Data[AsyncAccessLogModuleKey] as Task<Lazy<double>>;

            if(task == null)
                return;

            var elapsedMilliseconds = task.Result.Value;

            if(IsDebugEnabled) {
                var context = (HttpApplication)sender;
                var request = context.Request;
                var response = context.Response;

                log.Debug(EndRequestLogFormat,
                          request.UserHostAddress,
                          request.RequestType,
                          request.CurrentExecutionFilePath,
                          response.Status);

                log.Debug("Page Processing tooks [{0}] msecs", elapsedMilliseconds);
            }
        }
    }
}