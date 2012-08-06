using System;
using System.Drawing;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Fusion Chart의 기본 Class입니다.
    /// 
    /// Documents : http://www.fusioncharts.com/docs/
    /// </summary>
    [Serializable]
    public abstract class ChartBase : ChartElementBase, IChart {
        /// <summary>
        /// 생성자
        /// </summary>
        protected ChartBase() : this("chart") {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="elementName"></param>
        protected ChartBase(string elementName)
            : base(elementName) {
            ExportDataMenuItemAttr.Show = true;
        }

        /// <summary>
        /// Chart Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Caption of the chart.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Sub-caption of the chart.
        /// </summary>
        public string SubCaption { get; set; }

        /// <summary>
        /// This attribute lets you set the configuration whether the chart should appear in an animated fashion. If you do not want to animate any part of the chart, set this as 0.
        /// </summary>
        public bool? Animation { get; set; }

        /// <summary>
        /// By default, each chart animates some of its elements. If you wish to switch off the default animation patterns, you can set this attribute to 0. It can be particularly useful when you want to define your own animation patterns using STYLE feature.
        /// </summary>
        public bool? DefaultAnimation { get; set; }

        /// <summary>
        /// FusionCharts v3 introduces the concept of Color Palettes. Each chart has 5 pre-defined color palettes which you can choose from. Each palette renders the chart in a different color theme. Valid values are 1-5.
        /// </summary>
        public int? Palette { get; set; }

        /// <summary>
        /// By default, each chart animates some of its elements. If you wish to switch off the default animation patterns, you can set this attribute to 0. It can be particularly useful when you want to define your own animation patterns using STYLE feature.
        /// </summary>
        public Color? PaletteThemeColor { get; set; }

        /// <summary>
        /// Chart Context Menu에 "Print" 메뉴를 보일 것인가?
        /// </summary>
        public bool? ShowPrintMenuItem { get; set; }

        /// <summary>
        /// Show Lables ?
        /// </summary>
        public bool? ShowLabels { get; set; }

        /// <summary>
        /// Whether to show shadow for the chart? 
        /// </summary>
        public bool? ShowShadow { get; set; }

        /// <summary>
        /// Sets the configuration whether data values would be displayed along with the data plot on chart.
        /// </summary>
        public bool? ShowValues { get; set; }

        /// <summary>
        /// If you've opted to show data values, you can rotate them using this attribute.
        /// </summary>
        public bool? RotateValues { get; set; }

        /// <summary>
        /// 전체 Chart 영역에서 click 시, open할 url. FusionChart URL Format 을 사용해야 한다.
        /// </summary>
        private FusionLink _clickURL;

        /// <summary>
        /// Chart click 시 연결할 URL 정보
        /// </summary>
        public FusionLink ClickURL {
            get { return _clickURL ?? (_clickURL = new FusionLink("clickURL")); }
            set { _clickURL = value; }
        }

        private BackgroundAttribute _backgroundAttr;

        public BackgroundAttribute BackgroundAttr {
            get { return _backgroundAttr ?? (_backgroundAttr = new BackgroundAttribute()); }
            set { _backgroundAttr = value; }
        }

        private BorderAttribute _borderAttr;

        public BorderAttribute BorderAttr {
            get { return _borderAttr ?? (_borderAttr = new BorderAttribute()); }
            set { _borderAttr = value; }
        }

        private CosmeticAttribute _cosmeticAttr;

        public CosmeticAttribute CosmeticAttr {
            get { return _cosmeticAttr ?? (_cosmeticAttr = new CosmeticAttribute()); }
            set { _cosmeticAttr = value; }
        }

        private FontAttribute _baseFontAttr;

        public FontAttribute BaseFontAttr {
            get { return _baseFontAttr ?? (_baseFontAttr = new FontAttribute("base")); }
            set { _baseFontAttr = value; }
        }

        private ToolTipAttribute _toolTipAttr;

        public ToolTipAttribute ToolTipAttr {
            get { return _toolTipAttr ?? (_toolTipAttr = new ToolTipAttribute()); }
            set { _toolTipAttr = value; }
        }

        private LogoAttribute _logoAttr;

        public LogoAttribute LogoAttr {
            get { return _logoAttr ?? (_logoAttr = new LogoAttribute()); }
            set { _logoAttr = value; }
        }

        private AboutMenuItemAttribute _aboutMenuItemAttr;

        public AboutMenuItemAttribute AboutMenuItemAttr {
            get { return _aboutMenuItemAttr ?? (_aboutMenuItemAttr = new AboutMenuItemAttribute()); }
            set { _aboutMenuItemAttr = value; }
        }

        private ExportDataMenuItemAttribute _exportDataMenuItemAttr;

        public ExportDataMenuItemAttribute ExportDataMenuItemAttr {
            get { return _exportDataMenuItemAttr ?? (_exportDataMenuItemAttr = new ExportDataMenuItemAttribute()); }
            set { _exportDataMenuItemAttr = value; }
        }

        private ExportAttribute _exportAttr;

        public ExportAttribute ExportAttr {
            get { return _exportAttr ?? (_exportAttr = new ExportAttribute()); }
            set { _exportAttr = value; }
        }

        private LegendAttribute _legendAttr;

        public LegendAttribute LegendAttr {
            get { return _legendAttr ?? (_legendAttr = new LegendAttribute()); }
            set { _legendAttr = value; }
        }

        private ScrollBarAttribute _scrollBarAttr;

        public ScrollBarAttribute ScrollBarAttr {
            get { return _scrollBarAttr ?? (_scrollBarAttr = new ScrollBarAttribute()); }
            set { _scrollBarAttr = value; }
        }

        private MarginAttribute _marginAttr;

        public MarginAttribute MarginAttr {
            get { return _marginAttr ?? (_marginAttr = new MarginAttribute("chart")); }
            set { _marginAttr = value; }
        }

        private NumberFormatAttribute _numberAttr;

        public NumberFormatAttribute NumberAttr {
            get { return _numberAttr ?? (_numberAttr = new NumberFormatAttribute()); }
            set { _numberAttr = value; }
        }

        /// <summary>
        /// This attribute lets you control the space (in pixels) between the sub-caption and top of the chart canvas. If the sub-caption is not defined, it controls the space between caption and top of chart canvas. If neither caption, nor sub-caption is defined, this padding does not come into play.
        /// </summary>
        public int? CaptionPadding { get; set; }

        /// <summary>
        /// Using this, you can set the distance between the top end of x-axis title and the bottom end of data labels (or canvas, if data labels are not to be shown).
        /// </summary>
        public int? XAxisNamePadding { get; set; }

        /// <summary>
        /// his attribute sets the vertical space between the labels and canvas bottom edge. If you want more space between the canvas and the x-axis labels, you can use this attribute to control it.
        /// </summary>
        public int? LabelPadding { get; set; }

        /// <summary>
        /// It sets the vertical space between the end of columns and start of value textboxes. This basically helps you control the space you want between your columns/anchors and the value textboxes.
        /// </summary>
        public int? ValuePadding { get; set; }

        /// <summary>
        /// Lets you set the space between the canvas border and first & last data points
        /// </summary>
        public int? CanvasPadding { get; set; }

        /// <summary>
        /// Padding of legend from right/bottom side of canvas
        /// </summary>
        public int? LegendPadding { get; set; }

        /// <summary>
        /// On a column chart, there is spacing defined between two columns. By default, the spacing is set to 20% of canvas width. If you intend to increase or decrease the spacing between columns, you can do so using this attribute. For example, if you wanted all columns to stick to each other without any space in between, you can set plotSpacePercent to 0. Similarly, if you want very thin columns, you can set plotSpacePercent to its max value of 80. 
        /// </summary>
        public int? PlotSpacePercent { get; set; }

        private MarginAttribute _chartMarginAtrr;

        public MarginAttribute ChartMarginAttr {
            get { return _chartMarginAtrr ?? (_chartMarginAtrr = new MarginAttribute("chart")); }
            set { _chartMarginAtrr = value; }
        }

        private MarginAttribute _canvasMarginAttr;

        public MarginAttribute CanvasMarginAttr {
            get { return _canvasMarginAttr ?? (_canvasMarginAttr = new MarginAttribute("canvas")); }
            set { _canvasMarginAttr = value; }
        }

        [CLSCompliant(false)] protected StylesElement _styles;

        /// <summary>
        /// FusionChart 3.0 부터 제공되는 스타일 정보입니다. HTML의 CSS와 같은 역할을 수행합니다.
        /// </summary>
        // NOTE : XML 생성 시 꼭 해줘야 합니다.
        public StylesElement Styles {
            get { return _styles ?? (_styles = new StylesElement()); }
            set { _styles = value; }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            // NOTE: 자동으로 속성을 XmlAttribute로 생성합니다 (개발 중)
            // ChartExtensions.WriteChartAttribute((ChartBase)this, writer);

            if(Id.IsNotWhiteSpace())
                writer.WriteAttributeString("id", Id);
            if(Caption.IsNotWhiteSpace())
                writer.WriteAttributeString("caption", Caption);
            if(SubCaption.IsNotWhiteSpace())
                writer.WriteAttributeString("subCaption", SubCaption);

            if(Animation.HasValue)
                writer.WriteAttributeString("animation", Animation.Value.GetHashCode().ToString());
            if(DefaultAnimation.HasValue)
                writer.WriteAttributeString("defaultAnimation", DefaultAnimation.Value.GetHashCode().ToString());

            if(Palette.HasValue)
                writer.WriteAttributeString("palette", Palette.Value.ToString());

            if(PaletteThemeColor.HasValue)
                writer.WriteAttributeString("paletteThemeColor", PaletteThemeColor.Value.ToHexString());

            if(ShowPrintMenuItem.HasValue)
                writer.WriteAttributeString("showPrintMenuItem", ShowPrintMenuItem.Value.GetHashCode().ToString());

            if(ShowLabels.HasValue)
                writer.WriteAttributeString("showLabels", ShowLabels.Value.GetHashCode().ToString());
            if(ShowShadow.HasValue)
                writer.WriteAttributeString("ShowShadow", ShowShadow.GetHashCode().ToString());
            if(ShowValues.HasValue)
                writer.WriteAttributeString("ShowValues", ShowValues.GetHashCode().ToString());
            if(RotateValues.HasValue)
                writer.WriteAttributeString("RotateValues", RotateValues.GetHashCode().ToString());

            if(_clickURL != null)
                _clickURL.GenerateXmlAttributes(writer);

            if(_backgroundAttr != null)
                _backgroundAttr.GenerateXmlAttributes(writer);
            if(_borderAttr != null)
                _borderAttr.GenerateXmlAttributes(writer);
            if(_cosmeticAttr != null)
                _cosmeticAttr.GenerateXmlAttributes(writer);
            if(_baseFontAttr != null)
                _baseFontAttr.GenerateXmlAttributes(writer);

            if(_toolTipAttr != null)
                _toolTipAttr.GenerateXmlAttributes(writer);
            if(_logoAttr != null)
                _logoAttr.GenerateXmlAttributes(writer);
            if(_aboutMenuItemAttr != null)
                _aboutMenuItemAttr.GenerateXmlAttributes(writer);
            if(_exportDataMenuItemAttr != null)
                _exportDataMenuItemAttr.GenerateXmlAttributes(writer);
            if(_exportAttr != null)
                _exportAttr.GenerateXmlAttributes(writer);
            if(_legendAttr != null)
                _legendAttr.GenerateXmlAttributes(writer);
            if(_scrollBarAttr != null)
                _scrollBarAttr.GenerateXmlAttributes(writer);
            if(_marginAttr != null)
                _marginAttr.GenerateXmlAttributes(writer);
            if(_numberAttr != null)
                _numberAttr.GenerateXmlAttributes(writer);

            if(CaptionPadding.HasValue)
                writer.WriteAttributeString("CaptionPadding", CaptionPadding.ToString());
            if(XAxisNamePadding.HasValue)
                writer.WriteAttributeString("XAxisNamePadding", XAxisNamePadding.ToString());
            if(LabelPadding.HasValue)
                writer.WriteAttributeString("LabelPadding", LabelPadding.ToString());
            if(ValuePadding.HasValue)
                writer.WriteAttributeString("ValuePadding", ValuePadding.ToString());
            if(CanvasPadding.HasValue)
                writer.WriteAttributeString("CanvasPadding", CanvasPadding.ToString());
            if(LegendPadding.HasValue)
                writer.WriteAttributeString("LegendPadding", LegendPadding.ToString());
            if(PlotSpacePercent.HasValue)
                writer.WriteAttributeString("PlotSpacePercent", PlotSpacePercent.ToString());

            if(_chartMarginAtrr != null)
                _chartMarginAtrr.GenerateXmlAttributes(writer);
            if(_canvasMarginAttr != null)
                _canvasMarginAttr.GenerateXmlAttributes(writer);
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(XmlWriter writer) {
            base.GenerateXmlElements(writer);

            // NOTE: 자동으로 속성을 XmlElement로 생성합니다 (개발 중)
            // ChartExtensions.WriteChartElement((ChartBase)this, writer);

            if(_styles != null)
                _styles.WriteXmlElement(writer);
        }
    }
}