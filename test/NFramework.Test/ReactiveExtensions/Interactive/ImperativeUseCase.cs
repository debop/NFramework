using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NSoft.NFramework.ReactiveExtensions.Interactive {
    /// <summary>
    /// Reactive Extensions의 Interactive 중에 EnumerableEx의 기본적인 Do, Run 메소드에 설명입니다.
    /// 참고: http://blogs.bartdesmet.net/blogs/bart/archive/2009/12/26/more-linq-with-system-interactive-the-ultimate-imperative.aspx
    /// </summary>
    [TestFixture]
    public class ImperativeUseFixture : AbstractFixture {
        private static IEnumerable<int> GetRandomNumbers(int maxValue) {
            while(true)
                yield return Rnd.Next(maxValue);
        }

        /// <summary>
        /// Do 메소드는 쉽게 얘기해서 Enumerator가 열거 할 때 마다 로그 쓰는 것이다. 이를 통해, LINQ 작업의 단계별 내용을 파악할 수 있습니다.
        /// Run 메소드는 모든 시쿼스의 요소에 대해 지정한 delegate를 실행하도록 합니다.
        /// </summary>
        [Test]
        public void Do_Run_Sample() {
            var numbers =
                GetRandomNumbers(100)
                    .Take(10).Do(x => Console.WriteLine("Source->" + x))
                    .Where(x => x % 2 == 0).Do(x => Console.WriteLine("Where->" + x))
                    .OrderBy(x => x).Do(x => Console.WriteLine("OrderBy->" + x))
                    .Select(x => x + 1).Do(x => Console.WriteLine("Select->" + x));


            numbers.Run(Console.WriteLine);
        }
    }
}