using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// 여러 개의 Mapping를 순차적으로 수행합니다. 이것이 Pipeline을 통과하면서, Data가 변화되는 것과 유사하다고 해서 Pipeline이라 합니다.
    /// 이 클래스는 데이터에 대한 pipeline 작업을 병렬로 수행합하도록 해줍니다.
    /// </summary>
    /// <typeparam name="TInput">입력 변수의 수형</typeparam>
    /// <typeparam name="TOutput">출력 변수의 수형</typeparam>
    public class Pipeline<TInput, TOutput> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly Func<TInput, TOutput> _stageFunc;
        private readonly int _degreeOfParallelism;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="degreeOfParallelism">병렬 차원</param>
        internal Pipeline(int degreeOfParallelism) : this(null, degreeOfParallelism) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="func">매핑 함수</param>
        /// <param name="degreeOfParallelism">병렬 차원</param>
        internal Pipeline(Func<TInput, TOutput> func, int degreeOfParallelism) {
            _stageFunc = func;
            _degreeOfParallelism = Math.Max(1, degreeOfParallelism);

            if(IsDebugEnabled)
                log.Debug("Pileline 인스턴스를 생성했습니다. degreeOfParallelism=[{0}]", _degreeOfParallelism);
        }

        /// <summary>
        /// 여러 단계의 매핑을 거쳐야 할 때, 매핑함수 chain을 만들도록 해준다.
        /// </summary>
        /// <typeparam name="TNextOutput"></typeparam>
        /// <param name="nextFunc"></param>
        /// <param name="degreeOfParallelism"></param>
        /// <returns></returns>
        public Pipeline<TInput, TNextOutput> Next<TNextOutput>(Func<TOutput, TNextOutput> nextFunc, int degreeOfParallelism = 1) {
            nextFunc.ShouldNotBeNull("nextFunc");
            degreeOfParallelism.ShouldBePositive("degreeOfParallelism");

            return new InternalPipeline<TNextOutput>(this, nextFunc, degreeOfParallelism);
        }

        /// <summary>
        /// pipeline data processing을 실제 처리하고, 결과를 반환합니다.
        /// </summary>
        public IEnumerable<TOutput> Process(IEnumerable<TInput> source) {
            return Process(source, new CancellationToken());
        }

        /// <summary>
        /// pipeline data processing을 실제 처리하고, 결과를 반환합니다.
        /// </summary>
        public IEnumerable<TOutput> Process(IEnumerable<TInput> source, CancellationToken cancellationToken) {
            source.ShouldNotBeNull("source");

            return With.TryFunctionAsync(() => ProcessNoArgValidation(source, cancellationToken),
                                         () => Enumerable.Empty<TOutput>());
        }

        /// <summary>
        /// pipeline data processing을 실제 처리하고, 결과를 반환합니다.
        /// </summary>
        private IEnumerable<TOutput> ProcessNoArgValidation(IEnumerable<TInput> source, CancellationToken cancellationToken) {
            if(IsDebugEnabled)
                log.Debug("입력 정보를 처리합니다... cancellationToken=[{0}]", cancellationToken);

            using(var outputs = new BlockingCollection<TOutput>()) {
                var processingTask =
                    Task.Factory.StartNew(() =>
                                          With.TryAction(() => ProcessInternal(source, cancellationToken, outputs),
                                                         finallyAction: outputs.CompleteAdding),
                                          CancellationToken.None,
                                          TaskCreationOptions.None,
                                          Pipeline.Scheduler);

                // 지연된 실행을 할 수 있도록, 실제 실행하지 않고 기다립니다.
                //
                foreach(var result in outputs.GetConsumingEnumerable(cancellationToken))
                    yield return result;

                // 작업이 완전히 완료될 때까지 기다린다.
                //
                processingTask.Wait();
            }
        }

        /// <summary>
        /// 병렬로 입력을 출력으로 변화하는 작업을 수행합니다.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="outputs"></param>
        protected virtual void ProcessInternal(IEnumerable<TInput> source, CancellationToken cancellationToken,
                                               BlockingCollection<TOutput> outputs) {
            var options = new ParallelOptions
                          {
                              CancellationToken = cancellationToken,
                              MaxDegreeOfParallelism = _degreeOfParallelism,
                              TaskScheduler = Pipeline.Scheduler
                          };

            if(IsDebugEnabled)
                log.Debug("시퀀스 정보를 여러단계의 매핑과정을 거치도록 하는 pipeline 처리를 수행합니다...");

            Parallel.ForEach(source, options, input => outputs.Add(_stageFunc(input)));

            if(IsDebugEnabled)
                log.Debug("시퀀스 정보를 여러단계의 매핑과정을 거치도록 하는 pipeline 처리를 완료했습니다!!!");
        }

        /// <summary>
        /// 다음 pipeline 처리를 위한 Helper class 입니다.
        /// </summary>
        /// <typeparam name="TNextOutput"></typeparam>
        private sealed class InternalPipeline<TNextOutput> : Pipeline<TInput, TNextOutput> {
            private readonly Pipeline<TInput, TOutput> _beginingPipeline;
            private readonly Func<TOutput, TNextOutput> _lastStateFunc;

            public InternalPipeline(Pipeline<TInput, TOutput> beginingPipeline, Func<TOutput, TNextOutput> func, int degreeOfParallelism)
                : base(degreeOfParallelism) {
                _beginingPipeline = beginingPipeline;
                _lastStateFunc = func;
            }

            protected override void ProcessInternal(IEnumerable<TInput> source, CancellationToken cancellationToken,
                                                    BlockingCollection<TNextOutput> outputs) {
                if(IsDebugEnabled)
                    log.Debug("Pipeline 프로세싱을 수행합니다...");

                var options = new ParallelOptions
                              {
                                  CancellationToken = cancellationToken,
                                  MaxDegreeOfParallelism = _degreeOfParallelism,
                                  TaskScheduler = Pipeline.Scheduler
                              };

                Parallel.ForEach(_beginingPipeline.Process(source, cancellationToken),
                                 options,
                                 input => outputs.Add(_lastStateFunc(input)));

                if(IsDebugEnabled)
                    log.Debug("Pipeline 프로세싱을 완료했습니다!!!");
            }
        }
    }
}