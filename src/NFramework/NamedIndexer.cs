using System;

namespace NSoft.NFramework {
    /// <summary>
    /// C# 처럼 기본 Indexer 말고, 이름을 가진 Indexer를 제공하기 위해 사용된다.
    /// </summary>
    /// <typeparam name="TResult">속성의 값의 형식</typeparam>
    /// <typeparam name="TIndex">Indexer의 index의 형식</typeparam>
    [Serializable]
    public class NamedIndexer<TResult, TIndex> {
        private readonly Func<TIndex, TResult> _getter;
        private readonly Action<TIndex, TResult> _setter;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="getter">Indexer 요소 조회 함수</param>
        /// <param name="setter">Indexer 요소 설정 함수</param>
        public NamedIndexer(Func<TIndex, TResult> getter, Action<TIndex, TResult> setter) {
            getter.ShouldNotBeNull("getter");
            setter.ShouldNotBeNull("setter");

            _getter = getter;
            _setter = setter;
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TResult this[TIndex index] {
            get { return _getter(index); }
            set { _setter(index, value); }
        }
    }

    /// <summary>
    /// Allow to add named getter indexers
    /// </summary>
    /// <typeparam name="TResult">속성의 값의 형식</typeparam>
    /// <typeparam name="TIndex">Indexer의 index의 형식</typeparam>
    [Serializable]
    public class NamedIndexerGetter<TResult, TIndex> {
        private readonly Func<TIndex, TResult> _getter;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="getter">Index의 속성 값을 얻기 위한 함수</param>
        public NamedIndexerGetter(Func<TIndex, TResult> getter) {
            getter.ShouldNotBeNull("getter");

            _getter = getter;
        }

        ///<summary>
        /// Indexer
        ///</summary>
        ///<param name="index"></param>
        public TResult this[TIndex index] {
            get { return _getter(index); }
        }
    }

    /// <summary>
    /// Allow to add named setter indexers
    /// </summary>
    /// <typeparam name="TResult">속성의 값의 형식</typeparam>
    /// <typeparam name="TIndex">Indexer의 index의 형식</typeparam>
    [Serializable]
    public class NamedIndexerSetter<TResult, TIndex> {
        private readonly Action<TIndex, TResult> _setter;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="setter">Indexer 요소 설정 함수</param>
        public NamedIndexerSetter(Action<TIndex, TResult> setter) {
            setter.ShouldNotBeNull("setter");
            _setter = setter;
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TResult this[TIndex index] {
            set { _setter(index, value); }
        }
    }
}