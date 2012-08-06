using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.Threading;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Reflections.Emit {
    [TestFixture]
    public class DynamicAccessorToolFixture : AbstractFixture {
        [Test]
        public void ThreadTest() {
            TestTool.RunTasks(4,
                              () => {
                                  GetAllFields();
                                  GetAllFields_By_Generic();
                                  GetAllProperties();
                                  GetAllProperties_By_Generic();
                              });
        }

        [Test]
        public void GetAllFields() {
            var widget = new Widget(1, "Widget No1");
            var accessor = DynamicAccessorFactory.CreateDynamicAccessor(typeof(Widget));
            var fields = accessor.GetFieldNameValueCollection(widget).ToDictionary(pair => pair.Key, pair => pair.Value);

            VerifyFieldCollection(fields);
        }

        [Test]
        public void GetAllFields_By_Generic() {
            var widget = new Widget(1, "Widget No1");
            var accessor = DynamicAccessorFactory.CreateDynamicAccessor<Widget>();
            var fields = accessor.GetFieldNameValueCollection(widget).ToDictionary(pair => pair.Key, pair => pair.Value);

            VerifyFieldCollection(fields);
        }

        [Test]
        public void GetAllProperties() {
            var widget = new Widget(1, "Widget No1");
            var accessor = DynamicAccessorFactory.CreateDynamicAccessor(typeof(Widget));
            var properties = accessor.GetPropertyNameValueCollection(widget).ToDictionary(pair => pair.Key, pair => pair.Value);

            VerifyPropertyCollection(properties);
        }

        [Test]
        public void GetAllProperties_By_Generic() {
            var widget = new Widget(1, "Widget No1");
            var accessor = DynamicAccessorFactory.CreateDynamicAccessor<Widget>();
            var properties = accessor.GetPropertyNameValueCollection(widget).ToDictionary(pair => pair.Key, pair => pair.Value);

            VerifyPropertyCollection(properties);
        }

        private static void VerifyFieldCollection(IDictionary<string, object> fields) {
            fields.Count().Should().Be.GreaterThan(0);
            fields.Keys.Contains("_guid").Should().Be.True();
            fields.Keys.Contains("_id").Should().Be.True();

            fields.Keys.Contains("_name").Should().Be.True();
            fields["_name"].Should().Be("Widget No1");
        }

        private static void VerifyPropertyCollection(IDictionary<string, object> properties) {
            properties.Count().Should().Be.GreaterThan(0);
            properties.Keys.Contains("Guid").Should().Be.True();
            properties.Keys.Contains("DummyString").Should().Be.True();

            properties.Keys.Contains("Name").Should().Be.True();
            properties["Name"].Should().Be("Widget No1");
        }

        /// <summary>
        /// Sample class
        /// </summary>
        [Serializable]
        private class Widget : ValueObjectBase {
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

            public override int GetHashCode() {
                return HashTool.Compute(_id, _name, _guid);
            }

            public override string ToString() {
                return _id.ToString();
            }
        }
    }
}