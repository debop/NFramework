using System;
using System.Threading;
using NSoft.NFramework.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Reflections {
    [TestFixture]
    public class FasterflectToolFixture : AbstractFixture {
        public const int RunCount = 100;
        public static Type PersonType = typeof(Person);

        [Test]
        public void GetMethodInvoker() {
            var person = ActivatorTool.CreateInstance(PersonType, new object[] { 0, "John" });

            var methodInvoker = PersonType.GetMethodInvoker("Walk", typeof(int));
            for(int i = 0; i < RunCount; i++)
                methodInvoker(person, 1);

            Assert.AreEqual(RunCount, person.GetField("milesTraveled").AsInt());

            person = ActivatorTool.CreateInstance(PersonType, new object[] { 1, "Merry" });

            methodInvoker = PersonType.GetMethodInvoker("Walk");
            for(int i = 0; i < RunCount; i++)
                methodInvoker(person);

            Assert.AreEqual(RunCount, person.GetField("milesTraveled").AsInt());
        }

        [Test]
        public void InvokeMethod() {
            var person = ActivatorTool.CreateInstance(PersonType, new object[] { 0, "John" });

            for(int i = 0; i < RunCount; i++)
                PersonType.InvokeMethod(invoker => invoker(person, 1), "Walk", typeof(int));

            Assert.AreEqual(RunCount, person.GetField("milesTraveled").AsInt());
        }

        /// <summary>
        /// 이 함수는 여러 Object에 한꺼번에 같은 메소드를 호출할 때 유용합니다.
        /// </summary>
        [Test]
        public void InvokeMethodAsParallel() {
            var person = ActivatorTool.CreateInstance(typeof(Person), new object[] { 0, "John" });

            PersonType.InvokeMethodAsParallel(0, RunCount, null, (i, invoker) => invoker(person, 1), "Walk", typeof(int));

            Assert.AreEqual(RunCount, person.GetField("milesTraveled").AsInt());
        }

        [Serializable]
        private class Person {
            private int id;
            private int milesTraveled;

            public int Id {
                get { return id; }
                set { id = value; }
            }

            public string Name { get; private set; }

            private static int InstanceCount;

            public Person() : this(0) {}

            public Person(int id) : this(id, string.Empty) {}

            public Person(int id, string name) {
                Id = id;
                Name = name;
                InstanceCount++;
            }

            public char this[int index] {
                get { return Name[index]; }
            }

            private void Walk() {
                Interlocked.Add(ref milesTraveled, 1);
                // milesTraveled += miles;
            }

            private void Walk(int miles) {
                Interlocked.Add(ref milesTraveled, miles);
                // milesTraveled += miles;
            }

            private static void IncreaseInstanceCount() {
                InstanceCount++;
            }

            private static int GetInstanceCount() {
                return InstanceCount;
            }

            public static void Swap(ref int i, ref int j) {
                int tmp = i;
                i = j;
                j = tmp;
            }

            public override string ToString() {
                return string.Format("Person# Id={0}, Name={1}, MilesTraveled={2}", Id, Name, milesTraveled);
            }
        }

        [Test]
        [TestCase("ProductName", MemberNamingRule.CamelCase, "productName")]
        [TestCase("ProductName", MemberNamingRule.CamelCaseUndercore, "_productName")]
        [TestCase("ProductName", MemberNamingRule.CamelCase_M_Underscore, "m_productName")]
        [TestCase("ProductName", MemberNamingRule.PascalCase, "ProductName")]
        [TestCase("ProductName", MemberNamingRule.PascalCaseUnderscore, "_ProductName")]
        [TestCase("ProductName", MemberNamingRule.PascalCase_M_Underscore, "m_ProductName")]
        public void GetFieldNameByNamingKind(string actualName, MemberNamingRule namingRule, string expectedName) {
            var memberName = FasterflectTool.GetMemberName(actualName, namingRule);

            Assert.AreEqual(expectedName, memberName);
        }
    }
}