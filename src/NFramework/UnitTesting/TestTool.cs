using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.Parallelism.DataStructures;

namespace NSoft.NFramework.UnitTesting {
    /// <summary>
    /// 단위 테스트 시 필요한 함수들 (대부분 멅티스레드로 테스트를 수행합니다)
    /// </summary>
    public static class TestTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 지정된 Action을 지정된 횟수만큼 병렬로 실행합니다. 멀티스레드 테스트가 가능합니다.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="action">수행할 델리게이트</param>
        public static void RunTasks(int count, Action @action) {
            Parallel.For(0, count, i => action());
        }

        /// <summary>
        /// 지정된 Action들을 지정된 횟수만큼 병렬 방식으로 실행합니다. 멀티스레드 테스트가 가능합니다.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="actions"></param>
        public static void RunTasks(int count, params Action[] @actions) {
            //var task = Task.Factory.ContinueWithActions(TaskContinuationOptions.ExecuteSynchronously, @actions);

            Parallel.For(0,
                         count,
                         i => Parallel.ForEach(@actions, @action => @action()));
        }

        /// <summary>
        /// <paramref name="actions"/>를 지정한 범위만큼 수행합니다.
        /// </summary>
        /// <param name="fromInclusive"></param>
        /// <param name="toExclusive"></param>
        /// <param name="actions"></param>
        public static void RunTasks(int fromInclusive, int toExclusive, Action<int> @actions) {
            Parallel.For(fromInclusive, toExclusive, @actions);
        }

        /// <summary>
        /// <paramref name="actions"/>를 지정된 범위만큼 수행합니다.
        /// </summary>
        /// <param name="fromInclusive"></param>
        /// <param name="toExclusive"></param>
        /// <param name="actions"></param>
        public static void RunTasks(int fromInclusive, int toExclusive, params Action<int>[] @actions) {
            Parallel.For(fromInclusive,
                         toExclusive,
                         i => Parallel.ForEach(@actions, @action => @action(i)));
        }

        /// <summary>
        /// <paramref name="actions"/>를 <paramref name="count"/> 수 만큼 순서대로 수행합니다.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="actions"></param>
        /// <seealso cref="SerialTaskQueue"/>
        public static void RunTaskQueue(int count, params Action[] actions) {
            Parallel.For(0,
                         count,
                         i => {
                             var serialTaskQueue = new SerialTaskQueue();

                             foreach(var @action in actions) {
                                 var @localAction = @action;
                                 serialTaskQueue.Enqueue(new Task(@localAction));
                             }

                             serialTaskQueue.Completed().Wait();
                         });
        }

        /// <summary>
        /// 지정된 Thread 메소드에 대해, 갯수 만큼 Thread 를 만들어서 실행시킨다. (ThreadPool 을 사용하는게 아니므로, 
        /// 좋은 방법은 아닙니다. <see cref="TestTool.RunTasks(int,System.Action)"/>을 사용하세요.
        /// </summary>
        /// <param name="testMethod">실행할 메소드를 ThreadStart로 delegate 한 메소드</param>
        /// <param name="threadCount">수행할 횟수</param>
        /// <example>
        /// <code>
        ///		TestTool.ThreadStress(new ThreadStart(WorkMethod), 100);
        /// 
        /// 
        ///		...
        /// 
        ///		public void WorkMethod()
        ///     {
        ///           // something to work
        ///           ...
        ///		}
        /// </code>
        /// </example>
        public static void ThreadStress(this ThreadStart testMethod, int threadCount = 4) {
            testMethod.ShouldNotBeNull("testMethod");
            threadCount.ShouldBePositive("threadCount");
            //if (threadCount <= 0)
            //    throw new ArgumentOutOfRangeException("threadCount", "threadCount should be greater than 0.");

            if(IsDebugEnabled)
                log.Debug("스트레스 테스트를 위해 멀트 스레드를 실행합니다... testMethod=[{0}], threadCount=[{1}]", testMethod.Method.Name, threadCount);

            IList<Thread> threads = new List<Thread>();

            for(var i = 0; i < threadCount; i++)
                threads.Add(new Thread(testMethod));

            threads.ThreadStress();
        }

