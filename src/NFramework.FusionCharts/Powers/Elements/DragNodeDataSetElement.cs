using System.Drawing;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Powers {
    /// <summary>
    /// Drag Node 의 DataSet 요소 정보
    /// </summary>
    public class DragNodeDataSetElement : CollectionElement<DragNodeSetElement> {
        /// <summary>
        /// 생성자
        /// </summary>
        public DragNodeDataSetElement() : base("dataset") {}

        public string Id { get; set; }
        public bool? AllowDrag { get; set; }
        public string SeriesName { get; set; }
        public Color? Color { get; set; }

        /// <summary>
        /// This attribute sets the alpha (transparency) of the entire data-set. 
        /// 0-100 or Comma Separated List 
        /// </summary>
        public string Alpha { get; set; }

        public bool? ShowValues { get; set; }
        public bool? IncludeInLegend { get; set; }

        private BorderAttribute _plotBorderAttr;

        public BorderAttribute PlotBorderAttr {
            get { return _plotBorderAttr ?? (_plotBorderAttr = new BorderAttribute("Plot")); }
            set { _plotBorderAttr = value; }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Id.IsNotWhiteSpace())
                writer.WriteAttributeString("id", Id);
            if(AllowDrag.HasValue)
                writer.WriteAttributeString("allowDrag", AllowDrag.Value.GetHashCode().ToString());
            if(SeriesName.IsNotWhiteSpace())
                writer.WriteAttributeString("SeriesName", SeriesName);
            if(Color.HasValue)
                writer.WriteAttributeString("Color", Color.Value.ToHexString());
            if(Alpha.IsNotWhiteSpace())
                writer.WriteAttributeString("Alpha", Alpha);
            if(ShowValues.HasValue)
                writer.WriteAttributeString("ShowValues", ShowValues.Value.GetHashCode().ToString());
            if(IncludeInLegend.HasValue)
                writer.WriteAttributeString("IncludeInLegend", IncludeInLegend.Value.GetHashCode().ToString());

            if(_plotBorderAttr != null)
                _plotBorderAttr.GenerateXmlAttributes(writer);
        }
    }
}