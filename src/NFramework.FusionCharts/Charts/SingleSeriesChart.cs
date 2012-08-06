using System.Collections.Generic;
using System.Drawing;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// Single Series Chart
    /// 
    /// Documents : http://www.fusioncharts.com/docs/
    /// </summary>
    public class SingleSeriesChart : FusionChartBase {
        private IList<ChartElementBase> _setElements;

        /// <summary>
        /// 내부에 <see cref="ValueSetElement"/> 와 <see cref="VerticalLineElement"/>를 넣을 수 있습니다.
        /// </summary>
        public virtual IList<ChartElementBase> SetElements {
            get { return _setElements ?? (_setElements = new List<ChartElementBase>()); }
            set { _setElements = value; }
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(System.Xml.XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_setElements != null && _setElements.Count > 0)
                foreach(var set in _setElements)
                    set.WriteXmlElement(writer);
        }

        #region << AddSet >>

        public virtual ValueSetElement AddSet(string label, double? value) {
            var set = new ValueSetElement(value) { Label = label };
            SetElements.Add(set);
            return set;
        }

        public virtual VerticalLineElement AddVLine(Color? color, int? thickness) {
            var line = new VerticalLineElement
                       {
                           Color = color,
                           Thickness = thickness
                       };
            SetElements.Add(line);
            return line;
        }

        #endregion
    }
}