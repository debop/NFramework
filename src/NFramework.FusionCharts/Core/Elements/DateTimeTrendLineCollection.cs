using System;
using System.Collections.Generic;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Trendline Collection
    /// </summary>
    [Obsolete("CollectionElement{DateTimeTrendLineElement} 를 직접 사용하세요.")]
    [Serializable]
    public class DateTimeTrendLineCollection : ChartElementBase {
        public DateTimeTrendLineCollection() : base("trendLines") {}

        private IList<DateTimeLineElement> _trendlineElements;

        public virtual IList<DateTimeLineElement> TrendlineElements {
            get { return _trendlineElements ?? (_trendlineElements = new List<DateTimeLineElement>()); }
        }

        protected override void GenerateXmlElements(System.Xml.XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_trendlineElements == null)
                return;

            foreach(var trend in _trendlineElements)
                trend.WriteXmlElement(writer);
        }
    }
}