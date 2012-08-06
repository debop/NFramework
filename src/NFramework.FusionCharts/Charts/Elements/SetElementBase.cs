using System;
using System.Drawing;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// Each <set> element (child of <chart> or <dataset> element) represents a set of data which is to be plotted on the graph and determines a set of data which would appear on the chart. 
    /// </summary>
    /// <example>
    /// <code>
    /// For a multi-series chart, it could look like:
    /// &lt;dataset seriesName='2006'&gt;
    /// &lt;set value='35445' /&gt;
    /// &lt;set value='35675' /&gt;
    /// .... and so on ....
    /// &lt;/dataset&gt;
    /// </code>
    /// </example>
    [Serializable]
    public abstract class SetElementBase : ChartElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        protected SetElementBase() : base("set") {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="elementName"></param>
        protected SetElementBase(string elementName) : base(elementName) {}

        /// <summary>
        /// This attribute determines the label for the data item. The label appears on the x-axis of chart.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// If instead of the numerical value of this data, you wish to display a custom string value, you can specify the same here. Examples are annotation for a data item etc.
        /// </summary>
        public string DisplayValue { get; set; }

        /// <summary>
        /// If you want to define your own colors for the data items on chart, use this attribute to specify color for the data item. This attribute accepts hex color codes without #.
        /// </summary>
        public Color? Color { get; set; }

        /// <summary>
        /// This attribute determines the transparency of a data item. The range for this attribute is 0 to 100. 0 means complete transparency (the data item won’t be shown on the graph) and 100 means opaque.
        /// </summary>
        public int? Alpha { get; set; }

        private FusionLink _link;

        /// <summary>
        /// You can define links for individual data items. That enables the end user to click on data items (columns, lines, bars etc.) and drill down to other pages. To define the link for data items, use the link attribute. You can define links that open in same window, new window, pop-up window or frames. Please see "Advanced Charting > Drill Down Charts" for more information. Also, you'll need to URL Encode all the special characters (like ? and &) present in the link.
        /// </summary>
        public FusionLink Link {
            get { return _link ?? (_link = new FusionLink()); }
            set { _link = value; }
        }

        /// <summary>
        /// By default, FusionCharts shows the data item name and value as tool tip text for that data item. But, if you want to display more information for the data item as tool tip, you can use this attribute to specify the same.
        /// </summary>
        public string ToolText { get; set; }

        /// <summary>
        /// You can individually opt to show/hide values of individual data items using this attribute.
        /// </summary>
        public bool? ShowLabel { get; set; }

        /// <summary>
        /// Whether the border of this data item should appear as dashed. This is particularly useful when you want to highlight a data (such as forecast or trend etc.).
        /// </summary>
        public bool? Dashed { get; set; }

        private AnchorAttribute _anchor;

        /// <summary>
        /// On line/area charts, anchors (or marker points) are polygons which appear at the joint of two consecutive lines/area points. These are indicators to show the position of data points. 
        /// The anchors handle tool tips and links for the data points. So, if you opt to not render anchors on a chart, the tool tips and links won't function. You can, however, hide them by setting alpha to 0 and still enable tool tips and links. 
        /// </summary>
        public AnchorAttribute Anchor {
            get { return _anchor ?? (_anchor = new AnchorAttribute()); }
            set { _anchor = value; }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Label.IsNotWhiteSpace())
                writer.WriteAttributeString("label", Label);
            if(DisplayValue.IsNotWhiteSpace())
                writer.WriteAttributeString("displayValue", DisplayValue);
            if(Color.HasValue)
                writer.WriteAttributeString("color", Color.Value.ToHexString());
            if(Alpha.HasValue)
                writer.WriteAttributeString("alpha", Alpha.ToString());

            if(_link != null)
                _link.GenerateXmlAttributes(writer);

            if(ToolText.IsNotWhiteSpace())
                writer.WriteAttributeString("toolText", ToolText);
            if(ShowLabel.HasValue)
                writer.WriteAttributeString("showLabel", ShowLabel.GetHashCode().ToString());
            if(Dashed.HasValue)
                writer.WriteAttributeString("dashed", Dashed.GetHashCode().ToString());

            if(_anchor != null)
                _anchor.GenerateXmlAttributes(writer);
        }
    }
}