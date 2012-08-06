using System;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// DivLine, Zero Pane 관련 Attributes
    /// </summary>
    [Obsolete("FusionChartBase에 속성으로 추가되었습니다.")]
    [Serializable]
    public class DivisionalLineAttribute : ChartAttributeBase {
        /// <summary>
        /// Number of horizontal axis division lines that you want on the chart.
        /// </summary>
        public int? NumDivLines { get; set; }

        private DashLineAttribute _divLineAttr;

        public DashLineAttribute DivLineAttr {
            get { return _divLineAttr ?? (_divLineAttr = new DashLineAttribute("divLine")); }
            set { _divLineAttr = value; }
        }

        private ZeroPaneAttribute _zeroPlaneAttr;

        public ZeroPaneAttribute ZeroPlaneAttr {
            get { return _zeroPlaneAttr ?? (_zeroPlaneAttr = new ZeroPaneAttribute()); }
            set { _zeroPlaneAttr = value; }
        }

        private AlternateGridAttribute _alternateHGridAttr;

        public AlternateGridAttribute AlternateHGridAttr {
            get { return _alternateHGridAttr ?? (_alternateHGridAttr = new AlternateGridAttribute("AlternateHGrid")); }
            set { _alternateHGridAttr = value; }
        }

        /// <summary>
        /// Number of horizontal axis division lines that you want on the chart.
        /// </summary>
        public int? NumVDivLines { get; set; }

        private DashLineAttribute _vDivLine;

        public DashLineAttribute VDivLineAttr {
            get { return _vDivLine ?? (_vDivLine = new DashLineAttribute("vDivLine")); }
            set { _vDivLine = value; }
        }

        private AlternateGridAttribute _alternateVGridAttr;

        public AlternateGridAttribute AlternateVGridAttr {
            get { return _alternateVGridAttr ?? (_alternateVGridAttr = new AlternateGridAttribute("AlternateVGrid")); }
            set { _alternateVGridAttr = value; }
        }

        /// <summary>
        /// You can apply emboss or bevel effect to both divisional lines and trendlines. The divLineEffect attribute will let you this. You can specify one of the three values: "EMBOSS", "BEVEL" or "NONE".
        /// </summary>
        public LineEffect? DivLineEffect { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML 속성으로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(NumDivLines.HasValue)
                writer.WriteAttributeString("numDivLines", NumDivLines.ToString());

            if(_divLineAttr != null)
                _divLineAttr.GenerateXmlAttributes(writer);

            if(_zeroPlaneAttr != null)
                _zeroPlaneAttr.GenerateXmlAttributes(writer);

            if(NumVDivLines.HasValue)
                writer.WriteAttributeString("numVDivLines", NumVDivLines.ToString());

            if(_vDivLine != null)
                _vDivLine.GenerateXmlAttributes(writer);

            if(_alternateHGridAttr != null)
                _alternateHGridAttr.GenerateXmlAttributes(writer);

            if(_alternateVGridAttr != null)
                _alternateVGridAttr.GenerateXmlAttributes(writer);

            if(DivLineEffect.HasValue)
                writer.WriteAttributeString("divLineEffect", DivLineEffect.ToString().ToUpper());
        }
    }
}