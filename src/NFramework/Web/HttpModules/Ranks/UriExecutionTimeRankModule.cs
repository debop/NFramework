using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Web.HttpModules {
    // TODO: 개발중입니다.

    /// <summary>
    /// 요청정보에 대한 처리 시간이 가장 긴 Top N 개를 주기적으로 계산해서, 로그에 쓰고, Application 에 "PagePerformanceRank" 라는 이름으로 목록을 저장합니다.
    /// </summary>
    /// <remarks>
    /// 만들 제품 : 웹 Application에서 실행시간이 긴 Top N 을 알려주는 프로그램
    ///
    /// 1. 형식 : HttpModule
    ///
    /// 2. 평가 방법
    ///
    ///     각 Page별 실행시간의 평균에 대한 순위
    ///
    ///     1. 단순 실행시간 방식 : 모든 페이지의 전체 실행 시간 (BeginRequest~EndRequest),  순수 페이지 실행 시간(PreRequestExecuteHandler ~ PostRequestExecuteHandler) 에 대한 단순 Ranking 선정
    ///     2. Page 요청 빈도에 따른 가중치에 의한 Ranking (이게 더 합리적이지요^^)
    ///
    /// 3. Ranking 정보 배포 방식
    ///
    ///     1. 주기적인 로깅 (Info Level)
    ///     2. HttpApplication 의 특정 변수에 List로 전달 (각자 알아서 사용 가능) ==> 당근 Readonly 로 활용하세요.
    ///
    /// 4. 주의 사항
    ///     1. 웹 서비스나 WCF 서비스는 순수 페이지 실행 시간을 측정할 수 없습니다.
    ///     2. 비동기 방식에 대해서도 측정이 가능하도록 합니다. (Stopwatch가 Local 에 담기도록...)
    /// </remarks>
    [Serializable]
    public class UriExecutionTimeRankModule : IHttpModule {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string UriExecutionRanks = @"NSoft.NFramework.Web.HttpModules.UriExecutionTimeRankModule.UriExecutionRanks";
        public const string PageExecutionRanks = @"NSoft.NFramework.Web.HttpModules.UriExecutionTimeRankModule.PageExecutionRanks";

        /// <summary>
        /// Top N 개의 순위
        /// </summary>
        public static int RankingCount = ConfigTool.GetAppSettings("UriExecutionTimeRank.TopN", 10);

        /// <summary>
        /// 평균이 아닌 실행시간 * 실행 횟수를 순위 값을 사용한다.
        /// </summary>
        public static bool RankByTotalTime = ConfigTool.GetAppSettings("UriExecutionTimeRank.RankByTotalTime", true);

        public static int RankingCapacity = RankingCount * 2;

        /// <summary>
        /// Ranking 정보를 갱신할 요청 갯수 (요쳥 갯수만큼을 받아서 Ranking을 재정렬 해야만 Ranking 정보를 Update합니다.)
        /// </summary>
        public const int RankingUpdateCount = 10;

        private static readonly object _syncLock = new object();

        /// <summary>
        /// BeginRequest ~ EndRequest 전체 실행 시간
        /// </summary>
        private static readonly BlockingCollection<UriExecutionTime> _uriExecutionResults = new BlockingCollection<UriExecutionTime>();

        /// <summary>
        /// PreRequestHandlerExecute ~ PostRequestHandlerExecute 의 순수 Page 실행 시간
        /// </summary>
        private static readonly BlockingCollection<UriExecutionTime> _pageExecutionResults = new BlockingCollection<UriExecutionTime>();

        // 실행 시간 (msec)
        private static readonly ConcurrentDictionary<string, long> _uriExecutionTimes = new ConcurrentDictionary<string, long>();
        private static readonly ConcurrentDictionary<string, long> _pageExecutionTimes = new ConcurrentDictionary<string, long>();

        // 실행 빈도
        private static readonly ConcurrentDictionary<string, int> _uriExecutionFrequency = new ConcurrentDictionary<string, int>();
        private static readonly ConcurrentDictionary<string, int> _pageExecutionFrequency = new ConcurrentDictionary<string, int>();

        private static CancellationTokenSource _cancellationTokenSource;

        public void Init(HttpApplication context) {
            if(log.IsInfoEnabled)
                log.Info("UriExecutionTimeRankModule 인스턴스를 초기화합니다.. Application=[{0}]", HostingEnvironment.ApplicationVirtualPath);

            // Page 처리 성능에 대해, Reporting을 수행합니다.
            RankingExecutionTime();

            context.BeginRequest += BeginRequestHandler;
            context.EndRequest += EndRequestHandler;

            context.PreRequestHandlerExecute += PreRequestHandlerExecuteHandler;
            context.PostRequestHandlerExecute += PostRequestHandlerExecuteHandler;
        }

        public void Dispose() {
            if(_cancellationTokenSource != null) {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }

            if(log.IsInfoEnabled)
                log.Info("UriExecutionTimeRankModule 인스턴스를 종료합니다. Application=[{0}]", HostingEnvironment.ApplicationVirtualPath);
        }

        private static readonly string UriStopwatchKey = Guid.NewGuid().ToString();
        private static readonly string PageStopwatchKey = Guid.NewGuid().ToString();

        private static Stopwatch UriStopwatch {
            get { return (Stopwatch)Local.Data[UriStopwatchKey]; }
            set { Local.Data[UriStopwatchKey] = value; }
        }

        private static Stopwatch PageStopwatch {
            get { return (Stopwatch)Local.Data[PageStopwatchKey]; }
            set { Local.Data[PageStopwatchKey] = value; }
        }

        private static void BeginRequestHandler(object sender, EventArgs e) {
            var context = sender as HttpApplication;
            if(context == null)
                return;

            if(IsDebugEnabled)
                log.Debug("요청에 대한 성능 측정을 시작합니다. Request=[{0}]", context.Request.RawUrl);

            UriStopwatch = new Stopwatch();
            UriStopwatch.Start();
        }

        private static void EndRequestHandler(object sender, EventArgs e) {
            var context = sender as HttpContext;

            if(context == null)
                return;

            if(UriStopwatch == null)
                return;

            UriStopwatch.Stop();
            var result = new UriExecutionTime(context.Request.RawUrl, UriStopwatch.ElapsedMilliseconds);

            // 실행 결과를 Producing 합니다.
            _uriExecutionResults.TryAdd(result);

            if(IsDebugEnabled)
                log.Debug("요청에 대한 성능 측정을 완료했습니다. Request=[{0}], ExecutionTime=[{1}]", result.UriString, result.ExecutionTime);
        }

        private static void PreRequestHandlerExecuteHandler(object sender, EventArgs e) {
            var context = sender as HttpApplication;

            if(context == null)
                return;

            if(IsDebugEnabled)
                log.Debug("Page 처리에 대한 성능 측정을 시작합니다. Request=[{0}]", context.Request.RawUrl);

            PageStopwatch = new Stopwatch();
            PageStopwatch.Start();
        }

        private static void PostRequestHandlerExecuteHandler(object sender, EventArgs e) {
            var context = sender as HttpContext;
            if(context == null)
                return;

            if(PageStopwatch == null)
                return;

            PageStopwatch.Stop();

            var result = new UriExecutionTime(context.Request.RawUrl, PageStopwatch.ElapsedMilliseconds);

            // 실행 결과를 Producing 합니다.
            _pageExecutionResults.TryAdd(result);

            if(IsDebugEnabled)
                log.Debug("Page 처리에 대한 성능 측정을 완료했습니다. Request=[{0}], ExecutionTime=[{1}]", result.UriString, result.ExecutionTime);
        }

        /// <summary>
        /// 요청에 대한 웹 응용프로그램의 처리 시간에 따른 순위를 매긴다.
        /// </summary>
        private static void RankingExecutionTime() {
            if(IsDebugEnabled)
                log.Debug("페이지 성능을 측정하여, 순위를 매기는 작업을 시작합니다...");

            _cancellationTokenSource = new CancellationTokenSource();

            // 전체 요청에 대한 실행 시간에 대한 순위
            Task.Factory.StartNew(DoRankingUriExecutionTime,
                                  _cancellationTokenSource.Token,
                                  _cancellationTokenSource.Token,
                                  TaskCreationOptions.LongRunning,
                                  TaskScheduler.Default);

            // 순수 Page 실행 시간에 대한 순위
            Task.Factory.StartNew(DoRankingPageExecutionTime,
                                  _cancellationTokenSource.Token,
                                  _cancellationTokenSource.Token,
                                  TaskCreationOptions.LongRunning,
                                  TaskScheduler.Default);
        }

        /// <summary>
        /// BeginRequest~EndRequest 실행에 대한 Rank 
        /// </summary>
        /// <param name="state"></param>
        private static void DoRankingUriExecutionTime(object state) {
            var token = (CancellationToken)state;
            var isFirst = true;
            var updateCount = 0;

            foreach(var uriResult in _uriExecutionResults.GetConsumingEnumerable(token)) {
                // 1. 새로운 페이지 처리 결과를 받았으므로, 현재 보관하고 있는 랭킹과 비교해서 처리합니다.
                lock(_syncLock) {
                    UpdateExecutionTime(_uriExecutionTimes, uriResult);
                    UpdateExecutionFrequency(_uriExecutionFrequency, uriResult);
                }

                var needUpdate = ++updateCount > RankingUpdateCount;

                // 2. Update가 필요하다면...
                if(isFirst || needUpdate) {
                    var uriRanks =
                        _uriExecutionTimes
                            .OrderByDescending(item => item.Value)
                            .Take(RankingCount)
                            .Select(item => new UriExecutionTime(item.Key, item.Value))
                            .ToArray();

                    if(IsDebugEnabled)
                        log.Debug("UriExecutionTimeRank=[{0}]", uriRanks.CollectionToString());

                    HttpContext.Current.Application.Lock();
                    try {
                        HttpContext.Current.Application[UriExecutionRanks] = uriRanks;
                    }
                    finally {
                        HttpContext.Current.Application.UnLock();
                    }
                    updateCount = 0;
                }

                if(isFirst)
                    isFirst = false;
            }
        }

        /// <summary>
        /// PreRequestHandlerExecute ~ PostRequestHandlerExecute 즉 순수 Page 실행 시간에 대한 Ranking
        /// </summary>
        /// <param name="state"></param>
        private static void DoRankingPageExecutionTime(object state) {
            var token = (CancellationToken)state;
            bool isFirst = true;
            var updateCount = 0;

            foreach(var pageResult in _pageExecutionResults.GetConsumingEnumerable(token)) {
                // 1. 새로운 페이지 처리 결과를 받았으므로, 현재 보관하고 있는 랭킹과 비교해서 처리합니다.
                lock(_syncLock) {
                    UpdateExecutionTime(_pageExecutionTimes, pageResult);
                    UpdateExecutionFrequency(_pageExecutionFrequency, pageResult);
                }

                var needUpdate = ++updateCount > RankingUpdateCount;

                // 2. 만약 순위 변동이 있다면, Reporting을 수행합니다. )
                if(isFirst || needUpdate) {
                    var pageRanks =
                        _pageExecutionTimes
                            .OrderByDescending(item => item.Value)
                            .Take(RankingCount)
                            .Select(item => new UriExecutionTime(item.Key, item.Value))
                            .ToArray();

                    if(IsDebugEnabled)
                        log.Debug("PageExecutionTimeRank=[{0}]", pageRanks.CollectionToString());

                    HttpContext.Current.Application.Lock();
                    try {
                        HttpContext.Current.Application[PageExecutionRanks] = pageRanks;
                    }
                    finally {
                        HttpContext.Current.Application.UnLock();
                    }

                    updateCount = 0;
                }

                if(isFirst)
                    isFirst = false;
            }
        }

        /// <summary>
        /// 요청 Uri 별 평균 처리 시간을 Update합니다.
        /// </summary>
        /// <param name="executionTimes">실행 결과 순위</param>
        /// <param name="result">새로운 처리 결과</param>
        private static void UpdateExecutionTime(ConcurrentDictionary<string, long> executionTimes, UriExecutionTime result) {
            Func<string, long, long> @updateFunc
                = (key, time) => (RankByTotalTime)
                                     ? time + result.ExecutionTime
                                     : (executionTimes[key] + result.ExecutionTime) / 2;

            executionTimes.AddOrUpdate(result.UriString, result.ExecutionTime, @updateFunc);

            //if(executionTimes.ContainsKey(result.UriString))
            //{
            //    if(RankByTotalTime)
            //        executionTimes[result.UriString] += result.ExecutionTime;
            //    else
            //        executionTimes[result.UriString] = (executionTimes[result.UriString] + result.ExecutionTime) / 2L;
            //}
            //else
            //    executionTimes.Add(result.UriString, result.ExecutionTime);
        }

        /// <summary>
        /// 페이지별 처리 빈도 수를 설정합니다.
        /// </summary>
        /// <param name="frequencies"></param>
        /// <param name="result"></param>
        private static void UpdateExecutionFrequency(ConcurrentDictionary<string, int> frequencies, UriExecutionTime result) {
            frequencies.AddOrUpdate(result.UriString, 1, (key, value) => value + 1);
        }
    }
}