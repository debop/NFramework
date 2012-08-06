namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Angular gauges are essentially like the speedometer or fuel gauge of your car. They use a radial scale to display your data range and a dial is used to indicate the data value. 
    /// An angular/meter/dial gauge chart is used to display a specific data set utilizing an indicator that moves within a circular range to indicate whether the monitored data is within defined limits. Colors can be selected for the data range to suit the application such as green for satisfactory, yellow for caution and red for alarm.
    /// 
    /// document : http://www.fusioncharts.com/widgets/docs/
    /// </summary>
    /// <remarks>
    /// To create a FusionWidgets Angular Gauge chart, you need to:
    ///		•Decide the lower and upper limit of chart. 
    ///		•Decide the cosmetic and functional properties of the chart	
    ///		•Decide the color range for the chart. That is, suppose you are plotting a chart to show the production output of a factory, the color range could be:
    ///			0-100 pieces/hr – Under Average – Red
    ///			100-200 pieces/hr – Average Production - Yellow
    ///			200-300 pieces/hr – Very good - Green 
    ///		•Decide trend points if any – for example, if you would like to mark an average at say 175 - say “Last week’s average production”, you can use trend points to do the same. 
    ///		•Decide the chart value(s). FusionWidgets allows you to indicate multiple values on the angular gauge chart. All you have to do is decide the value each spindle/dial would indicate and the width and color of each one of them. 
    /// </remarks>
    public class AngularGauge : GaugeBase {
        /// <summary>
        /// -360 ~ 360
        /// Angle from where the gauge will start drawing. Please see the section "Understanding angles" to learn more about this.
        /// </summary>
        public int? GaugeStartAngle { get; set; }

        /// <summary>
        /// -360 ~ 360
        /// Angle where the gauge will end drawing. Please see the section "Understanding angles" to learn more about this.
        /// </summary>
        public int? GaugeEndAngle { get; set; }

        /// <summary>
        /// In Pixels.
        /// You can manually specify the x co-ordinate at which the center of the gauge would be placed. This comes handy when you are designing small angle gauges or when you are using custom objects in the chart to draw certain objects and need to set the exact center position.
        /// </summary>
        public int? GaugeOriginX { get; set; }

        /// <summary>
        /// In Pixels.
        /// You can manually specify the y co-ordinate at which the center of the gauge would be placed. This comes handy when you are designing small angle gauges or when you are using custom objects in the chart to draw certain objects and need to set the exact center position.
        /// </summary>
        public int? GaugeOriginY { get; set; }

        /// <summary>
        /// In Pixels.
        /// Outer radius for the color range scale.
        /// </summary>
        public int? GaugeOuterRadius { get; set; }

        /// <summary>
        /// In Pixels.
        /// Innera radius for color range scale. It can either be in exact pixels or in percentage of outer radius.
        /// </summary>
        public int? GaugeInnerRadius { get; set; }

        public int? GaugeScaleAngle { get; set; }

        private PivotAttribute _pivot;

        public PivotAttribute Pivot {
            get { return _pivot ?? (_pivot = new PivotAttribute()); }
            set { _pivot = value; }
        }

        private CollectionElement<DialElement> _dials;

        public CollectionElement<DialElement> Dials {
            get { return _dials ?? (_dials = new CollectionElement<DialElement>("dials")); }
            set { _dials = value; }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(GaugeStartAngle.HasValue)
                writer.WriteAttributeString("GaugeStartAngle", GaugeStartAngle.ToString());
            if(GaugeEndAngle.HasValue)
                writer.WriteAttributeString("GaugeEndAngle", GaugeEndAngle.ToString());

            if(GaugeOriginX.HasValue)
                writer.WriteAttributeString("GaugeOriginX", GaugeOriginX.ToString());
            if(GaugeOriginY.HasValue)
                writer.WriteAttributeString("GaugeOriginY", GaugeOriginY.ToString());

            if(GaugeOuterRadius.HasValue)
                writer.WriteAttributeString("GaugeOuterRadius", GaugeOuterRadius.ToString());
            if(GaugeInnerRadius.HasValue)
                writer.WriteAttributeString("GaugeInnerRadius", GaugeInnerRadius.ToString());

            if(GaugeScaleAngle.HasValue)
                writer.WriteAttributeString("GaugeScaleAngle", GaugeScaleAngle.ToString());

            if(_pivot != null)
                _pivot.GenerateXmlAttributes(writer);
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(System.Xml.XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_dials != null)
                _dials.WriteXmlElement(writer);
        }

        /// <summary>
        /// Add dial element
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DialElement AddDial(string id, double? value) {
            var dial = new DialElement
                       {
                           Id = id,
                           Value = value
                       };
            Dials.Add(dial);
            return dial;
        }
    }
}