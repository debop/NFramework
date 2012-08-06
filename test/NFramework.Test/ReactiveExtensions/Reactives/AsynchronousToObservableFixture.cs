using NUnit.Framework;

namespace NSoft.NFramework.ReactiveExtensions.Reactives {
    /// <summary>
    /// Reactive Extensions 중에 IObservable, IObserver에 대한 예제입니다. (System.Reactive.dll)
    /// 참고 : http://codebetter.com/blogs/matthew.podwysocki/archive/2009/11/18/introduction-to-the-reactive-framework-part-v.aspx
    /// </summary>
    [TestFixture]
    public class AsynchronousToObservableFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion
    }
}