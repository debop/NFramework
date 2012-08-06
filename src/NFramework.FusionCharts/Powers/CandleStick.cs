using System.Drawing;
using NSoft.NFramework.FusionCharts.Charts;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Powers {
    /// <summary>
    /// The candle stick chart from FusionCharts v3 PowerCharts suite offers your a powerful and interactive interface to plot your stock data. It offers the following features:
    /// Features:
    ///	<list>
    ///		<item>Integrated price and volume plot. Volume plot can be turned on or off.</item>
    ///		<item>Multiple options for price plotting - Candle stick / bar / line</item>
    ///		<item>Interactive tool tip and customizable hover bar</item>
    ///		<item>Allows you to plot trend lines and zones on the chart</item>
    ///		<item>Apart from trend lines and zones, you can plot any number of custom trend sets on the chart. These trend sets can be customized to show any technical indicators.</item>
    ///		<item>You can easily provide missing data using this chart.</item>
    ///		<item>Any particular candle can be highlighted using color combination.</item>
    ///		<item>Vertical indicator lines can be drawn at any point on x-axis.</item>
    /// </list>
    /// </summary>
    public class CandleStick : FusionChartBase {
        #region << Attributes >>

        #region << Chart Attributes >>

        /// <summary>
        /// This attribute lets you configure how your price chart would be plotted as. You can use either of the three options - CANDLESTICK, BAR or LINE
        /// </summary>
        public PricePlotMethod? PlotPriceAs { get; set; }

        /// <summary>
        /// If you've opted to plot the price chart as line, this attribute lets you configure whether the closing price will be used for plot or opening price.
        /// </summary>
        public bool? PlotClosingPrice { get; set; }

        /// <summary>
        /// If you want to hide volume chart, you can use this attribute.
        /// </summary>
        public bool? ShowVolumeChart { get; set; }

        /// <summary>
        /// Height of the volume chart in percent (with respect to total chart). By default, it's set to 40%. Range is between 20-80%.
        /// </summary>
        public int? VolumeHeightPercent { get; set; }

        /// <summary>
        /// The candlestick chart has both x and y axis as numeric. This attribute lets you explicitly set the x-axis lower limit. If you do not specify this value, FusionCharts will automatically calculate the best value for you.
        /// </summary>
        public double? XAxisMinValue { get; set; }

        /// <summary>
        /// The candlestick chart has both x and y axis as numeric. This attribute lets you explicitly set the x-axis upper limit. If you do not specify this value, FusionCharts will automatically calculate the best value for you.
        /// </summary>
        public double? XAxisMaxValue { get; set; }

        /// <summary>
        /// This attribute helps you explicitly set the lower limit of the price chart. If you don't specify this value, it is automatically calculated by FusionCharts based on the data provided by you.
        /// </summary>
        public double? PYAxisMinValue { get; set; }

        /// <summary>
        /// This attribute helps you explicitly set the upper limit of the price chart. If you don't specify this value, it is automatically calculated by FusionCharts based on the data provided by you.
        /// </summary>
        public double? PYAxisMaxValue { get; set; }

        /// <summary>
        /// Lower limit for the volume chart axis. If you do not specify this, FusionCharts wll automatically calculate for you.
        /// </summary>
        public double? VYAxisMinValue { get; set; }

        /// <summary>
        /// Upper limit for the volume chart axis. If you do not specify this, FusionCharts wll automatically calculate for you.
        /// </summary>
        public double? VYAxisMaxValue { get; set; }

        /// <summary>
        /// FusionCharts y-axis is divided into vertical sections using div (divisional) lines. Each div line assumes a value based on its position. Using this attribute you can set whether to show those div line (y-axis) values or not.
        /// </summary>
        public bool? ShowYAxisValues { get; set; }

        /// <summary>
        /// 1 or above. 
        /// By default, all div lines show their values. However, you can opt to skip every x(th) div line value using this attribute.
        /// </summary>
        public double? YAxisValuesStep { get; set; }

        /// <summary>
        /// If you do not wish to rotate y-axis name, set this as 0. It specifically comes to use when you've special characters (UTF8) in your y-axis name that do not show up in rotated mode.
        /// </summary>
        public bool? RotateYAxisName { get; set; }

        /// <summary>
        /// Y-axis name for the price chart.
        /// </summary>
        public string PYAxisName { get; set; }

        /// <summary>
        /// Y-axis name for the volume chart.
        /// </summary>
        public string VYAxisName { get; set; }

        #endregion

        #region << Data Plot Cosmetics >>

        /// <summary>
        /// Border color for the bear candles/lines/bars.
        /// </summary>
        public Color? BearBorderColor { get; set; }

        /// <summary>
        /// Fill color for the bear candles/lines/bars.
        /// </summary>
        public Color? BearFillColor { get; set; }

        /// <summary>
        /// Border color for the bull candles/lines/bars.
        /// </summary>
        public Color? BullBorderColor { get; set; }

        /// <summary>
        /// Fill color for the bull candles/lines/bars.
        /// </summary>
        public Color? BullFillColor { get; set; }

        /// <summary>
        /// When you hover your mouse over any candle on the chart, you'll see a colored band moving with your mouse. This attribute lets you set the color of that band
        /// </summary>
        public Color? RollOverBandColor { get; set; }

        /// <summary>
        /// Lets you set the alpha of the color band.
        /// </summary>
        public int? RollOverBandAlpha { get; set; }

        /// <summary>
        /// Whther to show plot border for the volume chart?
        /// </summary>
        public bool? ShowVPlotBorder { get; set; }

        /// <summary>
        /// Border color for the volume chart bars.
        /// </summary>
        public Color? VPlotBorderColor { get; set; }

        /// <summary>
        /// Thickness of border of the volume chart bars.
        /// </summary>
        public int? VPlotBorderThickness { get; set; }

        /// <summary>
        /// Alpha of border of the volume chart bars.
        /// </summary>
        public int? VPlotBorderAlpha { get; set; }

        /// <summary>
        /// Thickness of the lines on the chart, if you're plotting the price chart as lines.
        /// </summary>
        public int? PlotLineThickness { get; set; }

        /// <summary>
        /// Alpha of the lines on the chart.
        /// </summary>
        public int? PlotLineAlpha { get; set; }

        /// <summary>
        /// If the lines are to be shown as dashes, this attribute defines the length of dash.
        /// </summary>
        public int? PlotLineDashLen { get; set; }

        /// <summary>
        /// If the lines are to be shown as dashes, this attribute defines the length of dash gap.
        /// </summary>
        public int? PlotLineDashGap { get; set; }

        #endregion

        #region << Anchors Attributes >>

        /// <summary>
        /// Whether to draw anchors on the chart (in case of line plot only).
        /// </summary>
        public bool? DrawAnchors { get; set; }

        /// <summary>
        /// 3-20
        /// This attribute sets the number of sides the anchor will have. For e.g., an anchor with 3 sides would represent a triangle, with 4 it would be a square and so on.
        /// </summary>
        public int? AnchorSides { get; set; }

        /// <summary>
        /// This attribute sets the radius (in pixels) of the anchor. Greater the radius, bigger would be the anchor size.
        /// </summary>
        public int? AnchorRadius { get; set; }

        /// <summary>
        /// Lets you set the border color of anchors.
        /// </summary>
        public Color? AnchorBorderColor { get; set; }

        /// <summary>
        /// Helps you set border thickness of anchors.
        /// </summary>
        public int? AnchorBorderThickness { get; set; }

        /// <summary>
        /// Helps you set the background color of anchors.
        /// </summary>
        public Color? AnchorBgColor { get; set; }

        /// <summary>
        /// Helps you set the alpha of entire anchors. If you need to hide the anchors on chart but still enable tool tips, set this as 0.
        /// </summary>
        public int? AnchorAlpha { get; set; }

        /// <summary>
        /// Helps you set the alpha of anchor background.
        /// </summary>
        public int? AnchorBgAlpha { get; set; }

        #endregion

        #region << Divisional Lines & Grids >>

        /// <summary>
        /// Number of divisional lines to be plotted on the price chart.
        /// </summary>
        public int? NumPDivLines { get; set; }

        /// <summary>
        /// Alpha (transparency) of the alternate horizontal grid bands.
        /// </summary>
        public int? AlternateHGridAlpha { get; set; }

        /// <summary>
        /// Whether to show alternate colored horizontal grid bands?
        /// </summary>
        public bool? ShowAlternateHGridColor { get; set; }

        /// <summary>
        /// Color of the alternate horizontal grid bands.
        /// </summary>
        public Color? AlternateHGridColor { get; set; }

        #endregion

        #region << Number Format >>

        private CandleStickNumberFormatAttribute _numberAttr;

        /// <summary>
        /// CandleStick Chart 용 Number Format
        /// </summary>
        public new CandleStickNumberFormatAttribute NumberAttr {
            get { return _numberAttr ?? (_numberAttr = new CandleStickNumberFormatAttribute()); }
            set { _numberAttr = value; }
        }

        #endregion

        /// <summary>
        /// 속성 중 Attribute Node로 표현해야 한다.
        /// </summary>
        /// <param name="writer"></param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            #region << Chart Attributes >>

            if(PlotPriceAs.HasValue)
                writer.WriteAttributeString("plotPriceAs", PlotPriceAs.ToString());
            if(PlotClosingPrice.HasValue)
                writer.WriteAttributeString("plotClosingPrice", PlotClosingPrice.GetHashCode().ToString());
            if(ShowVolumeChart.HasValue)
                writer.WriteAttributeString("showVolumeChart", ShowVolumeChart.GetHashCode().ToString());
            if(VolumeHeightPercent.HasValue)
                writer.WriteAttributeString("volumeHeightPercent", VolumeHeightPercent.ToString());
            if(XAxisMinValue.HasValue)
                writer.WriteAttributeString("XAxisMinValue", XAxisMinValue.ToString());
            if(XAxisMaxValue.HasValue)
                writer.WriteAttributeString("XAxisMaxValue", XAxisMaxValue.ToString());
            if(PYAxisMinValue.HasValue)
                writer.WriteAttributeString("PYAxisMinValue", PYAxisMinValue.ToString());
            if(PYAxisMaxValue.HasValue)
                writer.WriteAttributeString("PYAxisMaxValue", PYAxisMaxValue.ToString());
            if(VYAxisMinValue.HasValue)
                writer.WriteAttributeString("VYAxisMinValue", VYAxisMinValue.ToString());
            if(VYAxisMaxValue.HasValue)
                writer.WriteAttributeString("VYAxisMaxValue", VYAxisMaxValue.ToString());

            if(ShowYAxisValues.HasValue)
                writer.WriteAttributeString("ShowYAxisValues", ShowYAxisValues.GetHashCode().ToString());
            if(YAxisValuesStep.HasValue)
                writer.WriteAttributeString("YAxisValuesStep", YAxisValuesStep.ToString());
            if(RotateYAxisName.HasValue)
                writer.WriteAttributeString("RotateYAxisName", RotateYAxisName.GetHashCode().ToString());

            if(PYAxisName.IsNotWhiteSpace())
                writer.WriteAttributeString("PYAxisName", PYAxisName);
            if(VYAxisName.IsNotWhiteSpace())
                writer.WriteAttributeString("VYAxisName", VYAxisName);

            #endregion

            #region << Data Plot Cosmetics >>

            if(BearBorderColor.HasValue)
                writer.WriteAttributeString("BearBorderColor", BearBorderColor.Value.ToHexString());
            if(BearFillColor.HasValue)
                writer.WriteAttributeString("BearFillColor", BearFillColor.Value.ToHexString());
            if(BullBorderColor.HasValue)
                writer.WriteAttributeString("BullBorderColor", BullBorderColor.Value.ToHexString());
            if(BullFillColor.HasValue)
                writer.WriteAttributeString("BullFillColor", BullFillColor.Value.ToHexString());

            if(RollOverBandColor.HasValue)
                writer.WriteAttributeString("RollOverBandColor", RollOverBandColor.Value.ToHexString());
            if(RollOverBandAlpha.HasValue)
                writer.WriteAttributeString("RollOverBandAlpha", RollOverBandAlpha.ToString());
            if(ShowVPlotBorder.HasValue)
                writer.WriteAttributeString("ShowVPlotBorder", ShowVPlotBorder.GetHashCode().ToString());

            if(VPlotBorderColor.HasValue)
                writer.WriteAttributeString("VPlotBorderColor", VPlotBorderColor.Value.ToHexString());
            if(VPlotBorderThickness.HasValue)
                writer.WriteAttributeString("VPlotBorderThickness", VPlotBorderThickness.ToString());
            if(VPlotBorderAlpha.HasValue)
                writer.WriteAttributeString("VPlotBorderAlpha", VPlotBorderAlpha.ToString());
            if(PlotLineThickness.HasValue)
                writer.WriteAttributeString("PlotLineThickness", PlotLineThickness.ToString());
            if(PlotLineAlpha.HasValue)
                writer.WriteAttributeString("PlotLineAlpha", PlotLineAlpha.ToString());
            if(PlotLineDashLen.HasValue)
                writer.WriteAttributeString("PlotLineDashLen", PlotLineDashLen.ToString());
            if(PlotLineDashGap.HasValue)
                writer.WriteAttributeString("PlotLineDashGap", PlotLineDashGap.ToString());

            #endregion

            #region << Anchors Attributes >>

            if(DrawAnchors.HasValue)
                writer.WriteAttributeString("drawAnchors", DrawAnchors.GetHashCode().ToString());
            if(AnchorSides.HasValue)
                writer.WriteAttributeString("AnchorSides", AnchorSides.ToString());
            if(AnchorRadius.HasValue)
                writer.WriteAttributeString("AnchorRadius", AnchorRadius.ToString());
            if(AnchorBorderColor.HasValue)
                writer.WriteAttributeString("AnchorBorderColor", AnchorBorderColor.Value.ToHexString());
            if(AnchorBorderThickness.HasValue)
                writer.WriteAttributeString("AnchorBorderThickness", AnchorBorderThickness.ToString());
            if(AnchorBgColor.HasValue)
                writer.WriteAttributeString("AnchorBgColor", AnchorBgColor.Value.ToHexString());
            if(AnchorAlpha.HasValue)
                writer.WriteAttributeString("AnchorAlpha", AnchorAlpha.ToString());
            if(AnchorBgAlpha.HasValue)
                writer.WriteAttributeString("AnchorBgAlpha", AnchorBgAlpha.ToString());

            #endregion

            #region << Divisional Lines & Grids >>

            if(NumPDivLines.HasValue)
                writer.WriteAttributeString("numPDivLines", NumPDivLines.ToString());

            if(AlternateHGridAlpha.HasValue)
                writer.WriteAttributeString("alternateHGridAlpha", AlternateHGridAlpha.ToString());

            if(ShowAlternateHGridColor.HasValue)
                writer.WriteAttributeString("ShowAlternateHGridColor", ShowAlternateHGridColor.GetHashCode().ToString());
            if(AlternateHGridColor.HasValue)
                writer.WriteAttributeString("AlternateHGridColor", AlternateHGridColor.Value.ToHexString());

            #endregion

            if(_numberAttr != null)
                _numberAttr.GenerateXmlAttributes(writer);
        }

        #endregion

        #region << Elements >>

        private CandleStickCategoriesElement _categories;

        /// <summary>
        /// The categories element lets you bunch together x-axis labels of the chart. 
        /// </summary>
        public new CandleStickCategoriesElement Categories {
            get { return _categories ?? (_categories = new CandleStickCategoriesElement()); }
            set { _categories = value; }
        }

        private CollectionElement<CandleStickSetElement> _dataset;

        public CollectionElement<CandleStickSetElement> Dataset {
            get { return _dataset ?? (_dataset = new CollectionElement<CandleStickSetElement>("dataset")); }
            set { _dataset = value; }
        }

        private TrendSetElement _trendset;

        public TrendSetElement Trendset {
            get { return _trendset ?? (_trendset = new TrendSetElement()); }
            set { _trendset = value; }
        }

        private TrendLinesElement<DoubleLineElement> _trendLines;

        public new TrendLinesElement<DoubleLineElement> TrendLines {
            get { return _trendLines ?? (_trendLines = new TrendLinesElement<DoubleLineElement>("trendLines")); }
            set { _trendLines = value; }
        }

        private TrendLinesElement<DoubleLineElement> _vTrendLines;

        public new TrendLinesElement<DoubleLineElement> VTrendLines {
            get { return _vTrendLines ?? (_vTrendLines = new TrendLinesElement<DoubleLineElement>("vTrendLines")); }
            set { _vTrendLines = value; }
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(System.Xml.XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_categories != null)
                _categories.WriteXmlElement(writer);
            if(_dataset != null)
                _dataset.WriteXmlElement(writer);
            if(_trendset != null)
                _trendset.WriteXmlElement(writer);

            if(_trendLines != null)
                _trendLines.WriteXmlElement(writer);
            if(_vTrendLines != null)
                _vTrendLines.WriteXmlElement(writer);
        }

        #endregion

        /// <summary>
        /// dataset element 하위로 set element를 추가합니다.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="open"></param>
        /// <param name="close"></param>
        /// <param name="high"></param>
        /// <param name="low"></param>
        /// <param name="volume"></param>
        /// <returns></returns>
        public virtual CandleStickSetElement AddSet(double x, double? open, double? close,
                                                    double? high, double? low, double? volume) {
            var set = new CandleStickSetElement
                      {
                          X = x,
                          Open = open,
                          Close = close,
                          High = high,
                          Low = low,
                          Volume = volume
                      };

            Dataset.Add(set);
            return set;
        }
    }
}