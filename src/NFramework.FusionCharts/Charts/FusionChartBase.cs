using System;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// FusionCharts 의 기본 Chart
    /// </summary>
    [Serializable]
    public abstract class FusionChartBase : ChartBase {
        /// <summary>
        /// 생성자
        /// </summary>
        protected FusionChartBase() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="elementName"></param>
        protected FusionChartBase(string elementName) : base(elementName) {}

        #region << Attribute >>

        /// <summary>
        /// While the palette attribute allows to select a palette theme that applies to chart background, canvas, font and tool-tips, it does not change the colors of data items (i.e., column, line, pie etc.). Using paletteColors attribute, you can specify your custom list of hex colors for the data items. The list of colors have to be separated by comma e.g., <chart paletteColors='FF0000,0372AB,FF5904...'>. The chart will cycle through the list of specified colors and then render the data plot accordingly.
        /// To use the same set of colors throughout all your charts in a web application, you can store the list of palette colors in your application globally and then provide the same in each chart XML.
        /// </summary>
        public string PaletteColors { get; set; }

        /// <summary>
        /// Using this attribute, you can control how your data labels (x-axis labels) would appear on the chart. There are 4 options: WRAP, STAGGER, ROTATE or NONE. WRAP wraps the label text if it's longer than the allotted area. ROTATE rotates the label in vertical or slanted position. STAGGER divides the labels into multiple lines.
        /// </summary>
        public LabelDisplayStyle? LabelDisplay { get; set; }

        /// <summary>
        /// This attribute lets you set whether the data labels would show up as rotated labels on the chart.
        /// </summary>
        public bool? RotateLabels { get; set; }

        /// <summary>
        /// If you've opted to show rotated labels on chart, this attribute lets you set the configuration whether the labels would show as slanted labels or fully vertical ones.
        /// </summary>
        public bool? SlantLabels { get; set; }

        /// <summary>
        /// By default, all the labels are displayed on the chart. However, if you've a set of streaming data (like name of months or days of week), you can opt to hide every n-th label for better clarity. This attributes just lets you do so. It allows to skip every n(th) X-axis label.
        /// </summary>
        public int? LabelStep { get; set; }

        /// <summary>
        /// If you've opted for STAGGER mode as labelDisplay, using this attribute you can control how many lines to stagger the label to. By default, all labels are displayed in a single line.
        /// </summary>
        public int? StaggerLines { get; set; }

        public bool? PlaceValuesInside { get; set; }
        public bool? ShowLimits { get; set; }
        public bool? ShowDivLineValues { get; set; }
        public bool? ShowPercentValues { get; set; }
        public bool? AdjustDiv { get; set; }
        public int? MaxColWidth { get; set; }
        public bool? Use3DLighting { get; set; }

        /// <summary>
        /// This attribute lets you set whether the y-axis lower limit would be 0 (in case of all positive values on chart) or should the y-axis lower limit adapt itself to a different figure based on values provided to the chart.
        /// </summary>
        public bool? SetAdaptiveYMin { get; set; }

        /// <summary>
        /// You can render a combination 3D chart as a 2D chart initially. You need to use is2D attribute for this.
        /// </summary>
        public bool? Is2D { get; set; }

        /// <summary>
        /// The chart area can overlap the extra chart elements (caption, sub caption and legend) at the time of scaling (zooming) or at 100% view. If you set chartOnTop attribute to 1 the chart area would be placed above these elements. On the other hand, if you set chartOnTop to 0, caption, sub caption and legend would always appear on the top of the chart.
        /// </summary>
        public bool? ChartOnTop { get; set; }

        public string XAxisName {
            get { return XAxisAttr.Name; }
            set { XAxisAttr.Name = value; }
        }

        public string YAxisName {
            get { return YAxisAttr.Name; }
            set { YAxisAttr.Name = value; }
        }

        /// <summary>
        /// This attribute creates a vertical space between the X-Axis and X-Axis labels.
        /// </summary>
        public int? XLabelGap { get; set; }

        /// <summary>
        /// This attribute creates a horizontal space between the Y-Axis and Y-Axis values.
        /// </summary>
        public int? YLabelGap { get; set; }

        /// <summary>
        /// 2-number of data items present in XML
        /// This attribute lets you control how the number of columns to show in the visible area of the scroll pane.
        /// For example, if you're plotting a chart with 10 columns each in 3 dataset, totalling to 30 columns, 
        /// and you wish to show only 10 columns in the visible area, set this attribute as 10. 
        /// You would now see only 10 columns in the chart - rest of the columns would be visible upon scrolling.
        /// If you want to show all the columns on the chart irrespective of the number of data points in your XML data document, set this attribute as 0.
        /// </summary>
        public int? NumVisiblePlot { get; set; }

        /// <summary>
        /// When the chart renders, you can opt to scroll to the end of the chart (to show data at extreme right) by setting this attribute as 1.
        /// </summary>
        public bool? ScrollToEnd { get; set; }

        private AnchorAttribute _anchor;

        public AnchorAttribute Anchor {
            get { return _anchor ?? (_anchor = new AnchorAttribute()); }
            set { _anchor = value; }
        }

        /// <summary>
        /// Number of horizontal axis division lines that you want on the chart.
        /// </summary>
        public int? NumDivLines { get; set; }

        private DashLineAttribute _divLine;

        public DashLineAttribute DivLine {
            get { return _divLine ?? (_divLine = new DashLineAttribute("divLine")); }
        }

        private ZeroPaneAttribute _zeroPlane;

        public ZeroPaneAttribute ZeroPlane {
            get { return _zeroPlane ?? (_zeroPlane = new ZeroPaneAttribute()); }
            set { _zeroPlane = value; }
        }

        private AlternateGridAttribute _alternateHGrid;

        public AlternateGridAttribute AlternateHGrid {
            get { return _alternateHGrid ?? (_alternateHGrid = new AlternateGridAttribute("AlternateHGrid")); }
            set { _alternateHGrid = value; }
        }

        /// <summary>
        /// Number of horizontal axis division lines that you want on the chart.
        /// </summary>
        public int? NumVDivLines { get; set; }

        private DashLineAttribute _vDivLine;

        public DashLineAttribute VDivLine {
            get { return _vDivLine ?? (_vDivLine = new DashLineAttribute("vDivLine")); }
            set { _vDivLine = value; }
        }

        private AlternateGridAttribute _alternateVGrid;

        public AlternateGridAttribute AlternateVGrid {
            get { return _alternateVGrid ?? (_alternateVGrid = new AlternateGridAttribute("AlternateVGrid")); }
            set { _alternateVGrid = value; }
        }

        /// <summary>
        /// You can apply emboss or bevel effect to both divisional lines and trendlines. The divLineEffect attribute will let you this. 
        /// You can specify one of the three values: "EMBOSS", "BEVEL" or "NONE".
        /// </summary>
        public LineEffect? DivLineEffect { get; set; }

        private FontAttribute _outCnvBaseFontAttr;

        public FontAttribute OutCnvBaseFontAttr {
            get { return _outCnvBaseFontAttr ?? (_outCnvBaseFontAttr = new FontAttribute("outCnvBase")); }
            set {
                _outCnvBaseFontAttr = value;
                if(_outCnvBaseFontAttr != null)
                    _outCnvBaseFontAttr.Prefix = "outCnvBase";
            }
        }

        private DataPlotAttribute _dataPlotAttr;

        public DataPlotAttribute DataPlotAttr {
            get { return _dataPlotAttr ?? (_dataPlotAttr = new DataPlotAttribute()); }
            set { _dataPlotAttr = value; }
        }

        private AxisAttribute _xAxisAttr;

        public AxisAttribute XAxisAttr {
            get { return _xAxisAttr ?? (_xAxisAttr = new AxisAttribute("xAxis")); }
            set { _xAxisAttr = value; }
        }

        private AxisAttribute _yAxisAttr;

        public AxisAttribute YAxisAttr {
            get { return _yAxisAttr ?? (_yAxisAttr = new AxisAttribute("yAxis")); }
            set { _yAxisAttr = value; }
        }

        #endregion

        #region << Element >>

        private CategoriesElement _categories;

        public CategoriesElement Categories {
            get { return _categories ?? (_categories = new CategoriesElement()); }
            set { _categories = value; }
        }

        private TrendLinesElement<LineElementBase> _trendLines;

        public TrendLinesElement<LineElementBase> TrendLines {
            get { return _trendLines ?? (_trendLines = new TrendLinesElement<LineElementBase>("trendlines")); }
            set { _trendLines = value; }
        }

        private TrendLinesElement<LineElementBase> _hTrendLines;

        public TrendLinesElement<LineElementBase> HTrendLines {
            get { return _hTrendLines ?? (_hTrendLines = new TrendLinesElement<LineElementBase>("hTrendLines")); }
            set { _hTrendLines = value; }
        }

        private TrendLinesElement<LineElementBase> _vTrendLines;

        public TrendLinesElement<LineElementBase> VTrendLines {
            get { return _vTrendLines ?? (_vTrendLines = new TrendLinesElement<LineElementBase>("vTrendLines")); }
            set { _vTrendLines = value; }
        }

        private DataSetElement _lineSet;

        public DataSetElement LineSet {
            get { return _lineSet ?? (_lineSet = new DataSetElement("lineSet")); }
            set { _lineSet = value; }
        }

        #endregion

        /// <summary>
        /// Category를 추가합니다.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="showLabel"></param>
        public virtual void AddCategory(string label, bool? showLabel) {
            Categories.Add(new CategoryElement
                           {
                               Label = label,
                               ShowLabel = showLabel
                           });
        }

        /// <summary>
        /// 속성 중 Attribute Node로 표현해야 한다.
        /// </summary>
        /// <param name="writer"></param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            // NOTE: 자동으로 속성을 XmlAttribute로 생성합니다 (개발 중)
            // ChartExtensions.WriteChartAttribute((FusionChartBase)this, writer);

            if(Animation.HasValue)
                writer.WriteAttributeString("Animation", Animation.Value.GetHashCode().ToString());
            if(PaletteColors.IsNotWhiteSpace())
                writer.WriteAttributeString("PaletteColors", PaletteColors);
            if(LabelDisplay.HasValue)
                writer.WriteAttributeString("LabelDisplay", LabelDisplay.Value.ToString());
            if(RotateLabels.HasValue)
                writer.WriteAttributeString("RotateLabels", RotateLabels.GetHashCode().ToString());
            if(SlantLabels.HasValue)
                writer.WriteAttributeString("SlantLabels", SlantLabels.GetHashCode().ToString());
            if(LabelStep.HasValue)
                writer.WriteAttributeString("LabelStep", LabelStep.ToString());
            if(StaggerLines.HasValue)
                writer.WriteAttributeString("StaggerLines", StaggerLines.ToString());

            if(PlaceValuesInside.HasValue)
                writer.WriteAttributeString("PlaceValuesInside", PlaceValuesInside.GetHashCode().ToString());
            if(ShowLimits.HasValue)
                writer.WriteAttributeString("ShowLimits", ShowLimits.GetHashCode().ToString());
            if(ShowDivLineValues.HasValue)
                writer.WriteAttributeString("ShowDivLineValues", ShowDivLineValues.GetHashCode().ToString());
            if(ShowPercentValues.HasValue)
                writer.WriteAttributeString("showPercentValues", ShowPercentValues.GetHashCode().ToString());

            if(AdjustDiv.HasValue)
                writer.WriteAttributeString("AdjustDiv", AdjustDiv.GetHashCode().ToString());

            if(MaxColWidth.HasValue)
                writer.WriteAttributeString("MaxColWidth", MaxColWidth.ToString());
            if(Use3DLighting.HasValue)
                writer.WriteAttributeString("Use3DLighting", Use3DLighting.GetHashCode().ToString());

            if(SetAdaptiveYMin.HasValue)
                writer.WriteAttributeString("SetAdaptiveYMin", SetAdaptiveYMin.GetHashCode().ToString());

            if(Is2D.HasValue)
                writer.WriteAttributeString("is2D", Is2D.GetHashCode().ToString());

            if(ChartOnTop.HasValue)
                writer.WriteAttributeString("chartOnTop", ChartOnTop.GetHashCode().ToString());

            if(XLabelGap.HasValue)
                writer.WriteAttributeString("xLabelGap", XLabelGap.ToString());
            if(YLabelGap.HasValue)
                writer.WriteAttributeString("yLabelGap", YLabelGap.ToString());

            if(NumVisiblePlot.HasValue)
                writer.WriteAttributeString("numVisiblePlot", NumVisiblePlot.ToString());
            if(ScrollToEnd.HasValue)
                writer.WriteAttributeString("scrollToEnd", ScrollToEnd.GetHashCode().ToString());

            if(_anchor != null)
                _anchor.GenerateXmlAttributes(writer);

            if(NumDivLines.HasValue)
                writer.WriteAttributeString("numDivLines", NumDivLines.ToString());

            if(_divLine != null)
                _divLine.GenerateXmlAttributes(writer);

            if(_zeroPlane != null)
                _zeroPlane.GenerateXmlAttributes(writer);

            if(NumVDivLines.HasValue)
                writer.WriteAttributeString("numVDivLines", NumVDivLines.ToString());

            if(_vDivLine != null)
                _vDivLine.GenerateXmlAttributes(writer);

            if(_alternateHGrid != null)
                _alternateHGrid.GenerateXmlAttributes(writer);
            if(_alternateVGrid != null)
                _alternateVGrid.GenerateXmlAttributes(writer);

            if(DivLineEffect.HasValue)
                writer.WriteAttributeString("divLineEffect", DivLineEffect.ToString().ToUpper());

            if(_outCnvBaseFontAttr != null)
                _outCnvBaseFontAttr.GenerateXmlAttributes(writer);

            if(_dataPlotAttr != null)
                _dataPlotAttr.GenerateXmlAttributes(writer);

            if(_xAxisAttr != null)
                _xAxisAttr.GenerateXmlAttributes(writer);
            if(_yAxisAttr != null)
                _yAxisAttr.GenerateXmlAttributes(writer);
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(XmlWriter writer) {
            base.GenerateXmlElements(writer);

            // NOTE: 자동으로 속성을 XmlElement로 생성합니다 (개발 중)
            // ChartExtensions.WriteChartElement((FusionChartBase)this, writer);

            if(_categories != null)
                _categories.WriteXmlElement(writer);

            if(_trendLines != null)
                _trendLines.WriteXmlElement(writer);
            if(_hTrendLines != null)
                _hTrendLines.WriteXmlElement(writer);
            if(_vTrendLines != null)
                _vTrendLines.WriteXmlElement(writer);

            if(_lineSet != null)
                _lineSet.WriteXmlElement(writer);
        }
    }
}