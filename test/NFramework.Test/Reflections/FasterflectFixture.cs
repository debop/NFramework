using System;
using System.Linq;
using System.Reflection;
using Fasterflect;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;

namespace NSoft.NFramework.Reflections {
    [TestFixture]
    public class FasterflectFixture : AbstractFixture {
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

            private void Walk(int miles) {
                milesTraveled += miles;
            }

            private static void IncreaseInstanceCount() {
                InstanceCount++;
            }

            private static int GetInstanceCount() {
                return InstanceCount;
            }

            public static void ResetInstanceCount() {
                InstanceCount = 0;
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

        private const string PersonTypeName = @"NSoft.NFramework.Reflections.FasterflectFixture+Person";

        [Test]
        public void ExecuteNormalApi() {
            var type = Assembly.GetExecutingAssembly().GetType(PersonTypeName);
            Assert.IsNotNull(type);
            Assert.AreEqual(PersonTypeName, type.FullName);

            Person.ResetInstanceCount();

            // Person.InstanceCount static field 값 조회. 아직 생성된 인스턴스가 없기 때문에 0
            Assert.AreEqual(0, type.GetFieldValue("InstanceCount"));

            // Person 생성
            var person = type.CreateInstance();
            Assert.IsNotNull(person);
            Assert.AreEqual(1, type.GetFieldValue("InstanceCount"));

            // static field의 값을 설정함
            type.SetFieldValue("InstanceCount", 2);
            Assert.AreEqual(2, type.GetFieldValue("InstanceCount"));

            // static method를 호출합니다.
            type.CallMethod("IncreaseInstanceCount");
            Assert.AreEqual(3, type.GetFieldValue("InstanceCount"));

            // static method를 호출합니다.
            Assert.AreEqual(3, type.CallMethod("GetInstanceCount"));

            // ref/out 파라미터를 가진 메소드 호출. parameter array가 필요
            var arguments = new object[] { 1, 2 };
            type.CallMethod("Swap",
                            new Type[] { typeof(int).MakeByRefType(), typeof(int).MakeByRefType() },
                            arguments);
            Assert.AreEqual(2, arguments[0]);
            Assert.AreEqual(1, arguments[1]);

            // 2개의 인자를 가진 생성자를 호출할 것임. 단 인자의 수형을 몰라도 된다.
            var obj = type.CreateInstance(1, "Doe");
            Assert.IsNotNull(obj);
            Assert.AreEqual(1, obj.GetFieldValue("id").AsInt());
            Assert.AreEqual("Doe", obj.GetPropertyValue("Name").AsText());

            // 인덱서에서 값을 얻습니다.
            Assert.AreEqual('o', obj.GetIndexer(1).AsChar('a'));

            // 인자에 null 값을 넣어도 된다.
            obj = type.CreateInstance(new[] { typeof(int), typeof(string) }, 1, null);
            Assert.IsNotNull(obj);
            Assert.AreEqual(1, obj.GetFieldValue("id").AsInt());
            Assert.IsNull(obj.GetPropertyValue("Name"));
            Assert.AreEqual(string.Empty, obj.GetPropertyValue("Name").AsText());

            // id 필드 값을 변경
            obj.SetFieldValue("id", 2);
            Assert.AreEqual(2, obj.GetFieldValue("id").AsInt());
            Assert.AreEqual(2, obj.GetPropertyValue("Id").AsInt());

            // Fluent 방식 호출 가능
            obj.SetFieldValue("id", 3)
                .SetPropertyValue("Name", "Buu");

            Assert.AreEqual(3, obj.GetPropertyValue("Id").AsInt());
            Assert.AreEqual("Buu", obj.GetPropertyValue("Name").AsText());

            // Mapping property from a source to a target
            new { Id = 4, Name = "Atom" }.MapProperties(obj);
            Assert.AreEqual(4, obj.GetPropertyValue("Id").AsInt());
            Assert.AreEqual("Atom", obj.GetPropertyValue("Name").AsText());

            // 메소드 호출
            obj.CallMethod("Walk", 6);
            Assert.AreEqual(6, obj.GetFieldValue("milesTraveled").AsInt());

            // 현재 수형을 요소로 가지는 배열을 생성합니다. var arr = new Person[10]; 와 같다.
            var arr = type.MakeArrayType().CreateInstance(10);

            obj = type.CreateInstance();
            arr.SetElement(4, obj)
                .SetElement(9, obj);

            Assert.AreEqual(obj, arr.GetElement(4));
            Assert.AreEqual(obj, arr.GetElement(9));
            Assert.IsNull(arr.GetElement(0));
        }

        [Test]
        public void ExecuteCacheApi() {
            var type = Assembly.GetExecutingAssembly().GetType(PersonTypeName);
            Assert.IsNotNull(type);
            Assert.AreEqual(PersonTypeName, type.FullName);

            var range = Enumerable.Range(0, 10).ToList();

            // Static Property Getter를 가져옵니다.
            MemberGetter count = type.DelegateForGetFieldValue("InstanceCount", Flags.StaticAnyVisibility);
            // StaticMemberGetter count = type.DelegateForGetStaticFieldValue("InstanceCount");

            Assert.AreEqual(type.GetFieldValue("InstanceCount", Flags.StaticAnyVisibility), count(null).AsInt());

            // Person의 두개의 인자를 사용하는 생성자의 Delegate를 가져옵니다.
            int currentInstanceCount = count(null).AsInt();
            ConstructorInvoker ctor = type.DelegateForCreateInstance(new[] { typeof(int), typeof(string) });
            Assert.IsNotNull(ctor);

            // 생성자 델리게이트를 이용하여 10개의 인스턴스를 생성합니다.
            range.ForEach(i => {
                              object obj = ctor(i, "_" + i);
                              Assert.AreEqual(++currentInstanceCount, count(null).AsInt());
                              Assert.AreEqual(i, obj.GetFieldValue("id").AsInt());
                              Assert.AreEqual(i, obj.GetPropertyValue("Id").AsInt());
                              Assert.AreEqual("_" + i, obj.GetPropertyValue("Name").AsText());

                              Console.WriteLine(obj);
                          });

            // 속성 조회/설정 (Property Getter / Setter) : Field 및 Indexer에 대해서도 가능합니다.

            MemberGetter nameGetter = type.DelegateForGetPropertyValue("Name");
            MemberSetter nameSetter = type.DelegateForSetPropertyValue("Name");

            object person = ctor(1, "John");
            Assert.AreEqual("John", nameGetter(person));
            nameSetter(person, "Jane");
            Assert.AreEqual("Jane", nameGetter(person));


            // 메소드 호출
            person = type.CreateInstance();
            MethodInvoker walkInvoker = type.DelegateForCallMethod("Walk", new[] { typeof(int) });
            EnumerableTool.RunEach(range, i => walkInvoker(person, i));
            Assert.AreEqual(range.Sum(), person.GetFieldValue("milesTraveled").AsInt());

            // Mapping properties
            //
            var ano = new { Id = 4, Name = "Doe" };
            var mapper = ano.GetType().DelegateForMap(type);
            mapper(ano, person);

            Assert.AreEqual(ano.Id, person.GetPropertyValue("Id").AsInt());
            Assert.AreEqual(ano.Name, person.GetPropertyValue("Name").AsText());
        }
    }
}