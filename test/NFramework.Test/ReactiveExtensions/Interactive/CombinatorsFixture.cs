using System;
using System.Linq;
using NUnit.Framework;

namespace NSoft.NFramework.ReactiveExtensions.Interactive {
    /// <summary>
    /// Reactive Extensions의 Interactive 중에 시퀀스들의 Combination 관련 함수들에 대한 예제입니다. (Concat, Amb, Repeat, Zip, Scan, SelectMany)
    /// 참고: http://blogs.bartdesmet.net/blogs/bart/archive/2009/12/30/more-linq-with-system-interactive-more-combinators-for-your-swiss-army-knife.aspx
    /// </summary>
    [TestFixture]
    public class CombinatorsFixture : AbstractFixture {
        /// <summary>
        /// 여러 시퀀스들을 순서대로 하나의 시퀀스로 만든다.
        /// </summary>
        [Test]
        public void Concat_Sample() {
            // Concat 은 여러 시퀀스들을 하나의 시퀀스로 만듭니다.
            new[]
            {
                Enumerable.Range(1, 2),
                Enumerable.Range(3, 2),
                Enumerable.Range(5, 2)
            }
                .Concat() // 1 ~ 6 까지 열거하도록 한다.
                .Materialize( /* 보기좋게하기 위해 */) // Notification<T>로 구체화한다.
                .Run(System.Console.WriteLine);
        }

        [Test]
        public void Concat_With_Exception() {
            // Concat 은 여러 시퀀스들을 하나의 시퀀스로 만듭니다.
            new[]
            {
                Enumerable.Range(1, 2),
                Enumerable.Range(3, 2).Concat(EnumerableEx.Throw<int>(new Exception())),
                Enumerable.Range(5, 2)
            }
                .Concat() // 1 ~ 6 까지 열거하도록 한다.
                .Catch((Exception ex) => Enumerable.Empty<int>()) // 4까지 열거한 후 예외가 발생하면, 예외를 캐치하여 무시해버립니다.
                .OnErrorResumeNext(Enumerable.Range(5, 2)) // 예외가 발생했을 시에 5부터 재시작 합니다.
                .Materialize() // Notification<T>로 구체화한다.
                .Run(System.Console.WriteLine);
        }

        [Test]
        public void Merge_Sample() {
            // Merge는 Zip과 유사하게 여러 시퀀스에서 순서대로 요소들을 취합합니다. 즉 시퀀스의 첫번째 요소들인 1,3,5 를 열거하고, 두번째 요소들인 2,4,6을 열거합니다.
            new[]
            {
                new[] { 1, 2 }.ToObservable().Do(Console.WriteLine),
                new[] { 3, 4 }.ToObservable(),
                new[] { 5, 6 }.ToObservable()
            }
                .Merge()
                .ToEnumerable()
                .Materialize()
                .Run(Console.WriteLine);
        }

        //
        // NOTE: 기존 EnumerableEx.Amb() 가 Observable.Amb() 로 이동하였음. 사용 방법을 더 찾아봐야 할 것 같음. 
        //
        //[Test]
        //public void Amb_Sample()
        //{
        //    // EnumerableEx.Amb() 는 선착순 선택을 수행합니다. TPL의 Speculative Processing과 개념은 같습니다.

        //    // 지정된 지연 시간 이후에 시퀀스를 제공합니다.
        //    Func<IEnumerable<int>, int, IObservable<int>> @delay =
        //        (src, delayedMsecs) => EnumerableEx.Defer(() =>
        //                                                  {
        //                                                      Thread.Sleep(delayedMsecs);
        //                                                      return src;
        //                                                  }).ToObservable();

        //    // Amb 는 선착순으로, 결과 값을 먼저 제공한 것을 채택합니다.

        //    var src1 = @delay(Enumerable.Range(1, 2), 300);
        //    var src2 = @delay(Enumerable.Range(3, 2), 400);

        //    // 먼저 결과를 반환하는 src1의 것을 가져온다.
        //    src1.Amb(src2).Materialize().Run(Console.WriteLine);

        //    var src3 = @delay(Enumerable.Range(5, 2), 400);
        //    var src4 = @delay(new[] { 7, 8 }, 100);

        //    // 먼저 결과를 반환하는 src4의 것을 가져온다.
        //    Observable.Amb(src3, src4).Materialize().Run(Console.WriteLine);
        //    // src3.Amb(src4).Materialize().Run(Console.WriteLine);
        //}

        /// <summary>
        /// 반복
        /// </summary>
        [Test]
        public void Repeat_Sample() {
            EnumerableEx.Repeat(1).Take(4).Run(Console.WriteLine); // 1이 4번

            EnumerableEx.Repeat(1).Take(4).SequenceEqual(Enumerable.Repeat(1, 4));

            EnumerableEx.Repeat(2, 5).Run(Console.WriteLine); // 2가 5번

            EnumerableEx.Repeat(2, 5).SequenceEqual(EnumerableEx.Repeat(2).Take(5));


            var sequence = new[] { 3, 4 };
            sequence.Repeat().Take(4).Run(Console.WriteLine); // 3, 4, 3, 4 : "3, 4" 쌍이 반복되어 열거되는데, 4개의 요소를 취하므로

            sequence.Repeat().Take(4)
                .SequenceEqual(sequence.Repeat(2));
        }

        /// <summary>
        /// Zip은 두 시퀀스의 요소들을 순서대로 사용하여, 지정한 delegate를 수행합니다. 두 시퀀스 중 하나라도 먼저 끝나면, 연산은 중지합니다.
        /// </summary>
        [Test]
        public void Zip_Sample() {
            Enumerable.Range(1, 26)
                .Zip("abcdefghijklmnopqrstuvwxyz", (i, c) => "alphabet[" + i + "] = " + c)
                .Run(Console.WriteLine);

            Console.WriteLine();

            // 'A'가 ascii 값으로 65이다.

            Enumerable.Range(1, 26)
                .Zip(Enumerable.Range(65, 26), (i, c) => "alphabet[" + i + "] = " + (char)c)
                .Run(Console.WriteLine);
        }

        /// <summary>
        /// Scan은 Aggregate 와는 달리 단계별로 결과를 도출합니다.
        /// </summary>
        [Test]
        public void Scan_Sample() {
            Enumerable.Range(1, 10)
                .Scan((sum, i) => sum + i)
                .Run(Console.WriteLine);

            Console.WriteLine();

            Enumerable.Range(2, 9)
                .Reverse()
                .Scan(3628800, (prod, i) => prod / i)
                .Run(Console.WriteLine);
        }

        [Test]
        public void SelectMany_Sample() {
            // SelectMany는 요소를 1:N 관계로 만들고, 모두 하나의 시퀀스에 담습니다.
            var selectMany = Enumerable.Range(1, 10).SelectMany(x => Enumerable.Repeat(x, 3));

            selectMany.Run(Console.WriteLine);

            Console.WriteLine();

            var sequence = Enumerable.Range(1, 10);
            var sequences = EnumerableEx.Repeat(sequence.ToObservable(), 3);

            var repeated = sequences.Merge().ToEnumerable().Memoize();
            repeated.Run(Console.WriteLine);

            //
            repeated.SequenceEqual(selectMany);
        }
    }
}