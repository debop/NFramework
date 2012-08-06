using System;
using System.ComponentModel;
using NSoft.NFramework.InversionOfControl;
using NUnit.Framework;

namespace NSoft.NFramework.DynamicProxy {
    [Obsolete("DynProxyFactoryFixture로 테스트 하세요.")]
    [TestFixture]
    public class NotifyPropertyChangedInterceptorFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        protected override void OnFixtureSetUp() {
            base.OnFixtureSetUp();

            if(IoC.IsNotInitialized)
                IoC.Initialize();
        }

        [Test]
        public void Can_Resolve_Generic_IoC_Component_To_Proxy() {
            var simpleVM = (SimpleViewModel)IoC.CreateNotifyPropertyChangedProxy(typeof(SimpleViewModel));

            simpleVM.Name = "뷰모델";
            simpleVM.Description = "설명";

            var eventRaised = false;

            ((INotifyPropertyChanged)simpleVM).PropertyChanged +=
                (s, e) => {
                    eventRaised = true;
                    Assert.AreEqual("Name", e.PropertyName);
                };


            simpleVM.Name = "바보";
            Assert.IsTrue(eventRaised);
        }

        [Test]
        public void Can_Resolve_IoC_Component_To_Proxy() {
            var simpleVM = IoC.CreateNotifyPropertyChangedProxy<SimpleViewModel>();

            simpleVM.Name = "뷰모델";
            simpleVM.Description = "설명";

            var eventRaised = false;

            ((INotifyPropertyChanged)simpleVM).PropertyChanged +=
                (s, e) => {
                    eventRaised = true;
                    Assert.AreEqual("Name", e.PropertyName);
                };

            simpleVM.Name = "바보";

            Assert.IsTrue(eventRaised);
        }
    }
}