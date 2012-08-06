using System.Drawing;
using NSoft.NFramework.FusionCharts.Charts;

namespace NSoft.NFramework.FusionCharts.Powers {
    /// <summary>
    /// A type of chart developed by the Japanese in the 1870s that uses a series of vertical lines to illustrate general levels of supply and demand for certain assets. 
    /// Thick lines are drawn when the price of the underlying asset breaks above the previous high price and is interpreted as an increase in demand for the asset. 
    /// Thin lines are used to represent increased supply when the price falls below the previous low.
    /// </summary>
    /// <remarks>
    /// As you can see, the Kagi chart shows a series of connecting vertical lines. The thickness and direction of the lines are dependent on the price. Suppose the prices are moving in the same direction then the line is extended. However, if prices reverse by a negative amount, a new Kagi line is drawn in a new column. When prices enter a previous high or low, the thickness of the kagi line changes.
    /// One important note about these charts is that they are independent of time and only change direction once a predefined reversal amount is reached. FusionCharts allows you to configure this reversal value either as percentage of range value or absolute value.
    /// FusionCharts also allows you to plot anchors at each data point to show the data points individually. You can opt to show or hide them.
    /// </remarks>
    public class KagiChart : SingleSeriesChart {
        /// <summary>
        /// The numerical (absolute) value which determines whether to make a horizontal shift to plot the next point. For example, if the kagi line is running up (rally or decline in trend doesn’t matter) and the next point to plot is having a value lower than the current point, then we have to run down. If the deviation between the 2 points is greater than the reversalValue, only then we make a horizontal shift to right and move down. Else, the next point is neglected in plotting and we do not move horizontally.
        /// </summary>
        public double? ReversalValue { get; set; }

        /// <summary>
        /// Same as reversalValue attribute, but in percentage (instead of absolute value).
        /// </summary>
        public int? ReversalPercentage { get; set; }

        /// <summary>
        /// Maximum horizontal shift possible. Percentage value with respect to the canvas width.
        /// </summary>
        public double? MaxHShiftPercent { get; set; }

        /// <summary>
        /// Color of line denoting rally trend.
        /// </summary>
        public Color? RallyColor { get; set; }

        /// <summary>
        /// Color of line denoting decline trend.
        /// </summary>
        public Color? DeclineColor { get; set; }

        /// <summary>
        ///	Thickness of line denoting rally trend.
        /// </summary>
        public int? RallyThickness { get; set; }

        /// <summary>
        /// Thickness of line denoting decline trend.
        /// </summary>
        public int? DeclineThickness { get; set; }

        private DashLineAttribute _line;

        /// <summary>
        /// 라인 속성
        /// </summary>
        public DashLineAttribute Line {
            get { return _line ?? (_line = new DashLineAttribute()); }
            set { _line = value; }
        }

        /// <summary>
        /// 속성 중 Attribute Node로 표현해야 한다.
        /// </summary>
        /// <param name="writer"></param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(ReversalValue.HasValue)
                writer.WriteAttributeString("ReversalValue", ReversalValue.Value.ToString());
            if(ReversalPercentage.HasValue)
                writer.WriteAttributeString("ReversalPercentage", ReversalPercentage.Value.ToString());
            if(MaxHShiftPercent.HasValue)
                writer.WriteAttributeString("MaxHShiftPercent", MaxHShiftPercent.Value.ToString());

            if(RallyColor.HasValue)
                writer.WriteAttributeString("RallyColor", RallyColor.Value.ToHexString());
            if(DeclineColor.HasValue)
                writer.WriteAttributeString("DeclineColor", DeclineColor.Value.ToHexString());

            if(RallyThickness.HasValue)
                writer.WriteAttributeString("RallyThickness", RallyThickness.Value.ToString());
            if(DeclineThickness.HasValue)
                writer.WriteAttributeString("DeclineThickness", DeclineThickness.Value.ToString());

            if(_line != null)
                _line.GenerateXmlAttributes(writer);
        }
    }
}