using System;
using System.Collections.Generic;
using NSoft.NFramework.Threading;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Reflections {
    [TestFixture]
    public class DynamicAccessorFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private const int WidgetCount = 100;
        private readonly IList<Widget> _widgets = new List<Widget>(WidgetCount);

        [TestFixtureSetUp]
        public void ClassSetUp() {
            using(new OperationTimer(string.Format("Create RwList with {0} Widget", WidgetCount))) {
                _widgets.Clear();

                for(int i = 0; i < WidgetCount; i++)
                    _widgets.Add(new Widget(i, "Widget " + i));
            }
        }

        [Test]
        public void GetPropertyByCode() {
            using(new OperationTimer("Get Property By Compiled Code")) {
                foreach(var widget in _widgets) {
                    Assert.IsNotNull(widget.Id);
                    Assert.IsNotNull(widget.Name);
                    Assert.AreNotEqual(Guid.Empty, widget.Guid);
                }
            }
        }

        [Test]
        public void GetPropertyByDynamicMethod() {
            using(new OperationTimer("GetPropertyValue by Dynamic Method")) {
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor(typeof(Widget));

                foreach(var widget in _widgets) {
                    var id = (int)accessor.GetPropertyValue(widget, "Id");
                    var name = (string)accessor.GetPropertyValue(widget, "Name");
                    var guid = (Guid)accessor.GetPropertyValue(widget, "Guid");

                    Assert.AreEqual(id, widget.Id);
                    Assert.AreEqual(name, widget.Name);
                    Assert.AreEqual(guid, widget.Guid);
                }
            }
        }

        [Test]
        public void TryGetPropertyByDynamicMethod() {
            using(new OperationTimer("TryGetPropertyValue by Dynamic Method")) {
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor(typeof(Widget));

                foreach(var widget in _widgets) {
                    object id, name, guid;

                    Assert.IsTrue(accessor.TryGetPropertyValue(widget, "Id", out id));
                    Assert.IsTrue(accessor.TryGetPropertyValue(widget, "Name", out name));
                    Assert.IsTrue(accessor.TryGetPropertyValue(widget, "Guid", out guid));

                    object notExists;
                    Assert.IsFalse(accessor.TryGetFieldValue(widget, "NotExistProperty", out notExists));

                    Assert.AreEqual(id, widget.Id);
                    Assert.AreEqual(name, widget.Name);
                    Assert.AreEqual(guid, widget.Guid);
                }
            }
        }

        [Test]
        public void GetFieldByDynamicMethod() {
            using(new OperationTimer("Get Field by Dynamic Method")) {
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor(typeof(Widget));

                foreach(var widget in _widgets) {
                    var id = (int)accessor.GetFieldValue(widget, "_id");
                    var name = (string)accessor.GetFieldValue(widget, "_name");
                    var guid = (Guid)accessor.GetFieldValue(widget, "_guid");

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
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor(typeof(Widget));

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
        public void GetPropertyByReflection() {
            var widgetType = typeof(Widget);

            using(new OperationTimer("Get Property by Reflection")) {
                var id_pi = widgetType.GetProperty("Id");
                var name_pi = widgetType.GetProperty("Name");
                var guid_pi = widgetType.GetProperty("Guid");

                foreach(var widget in _widgets) {
                    var id = (Int32)id_pi.GetValue(widget, null);
                    var name = (String)name_pi.GetValue(widget, null);
                    var guid = (Guid)guid_pi.GetValue(widget, null);

                    Assert.AreEqual(id, widget.Id);
                    Assert.AreEqual(name, widget.Name);
                    Assert.AreEqual(guid, widget.Guid);
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
        public void SetPropertyByDynamicMethod() {
            using(new OperationTimer("Set Property value by Dynamic method")) {
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor(typeof(Widget));

                foreach(var widget in _widgets) {
                    accessor.SetPropertyValue(widget, "Id", widget.Id);
                    accessor.SetPropertyValue(widget, "Name", widget.Name);
                    accessor.SetPropertyValue(widget, "Guid", widget.Guid);
                }
            }
        }

        [Test]
        public void SetFieldByDynamicMethod() {
            using(new OperationTimer("Set Field value by dynamic method")) {
                var accessor = DynamicAccessorFactory.CreateDynamicAccessor(typeof(Widget));

                foreach(var widget in _widgets) {
                    accessor.SetFieldValue(widget, "_id", widget.Id);
                    accessor.SetFieldValue(widget, "_name", widget.Name);
                    accessor.SetFieldValue(widget, "_guid", widget.Guid);
                }
            }
        }

        [Test]
        public void SetPropertyByReflection() {
            using(new OperationTimer("Set Property by Reflection")) {
                var widgetType = typeof(Widget);
                var id_pi = widgetType.GetProperty("Id");
                var name_pi = widgetType.GetProperty("Name");

                foreach(var widget in _widgets) {
                    id_pi.SetValue(widget, widget.Id, null);
                    name_pi.SetValue(widget, widget.Name, null);
                }
            }
        }

        //[Test]
        //public void DynamicComparer()
        //{
        //    using(new OperationTimer("Sort By Name :"))
        //    {
        //        _widgets.Sort(new DynamicPropertyComparer<Widget>("Name"));
        //    }

        //    if(IsDebugEnabled)
        //        log.Debug(_widgets.CollectionToString());

        //    using(new OperationTimer("Sort By Id :"))
        //    {
        //        _widgets.Sort(new DynamicPropertyComparer<Widget>("Id"));
        //    }

        //    if(IsDebugEnabled)
        //        log.Debug(_widgets.CollectionToString());
        //}

        [Test]
        //[ThreadedRepeat(3)]
        public void MultiThreadMixingTest() {
            using(new OperationTimer("MultiThreadMixingTest")) {
                GetPropertyByDynamicMethod();
                SetPropertyByDynamicMethod();

                //DynamicComparer();
            }
        }

        /// <summary>
        /// Sample class
        /// </summary>
        public class Widget {
            private static readonly Random rnd = new ThreadSafeRandom();

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

            public Widget(int id, string name) {
                _id = id;
                _name = name;

                DummyInt = rnd.Next(100);
                DummyDouble = rnd.NextDouble();
                DummyString = GetType().AssemblyQualifiedName;
            }

            protected virtual int Width { get; set; }

            private int Height { get; set; }

            public int DummyInt { get; set; }
            public double DummyDouble { get; set; }
            public string DummyString { get; set; }

            public float DummyFloat { get; set; }
            public DateTime? DummyDateTime { get; set; }

            public override string ToString() {
                return _id.ToString();
            }
        }
    }
}