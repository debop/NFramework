using System;
using NSoft.NFramework.TimePeriods;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.Timesheets {
    public interface ITimephasedRule : IDataObject {
        ITimeRange TimeRange { get; set; }

        /// <summary>
        /// 예외 규칙의 종류
        /// </summary>
        bool IsException { get; set; }

        /// <summary>
        /// 보할 값
        /// </summary>
        Decimal? WeightValue { get; set; }

        /// <summary>
        /// 작업량 값
        /// </summary>
        Decimal? WorkValue { get; set; }

        /// <summary>
        /// 진척률 값
        /// </summary>
        Decimal? ProgressValue { get; set; }

        /// <summary>
        /// 금액 값
        /// </summary>
        Decimal? CostValue { get; set; }

        /// <summary>
        /// 확장 값1
        /// </summary>
        Decimal? Ext1Value { get; set; }

        /// <summary>
        /// 확장 값2
        /// </summary>
        Decimal? Ext2Value { get; set; }

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