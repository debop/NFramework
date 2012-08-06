using NSoft.NFramework.Tools;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// FusionWidget 의 기본 클래스입니다.
    /// 
    /// document : http://www.fusioncharts.com/widgets/docs/
    /// </summary>
    public abstract class WidgetBase : ChartBase {
        /// <summary>
        /// 실시간 Data를 제공하는 URL.
        /// This parameter sets the path of the page which is supposed to relay real-time data to the chart. If you've special characters as a part of your data stream URL, like ? or &, you'll need to URL Encode the entire dataStreamURL
        /// This page needs to be on the same sub-domain on which the chart is hosted and invoked from. Otherwise, the Flash sandbox security would restrict it from accessing the data and hence the real-time feature won't work.
        /// Example: dataStreamURL='liveQuote.aspx?name=xyz' 
        /// </summary>
        public string DataStreamUrl { get; set; }

        /// <summary>
        /// For this parameter, you can specify the number of seconds after which the chart will look for new data. This process will happen continuously - i.e., if you specify 5 seconds here, the chart will look for new data every 5 seconds. 
        /// </summary>
        public int? RefreshInterval { get; set; }

        /// <summary>
        /// Constantly changing data stamp that can be added to real time data URL, so as to maintain a state. Please see the section "Real time capabilities > Adding data-stamp" 
        /// </summary>
        public string DataStamp { get; set; }

        /// <summary>
        /// TODO : MenuAttribute로 빼야 하지 않나?
        /// Whether to show the real-time update related menu items (like Start/Stop Update) in chart's context menu?
        /// </summary>
        public bool? ShowRTMenuItem { get; set; }

        /// <summary>
        /// Animation시 Delay time ( unit : seconds )
        /// </summary>
        public int? AnnRenderDelay { get; set; }

        /// <summary>
        /// If you've defined the chart parameters (like gauge origin, co-ordinates etc.) using a different chart size and now want to scale the chart to a different size, you need to specify the original width and height and then set this attribute to 1.
        /// </summary>
        public bool? AutoScale { get; set; }

        /// <summary>
        /// Number (Pixels)
        /// If you've opted to auto-scale the chart, you need to convey the original width of chart using this attribute. During auto-scaling, this attribute is mandatory to specify.
        /// </summary>
        public int? OrigW { get; set; }

        /// <summary>
        /// Number (Pixels)
        /// If you've opted to auto-scale the chart, you need to convey the original height of chart using this attribute. During auto-scaling, this attribute is mandatory to specify.
        /// </summary>
        public int? OrigH { get; set; }

        /// <summary>
        /// Whether to render the gauge in edit mode? In edit mode, user can drag and rotate the dial to new values.
        /// </summary>
        public bool? IsEditMode { get; set; }

        /// <summary>
        /// This attributes lets you control whether empty data sets in your data will be connected to each other OR would they appear as broken data sets? 
        /// </summary>
        public bool? ConnectNullData { get; set; }

        private WidgetFontAttribute _fontAttr;

        public WidgetFontAttribute FontAttr {
            get { return _fontAttr ?? (_fontAttr = new WidgetFontAttribute()); }
            set { _fontAttr = value; }
        }

        private TickMarkAttribute _tickMarkAttr;

        public TickMarkAttribute TickMarkAttr {
            get { return _tickMarkAttr ?? (_tickMarkAttr = new TickMarkAttribute()); }
            set { _tickMarkAttr = value; }
        }

        private MessageLoggerAttribute _messageLogAttr;

        public MessageLoggerAttribute MessageLogAttr {
            get { return _messageLogAttr ?? (_messageLogAttr = new MessageLoggerAttribute()); }
            set { _messageLogAttr = value; }
        }

        private CollectionElement<AlertElement> _alerts;

        public CollectionElement<AlertElement> Alerts {
            get { return _alerts ?? (_alerts = new CollectionElement<AlertElement>("alerts")); }
            set { _alerts = value; }
        }

        private AnnotationsElement _annotations;

        public AnnotationsElement Annotations {
            get { return _annotations ?? (_annotations = new AnnotationsElement()); }
            set { _annotations = value; }
        }

        private CollectionElement<PointElement> _trendPoints;

        public CollectionElement<PointElement> TrendPoints {
            get { return _trendPoints ?? (_trendPoints = new CollectionElement<PointElement>("trendpoints")); }
            set { _trendPoints = value; }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(DataStreamUrl.IsNotWhiteSpace())
                writer.WriteAttributeString("dataStreamUrl", DataStreamUrl.UrlEncode());
            if(RefreshInterval.HasValue)
                writer.WriteAttributeString("refreshInterval", RefreshInterval.ToString());
            if(DataStamp.IsNotWhiteSpace())
                writer.WriteAttributeString("dataStamp", DataStamp);
            if(ShowRTMenuItem.HasValue)
                writer.WriteAttributeString("ShowRTMenuItem", ShowRTMenuItem.GetHashCode().ToString());

            if(AnnRenderDelay.HasValue)
                writer.WriteAttributeString("AnnRenderDelay", AnnRenderDelay.Value.ToString());
            if(AutoScale.HasValue)
                writer.WriteAttributeString("AutoScale", AutoScale.GetHashCode().ToString());

            if(IsEditMode.HasValue)
                writer.WriteAttributeString("IsEditMode", IsEditMode.GetHashCode().ToString());

            if(ConnectNullData.HasValue)
                writer.WriteAttributeString("ConnectNullData", ConnectNullData.GetHashCode().ToString());

            if(_tickMarkAttr != null)
                _tickMarkAttr.GenerateXmlAttributes(writer);
            if(_messageLogAttr != null)
                _messageLogAttr.GenerateXmlAttributes(writer);

            if(_fontAttr != null)
                _fontAttr.GenerateXmlAttributes(writer);
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(System.Xml.XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_alerts != null)
                _alerts.WriteXmlElement(writer);

            if(_annotations != null)
                _annotations.WriteXmlElement(writer);

            if(_trendPoints != null)
                _trendPoints.WriteXmlElement(writer);
        }

        #region << Public Method >>

        public virtual AnnotationGroupElement AddAnnotationGroup(double? x, double? y) {
            var annotationGroup = new AnnotationGroupElement
                                  {
                                      X = x,
                                      Y = y,
                                  };

            Annotations.Add(annotationGroup);

            return annotationGroup;
        }

        #endregion
    }
}