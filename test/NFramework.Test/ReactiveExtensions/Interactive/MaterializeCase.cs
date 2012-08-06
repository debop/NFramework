using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NSoft.NFramework.ReactiveExtensions.Interactive {
    /// <summary>
    /// Reactive Extensions 중에 EnumerableEx 클래스의 Materialize, Dematerialize 메소드에 대한 예제입니다.
    /// 참고 : http://blogs.bartdesmet.net/blogs/bart/archive/2010/01/07/more-linq-with-system-interactive-functional-fun-and-taming-side-effects.aspx
    /// </summary>
    [TestFixture]
    public class MaterializeFixture : AbstractFixture {
        [Test]
        [ExpectedException(typeof(OutOfMemoryException))]
        public void NotificationTest() {
            var xs1 = new[] { 1, 2 }.Concat(EnumerableEx.Throw<int>(new InvalidOperationException()));
            var xs2 = new[] { 3, 4 }.Concat(EnumerableEx.Throw<int>(new ArgumentException()));
            var xs3 = new[] { 5, 6 }.Concat(EnumerableEx.Throw<int>(new OutOfMemoryException()));
            var xs4 = new[] { 7, 8 }.Concat(EnumerableEx.Throw<int>(new ArgumentException()));

            var xss = new[] { xs1, xs2, xs3, xs4 };
            var xns = xss.Select(xs => xs.Materialize()).Concat();

            var res = xns.Where(xn => {
                                    var isError = xn.Kind == NotificationKind.OnError;
                                    var exception = isError ? ((Notification<int>.OnError)xn).Exception : null;

                                    return (isError == false) || (exception is OutOfMemoryException);
                                });

            res.Dematerialize()
                .Run(Console.WriteLine);
        }

        [Test]
        public void MaterializeAndDematerializeTest() {
            var src =
                Enumerable.Range(1, 2)
                    .Finally(() => Console.WriteLine("Finally inner"))
                    .Concat(EnumerableEx.Throw<int>(new InvalidOperationException()))
                    .Catch((InvalidOperationException _) => new[] { 3, 4 }.Concat(EnumerableEx.Throw<int>(new Exception())))
                    .Finally(() => Console.WriteLine("Finally outer"))
                    .OnErrorResumeNext(new[] { 5, 6 })
                    .Concat(EnumerableEx.Throw<int>(new ArgumentException()));


            src.Materialize()
                .Where(xn => {
                           var isError = xn.Kind == NotificationKind.OnError;
                           var exception = isError ? ((Notification<int>.OnError)xn).Exception : null;
                           return (isError == false);
                       })
                .Dematerialize()
                .Repeat(2)
                .Run(Console.WriteLine);
        }
    }
}