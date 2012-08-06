using System;
using System.Collections.Generic;
using System.Drawing;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// Each <dataset> element contains a series of data.
    /// For example, if we're plotting a monthly sales comparison chart for 2 successive years, the first data-set would contain the data for first year and the next one for the second year. 
    /// You can provide data-set level cosmetics so that all data within that data-set will be plotted in the same color/alpha/etc.
    /// Also, each data-set can be named using the seriesName attribute of <dataset/> element. The series name appears in the legend of the chart. In our previous example, the series name could have well been 2005 and 2006 for first and second data-set respectively.
    /// Depending on the chart type, there are a number of properties which you can define for each <dataset/> element.
    /// </summary>
    [Serializable]
    public class DataSetElement : ChartElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public DataSetElement() : base("dataset") {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="elementName"></param>
        public DataSetElement(string elementName) : base(elementName) {}

        /// <summary>
        /// Lets you specify the series name for a particular data-set. For example, if you're plotting a chart to indicate monthly sales analysis for 2005 and 2006, the seriesName for the first dataset would be 2005 and that of the second would be 2006. The seriesName of a data-set is shown in legend.
        /// </summary>
        public string SeriesName { get; set; }

        /// <summary>
        /// This attribute sets the color using which columns, lines, area of that data-set would be drawn. For column chart, you can specify a list of comma separated hex codes to get a gradient plot.
        /// </summary>
        public Color? Color { get; set; }

        /// <summary>
        /// 0-100 or Comma Separated List.
        /// This attribute sets the alpha (transparency) of the entire data-set.
        /// </summary>
        public string Alpha { get; set; }

        /// <summary>
        /// Comma separated list of ratios.
        /// If you've opted to show columns as gradients, this attribute lets you control the ratio of each color. Please see Gradients under Advanced Charting Section to get more information.
        /// </summary>
        public string Ratio { get; set; }

        /// <summary>
        /// Whether to show the values for this data-set.
        /// </summary>
        public bool? ShowValues { get; set; }

        /// <summary>
        /// Whether this data-set would appear as dashed.
        /// </summary>
        public bool? Dashed { get; set; }

        /// <summary>
        /// Whether to include the seriesName of this data-set in legend. This can be particularly useful when you're using combination charts and you've used the area/line chart to plot a trend, and you do not want the seriesName of that trend to appear in legend.
        /// </summary>
        public bool? IncludeInLegend { get; set; }

        /// <summary>
        /// This attribute defines what the particular data-set will be plotted as. Valid values are COLUMN, AREA or LINE.
        /// </summary>
        public GraphKind? RenderAs { get; set; }

        /// <summary>
        /// P or S.
        /// renderAs attribute over-rides this attribute in FusionCharts v3 for Single Y Combination Charts. This attribute allows you to set the parent axis of the dataset - P (primary) or S (secondary). Primary data-sets are drawn as Columns and secondary data-sets as lines.
        /// </summary>
        public YAxisKind? ParentYAxis { get; set; }

        private AnchorAttribute _anchorAttr;

        public AnchorAttribute AnchorAttr {
            get { return _anchorAttr ?? (_anchorAttr = new AnchorAttribute()); }
            set { _anchorAttr = value; }
        }

        private DashLineAttribute _lineAttr;

        public DashLineAttribute LineAttr {
            get { return _lineAttr ?? (_lineAttr = new DashLineAttribute("line")); }
            set { _lineAttr = value; }
        }

        private IList<ChartElementBase> _setElements;

        /// <summary>
        /// set element와 vLine 등이 들어갈 수 있어서, CollectionElement{T} 를 사용하지 않았습니다.
        /// </summary>
        public IList<ChartElementBase> SetElements {
            get { return _setElements ?? (_setElements = new List<ChartElementBase>()); }
            set { _setElements = value; }
        }

        /// <summary>
        /// set element나 vLine element를 추가합니다.
        /// </summary>
        /// <param name="set"></param>
        public virtual void AddSet(ChartElementBase set) {
            SetElements.Add(set);
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(SeriesName.IsNotWhiteSpace())
                writer.WriteAttributeString("seriesName", SeriesName);
            if(Color.HasValue)
                writer.WriteAttributeString("color", Color.Value.ToHexString());
            if(Alpha.IsNotWhiteSpace())
                writer.WriteAttributeString("alpha", Alpha);
            if(Ratio.IsNotWhiteSpace())
                writer.WriteAttributeString("ratio", Ratio);
            if(ShowValues.HasValue)
                writer.WriteAttributeString("showValues", ShowValues.GetHashCode().ToString());
            if(Dashed.HasValue)
                writer.WriteAttributeString("dashed", Dashed.GetHashCode().ToString());
            if(IncludeInLegend.HasValue)
                writer.WriteAttributeString("includeInLegend", IncludeInLegend.GetHashCode().ToString());

            if(RenderAs.HasValue)
                writer.WriteAttributeString("renderAs", RenderAs.ToString());
            if(ParentYAxis.HasValue)
                writer.WriteAttributeString("parentYAxis", ParentYAxis.ToString());

            if(_anchorAttr != null)
                _anchorAttr.GenerateXmlAttributes(writer);
            if(_lineAttr != null)
                _lineAttr.GenerateXmlAttributes(writer);
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(System.Xml.XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_setElements != null)
                foreach(var set in _setElements)
                    set.WriteXmlElement(writer);
        }
    }
}