using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// 멀티쓰레드에 걸쳐져 도출되는 데이터를 하나로 취합하기 위한 변수입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("Count={_values.Count}")]
    [DebuggerTypeProxy(typeof(ReductionVariable_DebugView<>))]
    [Serializable]
    public class ReductionVariable<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private readonly Func<T> _seedFactory;
        private readonly ThreadLocal<StrongBox<T>> _threadLocal;
        private readonly ConcurrentQueue<StrongBox<T>> _values = new ConcurrentQueue<StrongBox<T>>();

        /// <summary>
        /// 생성자
        /// </summary>
        public ReductionVariable() {
            _threadLocal = new ThreadLocal<StrongBox<T>>(CreateValue);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="seedFactory"></param>
        public ReductionVariable(Func<T> seedFactory) : this() {
            seedFactory.ShouldNotBeNull("seedFactory");
            _seedFactory = seedFactory;
        }

        /// <summary>
        /// Creates a value for the current thread and stores it in the central list of values.
        /// </summary>
        /// <returns></returns>
        private StrongBox<T> CreateValue() {
            var item = new StrongBox<T>(_seedFactory != null ? _seedFactory() : default(T));
            _values.Enqueue(item);
            return item;
        }

        /// <summary>
        /// Value for the current thread.
        /// </summary>
        public T Value {
            get { return _threadLocal.Value.Value; }
            set { _threadLocal.Value.Value = value; }
        }

        /// <summary>
        /// 이 인스턴스를 사용하는 모든 쓰레드의 값들을 가져옵니다.
        /// </summary>
        public IEnumerable<T> Values {
            get { return _values.Select(item => item.Value); }
        }

        /// <summary>
        /// <paramref name="function"/>을 사용하여 다중 스레드의 값들을 취합합니다.
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public T Reduce(Func<T, T, T> function) {
            function.ShouldNotBeNull("function");
            return Values.Aggregate(function);
        }

        /// <summary>
        /// <paramref name="seed"/>값을 초기값으로, <paramref name="function"/> 을 사용하여 다중 스레드의 값들을 취합합니다.
        /// </summary>
        /// <typeparam name="TAccumulate"></typeparam>
        /// <param name="seed"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public TAccumulate Reduce<TAccumulate>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> function) {
            function.ShouldNotBeNull("function");
            return Values.Aggregate(seed, function);
        }
    }

    /// <summary>
    /// Debug view for the reductino variable
    /// </summary>
    internal sealed class ReductionVariable_DebugView<T> {
        private readonly ReductionVariable<T> _variable;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="variable"></param>
        public ReductionVariable_DebugView(ReductionVariable<T> variable) {
            _variable = variable;
        }

        /// <summary>
        /// 디버그 시에 스레드별로 관리하는 값들을 하나의 배열로 보여줍니다.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Values {
            get { return (_variable != null) ? _variable.Values.ToArray() : new T[0]; }
        }
    }
}