        /// <summary>
        /// 지정된 Thread 컬렉션들을 모두 실행시킵니다.
        /// </summary>
        /// <param name="threads"></param>
        /// <example>
        /// <code>
        ///     List{Thread} threads = new List{Thread}();
        ///     for(int i=0;i &lt; 100; i++)
        ///         threads.Add(new Thread(new ThreadStart(WorkMethod)));
        /// 
        ///		TestTool.ThreadStress(threads);
        /// 
        ///		...
        /// 
        ///		public void WorkMethod()
        ///     {
        ///           // something to work
        ///           ...
        ///		}
        /// </code>
        /// </example>
        public static void ThreadStress(this IList<Thread> threads) {
            foreach(var thread in threads) {
                if(thread.ThreadState == ThreadState.Unstarted)
                    thread.Start();
            }

            Thread.Sleep(0);

            foreach(var thread in threads)
                thread.Join();
        }

        /// <summary>
        /// <see cref="ThreadPool"/> 에 callback 을 지정된 갯수만큼 넣고 실행시킵니다. 일반적으로 Thread 컬렉션보다 성능이 좋습니다.
        /// </summary>
        /// <param name="maxThreads"></param>
        /// <param name="workerThreads"></param>
        /// <param name="callback"></param>
        public static void RunThread(int maxThreads, int workerThreads, WaitCallback callback = null) {
            ThreadPool.SetMinThreads(workerThreads, workerThreads);

            for(var i = 0; i < maxThreads; i++) {
                ThreadPool.QueueUserWorkItem(callback);
            }
        }

#if !SILVERLIGHT

        /// <summary>
        /// <see cref="ThreadPool"/>을 이용하여, 지정한 callback 함수를 지정한 갯수만큼 수행합니다.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="count"></param>
        public static void ThreadStressByThreadPool(WaitCallback callback, int count = 4) {
            callback.ShouldNotBeNull("callback");

            using(var countdown = new CountdownEvent(count)) {
                for(var i = 0; i < count; i++)
                    ThreadPool.QueueUserWorkItem(state => {
                                                     callback(state);
                                                     countdown.Signal();
                                                 });

                countdown.Wait();
            }
        }

        /// <summary>
        /// <see cref="ThreadPool"/>을 이용하여, 지정된 callback 함수를 지정된 state 인자값 수만큼 실행합니다.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="states"></param>
        public static void ThreadStressByThreadPool(WaitCallback callback, IEnumerable<object> states) {
            callback.ShouldNotBeNull("callback");
            states.ShouldNotBeNull("states");

            var stateArray = states.ToArray();

            using(var countdown = new CountdownEvent(stateArray.Length)) {
                foreach(var state in stateArray)
                    ThreadPool.QueueUserWorkItem(arg => {
                                                     callback(arg);
                                                     countdown.Signal();
                                                 },
                                                 state);

                countdown.Wait();
            }
        }

        /// <summary>
        /// <see cref="ThreadPool"/>을 이용하여, 지정한 callback 함수들을 지정한 갯수만큼 수행합니다.
        /// </summary>
        /// <param name="callbacks">수행할 callback 함수들</param>
        /// <param name="count">callback 함수 수행 횟수</param>
        public static void ThreadStressByThreadPool(IEnumerable<WaitCallback> callbacks, int count = 4) {
            callbacks.ShouldNotBeEmpty("callbacks");
            count.ShouldBePositive("count");

            var callbackArray = callbacks.ToArray();

            using(var countdown = new CountdownEvent(count * callbackArray.Length)) {
                for(var i = 0; i < count; i++)
                    foreach(var callback in callbackArray) {
                        var callbackLocal = callback;
                        ThreadPool.QueueUserWorkItem(state => {
                                                         callbackLocal(state);
                                                         countdown.Signal();
                                                     });
                    }
                countdown.Wait();
            }
        }
#endif
    }
}