using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace NSoft.NFramework.ReactiveExtensions.Interactive {
    /// <summary>
    /// Reactive Extensions 의 EnumerableEx Class의 생성과 관련된 함수들에 대한 예제입니다. (Return, Throw, StartWith, Generate, Defer)
    /// 참고:http://blogs.bartdesmet.net/blogs/bart/archive/2009/12/28/more-linq-with-system-interactive-sequences-under-construction.aspx
    /// </summary>
    [TestFixture]
    public class ConstructorsFixture : AbstractFixture {
        [Test]
        public void LazyConstructor() {
            var source = Enumerable.Range(0, 10);

            // 0~9 중에 홀수만 반환합니다.
            var res = source.SelectMany(i => i % 2 == 0
                                                 ? Enumerable.Empty<int>()
                                                 : EnumerableEx.Return(i));

            res.Run(Console.WriteLine);
        }

        /// <summary>
        /// StartWith()는 시퀀스의 제일 앞에 요소를 추가하는 방법입니다. (여지껏 이런 방법이 없었지요... 필요했나요?)
        /// </summary>
        [Test]
        public void StartWithTest() {
            EnumerableEx.Return(3)
                .StartWith(2)
                .StartWith(1)
                .Run(Console.WriteLine);
        }

        /// <summary>
        /// Enumerable.Aggregate() 메소드의 다양한 적용 방법입니다.
        /// </summary>
        [Test]
        public void Aggregation_Sample() {
            var src = Enumerable.Range(1, 10); //.AsParallel().AsOrdered();

            Console.WriteLine("Numbers = " + string.Join(",", src.Select(x => x.ToString()).ToArray()));

            Console.WriteLine();
            Console.WriteLine("Sum     = " + src.Aggregate(0, (sum, x) => sum + x));
            Console.WriteLine("Product = " + src.Aggregate(1, (sum, x) => sum * x));
            Console.WriteLine("Min     = " + src.Aggregate(int.MaxValue, (min, x) => (x < min) ? x : min));
            Console.WriteLine("Max     = " + src.Aggregate(int.MinValue, (max, x) => (x > max) ? x : max));
            Console.WriteLine("First   = " + src.Aggregate((int?)null, (first, x) => first ?? x));
            Console.WriteLine("Last    = " + src.Aggregate((int?)null, (last, x) => x));

            Console.WriteLine("AllEven = " + src.Aggregate(true, (all, x) => all && (x % 2 == 0)));
            Console.WriteLine("AnyEven = " + src.Aggregate(false, (any, x) => any || (x % 2 == 0)));
        }

        ///// <summary>
        ///// EnumerableEx.Generate() 메소드에 대한 샘플입니다. Enumerable.Range()보다 더 다양하게 Sample Data를 생성할 수 있습니다.
        ///// </summary>
        //[Test]
        //public void Generate_Sample()
        //{
        //    // for(i=fromInclusive; i < toExclusive; i++)
        //    //     yield return i;
        //    Func<int, int, IEnumerable<int>> range =
        //        (fromInclusive, toExclusive) =>
        //        EnumerableEx.Generate<int, int>(0, i => i < toExclusive, i => i + fromInclusive, i => i + 1);

        //    // return empty enumerable
        //    Func<IEnumerable<int>> empty = () => EnumerableEx.Generate<int, int>(0, i => false, i => i, i => i + 1);

        //    // 입력값 하나를 요소로 가지는 열거자를 만듭니다.
        //    Func<int, IEnumerable<int>> @return =
        //        n => EnumerableEx.Generate<int, int>(0, i => i < 1, i => n, i => i + 1);

        //    // 예외를 발생시키는 열거자를 반환하는 함수
        //    Func<Exception, IEnumerable<int>> @throw =
        //        ex => EnumerableEx.Generate<Exception, int>(null,
        //                                                    e => true,
        //                                                    e =>
        //                                                        {
        //                                                            throw ex;
        //                                                            return null;
        //                                                        },
        //                                                    e => 0);

        //    // int 와 int 를 결합해서 하나의 IEnumerable<int>로 만든다. (StartWith 가 더 낫다)
        //    Func<int, int, IEnumerable<int>> cons =
        //        (n, source) => EnumerableEx.Generate<int, int>(0,
        //                                                       i => i < 2,
        //                                                       i => i == 0 ? n : source,
        //                                                       i => i + 1);

        //    // 1 하나만 열거한다.
        //    @return(1).Run(Console.WriteLine);

        //    Console.WriteLine();

        //    // 예외가 발생하도록 하면, 예외를 Catch하고 지정된 22 값을 반환한다.
        //    @throw(new Exception()).Catch((Exception ex) => @return(22)).Run(Console.WriteLine);

        //    Console.WriteLine();

        //    // EnumerableEx.StartWith()와 같은 결과를 도출한다. 1,2,3 을 출력한다.
        //    cons(1, cons(2, cons(3, empty()))).Run(Console.WriteLine);
        //}

        [Test]
        public void Defer_Executions() {
            Func<IEnumerable<DateTime>> @func = () => {
                                                    Console.WriteLine("Factory from Func!");
                                                    return EnumerableEx.Return(DateTime.Now);
                                                };
            var xs = EnumerableEx.Defer(() => {
                                            Console.WriteLine("Factory from Defer");
                                            return EnumerableEx.Return(DateTime.Now);
                                        });

            Console.WriteLine("Start");

            @func().Run(x => Console.WriteLine(x));
            xs.Run(x => Console.WriteLine(x));

            Thread.Sleep(1000);

            @func().Run(x => Console.WriteLine(x));
            xs.Run(x => Console.WriteLine(x));

            Console.WriteLine("End");
        }
    }
}