using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Parallelism.Tools {
    /// <summary>
    /// <see cref="AggregateException"/>을 위한 확장 메소드 들입니다.
    /// </summary>
    public static class AggregateExceptionTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <see cref="AggregateException"/>이 가진 모든 Exception에 대해 handler를 호출합니다.
        /// </summary>
        /// <param name="aggregateException"></param>
        /// <param name="predicate"></param>
        /// <param name="leaveStructureIntact">AggregateException의 내부 Exception이 AggregateException인 경우, 재귀호출을 통해 점검할 것인가 여부</param>
        public static void Handle(this AggregateException aggregateException, Func<Exception, bool> predicate = null,
                                  bool leaveStructureIntact = false) {
            aggregateException.ShouldNotBeNull("aggregateException");
            predicate = predicate ?? (_ => true);

            if(IsDebugEnabled)
                log.Debug("AggregateException의 모든 Exception에 대해 Handler를 호출합니다. leaveStructureIntact=[{0}]", leaveStructureIntact);

            if(leaveStructureIntact) {
                var result = HandleRecursively(aggregateException, predicate);
                if(result != null)
                    throw result;
            }
            else
                aggregateException.Handle(predicate);
        }

        /// <summary>
        /// AggregateException 의 내부 Exception이 AggregateException인 경우, 재귀호출을 통해, Exception만을 <paramref name="predicate"/>를 통해 실행시킨다.
        /// </summary>
        /// <param name="aggregateException"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private static AggregateException HandleRecursively(this AggregateException aggregateException,
                                                            Func<Exception, bool> predicate = null) {
            predicate = predicate ?? (_ => true);

            var innerExceptions = new List<Exception>();

            foreach(var inner in aggregateException.InnerExceptions) {
                var innerAsAggregate = inner as AggregateException;

                // 내부 Exception이 AggregateException인 경우
                if(innerAsAggregate != null) {
                    AggregateException newChildAggregate = HandleRecursively(innerAsAggregate, predicate);

                    if(newChildAggregate != null) {
                        if(innerExceptions.Count > 0)
                            innerExceptions.Clear();
                        innerExceptions.Add(newChildAggregate);
                    }
                }
                else if(predicate(inner) == false) {
                    if(innerExceptions.Count > 0)
                        innerExceptions.Clear();
                    innerExceptions.Add(inner);
                }
            }

            return innerExceptions.Count > 0 ? new AggregateException(aggregateException.Message, innerExceptions) : null;
        }
    }
}