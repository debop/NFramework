using System;
using System.Linq;
using NUnit.Framework;

namespace NSoft.NFramework.ReactiveExtensions.Interactive {
    /// <summary>
    /// Reactive Extensions 중에 System.Interactive.dll 에 있는 LINQ 확장 메소드에 대한 예제입니다.
    /// 참고 : http://blogs.bartdesmet.net/blogs/bart/archive/2010/01/07/more-linq-with-system-interactive-functional-fun-and-taming-side-effects.aspx
    /// </summary>
    [TestFixture]
    public class FunctionalFixture : AbstractFixture {
        [Test]
        public void SideEffect() {
            // 난수를 생성하고, 생성된 난수를 Console에 출력합니다.
            var xrs =
                EnumerableEx
                    .Generate(Rnd, rnd => true, rnd => rnd, rnd => rnd.Next(100))
                    .Do(xr => Console.WriteLine("! -> " + xr));

            //var xrs =
            //    EnumerableEx
            //        .Generate(Rnd, rnd => true, rnd => EnumerableEx.Return(rnd.Next(100)), r => r)
            //        .Do(xr => Console.WriteLine("! -> " + xr));

            // 난수 시퀀스를 순서대로 합산한 결과를 Console에 씁니다.
            // 1. 위의 xrs 시퀀스가 하나씩 실행될 때마다, Do 함수가 실행되고, Run 함수가 실행될 것이다.
            // 2. l 값과 r값이 같은 값이 아닙니다!!! 즉 이 코드는 Generate를 20번 수행하는 것입니다.

            // Zip 연산은 두 시퀀스의 같은 인덱스의 요소를 이용하여, 지정된 delegate를 수행한 결과를 반환하는 연산입니다.
            xrs.Zip(xrs, (l, r) => l + r)
                .Take(10)
                .Run(Console.WriteLine);
        }

        [Test]
        public void SideEffect2() {
            // 난수를 생성하고, 생성된 난수를 Console에 출력합니다.
            var xrs =
                EnumerableEx
                    .Generate(Rnd, rnd => true, rnd => rnd, rnd => rnd.Next(100))
                    //var xrs =
                    //    EnumerableEx
                    //        .Generate(Rnd, rnd => EnumerableEx.Return(rnd.Next(100)), r => r)
                    .Take(10)
                    .ToArray();

            // 위에 xrs가 10번 난수 발생하여 ToArray()를 통해 이미 값을 배열로 가지고 있습니다.
            // 그 값이 홀수던 짝수던 같은 값을 더하면 짝수가 됩니다.
            var randomEvens = xrs.Zip(xrs, (l, r) => l + r);

            randomEvens.Run(Console.WriteLine);
            Console.WriteLine("-------------------------");
            randomEvens.Run(Console.WriteLine);
        }

        /// <summary>
        /// Let() 메소드는 시퀀스의 요소를 다른 값으로 대체시키는 작업을 수행합니다.
        /// </summary>
        [Test]
        public void Let_Sample() {
            // 난수를 발생시킨 후, 시퀀스의 요소를 다른 값으로 대체 시킵니다.
            EnumerableEx
                .Generate(Rnd, rnd => true, rnd => rnd, rnd => rnd.Next(1000))
                .Let(r => r.Zip(r, (x1, x2) => x1 + x2))
                .Take(100)
                .Run(Console.WriteLine);
        }

        [Test]
        public void Let_Sample2() {
            Enumerable
                .Range(1, 100)
                .Let(seq => seq.Scan((d, i) => i * i))
                .Run(Console.WriteLine);
        }

