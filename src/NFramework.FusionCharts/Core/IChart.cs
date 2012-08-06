namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Fusion Chart의 기본 인터페이스입니다.
    /// </summary>
    public interface IChart : IChartElement {
        /// <summary>
        /// Chart 클릭 시의 링크 정보 (<see cref="FusionLink"/>)
        /// </summary>
        FusionLink ClickURL { get; }

        /// <summary>
        /// About Menu 설정 정보
        /// </summary>
        AboutMenuItemAttribute AboutMenuItemAttr { get; }

        /// <summary>
        /// Exporting 속성
        /// </summary>
        ExportAttribute ExportAttr { get; }
    }
}