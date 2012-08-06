using System.Threading.Tasks;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.DataStructures {
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class ObjectPoolTestCase : ParallelismFixtureBase {
        private const int TestCount = 100;

        [Test]
        public void ObjectPoolTest() {
            var pool = new ObjectPool<int>(() => Rnd.Next(1, 99999));

            Parallel.For(0, TestCount, i => pool.PutObject(Rnd.Next(1, 99999)));

            Parallel.For(0, TestCount,
                         i => {
                             var item = pool.GetObject();

                             if(IsDebugEnabled)
                                 log.Debug("item=" + item);
                         });

            Assert.AreEqual(0, pool.Count);
        }

        [Test]
        public void Can_Create_Object_When_Not_Exists() {
            var pool = new ObjectPool<int>(() => {
                                               var result = Rnd.Next(1, 99999);

                                               if(IsDebugEnabled)
                                                   log.Debug("Pool에 객체가 없어서 객체 생성 함수로부터 생성합니다. 생성 객체=" + result);

                                               return result;
                                           });

            // Pool 에 넣지는 않고 꺼내기만 합니다.
            //
            Parallel.For(0, TestCount,
                         i => {
                             var item = pool.GetObject();

                             if(IsDebugEnabled)
                                 log.Debug("Object Pool로부터 얻은 객체=" + item);
                         });

            Assert.AreEqual(0, pool.Count);
        }
    }
}