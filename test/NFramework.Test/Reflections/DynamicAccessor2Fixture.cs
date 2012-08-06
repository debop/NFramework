using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fasterflect;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.Threading;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Reflections.Emit {
    [TestFixture]
    public class DynamicAccessor2Fixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private readonly object _syncLock = new object();

        private const int WidgetCount = 100;
        private readonly IList<Widget> _widgets = new List<Widget>(WidgetCount);
        private readonly Type widgetType = typeof(Widget);

        [TestFixtureSetUp]
        public void ClassSetUp() {
            using(new OperationTimer(string.Format("Create RwList with {0} Widget", WidgetCount))) {
                for(int i = 0; i < WidgetCount; i++)
                    _widgets.Add(new Widget(i, "Widget " + i));
            }
        }

        [Test]
        public void GetPropertyByCode() {
            using(new OperationTimer("Get Property By Compiled Code")) {
                foreach(Widget widget in _widgets) {
                    Assert.IsNotNull(widget.Id);
                    Assert.IsNotNull(widget.Name);
                    Assert.AreNotEqual(Guid.Empty, widget.Guid);
                }
            }
        }

        [Test]
        public void GetPropertyByReflection() {
            int id;
            string name;
            Guid guid;

            using(new OperationTimer("Get Property by Reflection")) {
                var widgetType = typeof(Widget);
                var id_pi = widgetType.GetProperty("Id");
                var name_pi = widgetType.GetProperty("Name");
                var guid_pi = widgetType.GetProperty("Guid");

                foreach(var widget in _widgets) {
                    id = (Int32)id_pi.GetValue(widget, null);
                    name = (String)name_pi.GetValue(widget, null);
                    guid = (Guid)guid_pi.GetValue(widget, null);

                    Assert.AreEqual(id, widget.Id);
                    Assert.AreEqual(name, widget.Name);
                    Assert.AreEqual(guid, widget.Guid);
                }
            }
        }

        [Test]
        public void GetPropertyByDynamicMethod() {
            using(new OperationTimer("Get Property by Dynamic Method")) {
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor<Widget>();

                foreach(var widget in _widgets) {
                    var id = (int)accessor.GetPropertyValue(widget, "Id");
                    var name = (string)accessor.GetPropertyValue(widget, "Name");
                    var guid = (Guid)accessor.GetPropertyValue(widget, "Guid");

                    Assert.AreEqual(widget.Id, id);
                    Assert.AreEqual(widget.Name, name);
                    Assert.AreEqual(widget.Guid, guid);
                }
            }
        }

        [Test]
        public void GetPropertyByDynamicMethodWithIgnoreCase() {
            using(new OperationTimer("Get Property by Dynamic Method with IgnoreCase")) {
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor<Widget>(false, true);

                foreach(var widget in _widgets) {
                    var id = (int)accessor.GetPropertyValue(widget, "ID");
                    var name = (string)accessor.GetPropertyValue(widget, "NAME");
                    var guid = (Guid)accessor.GetPropertyValue(widget, "GUID");

                    Assert.AreEqual(widget.Id, id);
                    Assert.AreEqual(widget.Name, name);
                    Assert.AreEqual(widget.Guid, guid);
                }
            }
        }

        [Test]
        public void TryGetPropertyByDynamicMethod() {
            using(new OperationTimer("TryGetPropertyValue by Dynamic Method")) {
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor<Widget>(true, true);

                object id, name, guid;

                foreach(var widget in _widgets) {
                    Assert.IsTrue(accessor.TryGetPropertyValue(widget, "Id", out id));
                    Assert.IsTrue(accessor.TryGetPropertyValue(widget, "Name", out name));
                    Assert.IsTrue(accessor.TryGetPropertyValue(widget, "Guid", out guid));

                    object notExists;
                    Assert.IsFalse(accessor.TryGetPropertyValue(widget, "NotExistProperty", out notExists));


                    Assert.AreEqual(id, widget.Id);
                    Assert.AreEqual(name, widget.Name);
                    Assert.AreEqual(guid, widget.Guid);
                }
            }
        }

        [Test]
        public void GetFieldByDynamicMethod() {
            int id;
            string name;
            Guid guid;

            using(new OperationTimer("Get Field by Dynamic Method")) {
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor<Widget>();

                foreach(var widget in _widgets) {
                    id = (int)accessor.GetFieldValue(widget, "_id");
                    name = (string)accessor.GetFieldValue(widget, "_name");
                    guid = (Guid)accessor.GetFieldValue(widget, "_guid");

                    Assert.AreEqual(id, widget.Id);
                    Assert.AreEqual(name, widget.Name);
                    Assert.AreEqual(guid, widget.Guid);
                }
            }
        }

        [Test]
        public void GetFieldByDynamicMethodWithIgnoreCase() {
            int id;
            string name;
            Guid guid;

            using(new OperationTimer("Get Field by Dynamic Method with IgnoreCase")) {
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor<Widget>(false, true);

                foreach(var widget in _widgets) {
                    id = (int)accessor.GetFieldValue(widget, "_ID");
                    name = (string)accessor.GetFieldValue(widget, "_NAME");
                    guid = (Guid)accessor.GetFieldValue(widget, "_GUID");

                    Assert.AreEqual(id, widget.Id);
                    Assert.AreEqual(name, widget.Name);
                    Assert.AreEqual(guid, widget.Guid);
                }
            }
        }

        [Test]
        public void TryGetFieldByDynamicMethod() {
            object id, name, guid;

            using(new OperationTimer("TryGetFieldValue by Dynamic Method")) {
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor<Widget>(true, true);

                foreach(var widget in _widgets) {
                    Assert.IsTrue(accessor.TryGetFieldValue(widget, "_id", out id));
                    Assert.IsTrue(accessor.TryGetFieldValue(widget, "_name", out name));
                    Assert.IsTrue(accessor.TryGetFieldValue(widget, "_guid", out guid));

                    object notExists;
                    Assert.IsFalse(accessor.TryGetFieldValue(widget, "_notExistField", out notExists));

                    Assert.AreEqual(id, widget.Id);
                    Assert.AreEqual(name, widget.Name);
                    Assert.AreEqual(guid, widget.Guid);
                }
            }
        }

        [Test]
        public void GetPropertyByDynamicMethodAndParallel() {
            using(new OperationTimer("Get Property by Dynamic Method and Parallel Programming")) {
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor<Widget>();

                foreach(var widget in _widgets.AsParallel()) {
                    var id = (int)accessor.GetPropertyValue(widget, "Id");
                    var name = (string)accessor.GetPropertyValue(widget, "Name");
                    var guid = (Guid)accessor.GetPropertyValue(widget, "Guid");

                    Assert.AreEqual(widget.Id, id);
                    Assert.AreEqual(widget.Name, name);
                    Assert.AreEqual(widget.Guid, guid);
                }
            }
        }

        [Test]
        public void SetPropertyByCode() {
            using(new OperationTimer("Set Property value by code")) {
                foreach(var widget in _widgets) {
                    widget.Id = widget.Id;
                    widget.Name = widget.Name;
                }
            }
        }

        [Test]
        public void SetPropertyByReflection() {
            using(new OperationTimer("Set Property by Reflection")) {
                var widgetType = typeof(Widget);

                var id_pi = widgetType.GetProperty("Id");
                var name_pi = widgetType.GetProperty("Name");

                foreach(Widget widget in _widgets) {
                    id_pi.SetValue(widget, widget.Id, null);
                    name_pi.SetValue(widget, widget.Name, null);
                }
            }
        }

        [Test]
        public void SetPropertyByDynamicMethod() {
            using(new OperationTimer("Set Property value by Dynamic method")) {
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor<Widget>(() => new DynamicAccessor<Widget>(false));

                foreach(var widget in _widgets) {
                    var id = widget.Id;
                    var name = widget.Name + " by SetPropertyValue";
                    var guid = Guid.NewGuid();
                    accessor.SetPropertyValue(widget, "Id", id);
                    accessor.SetPropertyValue(widget, "Name", name);
                    accessor.SetPropertyValue(widget, "Guid", guid);

                    Assert.AreEqual(id, widget.Id);
                    Assert.AreEqual(name, widget.Name);
                    Assert.AreEqual(guid, widget.Guid);
                }
            }

            using(new OperationTimer("Set Property value by Dynamic method")) {
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor<Widget>();

                foreach(var widget in _widgets) {
                    var id = widget.Id;
                    var name = widget.Name + " by SetPropertyValue";
                    var guid = Guid.NewGuid();
                    accessor.SetPropertyValue(widget, "Id", id);
                    accessor.SetPropertyValue(widget, "Name", name);
                    accessor.SetPropertyValue(widget, "Guid", guid);

                    Assert.AreEqual(id, widget.Id);
                    Assert.AreEqual(name, widget.Name);
                    Assert.AreEqual(guid, widget.Guid);
                }
            }
        }

        [Test]
        public void SetPropertyByDynamicMethodWithIgnoreCase() {
            using(new OperationTimer("Set Property value by Dynamic method with IgnoreCase")) {
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor<Widget>(() => new DynamicAccessor<Widget>(false, true));

                foreach(var widget in _widgets) {
                    var id = widget.Id;
                    var name = widget.Name + " by SetPropertyValue";
                    var guid = Guid.NewGuid();
                    accessor.SetPropertyValue(widget, "ID", id);
                    accessor.SetPropertyValue(widget, "NAME", name);
                    accessor.SetPropertyValue(widget, "GUID", guid);

                    Assert.AreEqual(id, widget.Id);
                    Assert.AreEqual(name, widget.Name);
                    Assert.AreEqual(guid, widget.Guid);
                }
            }

            using(new OperationTimer("Set Property value by Dynamic method with IgnoreCare")) {
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor<Widget>(false, true);

                foreach(var widget in _widgets) {
                    var id = widget.Id;
                    var name = widget.Name + " by SetPropertyValue";
                    var guid = Guid.NewGuid();
                    accessor.SetPropertyValue(widget, "ID", id);
                    accessor.SetPropertyValue(widget, "NAME", name);
                    accessor.SetPropertyValue(widget, "GUID", guid);

                    Assert.AreEqual(id, widget.Id);
                    Assert.AreEqual(name, widget.Name);
                    Assert.AreEqual(guid, widget.Guid);
                }
            }
        }

        [Test]
        public void SetFieldByDynamicMethod() {
            using(new OperationTimer("Set Field value by dynamic method")) {
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor<Widget>();

                foreach(var widget in _widgets) {
                    accessor.SetFieldValue(widget, "_id", widget.Id);
                    accessor.SetFieldValue(widget, "_name", widget.Name);
                    accessor.SetFieldValue(widget, "_guid", widget.Guid);
                }
            }
        }

        [Test]
        public void SetFieldByDynamicMethodWithIgnoreCase() {
            using(new OperationTimer("Set Field value by dynamic method with IgnoreCase")) {
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor<Widget>(false, true);

                foreach(var widget in _widgets) {
                    accessor.SetFieldValue(widget, "_ID", widget.Id);
                    accessor.SetFieldValue(widget, "_NAME", widget.Name);
                    accessor.SetFieldValue(widget, "_GUID", widget.Guid);
                }
            }
        }

        [Test]
        public void SetPropertyByDynamicMethodAndParellel() {
            using(new OperationTimer("Set Property value by Dynamic method and Parallel")) {
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor<Widget>();

                _widgets
                    .AsParallel()
                    .RunEach(widget => {
                                 var id = widget.Id;
                                 var name = widget.Name + " by SetPropertyValue";
                                 var guid = Guid.NewGuid();
                                 accessor.SetPropertyValue(widget, "Id", id);
                                 accessor.SetPropertyValue(widget, "Name", name);
                                 accessor.SetPropertyValue(widget, "Guid", guid);

                                 Assert.AreEqual(id, widget.Id);
                                 Assert.AreEqual(name, widget.Name);
                                 Assert.AreEqual(guid, widget.Guid);
                             });
            }
        }

        [Test]
        public void GetPropertyNames() {
            var accessor = DynamicAccessorFactory.CreateDynamicAccessor<Widget>();

            var propertyNames = accessor.GetPropertyNames();

            Assert.IsTrue(propertyNames.Count > 0);
            Assert.IsTrue(propertyNames.Contains("Id"));
            Assert.IsTrue(propertyNames.Contains("Name"));

            var customPropertyNames =
                accessor.GetPropertyNames(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

            Assert.IsTrue(customPropertyNames.Count > 0);
            Assert.IsTrue(propertyNames.Contains("Id"));
            Assert.IsTrue(propertyNames.Contains("Name"));
        }

        [Test]
        public void GetFieldNames() {
            var accessor = DynamicAccessorFactory.CreateDynamicAccessor<Widget>();

            var fieldNames = accessor.GetFieldNames();

            Assert.IsTrue(fieldNames.Count > 0);
            Assert.IsTrue(fieldNames.Contains("_id"));
            Assert.IsTrue(fieldNames.Contains("_name"));

            var customFieldNames = accessor.GetFieldNames(BindingFlags.Public | BindingFlags.Instance);
            Assert.AreEqual(0, customFieldNames.Count);
        }

        [Test]
        public void CallMethod() {
            foreach(var widget in _widgets) {
                var area = (double)widget.CallMethod("GetArea");
                Assert.AreEqual(widget.GetArea(), area);

                var ratio = (double)widget.CallMethod("GetRatio");
                Assert.AreEqual(widget.GetRatio(), ratio);

                var power = (double)widget.CallMethod("GetAreaPowers", ratio);
                Assert.AreEqual(Math.Pow(area, ratio), power);
            }
        }

        [Test]
        public void CallMethod_By_Delegate() {
            var getAreaInvoker = MethodExtensions.DelegateForCallMethod(widgetType, "GetArea");

            foreach(var widget in _widgets) {
                var area = (double)getAreaInvoker.Invoke(widget);
                Assert.AreEqual(widget.GetArea(), area);
            }
        }

        [Test]
        public void CallMethod_By_Delegate_Invoker() {
            var getAreaInvoker = widgetType.DelegateForCallMethod("GetArea");

            foreach(var widget in _widgets) {
                Widget widget1 = widget;
                var area = With.TryFunctionAsync(() => (double)DelegateAsync.Run(w => getAreaInvoker.Invoke(w), widget1).Result);
                // var area = (double)getAreaInvoker.Invoke(widget);
                Assert.AreEqual(widget.GetArea(), area);
            }
        }

        [Test]
        public void DelegateForCreateInstance_Invoke() {
            var createDelegator = ConstructorExtensions.DelegateForCreateInstance(widgetType, typeof(int), typeof(string));

            foreach(var widget in _widgets) {
                var dynamicWidget = createDelegator.Invoke(widget.Id, widget.Name);
                Assert.AreEqual(widget.GetArea(), dynamicWidget.CallMethod("GetArea"));
            }
        }

        /// <summary>
        /// Sample class
        /// </summary>
        [Serializable]
        public class Widget : ValueObjectBase {
            private static readonly Random rnd = new ThreadSafeRandom();

            public Widget(int id, string name) {
                _id = id;
                _name = name;

                DummyInt = rnd.Next(100);
                DummyDouble = rnd.NextDouble();
                DummyString = GetType().AssemblyQualifiedName;
            }

            private int _id;
            private string _name = string.Empty;
            protected Guid _guid = Guid.NewGuid();

            public int Id {
                get { return _id; }
                set { _id = value; }
            }

            public string Name {
                get { return _name; }
                set { _name = value; }
            }

            public Guid Guid {
                get { return _guid; }
                set { _guid = value; }
            }

            protected virtual int Width { get; set; }

            private int Height { get; set; }

            public double GetArea() {
                return Width * Height;
            }

            public double GetRatio() {
                return (double)Width / (double)Height;
            }

            public double GetAreaPowers(double power) {
                return Math.Pow(GetArea(), power);
            }

            public int DummyInt { get; set; }
            public double DummyDouble { get; set; }
            public string DummyString { get; set; }

            public override string ToString() {
                return _id.ToString();
            }

            public override int GetHashCode() {
                return HashTool.Compute(_id, _name, _guid);
            }
        }
    }
}