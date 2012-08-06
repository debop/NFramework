using System;
using NSoft.NFramework.Threading;
using NUnit.Framework;

namespace NSoft.NFramework {
    /// <summary>
    /// 단위테스트 클래스의 기본 클래스입니다.
    /// </summary>
    public abstract class AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        protected static Random Rnd = ThreadTool.CreateRandom();

        [TestFixtureSetUp]
        public void FixtureSetUp() {
            OnFixtureSetUp();
        }

        [SetUp]
        public void SetUp() {
            OnSetUp();
        }

        [TearDown]
        public void TearDown() {
            OnTearDown();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown() {
            OnFixtureTearDown();
        }

        protected virtual void OnFixtureSetUp() {
            if(IsInfoEnabled)
                log.Info("테스트 클래스를 시작합니다...");
        }

        protected virtual void OnSetUp() {
            if(IsDebugEnabled)
                log.Debug("테스트 함수를 시작합니다...");
        }

        protected virtual void OnTearDown() {
            if(IsInfoEnabled)
                log.Info("테스트 함수를 종료합니다.");
        }

        protected virtual void OnFixtureTearDown() {
            if(IsInfoEnabled)
                log.Info("테스트 클래스를 완료했습니다.");
        }
    }
}