        /// <summary>
        /// MemorizeAll()은 시퀀스를 도출하는 Query Expression을 수행하고, 결과 시퀀스를 메모리에 저장해 둔다. 
        /// 다음 요청에도, Query Expression을 수행하는 것이 아니라, 메모리에 저장된 내용을 제공한다.
        /// </summary>
        [Test]
        public void MemorizeAll_Sample() {
            var sequence =
                EnumerableEx
                    .Generate(Rnd, rnd => true, rnd => rnd, rnd => rnd.Next(100))
                    .Do(x => Console.WriteLine("! -> " + x))
                    .MemoizeAll();

            // 위의 MemorizeAll() 가 한번 수행한 값을 모두 메모리에 기억하므로, Zip 수행 시 같은 시퀀스라면, 같은 요소 값끼리 작업을 수행합니다.
            //
            sequence.Do(x => Console.WriteLine("L -> " + x))
                .Zip(sequence.Do(x => Console.WriteLine("R -> " + x)), (x1, x2) => x1 + x2)
                .Take(10)
                .Run(Console.WriteLine);

            Console.WriteLine("-------------------------");

            // 다시 수행한다해도, 기존 값을 사용하므로 위의 결과와 같다.
            //
            sequence.Do(x => Console.WriteLine("L -> " + x))
                .Zip(sequence.Do(x => Console.WriteLine("R -> " + x)), (x1, x2) => x1 + x2)
                .Take(10)
                .Run(Console.WriteLine);

            Console.WriteLine("-----------------");

            Console.WriteLine("Publish");

            // MemorizeAll()은 Publish와 같다.
            EnumerableEx
                .Generate(Rnd, rnd => true, rnd => rnd, rnd => rnd.Next(100))
                .Do(xr => Console.WriteLine("! -> " + xr))
                .Publish(seq => seq.Do(xr => Console.WriteLine("L -> " + xr))
                                    .Zip(seq.Do(xr => Console.WriteLine("R -> " + xr)),
                                         (l, r) => l + r))
                .Take(10)
                .Run(Console.WriteLine);
        }

        //
        // TODO : 재구성해야 함!!!
        //

        [Test]
        public void MemorizeTest() {
            var xrs =
                EnumerableEx
                    .Generate(Rnd, rnd => true, rnd => rnd, rnd => rnd.Next(100))
                    .Do(xr => Console.WriteLine("! -> " + xr))
                    .Memoize(1);

            xrs.Do(xr => Console.WriteLine("L -> " + xr))
                .Zip(xrs.Do(xr => Console.WriteLine("R -> " + xr)), (l, r) => l + r)
                .Take(10)
                .Run(Console.WriteLine);

            Console.WriteLine("-----------------");

            xrs.Do(xr => Console.WriteLine("L -> " + xr))
                .Zip(xrs.Do(xr => Console.WriteLine("R -> " + xr)), (l, r) => l + r)
                .Take(10)
                .Run(Console.WriteLine);
        }

        [Test]
        public void ReplayTest() {
            // Replay 는 주어진 sequence를 Memorize 수행한다.
            // Publish 는 주어진 sequence를 MemorizeAll 을 수행한다.
            var xxx =
                EnumerableEx
                    .Generate(Rnd, rnd => true, rnd => rnd, rnd => rnd.Next(100))
                    .Do(xr => Console.WriteLine("! -> " + xr));


            xxx.Replay(xrs => xrs.Do(xr => Console.WriteLine("L -> " + xr))
                                  .Zip(xrs.Do(xr => Console.WriteLine("R -> " + xr)), (l, r) => l + r),
                       1)
                .Take(10)
                .Run(Console.WriteLine);

            Console.WriteLine("-----------------");

            xxx.Replay(xrs => xrs.Do(xr => Console.WriteLine("L -> " + xr))
                                  .Zip(xrs.Do(xr => Console.WriteLine("R -> " + xr)), (l, r) => l + r),
                       1)
                .Take(10)
                .Run(Console.WriteLine);
        }

        [Test]
        public void PruneTest() {
            // Prune은 sequence를 공유하게 하여, 하나의 요소가 한번만 쓰이게 한다.
            //EnumerableEx.Generate(new Random((int)DateTime.Now.Ticks),
            //                      rnd => EnumerableEx.Return(rnd.Next(100)),
            //                      r => r)
            EnumerableEx.Generate(0, x => true, x => x + 1, x => x + 1)
                .Do(xr => Console.WriteLine("! -> " + xr))
                .Prune(xrs => xrs.Do(xr => Console.WriteLine("L -> " + xr))
                                  .Zip(xrs.Do(xr => Console.WriteLine("R -> " + xr)),
                                       (l, r) => l + r))
                .Take(10)
                .Run(Console.WriteLine);
        }

        [Test]
        public void PruneTest2() {
            Enumerable.Range(0, 10)
                .Prune(xs => from x in xs.Zip(xs, (l, r) => l + r)
                             from y in xs
                             select x + y)
                .Run(Console.WriteLine);

            Console.WriteLine("---------------");

            Enumerable.Range(0, 10)
                .Prune(xs => xs.Zip(xs, (l, r) => l + r).Zip(xs, (l, r) => l + r))
                .Run(Console.WriteLine);
        }
    }
}