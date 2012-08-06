using System;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// vLines are vertical separator lines that help you separate blocks of data. These lines run through the height of the chart, thereby segregating data into different blocks. In case of bar charts, they are horizontal and run through the width of chart.
    /// </summary>
    /// <example>
    /// For example, if you're plotting a chart showing monthly sales from October 2005-Mar 2006, you might want to plot a vertical separator line between Dec 2005 and January 2006 to indicate end of year. 
    /// <code>
    /// // In single series charts, the vertical lines are used as under:
    /// <set label='Oct 2005' value='35456' />
    /// <set label='Nov 2005' value='28556' />
    /// <set label='Dec 2005' value='36556' />
    /// <vLine color='FF5904' thickness='2' />
    /// <set label='Jan 2006' value='45456' />
    /// <set label='Feb 2006' value='35456' />
    /// 
    /// // In multi-series charts, it is used between <category> elements as under:
    /// <categories>
    ///     <category label='Oct 2005' />
    ///		<category label='Nov 2005' />
    ///		<category label='Dec 2005' />
    ///		<vLine color='FF5904' thickness='2' />
    ///		<category label='Jan 2006' />
    ///		<category label='Feb 2006' />
    /// </categories>
    /// </code>
    /// </example>
    [Serializable]
    public class VerticalLineElement : LineElement {
        /// <summary>
        /// 생성자
        /// </summary>
        public VerticalLineElement() : base("vLine") {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="elementName"></param>
        protected VerticalLineElement(string elementName) : base(elementName) {}

        /// <summary>
        /// Whether the vertical separator line should appear as dashed.
        /// </summary>
        public bool? Dashed { get; set; }

        /// <summary>
        /// Label for the vertical separator line, if to be shown.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Whether to show a border around the vLine label.
        /// </summary>
        public bool? ShowLabelBorder { get; set; }

        /// <summary>
        /// 0 / 1
        /// Helps configure the position of vertical line i.e., if a vLine is to be plotted between 2 points Jan and Feb, user can set any position between 0 and 1 to indicate the relative position of vLine between these two points (0 means Jan and 1 means Feb). By default, it’s 0.5 to show in between the points.
        /// </summary>
        public double? LinePosition { get; set; }

        /// <summary>
        /// 0 / 1
        /// Helps configure the position of the vLine label by setting a relative position between 0 and 1. In vertical charts, 0 means top of canvas and 1 means bottom. In horizontal charts, 0 means left of canvas and 1 means right.
        /// </summary>
        public HorizontalPosition? LabelPosition { get; set; }

        /// <summary>
        /// Horizontal anchor point for the alignment of vLine label.
        /// </summary>
        public FusionTextAlign? LabelHAlign { get; set; }

        /// <summary>
        /// Vertical anchor point for the alignment of vLine label.
        /// </summary>
        public FusionVerticalAlign? LabelVAlign { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Dashed.HasValue)
                writer.WriteAttributeString("dashed", Dashed.GetHashCode().ToString());
            if(Label.IsNotWhiteSpace())
                writer.WriteAttributeString("label", Label);
            if(ShowLabelBorder.HasValue)
                writer.WriteAttributeString("showLabelBorder", ShowLabelBorder.GetHashCode().ToString());

            if(LinePosition.HasValue)
                writer.WriteAttributeString("linePosition", LinePosition.ToString());
            if(LabelPosition.HasValue)
                writer.WriteAttributeString("labelPosition", LabelPosition.GetHashCode().ToString());
            if(LabelHAlign.HasValue)
                writer.WriteAttributeString("labelHAlign", LabelHAlign.GetHashCode().ToString());
            if(LabelVAlign.HasValue)
                writer.WriteAttributeString("labelVAlign", LabelVAlign.GetHashCode().ToString());
        }
    }
}