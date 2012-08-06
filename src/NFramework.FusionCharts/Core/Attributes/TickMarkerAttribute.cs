using System;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Tick Mark 속성
    /// </summary>
    [Serializable]
    public class TickMarkAttribute : ChartAttributeBase {
        /// <summary>
        /// Whether to show tick marks? 
        /// </summary>
        public bool? ShowTickMarks { get; set; }

        /// <summary>
        /// Whether to show tick values? 
        /// </summary>
        public bool? ShowTickValues { get; set; }

        /// <summary>
        /// Whether to show the first and last tick value (i.e., chart lower and upper limit)? 
        /// </summary>
        public bool? ShowLimits { get; set; }

        /// <summary>
        /// Whether to adjust major tick mark number so as to best distribute the specified chart scale. 
        /// </summary>
        public bool? AdjustTM { get; set; }

        /// <summary>
        /// Whether to place ticks inside the gauge (on the inner radius) or outside? 
        /// </summary>
        public bool? PlaceTicksInside { get; set; }

        /// <summary>
        /// Whether to place tick values inside the gauge? 
        /// </summary>
        public bool? PlaceValuesInside { get; set; }

        private MarkAttribute _majorTM;

        public MarkAttribute MajorTM {
            get { return _majorTM ?? (_majorTM = new MarkAttribute("major")); }
            set { _majorTM = value; }
        }

        private MarkAttribute _minorTM;

        public MarkAttribute MinorTM {
            get { return _minorTM ?? (_minorTM = new MarkAttribute("minor")); }
            set { _minorTM = value; }
        }

        public int? TickValueDistance { get; set; }
        public int? TrendValueDistance { get; set; }
        public double? TickValueStep { get; set; }
        public double? TickValueDecimals { get; set; }
        public bool? ForceTickValueDecimals { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(ShowTickMarks.HasValue)
                writer.WriteAttributeString("ShowTickMarks", ShowTickMarks.Value.GetHashCode().ToString());
            if(ShowTickValues.HasValue)
                writer.WriteAttributeString("ShowTickValues", ShowTickValues.Value.GetHashCode().ToString());
            if(ShowLimits.HasValue)
                writer.WriteAttributeString("ShowLimits", ShowLimits.Value.GetHashCode().ToString());
            if(AdjustTM.HasValue)
                writer.WriteAttributeString("AdjustTM", AdjustTM.Value.GetHashCode().ToString());
            if(PlaceTicksInside.HasValue)
                writer.WriteAttributeString("PlaceTicksInside", PlaceTicksInside.Value.GetHashCode().ToString());
            if(PlaceValuesInside.HasValue)
                writer.WriteAttributeString("PlaceValuesInside", PlaceValuesInside.Value.GetHashCode().ToString());

            if(_majorTM != null)
                _majorTM.GenerateXmlAttributes(writer);
            if(_minorTM != null)
                _minorTM.GenerateXmlAttributes(writer);

            if(TickValueDistance.HasValue)
                writer.WriteAttributeString("TickValueDistance", TickValueDistance.Value.ToString());
            if(TrendValueDistance.HasValue)
                writer.WriteAttributeString("TrendValueDistance", TrendValueDistance.Value.ToString());

            if(TickValueStep.HasValue)
                writer.WriteAttributeString("TickValueStep", TickValueStep.Value.ToString());
            if(TickValueDecimals.HasValue)
                writer.WriteAttributeString("TickValueDecimals", TickValueDecimals.Value.ToString());

            if(ForceTickValueDecimals.HasValue)
                writer.WriteAttributeString("ForceTickValueDecimals", ForceTickValueDecimals.Value.GetHashCode().ToString());
        }
    }
}