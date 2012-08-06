using System;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// &lt;trendpoints/&gt; element를 표현합니다.
    /// </summary>
    [Obsolete("CollectionElement{PointElement} 를 직접 사용하세요")]
    [Serializable]
    public class TrendPointsElement : CollectionElement<PointElement> {
        public TrendPointsElement()
            : base("trendpoints") {}
    }
}