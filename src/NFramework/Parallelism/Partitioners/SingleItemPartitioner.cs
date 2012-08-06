using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NSoft.NFramework.Parallelism.Partitioners {
    /// <summary>
    /// 하나의 단위로 나누는 Partitioner입니다.
    /// </summary>
    public static class SingleItemPartitioner {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static OrderablePartitioner<T> Create<T>(IEnumerable<T> source) {
            source.ShouldNotBeNull("source");

            if(source is IList<T>)
                return new SingleItemIListPartitioner<T>((IList<T>)source);

            return new SingleItemEnumerablePartitioner<T>(source);
        }

        /// <summary>
        /// 한번에 하나의 요소만을 열거하는 분할자입니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private sealed class SingleItemEnumerablePartitioner<T> : OrderablePartitioner<T> {
            private readonly IEnumerable<T> _source;

            internal SingleItemEnumerablePartitioner(IEnumerable<T> source)
                : base(true, false, true) {
                source.ShouldNotBeNull("source");
                _source = source;
            }

            public override bool SupportsDynamicPartitions {
                get { return true; }
            }

            public override IList<IEnumerator<KeyValuePair<long, T>>> GetOrderablePartitions(int partitionCount) {
                partitionCount.ShouldBePositive("partitionCount");

                var dynamicPartitioner = new DynamicGenerator(_source.GetEnumerator(), false);

                return
                    Enumerable.Range(0, partitionCount)
                        .Select(i => dynamicPartitioner.GetEnumerator())
                        .ToList();
            }

            public override IEnumerable<KeyValuePair<long, T>> GetOrderableDynamicPartitions() {
                return new DynamicGenerator(_source.GetEnumerator(), true);
            }

            #region << DynamicGenerator Class >>

            private class DynamicGenerator : IEnumerable<KeyValuePair<long, T>>, IDisposable {
                /// <summary>
                /// 모든 Partition에서 공유되는 열거자p
                /// </summary>
                private readonly IEnumerator<T> _sharedEnumerator;

                private long _nextAvailablePosition;
                private int _remainingPartitions;
                private bool _isDisposed;

                public DynamicGenerator(IEnumerator<T> sharedEnumerator, bool requiresDisposal) {
                    sharedEnumerator.ShouldNotBeNull("sharedEnumerator");

                    _sharedEnumerator = sharedEnumerator;
                    _nextAvailablePosition = -1;
                    _remainingPartitions = requiresDisposal ? 1 : 0;
                }

                void IDisposable.Dispose() {
                    if(_isDisposed)
                        return;

                    if(Interlocked.Decrement(ref _remainingPartitions) == 0) {
                        _sharedEnumerator.Dispose();
                        _isDisposed = true;
                    }
                }

                public IEnumerator<KeyValuePair<long, T>> GetEnumerator() {
                    Interlocked.Increment(ref _remainingPartitions);
                    return GetEnumeratorCore();
                }

                IEnumerator IEnumerable.GetEnumerator() {
                    return GetEnumerator();
                }

                private IEnumerator<KeyValuePair<long, T>> GetEnumeratorCore() {
                    if(IsDebugEnabled)
                        log.Debug("새로운 Partition을 생성합니다.");

                    try {
                        while(true) {
                            T nextItem;
                            long position;
                            lock(_sharedEnumerator) {
                                if(_sharedEnumerator.MoveNext()) {
                                    position = _nextAvailablePosition++;
                                    nextItem = _sharedEnumerator.Current;
                                }
                                else
                                    yield break;
                            }
                            yield return new KeyValuePair<long, T>(position, nextItem);
                        }
                    }
                    finally {
                        if(Interlocked.Decrement(ref _remainingPartitions) == 0)
                            _sharedEnumerator.Dispose();
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// 한번에 하나의 요소만을 제공하는 분할자입니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private sealed class SingleItemIListPartitioner<T> : OrderablePartitioner<T> {
            private readonly IList<T> _source;

            internal SingleItemIListPartitioner(IList<T> source)
                : base(true, false, true) {
                source.ShouldNotBeNull("source");
                _source = source;
            }

            /// <summary>
            /// 추가 파티션을 동적으로 만들 수 있는지 여부를 가져옵니다.
            /// </summary>
            /// <returns>
            /// <see cref="T:System.Collections.Concurrent.Partitioner`1"/>에서 요청을 받은 경우 동적으로 파티션을 만들 수 있으면 true이고, 
            /// <see cref="T:System.Collections.Concurrent.Partitioner`1"/>에서 정적으로만 파티션을 할당할 수 있으면 false입니다.
            /// </returns>
            public override bool SupportsDynamicPartitions {
                get { return true; }
            }

            /// <summary>
            /// 기본 컬렉션을 지정된 개수의 정렬할 수 있는 파티션으로 분할합니다.
            /// </summary>
            /// <returns>
            /// <paramref name="partitionCount"/> 열거자가 포함된 목록입니다.
            /// </returns>
            /// <param name="partitionCount">만들 파티션의 수입니다.</param>
            public override IList<IEnumerator<KeyValuePair<long, T>>> GetOrderablePartitions(int partitionCount) {
                partitionCount.ShouldBePositive("partitionCount");

                if(IsDebugEnabled)
                    log.Debug("지정된 갯수의 Partition을 생성하여 List로 반환합니다. partitionCount=[{0}]", partitionCount);

                var dynamicPartitioner = GetOrderableDynamicPartitions();

                return
                    Enumerable.Range(0, partitionCount)
                        .Select(i => dynamicPartitioner.GetEnumerator())
                        .ToList();
            }

            /// <summary>
            /// 기본 컬렉션을 여러 파티션으로 분할할 수 있는 개체를 만듭니다.
            /// </summary>
            /// <returns>
            /// 내부 데이터 소스에 대한 파티션을 만들 수 있는 개체입니다.
            /// </returns>
            /// <exception cref="T:System.NotSupportedException">동적 분할은 이 파티셔너에서 지원되지 않습니다.</exception>
            public override IEnumerable<KeyValuePair<long, T>> GetOrderableDynamicPartitions() {
                return GetOrderableDynamicPartitionsCore(_source, new StrongBox<int>(0));
            }

            private static IEnumerable<KeyValuePair<long, T>> GetOrderableDynamicPartitionsCore(IList<T> source,
                                                                                                StrongBox<int> nextIteration) {
                while(true) {
                    var iteration = Interlocked.Increment(ref nextIteration.Value) - 1;

                    if(iteration >= 0 && iteration < source.Count)
                        yield return new KeyValuePair<long, T>(iteration, source[iteration]);
                    else
                        yield break;
                }
            }
        }
    }
}