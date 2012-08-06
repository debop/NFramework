using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace NSoft.NFramework.Parallelism.Partitioners {
    /// <summary>
    /// 사용자가 제공하는 질의에 따라 컬렉션을 분할하여, 조각으로 제공하는 Static Class입니다.
    /// </summary>
    public static class ChunkPartitioner {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// 사용자가 제공하는 다음 조각 크기를 구하는 함수에 따라, 분할을 수행하는 분할자를 제공합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="nextChunkSizeFunc"></param>
        /// <returns></returns>
        public static OrderablePartitioner<TSource> Create<TSource>(IEnumerable<TSource> source, Func<int, int> nextChunkSizeFunc) {
            source.ShouldNotBeNull("source");
            nextChunkSizeFunc.ShouldNotBeNull("nextChunkSizeFunc");

            return new ChunkPartitioner<TSource>(source, nextChunkSizeFunc);
        }

        public static OrderablePartitioner<TSource> Create<TSource>(IEnumerable<TSource> source, int chunkSize) {
            source.ShouldNotBeNull("source");
            chunkSize.ShouldBePositive("chunkSize");

            return new ChunkPartitioner<TSource>(source, chunkSize);
        }

        public static OrderablePartitioner<TSource> Create<TSource>(IEnumerable<TSource> source, int minChunkSize, int maxChunkSize) {
            source.ShouldNotBeNull("source");
            minChunkSize.ShouldBePositive("minChunkSize");
            maxChunkSize.ShouldBePositive("maxChunkSize");

            return new ChunkPartitioner<TSource>(source, minChunkSize, maxChunkSize);
        }
    }

    /// <summary>
    /// 원하는 크기만큼 조각내어 파티션을 만드는 분할자입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class ChunkPartitioner<T> : OrderablePartitioner<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly IEnumerable<T> _source;
        private readonly Func<int, int> _nextChunkSizeFunc;

        #region << Constructors >>

        public ChunkPartitioner(IEnumerable<T> source, Func<int, int> nextChunkSizeFunc)
            : base(true, true, true) {
            source.ShouldNotBeNull("source");
            nextChunkSizeFunc.ShouldNotBeNull("nextChunkSizeFunc");

            _source = source;
            _nextChunkSizeFunc = nextChunkSizeFunc;

            if(IsDebugEnabled)
                log.Debug("임의의 조각 크기로 분할하는 ChunkedPartitioner를 생성했습니다.");
        }

        public ChunkPartitioner(IEnumerable<T> source, int chunkSize)
            : this(source, _ => chunkSize) {
            chunkSize.ShouldBePositive("chunkSize");
        }

        public ChunkPartitioner(IEnumerable<T> source, int minChunkSize, int maxChunkSize)
            : this(source, CreateFuncFromMinAndMax(minChunkSize, maxChunkSize)) {
            minChunkSize.ShouldBePositive("minChunkSize");
            maxChunkSize.ShouldBePositive("maxChunkSize");
            Guard.Assert(maxChunkSize >= minChunkSize,
                         "minChunkSize greater than maxChunkSize. minChunkSize=[{0}], maxChunkSize=[{1}]",
                         minChunkSize, maxChunkSize);
        }

        private static Func<int, int> CreateFuncFromMinAndMax(int minChunkSize, int maxChunkSize) {
            Func<int, int> @nextChunkSizeFunc =
                prev => {
                    if(prev < minChunkSize)
                        return minChunkSize;
                    if(prev > maxChunkSize)
                        return maxChunkSize;

                    int next = prev * 2;
                    if(next > maxChunkSize || next < 0)
                        return maxChunkSize;

                    return next;
                };
            return @nextChunkSizeFunc;
        }

        #endregion

        #region Overrides of OrderablePartitioner<T>

        /// <summary>
        /// 기본 컬렉션을 여러 파티션으로 분할할 수 있는 개체를 만듭니다.
        /// </summary>
        /// <returns>
        /// 내부 데이터 소스에 대한 파티션을 만들 수 있는 개체입니다.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">동적 분할은 이 파티셔너에서 지원되지 않습니다.</exception>
        public override IEnumerable<KeyValuePair<long, T>> GetOrderableDynamicPartitions() {
            return new EnumerableOfEnumerators(this, false);
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
                log.Debug("정렬가능한 파디션의 열거자들을 생성합니다... partitionCount=[{0}]" + partitionCount);

            // Create an array of dynamic partitions and return them
            var partitions = new IEnumerator<KeyValuePair<long, T>>[partitionCount];
            var dynamicPartitions = GetOrderableDynamicPartitions(true);

            for(var i = 0; i < partitionCount; i++)
                partitions[i] = dynamicPartitions.GetEnumerator();

            return partitions;
        }

        private IEnumerable<KeyValuePair<long, T>> GetOrderableDynamicPartitions(bool referenceCountForDisposal) {
            return new EnumerableOfEnumerators(this, referenceCountForDisposal);
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

        #endregion

        #region << EnumerableOfEnumerators Class >>

        /// <summary>
        /// 동적으로 파티션을 생성하기 위한 클래스입니다.
        /// </summary>
        private class EnumerableOfEnumerators : IEnumerable<KeyValuePair<long, T>>, IDisposable {
            #region << logger >>

            private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

            #endregion

            private readonly ChunkPartitioner<T> _parentPartitioner;
            private readonly object _sharedLock = new object();
            private readonly IEnumerator<T> _sharedEnumerator;
            private long _nextSharedIndex;
            private int _activeEnumerators;
            private bool _noMoreElements;
            private bool _isDisposed;
            private readonly bool _referenceCountForDisposal;

            public EnumerableOfEnumerators(ChunkPartitioner<T> parentPartitioner, bool referenceCountForDisposal) {
                parentPartitioner.ShouldNotBeNull("parentPartitioner");

                if(IsDebugEnabled)
                    log.Debug("동적으로 파티션을 생성하기 위한 열거자들을 위한 열거용 컬렉션을 생성합니다... " +
                              "parentPartitioner=[{0}], referenceCountForDisposable=[{1}]", parentPartitioner, referenceCountForDisposal);

                _parentPartitioner = parentPartitioner;
                _sharedEnumerator = parentPartitioner._source.GetEnumerator();
                _nextSharedIndex = -1;
                _referenceCountForDisposal = referenceCountForDisposal;
            }

            public IEnumerator<KeyValuePair<long, T>> GetEnumerator() {
                if(_referenceCountForDisposal) {
                    Interlocked.Increment(ref _activeEnumerators);

                    if(IsDebugEnabled)
                        log.Debug("새로운 열거자를 생성합니다...");
                }
                return new Enumerator(this);
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            private void DisposeEnumerator(Enumerator enumerator) {
                if(_referenceCountForDisposal) {
                    if(Interlocked.Decrement(ref _activeEnumerators) == 0) {
                        if(IsDebugEnabled)
                            log.Debug("공유 Enumerator인 _sharedEnumerator를 Dispose합니다...");

                        _sharedEnumerator.Dispose();
                    }
                }
            }

            public void Dispose() {
                if(_isDisposed)
                    return;

                if(_referenceCountForDisposal == false)
                    _sharedEnumerator.Dispose();

                _isDisposed = true;

                if(IsDebugEnabled)
                    log.Debug("리소스에서 해제했습니다.");
            }

            private class Enumerator : IEnumerator<KeyValuePair<long, T>> {
                #region << logger >>

                private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

                #endregion

                private readonly EnumerableOfEnumerators _parentEnumerable;
                private readonly List<KeyValuePair<long, T>> _currentChunk = new List<KeyValuePair<long, T>>();
                private int _currentChunkCurrentIndex;
                private int _lastRequestedChunkSize;
                private bool _isDisposed;

                public Enumerator(EnumerableOfEnumerators parentEnumerable) {
                    parentEnumerable.ShouldNotBeNull("parentEnumerable");
                    _parentEnumerable = parentEnumerable;
                }

                public bool MoveNext() {
                    ThrowIfDisposed();

                    ++_currentChunkCurrentIndex;
                    if(_currentChunkCurrentIndex >= 0 && _currentChunkCurrentIndex < _currentChunk.Count)
                        return true;

                    // 현재 Chunk 를 다 열거했으므로, 다음 Chunk의 크기를 얻고, 작업을 계속할 지 결정한다.
                    //
                    int nextChunkSize = _parentEnumerable._parentPartitioner._nextChunkSizeFunc(_lastRequestedChunkSize);
                    nextChunkSize.ShouldBePositive("nextChunkSize");

                    _lastRequestedChunkSize = nextChunkSize;

                    // Reset the list
                    _currentChunk.Clear();
                    _currentChunkCurrentIndex = 0;

                    // Size 재조정 (굳이 할 필요 있을까?)
                    if(nextChunkSize > _currentChunk.Capacity)
                        _currentChunk.Capacity = nextChunkSize;

                    // 다음 Chunked 데이터를 구해봅니다.
                    lock(_parentEnumerable._sharedEnumerator) {
                        // 더이상 데이타가 없다면, False를 반환하고 끝낸다.
                        if(_parentEnumerable._noMoreElements) {
                            if(IsDebugEnabled)
                                log.Debug("더 이상 열거할 요소가 없습니다.");

                            return false;
                        }

                        for(var i = 0; i < nextChunkSize; i++) {
                            if(_parentEnumerable._sharedEnumerator.MoveNext() == false) {
                                if(IsDebugEnabled)
                                    log.Debug("더 이상 열거할 요소가 없습니다.");

                                _parentEnumerable._noMoreElements = true;
                                return _currentChunk.Count > 0;
                            }

                            ++_parentEnumerable._nextSharedIndex;
                            _currentChunk.Add(new KeyValuePair<long, T>(_parentEnumerable._nextSharedIndex,
                                                                        _parentEnumerable._sharedEnumerator.Current));
                        }
                    }

                    // 어쨌든, Data가 있다.
                    return true;
                }

                public void Reset() {
                    throw new NotSupportedException("Reset은 지원하지 않습니다.");
                }

                public KeyValuePair<long, T> Current {
                    get {
                        Guard.Assert(_currentChunkCurrentIndex < _currentChunk.Count,
                                     "더 이상 열거할 요소가 없습니다. _currentChunkCurrentIndex=[{0}], _currentChunk.Count=[{1}]",
                                     _currentChunkCurrentIndex, _currentChunk.Count);

                        return _currentChunk[_currentChunkCurrentIndex];
                    }
                }

                object IEnumerator.Current {
                    get { return Current; }
                }

                private void ThrowIfDisposed() {
                    if(_isDisposed)
                        throw new ObjectDisposedException(GetType().FullName);
                }

                public void Dispose() {
                    if(_isDisposed == false) {
                        _parentEnumerable.DisposeEnumerator(this);
                        _isDisposed = true;
                    }
                }
            }
        }

        #endregion
    }
}