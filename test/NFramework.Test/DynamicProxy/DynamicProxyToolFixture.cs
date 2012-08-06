using System.ComponentModel;
using Castle.DynamicProxy;
using NSoft.NFramework.Tools;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.DynamicProxy {
    [Microsoft.Silverlight.Testing.Tag("DynamicProxy")]
    [TestFixture]
    public class DynamicProxyToolFixture : AbstractFixture {
        [Test]
        public void IsDynamicProxy_Test() {
            var proxy = DynamicProxyTool.CreateNotifyPropertyChanged<SimpleViewModel>();

            proxy.IsDynamicProxy().Should().Be.True();
            new SimpleViewModel().IsDynamicProxy().Should().Be.False();
        }

        [Test]
        public void IsDynamicProxyType_Test() {
            var proxy = DynamicProxyTool.CreateNotifyPropertyChanged<SimpleViewModel>();

            proxy.GetType().IsDynamicProxyType().Should().Be.True();
            typeof(SimpleViewModel).IsDynamicProxyType().Should().Be.False();
        }

        [Test]
        public void CreateProxy_ThreadTest() {
            TestTool.RunTasks(10,
                              () => {
                                  Can_Create_NotifyPropertyChanged_Proxy_Type();
                                  Can_Create_NotifyPropertyChanged_Proxy_Generic();
                                  Can_Create_NotifyPropertyChanged_Proxy_Factory();

                                  Can_Create_EditableObject_Proxy_Type();
                                  Can_Create_EditableObject_Proxy_Generic();
                                  Can_Create_EditableObject_Proxy_Factory();

                                  Can_Create_EditablePropertyChanged_Proxy_Type();
                                  Can_Create_EditablePropertyChanged_Proxy_Generic();
                                  Can_Create_EditablePropertyChanged_Proxy_Factory();
                              });
        }

        [Test]
        public void Can_Create_NotifyPropertyChanged_Proxy_Type() {
            var proxy = (SimpleViewModel)DynamicProxyTool.CreateNotifyPropertyChanged(typeof(SimpleViewModel));
            VerifyNotifyPropertyChanged(proxy);
        }

        [Test]
        public void Can_Create_NotifyPropertyChanged_Proxy_Generic() {
            var proxy = DynamicProxyTool.CreateNotifyPropertyChanged<SimpleViewModel>();
            VerifyNotifyPropertyChanged(proxy);
        }

        [Test]
        public void Can_Create_NotifyPropertyChanged_Proxy_Factory() {
            var proxy = (SimpleViewModel)DynamicProxyTool.CreateNotifyPropertyChanged(() => new SimpleViewModel());
            VerifyNotifyPropertyChanged(proxy);
        }

        [Test]
        public void Can_Create_EditableObject_Proxy_Type() {
            var proxy = (SimpleViewModel)DynamicProxyTool.CreateEditableObject(typeof(SimpleViewModel));

            VerifyEditableObject(proxy);
        }

        [Test]
        public void Can_Create_EditableObject_Proxy_Generic() {
            var proxy = DynamicProxyTool.CreateEditableObject<SimpleViewModel>();
            VerifyEditableObject(proxy);
        }

        [Test]
        public void Can_Create_EditableObject_Proxy_Factory() {
            var proxy = (SimpleViewModel)DynamicProxyTool.CreateEditableObject(() => ActivatorTool.CreateInstance<SimpleViewModel>());
            VerifyEditableObject(proxy);
        }

        [Test]
        public void Can_Create_EditablePropertyChanged_Proxy_Type() {
            var proxy = (SimpleViewModel)DynamicProxyTool.CreateEditablePropertyChanged(typeof(SimpleViewModel));

            VerifyEditableObject(proxy);
            VerifyNotifyPropertyChanged(proxy);
        }

        [Test]
        public void Can_Create_EditablePropertyChanged_Proxy_Generic() {
            var proxy = DynamicProxyTool.CreateEditablePropertyChanged<SimpleViewModel>();

            VerifyEditableObject(proxy);
            VerifyNotifyPropertyChanged(proxy);
        }

        [Test]
        public void Can_Create_EditablePropertyChanged_Proxy_Factory() {
            var proxy =
                (SimpleViewModel)DynamicProxyTool.CreateEditablePropertyChanged(() => ActivatorTool.CreateInstance<SimpleViewModel>());

            VerifyEditableObject(proxy);
            VerifyNotifyPropertyChanged(proxy);
        }

        private static void VerifyNotifyPropertyChanged(object proxy) {
            // IProxyTargetAccessor로 Casting이 된다면, 객체가 Proxy임을 나타낸다!!!
            var hack = proxy as IProxyTargetAccessor;
            Assert.IsNotNull(hack);

            var svm = (SimpleViewModel)proxy;

            svm.Name = "원본";

            var eventRaised = false;

            svm.Age = 100;

            eventRaised.Should().Be.False();

            PropertyChangedEventHandler hander = (s, e) => {
                                                     eventRaised = true;
                                                     e.PropertyName.Should().Be("Name");
                                                 };

            ((INotifyPropertyChanged)svm).PropertyChanged += hander;

            svm.Name = "변경";

            eventRaised.Should().Be.True();

            ((INotifyPropertyChanged)svm).PropertyChanged -= hander;

            svm.Age = 200;
        }

        private static void VerifyEditableObject(object proxy) {
            // IProxyTargetAccessor로 Casting이 된다면, 객체가 Proxy임을 나타낸다!!!
            var hack = proxy as IProxyTargetAccessor;
            Assert.IsNotNull(hack);


            var svm = (SimpleViewModel)proxy;
            svm.Age = 0;

            ((IEditableObject)svm).BeginEdit();

            svm.Age = 100;
            svm.Age.Should().Be(100);

            ((IEditableObject)svm).CancelEdit();

            svm.Age.Should().Be(0);


            ((IEditableObject)svm).BeginEdit();

            svm.Age = 999;
            svm.Age.Should().Be(999);

            ((IEditableObject)svm).EndEdit();

            svm.Age.Should().Be(999);

            ((IEditableObject)svm).BeginEdit();

            svm.Age = 555;
            svm.Age.Should().Be(555);

            ((IEditableObject)svm).CancelEdit();
            ((IEditableObject)svm).EndEdit();

            svm.Age.Should().Be(999);
        }
    }
}