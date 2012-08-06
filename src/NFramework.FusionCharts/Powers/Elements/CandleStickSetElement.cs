using System.Drawing;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Powers {
    /// <summary>
    /// Each <set> element (child of <dataset> element) represents a set of data which is to be plotted on the chart and determines a set of data which would appear on the chart. 
    /// e.g., &lt;set open='33.35' close='33.90' high='34.10' low='33.10' volume='3435670' /&gt;
    /// </summary>
    public class CandleStickSetElement : ChartElementBase {
        public CandleStickSetElement()
            : base("set") {}

        /// <summary>
        /// X-axis value for the plot. The candlestick point will be placed horizontally on the x-axis based on this value.
        /// </summary>
        public virtual double? X { get; set; }

        /// <summary>
        /// Opening price for the set.
        /// </summary>
        public virtual double? Open { get; set; }

        /// <summary>
        /// Closing price for the set.
        /// </summary>
        public virtual double? Close { get; set; }

        /// <summary>
        /// Highest price point reached for the set.
        /// </summary>
        public virtual double? High { get; set; }

        /// <summary>
        /// Lowest price point reached for the set.
        /// </summary>
        public virtual double? Low { get; set; }

        /// <summary>
        /// Volume of transaction. If you do not specify volume for any of the sets, FusionCharts wouldn't plot the volume chart at the bottom.
        /// </summary>
        public virtual double? Volume { get; set; }

        /// <summary>
        /// If you want to show a text label above a candlestick, you can assignt the text to this attribute.
        /// </summary>
        public virtual string ValueText { get; set; }

        /// <summary>
        /// If you need to highlight a particular candlestick, you can assisgn a color to that particular set using this attribute.
        /// </summary>
        public virtual Color? Color { get; set; }

        /// <summary>
        /// If you need to highlight a particular candlestick, you can assisgn a border color to that particular set using this attribute.
        /// </summary>
        public virtual Color? BorderColor { get; set; }

        /// <summary>
        /// Alpha of the particular set.
        /// </summary>
        public virtual int? Alpha { get; set; }

        /// <summary>
        /// Whether to show this set as dashed?
        /// </summary>
        public virtual bool? Dashed { get; set; }

        private FusionLink _link;

        /// <summary>
        /// You can define links for individual data items. That enables the end user to click on candlesticks and drill down to other pages. To define the link for data items, use the link attribute. You can define links that open in same window, new window, pop-up window or frames. Please see "Advanced Charting > Drill Down Charts" for more information. Also, you'll need to URL Encode all the special characters (like ? and &) present in the link.
        /// </summary>
        public FusionLink Link {
            get { return _link ?? (_link = new FusionLink()); }
            set { _link = value; }
        }

        /// <summary>
        /// By default, FusionCharts shows the series Name, Category Name and value as tool tip text for that data item. But, if you want to display more information for the data item as tool tip, you can use this attribute to specify the same.
        /// </summary>
        public virtual string ToolText { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(X.HasValue)
                writer.WriteAttributeString("x", X.ToString());

            if(Open.HasValue)
                writer.WriteAttributeString("open", Open.ToString());
            if(Close.HasValue)
                writer.WriteAttributeString("close", Close.ToString());
            if(High.HasValue)
                writer.WriteAttributeString("high", High.ToString());
            if(Low.HasValue)
                writer.WriteAttributeString("low", Low.ToString());
            if(Volume.HasValue)
                writer.WriteAttributeString("volume", Volume.ToString());

            if(ValueText.IsNotWhiteSpace())
                writer.WriteAttributeString("valueText", ValueText);
            if(Color.HasValue)
                writer.WriteAttributeString("color", Color.Value.ToHexString());
            if(BorderColor.HasValue)
                writer.WriteAttributeString("borderColor", BorderColor.Value.ToHexString());
            if(Alpha.HasValue)
                writer.WriteAttributeString("alpha", Alpha.ToString());
            if(Dashed.HasValue)
                writer.WriteAttributeString("dashed", Dashed.GetHashCode().ToString());

            if(_link != null)
                _link.GenerateXmlAttributes(writer);

            if(ToolText.IsNotWhiteSpace())
                writer.WriteAttributeString("toolText", ToolText);
        }
    }
}