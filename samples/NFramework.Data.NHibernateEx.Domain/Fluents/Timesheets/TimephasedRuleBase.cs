using System;
using NSoft.NFramework.TimePeriods;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.Timesheets {
    [Serializable]
    public abstract class TimephasedRuleBase : DataEntityBase<Guid>, ITimephasedRule {
        protected TimephasedRuleBase() {}

        protected TimephasedRuleBase(DateTime startTime, DateTime endTime) {
            TimeRange = new TimeRange(startTime, endTime, false);
        }

        protected TimephasedRuleBase(ITimeRange timeRange) {
            TimeRange = timeRange;
        }

        private ITimeRange _timeRange;

        public virtual ITimeRange TimeRange {
            get { return _timeRange ?? (_timeRange = new TimeRange(null, null)); }
            set { _timeRange = value; }
        }

        /// <summary>
        /// 예외 규칙의 종류
        /// </summary>
        public virtual bool IsException { get; set; }

        /// <summary>
        /// 보할 값
        /// </summary>
        public virtual decimal? WeightValue { get; set; }

        /// <summary>
        /// 작업량 값
        /// </summary>
        public virtual decimal? WorkValue { get; set; }

        /// <summary>
        /// 진척률 값
        /// </summary>
        public virtual decimal? ProgressValue { get; set; }

        /// <summary>
        /// 금액 값
        /// </summary>
        public virtual decimal? CostValue { get; set; }

        /// <summary>
        /// 확장 값1
        /// </summary>
        public virtual decimal? Ext1Value { get; set; }

        /// <summary>
        /// 확장 값2
        /// </summary>
        public virtual decimal? Ext2Value { get; set; }

        /// <summary>
        /// 등록자 정보
        /// </summary>
        public virtual string Creator { get; set; }

        /// <summary>
        /// 최종 수정자 정보
        /// </summary>
        public virtual string LastUpdator { get; set; }

        /// <summary>
        /// 등록일
        /// </summary>
        public virtual DateTime? CreateDate { get; set; }

        /// <summary>
        /// 최근 수정일
        /// </summary>
        public virtual DateTime? UpdateTimestamp { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(TimeRange, IsException);
        }
    }
}