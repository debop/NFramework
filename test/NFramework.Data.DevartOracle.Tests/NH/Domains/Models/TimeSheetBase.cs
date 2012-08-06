using System;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.TimePeriods;

namespace NSoft.NFramework.Data.DevartOracle.NH.Domains.Models {
    [Serializable]
    public abstract class TimeSheetBase : DataEntityBase<Guid>, ITimeSheet {
        protected TimeSheetBase() {
            base.Id = Guid.NewGuid();
        }

        protected TimeSheetBase(Guid id) {
            base.Id = id;
        }

        /// <summary>
        /// 시작일
        /// </summary>
        public virtual DateTime StartTime { get; set; }

        /// <summary>
        /// 종료일
        /// </summary>
        public virtual DateTime EndTime { get; set; }

        /// <summary>
        /// 기간 종류
        /// </summary>
        public virtual PeriodKind PeriodKind { get; set; }

        /// <summary>
        /// 보할 계획 값
        /// </summary>
        public virtual Decimal? PlanWeightValue { get; set; }

        /// <summary>
        /// 보할 실적 값
        /// </summary>
        public virtual Decimal? ActualWeightValue { get; set; }

        /// <summary>
        /// 진척률 계획 값
        /// </summary>
        public virtual Decimal? PlanProgressValue { get; set; }

        /// <summary>
        /// 진척률 실적 값
        /// </summary>
        public virtual Decimal? ActualProgressValue { get; set; }

        /// <summary>
        /// 계획 작업량
        /// </summary>
        public virtual Decimal? PlanWorkValue { get; set; }

        /// <summary>
        /// 실적 작업량
        /// </summary>
        public virtual Decimal? ActualWorkValue { get; set; }

        /// <summary>
        /// 계획 비용
        /// </summary>
        public virtual Decimal? PlanCostValue { get; set; }

        /// <summary>
        /// 실적 비용
        /// </summary>
        public virtual Decimal? ActualCostValue { get; set; }

        /// <summary>
        /// 완료 여부
        /// </summary>
        public virtual bool IsComplete { get; set; }

        /// <summary>
        /// 마감 여부
        /// </summary>
        public virtual bool IsClosed { get; set; }

        /// <summary>
        /// 등록자
        /// </summary>
        public virtual string Creator { get; set; }

        /// <summary>
        /// 수정자
        /// </summary>
        public virtual string LastUpdator { get; set; }

        public virtual DateTime? CreateDate { get; set; }

        public virtual DateTime? UpdateTimestamp { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Id);
        }
    }
}