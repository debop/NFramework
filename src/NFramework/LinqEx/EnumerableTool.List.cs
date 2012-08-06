using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.LinqEx {
    public static partial class EnumerableTool {
        /// <summary>
        /// 시퀀스에서 찾고자하는 요소의 첫번째 인덱스를 구한다.
        /// </summary>
        /// <typeparam name="T">Type of element to search</typeparam>
        /// <param name="source">sequence for search</param>
        /// <param name="searchItem">element to search</param>
        /// <returns>index of searched item, if not found element, return -1</returns>
        public static int IndexOf<T>(this IEnumerable<T> source, T searchItem) {
            searchItem.ShouldNotBeNull("searchItem");

            if(source == null)
                return -1;

            int index = 0;
            foreach(T item in source) {
                if(Equals(item, searchItem))
                    return index;
                index++;
            }

            return -1;
        }

        /// <summary>
        /// <paramref name="predicate"/> 조건에 만족하는 첫번째 요소의 인덱스를 반환합니다.
        /// </summary>
        /// <typeparam name="T">Type of element to search</typeparam>
        /// <param name="source">sequence for search</param>
        /// <param name="predicate">검색 조건</param>
        /// <returns>index of searched item, if not found element, return -1</returns>
        public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate) {
            predicate.ShouldNotBeNull("predicate");

            if(source == null)
                return -1;

            var index = 0;
            foreach(T item in source) {
                if(predicate(item))
                    return index;
                index++;
            }
            return -1;
        }

        /// <summary>
        /// 시퀀스에서 찾고자하는 요소의 마지막 인덱스를 구한다.
        /// </summary>
        /// <typeparam name="T">Type of element to search</typeparam>
        /// <param name="source">sequence for search</param>
        /// <param name="searchItem">element to search</param>
        /// <returns>index of searched item, if not found element, return -1</returns>
        public static int LastIndexOf<T>(this IEnumerable<T> source, T searchItem) {
            searchItem.ShouldNotBeNull("searchItem");

            if(source == null)
                return -1;

            int index = source.Count() - 1;

            foreach(T item in source.Reverse()) {
                if(Equals(item, searchItem))
                    return index;

                index--;
            }
            return -1;
        }

        /// <summary>
        /// 시퀀스에서 찾고자하는 요소의 마지막 인덱스를 구한다.
        /// </summary>
        /// <typeparam name="T">Type of element to search</typeparam>
        /// <param name="source">sequence for search</param>
        /// <param name="predicate">filtering function</param>
        /// <returns>index of searched item, if not found element, return -1</returns>
        public static int LastIndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate) {
            predicate.ShouldNotBeNull("predicate");

            if(source == null)
                return -1;

            var index = source.Count() - 1;

            foreach(T item in source.Reverse()) {
                if(predicate(item))
                    return index;
                index--;
            }
            return -1;
        }

        /// <summary>
        /// TInput 수형의 시퀀스를 매핑 함수를 통해 TOutput 시퀀스로 변환합니다.
        /// </summary>
        public static IEnumerable<TOutput> Map<TInput, TOutput>(this IEnumerable<TInput> inputs, Func<TInput, TOutput> mapping) {
            inputs.ShouldNotBeNull("inputs");
            mapping.ShouldNotBeNull("mapping");

            return inputs.Select(mapping);
        }

        /// <summary>
        /// 입력 시퀀스를 도출 함수를 이용하여, 하나의 값을 만들어 냅니다. 집계, 합, 평균 등을 계산할 때 사용합니다.
        /// </summary>
        public static TOutput Reduce<TInput, TOutput>(this IEnumerable<TInput> inputs, Func<TOutput, TInput, TOutput> reduceOperation) {
            reduceOperation.ShouldNotBeNull("reduceOperation");

            return Reduce(inputs, default(TOutput), reduceOperation);
        }

        /// <summary>
        /// 입력 시퀀스를 도출 함수를 이용하여, 하나의 값을 만들어 냅니다. 집계, 합, 평균 등을 계산할 때 사용합니다.
        /// </summary>
        public static TOutput Reduce<TInput, TOutput>(this IEnumerable<TInput> inputs, TOutput seed,
                                                      Func<TOutput, TInput, TOutput> reduceOperation) {
            reduceOperation.ShouldNotBeNull("reduceOperation");
            return inputs.Aggregate(seed, reduceOperation);
        }

        /// <summary>
        /// nullable 수형의 시퀀스를 단순 수형의 시퀀스로 변환한다.
        /// </summary>
        /// <typeparam name="T">Nullable이 가능한 수형</typeparam>
        /// <param name="sequence">nullable 수형을 가진 시퀀스</param>
        /// <param name="defaultValue">요소의 값이 null일 경우 제공할 기본 값</param>
        /// <returns>nullable의 단순 수형의 시퀀스</returns>
        public static IEnumerable<T> GetValueOrDefault<T>(IEnumerable<T?> sequence, T defaultValue = default(T)) where T : struct {
            return sequence.Select(v => v.GetValueOrDefault(defaultValue));
        }

        /// <summary>
        /// 시퀀스를 페이지 크기만큼의 요소로 Paging 처리를 수행합니다. TPL에서는 Partitioning 이라고도 합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Paging<T>(this IEnumerable<T> sequence, int pageSize = 10) {
            sequence.ShouldNotBeNull("sequece");
            pageSize.ShouldBePositive("pageSize");

            if(IsDebugEnabled)
                log.Debug("시퀀스를 Paging 처리하여, Page 별로 반환합니다. pageSize=[{0}]", pageSize);

            var currentIndex = 0;
            var page = sequence.Take(pageSize).AsEnumerable();

            while(page.Any()) {
                yield return page;

                currentIndex += pageSize;
                page = sequence.Skip(currentIndex).Take(pageSize).AsEnumerable();
            }
        }

        /// <summary>
        /// 지정된 시퀀스의 모든 요소에 대해 <paramref name="action"/> 을 수행합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence">입력 시퀀스</param>
        /// <param name="action">실행할 함수</param>
        public static void RunEach<T>(this IEnumerable<T> sequence, Action<T> action) {
            sequence.ShouldNotBeNull("sequence");
            action.ShouldNotBeNull("action");

            foreach(var item in sequence)
                action(item);
        }

        /// <summary>
        /// 지정된 시퀀스의 모든 요소에 대해 <paramref name="action"/> 을 수행하고, 중간마다 경계를 구분짓는 <paramref name="separatorAction"/>을 수행합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence">입력 시퀀스</param>
        /// <param name="action">실행할 메소드</param>
        /// <param name="separatorAction">구분을 위한 메소드 ("," 를 넣는 작업 등)</param>
        public static void RunEach<T>(this IEnumerable<T> sequence, Action<T> action, Action<T> separatorAction) {
            sequence.ShouldNotBeNull("sequence");
            action.ShouldNotBeNull("action");

            bool first = true;

            foreach(var item in sequence) {
                if(first)
                    first = false;

                else if(separatorAction != null)
                    separatorAction(item);

                action(item);
            }
        }

        //! BUG : Enumerable.Select() 와 같은데, 굳이 제작할 필요 없다. 위의 Action<T>를 사용하는 RunEach와 중복되어 제대로 실행 안되는 경우가 생긴다.
        ///// <summary>
        ///// <paramref name="sequence"/> 요소들을 <paramref name="function"/> 의 인자로 호출하고, 반환값들을 컬렉션으로 반환합니다.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <typeparam name="TResult"></typeparam>
        ///// <param name="sequence"></param>
        ///// <param name="function"></param>
        ///// <returns></returns>
        //public static IEnumerable<TResult> RunEach<T, TResult>(this IEnumerable<T> sequence, Func<T, TResult> function)
        //{
        //    sequence.ShouldNotBeNull("sequence");
        //    function.ShouldNotBeNull("runFunc");

        //    return sequence.Select(x => function(x));

        //    //var results = new List<TResult>();

        //    //foreach(var item in sequence)
        //    //    results.Add(function(item));

        //    //return results;
        //}

        /// <summary>
        /// 지정된 시퀀스의 모든 요소에 대해 <paramref name="action"/> 을 비동기 방식으로 수행합니다.
        /// </summary>
        /// <typeparam name="T">입력 인자의 수형</typeparam>
        /// <param name="sequence">입력 시퀀스</param>
        /// <param name="action">실행할 메소드</param>
        /// <returns>실행 결과 값의 시퀀스</returns>
        public static void RunEachAsync<T>(this IEnumerable<T> sequence, Action<T> action) {
            sequence.ShouldNotBeNull("sequence");
            action.ShouldNotBeNull("action");

            if(IsDebugEnabled)
                log.Debug("지정된 시퀀스의 모든 요소에 대해 action을 비동기 방식으로 수행합니다.");


            Task.WaitAll(sequence
                             .Select(item =>
                                     Task.Factory.StartNew(() => action(item),
                                                           TaskCreationOptions.PreferFairness))
                             .ToArray());
        }

        /// <summary>
        /// <paramref name="sequence"/> 요소들을 <paramref name="func"/> 의 인자로 호출하고, 반환값들을 컬렉션으로 반환합니다.
        /// </summary>
        /// <typeparam name="T">입력 인자의 수형</typeparam>
        /// <typeparam name="TResult">함수 반환 수형</typeparam>
        /// <param name="sequence">입력 시퀀스</param>
        /// <param name="func">실행할 함수</param>
        /// <returns>실행 결과 값의 시퀀스</returns>
        public static IEnumerable<TResult> RunEachAsync<T, TResult>(this IEnumerable<T> sequence, Func<T, TResult> func) {
            sequence.ShouldNotBeNull("sequence");
            func.ShouldNotBeNull("func");

            if(IsDebugEnabled)
                log.Debug("지정된 시퀀스의 모든 요소에 대해 function을 비동기 방식으로 수행합니다.");

            return sequence.Select(x => With.TryFunctionAsync(() => Task.Factory.StartNew(() => func(x)).Result));
        }

        /// <summary>
        /// 차집합 ( first - second )
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static IEnumerable<T> Different<T>(this IEnumerable<T> first, IEnumerable<T> second) {
            if(first.IsEmptySequence() || second.IsEmptySequence())
                return Enumerable.Empty<T>();

            return first.Except(second);
        }

        /// <summary>
        /// 여집합 ( (first-second) U (second-first) )
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static IEnumerable<T> Complement<T>(this IEnumerable<T> first, IEnumerable<T> second) {
            var d1 = first.Different(second);
            var d2 = second.Different(first);

            return d1.Union(d2);
        }

        /// <summary>
        /// value가 시퀀스의 요소로서 존재하는지 판단한다.
        /// </summary>
        /// <typeparam name="T">요소의 수형. IEquatable{T}를 구현한 수형이어야 한다</typeparam>
        /// <param name="value">검사할 요소</param>
        /// <param name="values">컬렉션</param>
        /// <returns>지정된 컬렉션에 요소가 존재한다면 True, 아니면 False를 반환한다.</returns>
        public static bool In<T>(this T value, params T[] values) where T : IEquatable<T> {
            if(values == null || values.Length == 0)
                return false;

            return In(value, (IEnumerable<T>)values);
        }

        /// <summary>
        /// value가 시퀀스의 요소로 존재하는지 판단한다.
        /// </summary>
        /// <typeparam name="T">요소의 수형</typeparam>
        /// <param name="value">검사할 요소</param>
        /// <param name="values">컬렉션</param>
        /// <returns>지정된 컬렉션에 요소가 존재한다면 True, 아니면 False를 반환한다.</returns>
        public static bool In<T>(this T value, IEnumerable<T> values) where T : IEquatable<T> {
            if(values == null)
                return false;

            return values.Any(v => Equals(v, value));
        }

        /// <summary>
        /// 시퀀스에 <paramref name="values"/>에 있는 요소와 같은 값을 가지는 요소가 하나라도 있으면 true를 반환한다.
        /// </summary>
        /// <typeparam name="T">시퀀스 요소의 수형</typeparam>
        /// <param name="sequence">시퀀스</param>
        /// <param name="values">검사할 값들</param>
        /// <returns>시퀀스 요소중 검사할 값과 일치하는 것이 하나라도 있으면 True를 반환한다.</returns>
        public static bool In<T>(this IEnumerable<T> sequence, params T[] values) {
            if(values == null || values.Length == 0)
                return false;

            return values.Any(value => sequence.ItemExists(item => item.Equals(value)));
        }

        /// <summary>
        /// 시퀀스에 <paramref name="values"/>에 있는 요소와 같은 값을 가지는 요소가 하나라도 있으면 true를 반환한다.
        /// </summary>
        /// <typeparam name="T">시퀀스 요소의 수형</typeparam>
        /// <param name="sequence">시퀀스</param>
        /// <param name="values">검사할 값들</param>
        /// <returns>시퀀스 요소중 검사할 값과 일치하는 것이 하나라도 있으면 True를 반환한다.</returns>
        public static bool In<T>(this IEnumerable<T> sequence, IEnumerable<T> values) {
            return values.Any(value => sequence.ItemExists(item => item.Equals(value)));
        }

        /// <summary>
        /// source 가 subset의 Sequencial한 superset인지 판단한다. (즉 요소의 순서 및 연속성도 같아야 한다)
        /// </summary>
        /// <remarks>See http://weblogs.asp.net/okloeten/archive/2008/04/22/6121373.aspx for more details.</remarks>
        public static bool SequenceSuperset<T>(this IEnumerable<T> source,
                                               IEnumerable<T> subset,
                                               Func<T, T, bool> equalityComparer = null) {
            source.ShouldNotBeNull("source");
            subset.ShouldNotBeNull("subset");
            // equalityComparer.ShouldNotBeNull("equalityComparer");

            equalityComparer = equalityComparer ?? EqualityComparer<T>.Default.Equals;

            using(IEnumerator<T> big = source.GetEnumerator(), small = subset.GetEnumerator()) {
                big.Reset();
                small.Reset();

                while(big.MoveNext()) {
                    if(!small.MoveNext())
                        return true;

                    if(equalityComparer(big.Current, small.Current) == false) {
                        // 비교 실패시, small을 reset한다.
                        small.Reset();

                        small.MoveNext();

                        if(equalityComparer(big.Current, small.Current) == false)
                            small.Reset();
                    }
                }
                // 둘다 끝이라면, small이 big의 마직막요소라는 뜻
                if(small.MoveNext() == false)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// <paramref name="fromInclusive"/> ~ <paramref name="toExclusive"/> 범위의 값을 <paramref name="step"/> 단위로 열거합니다.
        /// </summary>
        /// <param name="fromInclusive">하한 값(포함)</param>
        /// <param name="toExclusive">상한 값(제외)</param>
        /// <param name="step">단계 (1 이상)</param>
        /// <returns></returns>
        public static IEnumerable<int> Step(int fromInclusive, int toExclusive, int step = 1) {
            fromInclusive.ShouldBeLessOrEqual(toExclusive, "fromInclusive");
            toExclusive.ShouldBeGreaterOrEqual(fromInclusive, "toExclusive");
            step.ShouldBeGreaterThan(0, "step");

            for(var i = fromInclusive; i < toExclusive; i += step)
                yield return i;
        }

        /// <summary>
        /// <paramref name="fromInclusive"/> ~ <paramref name="toExclusive"/> 범위의 값을 <paramref name="step"/> 단위로 열거합니다.
        /// </summary>
        /// <param name="fromInclusive">하한 값(포함)</param>
        /// <param name="toExclusive">상한 값(제외)</param>
        /// <param name="step">단계 (1 이상)</param>
        /// <returns></returns>
        public static IEnumerable<long> Step(long fromInclusive, long toExclusive, long step = 1L) {
            fromInclusive.ShouldBeLessOrEqual(toExclusive, "fromInclusive");
            toExclusive.ShouldBeGreaterOrEqual(fromInclusive, "toExclusive");
            step.ShouldBeGreaterThan(0L, "step");

            for(var i = fromInclusive; i < toExclusive; i += step)
                yield return i;
        }

        /// <summary>
        /// <paramref name="fromInclusive"/> ~ <paramref name="toExclusive"/> 범위의 값을 <paramref name="step"/> 단위로 열거합니다.
        /// </summary>
        /// <param name="fromInclusive">하한 값(포함)</param>
        /// <param name="toExclusive">상한 값(제외)</param>
        /// <param name="step">단계 (1 이상)</param>
        /// <returns></returns>
        public static IEnumerable<float> Step(float fromInclusive, float toExclusive, float step = 1.0f) {
            fromInclusive.ShouldBeLessOrEqual(toExclusive, "fromInclusive");
            toExclusive.ShouldBeGreaterOrEqual(fromInclusive, "toExclusive");
            step.ShouldBeGreaterThan(0f, "step");

            for(var i = fromInclusive; i < toExclusive; i += step)
                yield return i;
        }

        /// <summary>
        /// <paramref name="fromInclusive"/> ~ <paramref name="toExclusive"/> 범위의 값을 <paramref name="step"/> 단위로 열거합니다.
        /// </summary>
        /// <param name="fromInclusive">하한 값(포함)</param>
        /// <param name="toExclusive">상한 값(제외)</param>
        /// <param name="step">단계 (1 이상)</param>
        /// <returns></returns>
        public static IEnumerable<double> Step(double fromInclusive, double toExclusive, double step = 1.0) {
            fromInclusive.ShouldBeLessOrEqual(toExclusive, "fromInclusive");
            toExclusive.ShouldBeGreaterOrEqual(fromInclusive, "toExclusive");
            step.ShouldBeGreaterThan(0d, "step");

            for(var i = fromInclusive; i < toExclusive; i += step)
                yield return i;
        }

        /// <summary>
        /// <paramref name="fromInclusive"/> ~ <paramref name="toExclusive"/> 범위의 값을 <paramref name="step"/> 단위로 열거합니다.
        /// </summary>
        /// <param name="fromInclusive">하한 값(포함)</param>
        /// <param name="toExclusive">상한 값(제외)</param>
        /// <param name="step">단계 (1 이상)</param>
        /// <returns></returns>
        public static IEnumerable<decimal> Step(decimal fromInclusive, decimal toExclusive, decimal step = 1) {
            fromInclusive.ShouldBeLessOrEqual(toExclusive, "fromInclusive");
            toExclusive.ShouldBeGreaterOrEqual(fromInclusive, "toExclusive");
            step.ShouldBeGreaterThan(0m, "step");

            for(var i = fromInclusive; i < toExclusive; i += step)
                yield return i;
        }

        /// <summary>
        /// 시퀀스의 요소를 문자열로 표현하고, 그 문자열로 표현된 요소를 구분자로 결합하여 하나의 문자열로 만듭니다.
        /// </summary>
        /// <example>
        ///	<code>
        ///     int[] numbers = new int[] { 1,2,3,4,5 };
        ///     var joinedText = numbers.AsJoinedText("|");  // joinedText is "1|2|3|4|5"
        /// </code>
        /// </example>
        public static string AsJoinedText<T>(this IEnumerable<T> sequence, string separator = ",") {
            return
                sequence
                    .Select(item => item.AsText())
                    .Join(separator);
        }

        /// <summary>
        /// 시퀀스의 요소를 문자열로 표현하고, 그 문자열로 표현된 요소를 구분자로 결합하여 하나의 문자열로 만듭니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence">시퀀스</param>
        /// <param name="toStringFunc">요소를 문자열로 표현하는 함수</param>
        /// <param name="delimiter">구분자</param>
        /// <returns></returns>
        /// <example>
        ///	<code>
        ///     int[] numbers = new int[] { 1,2,3,4,5 };
        ///     var joinedText = numbers.AsJoinedText("|");  // joinedText is "1|2|3|4|5"
        /// </code>
        /// </example>
        public static string AsJoinedText<T>(this IEnumerable<T> sequence, Func<T, string> toStringFunc = null, string delimiter = ",") {
            if(sequence == null || sequence.IsEmptySequence())
                return string.Empty;

            toStringFunc = toStringFunc ?? ((x) => x.AsText());

            var builder = new StringBuilder();

            bool isFirst = true;
            foreach(var item in sequence) {
                if(!isFirst)
                    builder.Append(delimiter);

                builder.Append(toStringFunc(item));

                if(isFirst)
                    isFirst = false;
            }

            return builder.ToString();
        }
    }
}