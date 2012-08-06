using System;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Threading;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism {
    /// <summary>
    /// 병렬 프로그래밍 관련 단위 테스트의 기본 클래스입니다.
    /// </summary>
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    public abstract class ParallelismFixtureBase {
        #region << logger >>

        protected static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        protected static readonly bool IsTraceEnabled = log.IsTraceEnabled;
        protected static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        protected static readonly bool IsInfoEnabled = log.IsInfoEnabled;
        protected static readonly bool IsWarnEnabled = log.IsWarnEnabled;
        protected static readonly bool IsErrorEnabled = log.IsErrorEnabled;

        #endregion

        public const string ComponentId_AdoRepository_Northwind = "AdoRepository.Northwind";

        internal static readonly Random Rnd = new ThreadSafeRandom();

        [TestFixtureSetUp]
        public void FixtureSetUp() {
            if(IoC.IsNotInitialized)
                IoC.Initialize();
        }
    }
}