using System;

namespace NSoft.NFramework {
    public static partial class With {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 지정된 Action을 수행합니다.
        /// </summary>
        /// <param name="tryAction">수행할 action</param>
        /// <param name="exceptionAction">예외 시 수행할 action</param>
        /// <param name="finallyAction">뒷처리를 위한 action</param>
        public static void TryAction(Action tryAction, Action<Exception> exceptionAction = null, Action finallyAction = null) {
            tryAction.ShouldNotBeNull("tryAction");

            try {
                if(tryAction != null)
                    tryAction();
            }
            catch(Exception ex) {
                if(exceptionAction != null)
                    exceptionAction(ex);
                else {
                    if(log.IsWarnEnabled) {
                        log.Warn("작업 중에 예외가 발생했습니다만 무시합니다.");
                        log.Warn(ex);
                    }
                }
            }
            finally {
                if(finallyAction != null)
                    finallyAction();
            }
        }

        /// <summary>
        /// 지정된 Function을 수행합니다.
        /// </summary>
        /// <param name="tryFunc">수행할 action</param>
        /// <param name="exceptionAction">예외 시 수행할 action</param>
        /// <param name="finallyAction">뒷처리를 위한 action</param>
        /// <param name="valueFactory">예외 시 반환할 기본 값을 생성하는 Factory</param>
        public static T TryFunction<T>(Func<T> tryFunc, Func<T> valueFactory = null, Action<Exception> exceptionAction = null,
                                       Action finallyAction = null) {
            tryFunc.ShouldNotBeNull("tryFunc");

            try {
                return tryFunc();
            }
            catch(Exception ex) {
                if(exceptionAction != null)
                    exceptionAction(ex);
                else {
                    if(log.IsWarnEnabled) {
                        log.Warn("작업 중에 예외가 발생했습니다만 무시합니다.");
                        log.Warn(ex);
                    }
                }
            }
            finally {
                if(finallyAction != null)
                    finallyAction();
            }
            return (valueFactory != null) ? valueFactory() : default(T);
        }

        /// <summary>
        /// 비동기 방식의 작업을 수행할 때 예외처리를 담당해 줍니다. 비동기 방식 작업을 수행할 때에는 이 함수를 호출하시기 바랍니다.
        /// </summary>
        /// <param name="asyncAction">비동기 방식 작업을 수행하는 Action</param>
        /// <param name="ageAction">예외 처리를 담당하는 Action</param>
        /// <param name="finallyAction">Finally Block 처리를 담당하는 Action</param>
        /// <returns>작업 성공 여부</returns>
        public static void TryActionAsync(Action asyncAction, Action<AggregateException> ageAction = null, Action finallyAction = null) {
            asyncAction.ShouldNotBeNull("asyncAction");

            try {
                asyncAction();
            }
            catch(AggregateException age) {
                if(ageAction != null) {
                    ageAction(age);
                }
                else {
                    if(log.IsErrorEnabled) {
                        log.Error("비동기 방식 Action 작업 시 AggregateException 예외가 발생했습니다!!!");
                        log.Error(age);
                    }
                    age.Handle(ex => false);
                }
            }
            finally {
                if(finallyAction != null)
                    finallyAction();
            }
        }

        /// <summary>
        /// 비동기 방식의 작업을 수행할 때 예외처리를 담당해 줍니다. 비동기 방식 작업을 수행할 때에는 이 함수를 호출하시기 바랍니다.
        /// </summary>
        /// <param name="asyncFunc">비동기 방식 작업을 수행하는 Function</param>
        /// <param name="valueFactory">함수 실행 실패 시의 반환할 값 생성 함수</param>
        /// <param name="ageAction">예외 처리를 담당하는 Action</param>
        /// <param name="finallyAction">Finally Block 처리를 담당하는 Action</param>
        /// <returns>함수 반환 값</returns>
        public static T TryFunctionAsync<T>(Func<T> asyncFunc, Func<T> valueFactory = null, Action<AggregateException> ageAction = null,
                                            Action finallyAction = null) {
            asyncFunc.ShouldNotBeNull("asyncFunc");

            try {
                return asyncFunc();
            }
            catch(AggregateException age) {
                if(ageAction != null) {
                    ageAction(age);
                }
                else {
                    if(log.IsErrorEnabled) {
                        log.Error("비동기 작업 함수 실행 시 AggregateException 예외가 발생했습니다!!!");
                        log.Error(age.Flatten());
                    }
                    age.Handle(ex => false);
                }
            }
            finally {
                if(finallyAction != null)
                    finallyAction();
            }
            return (valueFactory != null) ? valueFactory() : default(T);
        }

        /// <summary>
        /// 비동기 방식의 작업을 수행할 때 예외처리를 담당해 줍니다. 비동기 방식 작업을 수행할 때에는 이 함수를 호출하시기 바랍니다.
        /// </summary>
        /// <param name="asyncFunc">비동기 방식 작업을 수행하는 Function</param>
        /// <param name="resultValue"><paramref name="asyncFunc"/> 반환 값</param>
        /// <returns>작업 성공 여부</returns>
        public static bool TryFunctionAsync<T>(Func<T> asyncFunc, out T resultValue) {
            return TryFunctionAsync(asyncFunc, null, null, out resultValue);
        }

        /// <summary>
        /// 비동기 방식의 작업을 수행할 때 예외처리를 담당해 줍니다. 비동기 방식 작업을 수행할 때에는 이 함수를 호출하시기 바랍니다.
        /// </summary>
        /// <param name="asyncFunc">비동기 방식 작업을 수행하는 Function</param>
        /// <param name="ageAction">예외 처리를 담당하는 Action</param>
        /// <param name="resultValue"><paramref name="asyncFunc"/> 반환 값</param>
        /// <returns>작업 성공 여부</returns>
        public static bool TryFunctionAsync<T>(Func<T> asyncFunc, Action<AggregateException> ageAction, out T resultValue) {
            return TryFunctionAsync(asyncFunc, ageAction, null, out resultValue);
        }

        /// <summary>
        /// 비동기 방식의 작업을 수행할 때 예외처리를 담당해 줍니다. 비동기 방식 작업을 수행할 때에는 이 함수를 호출하시기 바랍니다.
        /// </summary>
        /// <param name="asyncFunc">비동기 방식 작업을 수행하는 Function</param>
        /// <param name="ageAction">예외 처리를 담당하는 Action</param>
        /// <param name="finallyAction">Finally Block 처리를 담당하는 Action</param>
        /// <param name="resultValue"><paramref name="asyncFunc"/> 반환 값</param>
        /// <returns>작업 성공 여부</returns>
        public static bool TryFunctionAsync<T>(Func<T> asyncFunc, Action<AggregateException> ageAction, Action finallyAction,
                                               out T resultValue) {
            asyncFunc.ShouldNotBeNull("asyncFunc");

            bool isSuccess = false;
            resultValue = default(T);

            try {
                resultValue = asyncFunc();
                isSuccess = true;
            }
            catch(AggregateException age) {
                if(ageAction != null) {
                    ageAction(age);
                }
                else {
                    if(log.IsErrorEnabled) {
                        log.Error("비동기 작업 함수 실행 시 AggregateException 예외가 발생했습니다!!!");
                        log.Error(age);
                    }
                    age.Handle(ex => false);
                }
            }
            finally {
                if(finallyAction != null)
                    finallyAction();
            }
            return isSuccess;
        }
    }
}