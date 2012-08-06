using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.ReactiveExtensions.Reactives {
    /// <summary>
    /// Reactive Extensions 중에 System.Reactive.dll에 있는 IObservable, IObserver에 대한 예제입니다.
    /// 참고: http://codebetter.com/blogs/matthew.podwysocki/archive/2009/11/15/introduction-to-the-reactive-framework-part-iv.aspx
    /// </summary>
    [TestFixture]
    public class ObservableFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        [Test]
        public void FromIEnumerableToObservable() {
            Observable
                .Range(5, 10).Do(x => Console.WriteLine("Start with " + x))
                .StartWith(1)
                .StartWith(2)
                .StartWith(3)
                .Subscribe(Console.WriteLine);
        }

        [Test]
        public void Observable_Generate() {
            Observable
                .Generate(1, x => x <= 10, x => x + 1, x => x)
                .Do(x => Console.WriteLine("Generated item = " + x))
                .Subscribe(Console.WriteLine);
        }

        [Test]
        public void ConvertEnumerableToObservable() {
            var enumerable =
                Enumerable
                    .Range(1, 10)
                    .Concat(EnumerableEx.Throw<int>(new InvalidOperationException("끝났어 임마.")));

            var observable = Observable.Create<int>(observer => {
                                                        With.TryAction(() => {
                                                                           foreach(var item in enumerable)
                                                                               observer.OnNext(item);
                                                                           observer.OnCompleted();
                                                                       },
                                                                       ex => observer.OnError(ex));

                                                        return () => { };
                                                    });

            observable.Subscribe(Console.WriteLine, exn => Console.WriteLine(exn.ToString()));

            Console.WriteLine("=================================");
            Console.WriteLine("Enumerable To Observable directly");

            var observable2 =
                enumerable
                    .ToObservable()
                    .Subscribe(Console.WriteLine, exp => Console.WriteLine(exp.ToString()));
        }

        [Test]
        public void ConvertEnumerableFromObservable() {
            var enumerable = Enumerable.Range(1, 10);
            var observable = enumerable.ToObservable();
            var enumerableAgain = observable.ToEnumerable();
            var enumerator = observable.GetEnumerator();

            Console.WriteLine("enumerableAgain:");
            enumerableAgain.Run(Console.WriteLine);

            Console.WriteLine("-------------------------------");

            Console.WriteLine("enumerator:");
            while(enumerator.MoveNext())
                Console.WriteLine(enumerator.Current);

            enumerator.Dispose();
        }

        [Test]
        public void StringsObservable() {
            var builder = new StringBuilder();
            Enumerable.Range(1, 100).Run(x => builder.AppendLine(x.ToString()));

            var textLines = builder.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            var textObservable = textLines.ToObservable();


            var letterCount = 0;
            var oneCount = 0;
            var nineCount = 0;

            var countLetters = Observer.Create<string>(x => letterCount += x.Length);
            var countOne = Observer.Create<string>(x => oneCount += (x.Contains("1") ? 1 : 0));
            var countNine = Observer.Create<string>(x => nineCount += (x.Contains("9") ? 1 : 0));

            var textLetterSubscribe = textObservable.Subscribe(countLetters);
            textObservable.Subscribe(countOne);
            textObservable.Subscribe(countNine);

            // textObservable.Do(x => Console.WriteLine("Do->" + x)).Subscribe(x => { });
            var textEnumerable = textObservable.ToEnumerable();
            textEnumerable.Run(x => { });

            letterCount.Should("letterCount").Be(192);
            oneCount.Should("oneCount").Be(20);
            nineCount.Should("nineCount").Be(19);

            if(IsDebugEnabled) {
                log.Debug("Letter count = " + letterCount);
                log.Debug("One count = " + oneCount);
                log.Debug("Nine count = " + nineCount);
            }
        }
    }
}