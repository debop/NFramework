using System;

namespace NSoft.NFramework.Parallelism {
    /// <summary>
    /// 델리게이트 기반의 관찰자입니다. 관찰 대상의 이벤트에 따라 지정된 델리게이트를 수행합니다.
    /// </summary>
    /// <typeparam name="T">관찰대상 앤티티</typeparam>
    public class DelegateBaseObserver<T> : IObserver<T> {

        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private readonly Action<T> _onNext;
        private readonly Action<Exception> _onError;
        private readonly Action _onCompleted;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="onNext">관찰대상이 onNext 일때 수행할 delegate</param>
        /// <param name="onError">관찰대상이 예외를 발생시켰을 때 수행할 delegate</param>
        /// <param name="onCompleted">관찰 대상이 완료했을 때, 수행할 delegate</param>
        public DelegateBaseObserver(Action<T> onNext, Action<Exception> onError = null, Action onCompleted = null) {
            _onNext = onNext;
            _onError = onError;
            _onCompleted = onCompleted;
        }

        public void OnNext(T value) {
            if(_onNext != null)
                _onNext(value);
        }

        public void OnError(Exception exception) {
            if(_onError != null)
                _onError(exception);
        }

        public void OnCompleted() {
            if(_onCompleted != null)
                _onCompleted();
        }
    }
}