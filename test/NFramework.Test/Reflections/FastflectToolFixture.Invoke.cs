using System;
using Fasterflect;
using NUnit.Framework;

namespace NSoft.NFramework.Reflections {
    /// <summary>
    /// Reflection, Dynamic Method를 이용하여, 인스턴스의 메소드를 실행해본다.
    /// </summary>
    /// http://blogs.msdn.com/haibo_luo/archive/2006/11/07/turn-methodinfo-to-dynamicmethod.aspx
    [TestFixture]
    public class FastflectToolFixture_Invoke {
        /// <summary>
        /// Sample용 클래스
        /// </summary>
        public class Widget {
            public Widget() {}

            public Widget(string name, int? age) {
                Name = name;
                Age = age;
            }

            public virtual string Name { get; set; }
            public virtual int? Age { get; set; }
            public virtual string Description { get; set; }

            public virtual void UpdateAll(string name, int? age) {
                Name = name;
                Age = age;
            }

            public virtual void UpdateAll(string name, int? age, string description) {
                Name = name;
                Age = age;
                Description = description;
            }

            public virtual void OutputAll(ref string name, ref int? age) {
                name = Name;
                age = Age;
            }

            public override string ToString() {
                return string.Format("Widget# Name={0}, Age={1}", Name, Age);
            }
        }

        private object _widget;

        public const string InputMethod = @"UpdateAll";
        public const string ProcedureMethod = @"ToString";
        public const string OutputMethod = @"OutputAll";

        [SetUp]
        public void SetUp() {
            _widget = new Widget("Dynamic", 22);
        }

        [Test]
        public void CanInvokeProcedureMethodByReflection() {
            var result = MethodExtensions.CallMethod(_widget, ProcedureMethod);
            Assert.IsNotNull(result);
        }

        [Test]
        [TestCase("배성혁", 43)]
        [TestCase("불명", null)]
        public void CanInvokeInputMethodByReflection(string name, int? age) {
            var mi = _widget.GetType().GetMethod(InputMethod, new Type[] { typeof(string), typeof(int?) });
            mi.Invoke(_widget, new object[] { name, age });

            Assert.AreEqual(name, ((Widget)_widget).Name);
            Assert.AreEqual(age, ((Widget)_widget).Age);

            mi = _widget.GetType().GetMethod(InputMethod, new Type[] { typeof(string), typeof(int?), typeof(string) });
            mi.Invoke(_widget, new object[] { name, age, "설명" });

            Assert.AreEqual(name, ((Widget)_widget).Name);
            Assert.AreEqual(age, ((Widget)_widget).Age);
            Assert.AreEqual("설명", ((Widget)_widget).Description);
        }

        // BUG : NullReference
        [Test]
        public void CanInvokeOutputMethodByReflection() {
            // NOTE: ref, out parameter에 대해서는 아래와 같이 parameters 컬렉션을 만들어서 주고, 
            // NOTE: 반환 값은 parameters 컬렉션에서 조회해야 한다.
            // NOTE: http://social.msdn.microsoft.com/Forums/en-US/netfxbcl/thread/c7ea0d57-dc8a-41ae-84df-42e5d0d8718e

            var parameters = new object[] { null, null };
            var mi = _widget.GetType().GetMethod(OutputMethod);
            var result = mi.Invoke(_widget, parameters);

            Assert.AreEqual(parameters[0], ((Widget)_widget).Name);
            Assert.AreEqual(parameters[1], ((Widget)_widget).Age);
        }
    }
}