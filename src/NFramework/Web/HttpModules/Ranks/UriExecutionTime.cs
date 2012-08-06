using System;

namespace NSoft.NFramework.Web.HttpModules {
    /// <summary>
    /// 페이지 처리 성능 정보
    /// </summary>
    [Serializable]
    public struct UriExecutionTime : IEquatable<UriExecutionTime>, IComparable<UriExecutionTime>, IComparable {
        public UriExecutionTime(string uriString, long executionTime)
            : this() {
            uriString.ShouldNotBeNull("uriString");

            UriString = uriString;
            ExecutionTime = executionTime;
            ExecutionCount = 1;
        }

        /// <summary>
        /// 대상 페이지
        /// </summary>
        public string UriString { get; set; }

        /// <summary>
        /// 평균 실행 시간
        /// </summary>
        public long ExecutionTime { get; set; }

        /// <summary>
        /// 실행 횟수
        /// </summary>
        public long ExecutionCount { get; set; }

        /// <summary>
        /// 실행 횟수 한 회 증가
        /// </summary>
        public void IncreseExecutionCount() {
            ExecutionCount++;
        }

        public long GetTotalExecutionTime() {
            return ExecutionTime * ExecutionCount;
        }

        public int CompareTo(object obj) {
            Guard.Assert(obj is UriExecutionTime, "대상 객체가 UriExecuteTime 형식이 아닙니다.");
            return CompareTo((UriExecutionTime)obj);
        }

        /// <summary>
        /// 이 비교 함수가 Ranking을 결정할 알고리즘을 담고 있습니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(UriExecutionTime other) {
            other.ShouldNotBeNull("other");

            // 페이지당 전체 실행 시간간의 비교);
            return GetTotalExecutionTime().CompareTo(other.GetTotalExecutionTime());
        }

        public bool Equals(UriExecutionTime other) {
            return GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null && obj is UriExecutionTime && Equals((UriExecutionTime)obj));
        }

        public override int GetHashCode() {
            return HashTool.Compute(UriString, ExecutionTime, ExecutionCount);
        }

        public override string ToString() {
            return string.Format("UriExecutionTime# UriString=[{0}], ExecutionTime=[{1}] (msec), ExecutionCount=[{2}]",
                                 UriString, ExecutionTime, ExecutionCount);
        }
    }
}