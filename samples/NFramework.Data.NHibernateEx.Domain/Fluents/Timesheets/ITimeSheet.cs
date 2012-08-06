using System;
using NSoft.NFramework.TimePeriods;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.Timesheets {
    public interface ITimeSheet : IDataObject {
        /// <summary>
        /// 시작일
        /// </summary>
        DateTime StartTime { get; set; }

        /// <summary>
        /// 종료일
        /// </summary>
        DateTime EndTime { get; set; }

        /// <summary>
        /// 기간 종류
        /// </summary>
        PeriodKind PeriodKind { get; set; }

        /// <summary>
        /// 보할 계획 값
        /// </summary>
        Decimal? PlanWeightValue { get; set; }

        /// <summary>
        /// 보할 실적 값
        /// </summary>
        Decimal? ActualWeightValue { get; set; }

        /// <summary>
        /// 진척률 계획 값
        /// </summary>
        Decimal? PlanProgressValue { get; set; }

        /// <summary>
        /// 진척률 실적 값
        /// </summary>
        Decimal? ActualProgressValue { get; set; }

        /// <summary>
        /// 계획 작업량
        /// </summary>
        Decimal? PlanWorkValue { get; set; }

        /// <summary>
        /// 실적 작업량
        /// </summary>
        Decimal? ActualWorkValue { get; set; }

        /// <summary>
        /// 계획 비용
        /// </summary>
        Decimal? PlanCostValue { get; set; }

        /// <summary>
        /// 실적 비용
        /// </summary>
        Decimal? ActualCostValue { get; set; }

        /// <summary>
        /// 완료 여부
        /// </summary>
        bool IsComplete { get; set; }

        /// <summary>
        /// 마감 여부
        /// </summary>
        bool IsClosed { get; set; }

        /// <summary>
        /// 등록자 정보
        /// </summary>
        string Creator { get; set; }

        /// <summary>
        /// 최종 수정자 정보
        /// </summary>
        string LastUpdator { get; set; }

        /// <summary>
        /// 등록일
        /// </summary>
        DateTime? CreateDate { get; set; }

        /// <summary>
        /// 최근 수정일
        /// </summary>
        DateTime? UpdateTimestamp { get; set; }
    }
}