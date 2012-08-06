using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.LinqEx {
    [TestFixture]
    public class IndexableCollectionFixture {
        private const int SampleSize = 500;

        private readonly IndexableCollection<PocoClass> _pocoClasses = new IndexableCollection<PocoClass>();

        [TestFixtureSetUp]
        public void ClassSetUp() {
            _pocoClasses.AddIndex("DemoStringIndexed");

            for(int i = 0; i < SampleSize; i++)
                _pocoClasses.Add(new PocoClass
                                 {
                                     DemoStringNonIndexed = Guid.NewGuid().ToString(),
                                     DemoStringIndexed = Guid.NewGuid().ToString()
                                 });
        }

        [Test]
        public void TestRemoveal() {
            var list = _pocoClasses;

            using(new OperationTimer("Remove Items")) {
                var itemToRemove = list[SampleSize - 1];
                list.Remove(itemToRemove);

                var shouldBeEmpty = list.Where(item => item.DemoStringIndexed == itemToRemove.DemoStringIndexed);
                Assert.IsTrue(shouldBeEmpty.Count() == 0);
            }
        }

        [Test]
        public void DynamicAddIndex() {
            var list = _pocoClasses;

            // 뭘 먼저 하냐에 따라 계속 바뀐다.

            using(new OperationTimer("Non indexed collection")) {
                string searchString = list[list.Count - 1].DemoStringNonIndexed;

                var query = list.Where(item => item.DemoStringNonIndexed == searchString);
                //var query = from item in list
                //            where item.DemoStringNonIndexed == searchString
                //            select item;

                var searched = query.ToList();
                Assert.AreEqual(searched[0].DemoStringNonIndexed, searchString);
            }

            using(new OperationTimer("Indexed collection")) {
                string searchString = list[list.Count - 1].DemoStringIndexed;

                var query = list.Where(item => item.DemoStringIndexed == searchString);
                //var query = from item in list
                //            where item.DemoStringIndexed == searchString
                //            select item;
                var searched = query.ToList();

                Assert.AreEqual(searched[0].DemoStringIndexed, searchString);
            }

            using(new OperationTimer("Non indexed collection")) {
                string searchString = list[list.Count - 1].DemoStringNonIndexed;

                var query = list.Where(item => item.DemoStringNonIndexed == searchString);
                //var query = from item in list
                //            where item.DemoStringNonIndexed == searchString
                //            select item;

                var searched = query.ToList();
                Assert.AreEqual(searched[0].DemoStringNonIndexed, searchString);
            }

            using(new OperationTimer("Indexed collection")) {
                string searchString = list[list.Count - 1].DemoStringIndexed;

                var query = list.Where(item => item.DemoStringIndexed == searchString);
                //var query = from item in list
                //            where item.DemoStringIndexed == searchString
                //            select item;
                var searched = query.ToList();

                Assert.AreEqual(searched[0].DemoStringIndexed, searchString);
            }
        }

        [Test]
        public void DynamicRemoveIndex() {
            var list = _pocoClasses;

            using(new OperationTimer("Indexed")) {
                string searchString = list[list.Count - 1].DemoStringIndexed;
                var query = list.Where(poco => poco.DemoStringIndexed == searchString);
                var result = query.ToList();

                Assert.AreEqual(1, result.Count());
                Assert.AreEqual(searchString, result[0].DemoStringIndexed);
            }

            using(new OperationTimer("NonIndexed")) {
                string searchString = list[list.Count - 1].DemoStringNonIndexed;
                var query = list.Where(poco => poco.DemoStringNonIndexed == searchString);
                var result = query.ToList();

                Assert.AreEqual(1, result.Count());
                Assert.AreEqual(searchString, result[0].DemoStringNonIndexed);
            }

            using(new OperationTimer("Indexed")) {
                string searchString = list[list.Count - 1].DemoStringIndexed;
                var query = list.Where(poco => poco.DemoStringIndexed == searchString);
                var result = query.ToList();

                Assert.AreEqual(1, result.Count());
                Assert.AreEqual(searchString, result[0].DemoStringIndexed);
            }

            using(new OperationTimer("NonIndexed")) {
                string searchString = list[list.Count - 1].DemoStringNonIndexed;
                var query = list.Where(poco => poco.DemoStringNonIndexed == searchString);
                var result = query.ToList();

                Assert.AreEqual(1, result.Count());
                Assert.AreEqual(searchString, result[0].DemoStringNonIndexed);
            }
        }

        [Test]
        public void RemoveBogusIndex() {
            var list = new IndexableCollection<PocoClass>();
            Assert.IsFalse(list.RemoveIndex("ThisIndexDoesNotExist"));
        }

        [Test]
        public void AddBogusIndex() {
            var list = new IndexableCollection<PocoClass>();
            Assert.IsFalse(list.AddIndex("ThisIndexDoesNotExist"));
        }

        [Test]
        public void IndexableAttributeIsRemovableDynamically() {
            var list = new IndexableCollection<DecoratedClass>();
            Assert.IsTrue(list.RemoveIndex("SomeProperty"));
        }

        [Test]
        public void IndexingWorksWhenHashCodeIsAlwaysSame() {
            var list = new IndexableCollection<DecoratedClass>(
                new List<DecoratedClass>
                {
                    new DecoratedClass { HashCodeAlwaysSame = new SillyHashedCodeReturnerClass() },
                    new DecoratedClass { HashCodeAlwaysSame = new SillyHashedCodeReturnerClass() },
                    new DecoratedClass { HashCodeAlwaysSame = new SillyHashedCodeReturnerClass() }
                });

            // reference equals
            Assert.AreEqual(1, list.Where(silly => silly.HashCodeAlwaysSame == list[0].HashCodeAlwaysSame).Count());

            // hashcode equals (equals 호출)
            Assert.AreEqual(3, list.Where(silly => Equals(silly.HashCodeAlwaysSame, list[1].HashCodeAlwaysSame)).Count());
        }

        [Test]
        public void ComplexLeftSide() {
            var list = new IndexableCollection<PocoClass>();

            var subList = list.Where(x => x.DemoStringIndexed + x.DemoStringNonIndexed == "foo");

            Assert.AreEqual(0, subList.Count());
        }

        /// <summary>
        /// 인덱스 자료를 만드는데 시간이 많이 걸리지만, 한번 만든 컬렉션은 속도가 무지 빠르다.
        /// </summary>
        [Test]
        public void JoinTest() {
            int setASize = 10;
            int setBSize = SampleSize;

            int nameCount = Convert.ToInt32(Math.Max(setASize, (double)setBSize) * 0.02);
            if(nameCount == 0)
                nameCount = 1;

            var studentsA = StudentIndexableCollection.CreateMockStudents(setASize, 0, nameCount);
            var studentsB = StudentIndexableCollection.CreateMockStudents(setBSize, 0, nameCount);

            var arrayStudentsA = new Student[studentsA.Count];
            var arrayStudentsB = new Student[studentsB.Count];

            studentsA.CopyTo(arrayStudentsA, 0);
            studentsB.CopyTo(arrayStudentsB, 0);

            Thread.Sleep(0);
            using(new OperationTimer("Indexed")) {
                //var q = from s1 in studentsA
                //        join s2 in studentsB on s1.FirstName equals s2.FirstName
                //        select new { s1.FirstName, s2.LastName };
                var q = studentsA.Join(studentsB,
                                       s1 => s1.FirstName, s2 => s2.FirstName,
                                       (s1, s2) => new { s1.FirstName, s2.LastName });

                Console.WriteLine("Indexed sequece joined results count: " + q.Count());
            }

            Thread.Sleep(0);
            using(new OperationTimer("Non Indexed")) {
                //var q = from s1 in arrayStudentsA
                //        join s2 in arrayStudentsB on s1.FirstName equals s2.FirstName
                //        select new {s1.FirstName, s2.LastName};
                var q = arrayStudentsA.Join(arrayStudentsB,
                                            s1 => s1.FirstName, s2 => s2.FirstName,
                                            (s1, s2) => new { s1.FirstName, s2.LastName });

                Console.WriteLine("Nonindexed sequece joined results count: " + q.Count());
            }

            Thread.Sleep(0);
            using(new OperationTimer("Indexed")) {
                //var q = from s1 in studentsA
                //        join s2 in studentsB on s1.FirstName equals s2.FirstName
                //        select new { s1.FirstName, s2.LastName };
                var q = studentsA.Join(studentsB,
                                       s1 => s1.FirstName, s2 => s2.FirstName,
                                       (s1, s2) => new { s1.FirstName, s2.LastName });

                Console.WriteLine("Indexed sequece joined results count: " + q.Count());
            }

            Thread.Sleep(0);
            using(new OperationTimer("Non Indexed")) {
                //var q = from s1 in arrayStudentsA
                //        join s2 in arrayStudentsB on s1.FirstName equals s2.FirstName
                //        select new {s1.FirstName, s2.LastName};
                var q = arrayStudentsA.Join(arrayStudentsB,
                                            s1 => s1.FirstName, s2 => s2.FirstName,
                                            (s1, s2) => new { s1.FirstName, s2.LastName });

                Console.WriteLine("Nonindexed sequece joined results count: " + q.Count());
            }
        }
    }

    #region << Sample Classes >>

    public class PocoClass {
        public string DemoStringNonIndexed { get; set; }
        public string DemoStringIndexed { get; set; } // 동적으로 Index로 추가한다.
        public int DemoInt { get; set; }
        public Guid ClassGuid { get; set; }
        public double ClassScore { get; set; }
        public string Description { get; set; }
        public DateTime? CreateDate { get; set; }
    }

    internal class DecoratedClass {
        [Indexable]
        public string SomeProperty { get; set; }

        [Indexable]
        public SillyHashedCodeReturnerClass HashCodeAlwaysSame { get; set; }
    }

    internal class SillyHashedCodeReturnerClass {
        public override int GetHashCode() {
            return 0;
        }

        public override bool Equals(object obj) {
            var target = obj as SillyHashedCodeReturnerClass;

            if(target != null)
                return GetHashCode().Equals(target.GetHashCode());

            return false;
        }
    }

    internal class Student {
        private static readonly string[] LastNames = { "김", "이", "박", "최", "정" };
        private static readonly Random Rand = new Random((int)DateTime.Now.Ticks);

        public static Student CreateRandomStudent() {
            return CreateRandomStudent(0, 1000);
        }

        public static Student CreateRandomStudent(int nameRangeStart, int nameRangeEnd) {
            int firstNameIndex = Rand.Next(nameRangeStart, nameRangeEnd);

            var student = new Student
                          {
                              Id = firstNameIndex,
                              FirstName = firstNameIndex.ToString(),
                              LastName = LastNames[Rand.Next(0, LastNames.Length - 1)],
                              GradePointAverage = (Rand.Next(0, 40)) / 10.0
                          };

            return student;
        }

        [Indexable]
        public int Id { get; set; }

        [Indexable]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Indexable]
        public double GradePointAverage { get; set; }
    }

    internal class StudentIndexableCollection : IndexableCollection<Student> {
        public static StudentIndexableCollection CreateMockStudents(int count) {
            return CreateMockStudents(count, 0, 1000);
        }

        public static StudentIndexableCollection CreateMockStudents(int count, int startRange, int endRange) {
            var students = new StudentIndexableCollection();
            for(int i = 0; i < count; i++) {
                var student = Student.CreateRandomStudent(startRange, endRange);
                student.Id = i;
                students.Add(student);
            }
            return students;
        }
    }

    internal class StudentCollection : Collection<Student> {
        public static StudentCollection CreateMockStudents(int count) {
            return CreateMockStudents(count, 0, 1000);
        }

        public static StudentCollection CreateMockStudents(int count, int startRange, int endRange) {
            var students = new StudentCollection();
            for(int i = 0; i < count; i++) {
                var student = Student.CreateRandomStudent(startRange, endRange);
                students.Add(student);
            }
            return students;
        }
    }

    #endregion
}