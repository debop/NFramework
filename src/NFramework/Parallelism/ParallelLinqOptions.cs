using System.Linq;
using System.Threading.Tasks;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Parallelism {

    /// <summary>
    /// <see cref="ParallelOptions"/>에 grouping 기능을 추가하였습니다.
    /// </summary>
    public class ParallelLinqOptions : ParallelOptions {

        /// <summary>
        /// <see cref="ParallelOptions"/>로부터 <see cref="ParallelLinqOptions"/>를 생성합니다.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ParallelLinqOptions CreateFrom(ParallelOptions options) {
            return options.MapProperty(() => new ParallelLinqOptions(), MapPropertyOptions.Safety);
        }

        private ParallelExecutionMode _executionMode = ParallelExecutionMode.Default;
        private ParallelMergeOptions _mergeOptions = ParallelMergeOptions.Default;

        /// <summary>
        /// 쿼리 실행 모드는 쿼리를 병렬화할 때 시스템에서 성능 저하를 처리하는 방식을 지정하는 힌트입니다.
        /// </summary>
        public ParallelExecutionMode ExecutionMode {
            get { return _executionMode; }
            set {
                var isValidExecutionMode = (value == ParallelExecutionMode.Default || value == ParallelExecutionMode.ForceParallelism);
                Guard.Assert(isValidExecutionMode, "ExecutionMode is not valid. ExecutionMode=[{0}]", value);

                _executionMode = value;
            }
        }

        /// <summary>
        /// 쿼리에 사용할 출력 병합의 기본 형식을 지정합니다.이것은 힌트일 뿐이며 모든 쿼리를 병렬화하는 경우 시스템에서 무시될 수 있습니다.
        /// </summary>
        public ParallelMergeOptions MergeOptions {
            get { return _mergeOptions; }
            set {
                var isValidMergeOptions = value == ParallelMergeOptions.Default ||
                                          value == ParallelMergeOptions.AutoBuffered ||
                                          value == ParallelMergeOptions.FullyBuffered ||
                                          value == ParallelMergeOptions.NotBuffered;

                Guard.Assert(isValidMergeOptions, "MergeOptions is not valid. MergeOptions=[{0}]", value);

                _mergeOptions = value;
            }
        }

        /// <summary>
        /// 요소가 정렬되어 있어야 하는지를 나타냅니다.
        /// </summary>
        public bool Ordered { get; set; }
    }
}