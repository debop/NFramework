using System.Collections.Generic;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// 복수의 Series을 가지는 Chart 입니다.
    /// 
    /// Documents : http://www.fusioncharts.com/docs/
    /// </summary>
    public class MultiSeriesChart : FusionChartBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public MultiSeriesChart() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="elementName"></param>
        protected MultiSeriesChart(string elementName) : base(elementName) {}

        private AxisAttribute _pYAxisAttr;

        /// <summary>
        /// Primary Y Axis attributes
        /// </summary>
        public virtual AxisAttribute PYAxisAttr {
            get { return _pYAxisAttr ?? (_pYAxisAttr = new AxisAttribute("pYAxis")); }
            set { _pYAxisAttr = value; }
        }

        private AxisAttribute _sYAxisAttr;

        /// <summary>
        /// Secondary Y Axis attributes
        /// </summary>
        public virtual AxisAttribute SYAxisAttr {
            get { return _sYAxisAttr ?? (_sYAxisAttr = new AxisAttribute("sYAxis")); }
            set { _sYAxisAttr = value; }
        }

        private IList<DataSetElement> _dataSets;

        /// <summary>
        /// Chart에 표현할 변량 컬렉션의 컬렉션
        /// </summary>
        public virtual IList<DataSetElement> DataSets {
            get { return _dataSets ?? (_dataSets = new List<DataSetElement>()); }
            set { _dataSets = value; }
        }

        /// <summary>
        /// 속성 중 Attribute Node로 표현해야 한다.
        /// </summary>
        /// <param name="writer"></param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(_pYAxisAttr != null)
                _pYAxisAttr.GenerateXmlAttributes(writer);

            if(_sYAxisAttr != null)
                _sYAxisAttr.GenerateXmlAttributes(writer);

            // NOTE: 자동으로 속성을 XmlAttribute로 생성합니다 (개발 중)
            // ChartExtensions.WriteChartAttribute(this, writer);
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(System.Xml.XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_dataSets != null && _dataSets.Count > 0)
                foreach(var dataSet in _dataSets)
                    dataSet.WriteXmlElement(writer);

            // NOTE: 자동으로 속성을 XmlElement로 생성합니다 (개발 중)
            // ChartExtensions.WriteChartElement(this, writer);
        }
    }
}