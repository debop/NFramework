using System;
using System.ComponentModel;
using System.IO;
using NHibernate.Linq;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.DynamicProxy;
using NSoft.NFramework.Reflections;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx.Interceptors {
    //
    // BUG: EditablePropertyChangedInterceptor, NotifyPropertyChangedInterceptor 등이 Association이 없는 경우에는 잘 되나, Association이 있는 경우에는 안된다!!!
    //
    [TestFixture]
    public class PropertyChangedInterceptorFixture : NHRepositoryTestFixtureBase {
        protected override string ContainerFilePath {
            get {
                return
                    Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Interceptors\IoC.PropertyChanged.config"));
            }
        }

        [Test]
        public void Can_Raise_PropertyChanged_In_PersitentObject() {
            foreach(var parent in UnitOfWork.CurrentSession.Query<Parent>()) {
                VerifyNotifyPropertyChanged(parent);
                VerifyEditableObject(parent);

                Assert.IsNotNull(parent.Children);
                Assert.Greater(parent.Children.Count, 0);
            }
        }

        [Test]
        public void Can_Raise_PropertyChanged_In_TransientObject() {
            var parent = DynamicProxyTool.CreateEditablePropertyChanged<Parent>();

            VerifyNotifyPropertyChanged(parent);
            VerifyEditableObject(parent);
        }

        [Test]
        public void Can_Raise_PropertyChanged_In_TransientObject_With_Mapping() {
            var parent = parentsInDB[0].MapProperty(() => DynamicProxyTool.CreateEditablePropertyChanged<Parent>(),
                                                    MapPropertyOptions.Safety);

            VerifyNotifyPropertyChanged(parent);
            VerifyEditableObject(parent);
        }

        private static void VerifyNotifyPropertyChanged(object proxy) {
            var parent = (Parent)proxy;

            parent.Name = "원본";

            var eventRaised = false;

            parent.Age = 100;
            Assert.IsFalse(eventRaised);

            PropertyChangedEventHandler hander = (s, e) => {
                                                     eventRaised = true;
                                                     Assert.AreEqual("Name", e.PropertyName);
                                                 };

            ((INotifyPropertyChanged)parent).PropertyChanged += hander;

            parent.Name = "변경";

            Assert.IsTrue(eventRaised);

            ((INotifyPropertyChanged)parent).PropertyChanged -= hander;

            parent.Age = 200;
        }

        private static void VerifyEditableObject(object proxy) {
            var parent = (Parent)proxy;
            parent.Age = 0;

            ((IEditableObject)parent).BeginEdit();

            parent.Age = 100;
            Assert.AreEqual(100, parent.Age);

            ((IEditableObject)parent).CancelEdit();
            Assert.AreEqual(0, parent.Age);


            ((IEditableObject)parent).BeginEdit();
            parent.Age = 999;
            ((IEditableObject)parent).EndEdit();

            Assert.AreEqual(999, parent.Age);
        }
    }
}