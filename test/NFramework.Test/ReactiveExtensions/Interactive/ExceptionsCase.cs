using System;
using System.Linq;
using NUnit.Framework;

namespace NSoft.NFramework.ReactiveExtensions.Interactive {
    /// <summary>
    /// Reactive Extensions 의 EnumerableEx Class의 Exception관련 메소드에 대한 예제입니다. (Catch, Finally, OnErrorResumeNext, Using, Retry)
    /// 참고:http://blogs.bartdesmet.net/blogs/bart/archive/2009/12/27/more-linq-with-system-interactive-exceptional-exception-handling.aspx
    /// </summary>
    [TestFixture]
    public class ExceptionsFixture : AbstractFixture {
        /// <summary>
        /// LINQ 작업 중, 특정 요소가 예외를 발생시킬 수 있습니다. 이때 Try/Catch/Finally 구문을 사용하여, 대처할 수 있습니다.
        /// foreach 구문을 사용하는 것과 달리 LINQ Expression은 지연된 실행이 가능하므로 다른 결과를 나타낼 수 있습니다.
        /// </summary>
        [Test]
        public void ExceptionHandling() {
            // 9, 8, 7 ... 0 으로 가면서, 100/9, 100/8, ... 100/0 을 실행하는데, 
            // 0으로 나누기에서 예외가 발생할 것이고, 그 예외를 Catch해서 Skip해 버리도록 한다.
            Enumerable.Range(0, 10)
                .Reverse()
                .Concat(EnumerableEx.Throw<int>(new Exception()))
                .Select(x => (int)(100 / x))
                .Catch((Exception ex) => Enumerable.Empty<int>()) // 예외가 발생하면, Skip하도록 한다.
                .Finally(() => Console.WriteLine("Finally"))
                .Run(Console.WriteLine);
        }

        /// <summary>
        /// Retry(N)은 앞의 연산이 예외가 발생하지 않을 때까지, 최대 N번 만큼 재시도를 수행합니다.
        /// </summary>
        [Test]
        public void Retry_Sample() {
            // 여기서 1,2,3 을 열거하다가 예외가 발생하므로, 1,2,3 열거를 예외가 발생하지 않을 때까지 최대 5번 재시도 합니다.
            Enumerable.Range(1, 3)
                .Concat(EnumerableEx.Throw<int>(new Exception()))
                .Retry(5)
                .Run(Console.WriteLine);
        }
    }
}