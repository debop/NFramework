using System;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.Tools {
    /// <summary>
    /// <see cref="TaskCompletionSource{TResult}"/>에 대한 확장 메소드들입니다.
    /// </summary>
    public static class TaskCompletionSourceTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <paramref name="task"/>의 상태에 따라 <paramref name="resultSetter"/>의 작업 결과를 설정합니다.
        /// </summary>
        /// <typeparam name="TResult">작업 결과의 수형</typeparam>
        /// <param name="resultSetter">작업 결과를 설정할 인스턴스</param>
        /// <param name="task">작업</param>
        public static void SetFromTask<TResult>(this TaskCompletionSource<TResult> resultSetter, Task task) {
            resultSetter.ShouldNotBeNull("resultSetter");
            task.ShouldNotBeNull("task");

            if(IsDebugEnabled)
                log.Debug("resultSetter의 작업결과를 task의 상태에 따라 설정합니다... Task Status=[{0}]", task.Status);

            switch(task.Status) {
                case TaskStatus.RanToCompletion:
                    resultSetter.SetResult(task is Task<TResult> ? ((Task<TResult>)task).Result : default(TResult));
                    break;

                case TaskStatus.Faulted:
                    resultSetter.SetException(task.Exception.InnerExceptions);
                    break;

                case TaskStatus.Canceled:
                    resultSetter.SetCanceled();
                    break;

                default:
                    throw new InvalidOperationException("작업이 완료되지 않았습니다!!!");
            }
        }

        /// <summary>
        /// <paramref name="task"/>의 상태에 따라 <paramref name="resultSetter"/>의 작업 결과를 설정합니다.
        /// </summary>
        /// <typeparam name="TResult">작업 결과의 수형</typeparam>
        /// <param name="resultSetter">작업 결과를 설정할 인스턴스</param>
        /// <param name="task">작업</param>
        public static void SetFromTask<TResult>(this TaskCompletionSource<TResult> resultSetter, Task<TResult> task) {
            SetFromTask(resultSetter, (Task)task);
        }

        /// <summary>
        /// <paramref name="task"/>의 상태에 따라 <paramref name="resultSetter"/>의 작업 결과를 설정을 시도합니다.
        /// </summary>
        /// <typeparam name="TResult">작업 결과의 수형</typeparam>
        /// <param name="resultSetter">작업 결과를 설정할 인스턴스</param>
        /// <param name="task">작업</param>
        /// <returns>설정 성공 여부</returns>
        public static bool TrySetFromTask<TResult>(this TaskCompletionSource<TResult> resultSetter, Task task) {
            resultSetter.ShouldNotBeNull("resultSetter");
            task.ShouldNotBeNull("task");

            if(IsDebugEnabled)
                log.Debug("resultSetter의 작업 결과를 task의 상태에 따라 설정을 시도합니다... task.Status=[{0}]", task.Status);

            switch(task.Status) {
                case TaskStatus.RanToCompletion:
                    return resultSetter.TrySetResult(task is Task<TResult> ? ((Task<TResult>)task).Result : default(TResult));

                case TaskStatus.Faulted:
                    return resultSetter.TrySetException(task.Exception.InnerExceptions);

                case TaskStatus.Canceled:
                    return resultSetter.TrySetCanceled();

                default:
                    throw new InvalidOperationException("작업이 완료되지 않았습니다!!!");
            }
        }

        /// <summary>
        /// <paramref name="task"/>의 상태에 따라 <paramref name="resultSetter"/>의 작업 결과를 설정을 시도합니다.
        /// </summary>
        /// <typeparam name="TResult">작업 결과의 수형</typeparam>
        /// <param name="resultSetter">작업 결과를 설정할 인스턴스</param>
        /// <param name="task">작업</param>
        /// <returns>설정 성공 여부</returns>
        public static bool TrySetFromTask<TResult>(this TaskCompletionSource<TResult> resultSetter, Task<TResult> task) {
            return TrySetFromTask(resultSetter, (Task)task);
        }
    }
}