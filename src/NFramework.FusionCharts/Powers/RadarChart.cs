using System.Drawing;
using NSoft.NFramework.FusionCharts.Charts;

namespace NSoft.NFramework.FusionCharts.Powers {
    /// <summary>
    /// Radar Chart
    /// </summary>
    public class RadarChart : MultiSeriesChart {
        /// <summary>
        /// In Pixels.
        /// If you want to explicitly specify a radius for the radar chart, use this attribute. Otherwise, FusionCharts will automatically calculate the best-fit radius.
        /// </summary>
        public int? RadarRadius { get; set; }

        private BorderAttribute _plotBorder;

        public BorderAttribute PlotBorder {
            get { return _plotBorder ?? (_plotBorder = new BorderAttribute("plot")); }
        }

        private BorderAttribute _radarBorder;

        public BorderAttribute RadarBorder {
            get { return _radarBorder ?? (_radarBorder = new BorderAttribute("radar")); }
        }

        /// <summary>
        /// Fill color for the radar.
        /// </summary>
        public Color? RadarFillColor { get; set; }

        /// <summary>
        /// Fill alpha for the radar.
        /// </summary>
        public int? RadarFillAlpha { get; set; }

        /// <summary>
        /// Color for radar spikes. Radar spikes are the lines that emanate from the center to the vertex of radar.
        /// </summary>
        public Color? RadarSpikeColor { get; set; }

        /// <summary>
        /// Thickness for radar spikes. Radar spikes are the lines that emanate from the center to the vertex of radar.
        /// </summary>
        public int? RadarSpikeThickness { get; set; }

        /// <summary>
        /// Alpha for radar spikes. Radar spikes are the lines that emanate from the center to the vertex of radar.
        /// </summary>
        public int? RadarSpikeAlpha { get; set; }

        /// <summary>
        /// 속성 중 Attribute Node로 표현해야 한다.
        /// </summary>
        /// <param name="writer"></param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(RadarRadius.HasValue)
                writer.WriteAttributeString("radarRadius", RadarRadius.ToString());
            if(_plotBorder != null)
                _plotBorder.GenerateXmlAttributes(writer);
            if(_radarBorder != null)
                _radarBorder.GenerateXmlAttributes(writer);

            if(RadarFillColor.HasValue)
                writer.WriteAttributeString("radarFillColor", RadarFillColor.Value.ToHexString());
            if(RadarFillAlpha.HasValue)
                writer.WriteAttributeString("radarFillAlpha", RadarFillAlpha.ToString());
            if(RadarSpikeColor.HasValue)
                writer.WriteAttributeString("radarSpikeColor", RadarSpikeColor.Value.ToHexString());
            if(RadarSpikeThickness.HasValue)
                writer.WriteAttributeString("radarSpikeThickness", RadarSpikeThickness.ToString());
            if(RadarSpikeAlpha.HasValue)
                writer.WriteAttributeString("radarSpikeAlpha", RadarSpikeAlpha.ToString());
        }

        private RadarCategoriesElement _categories;

        public new RadarCategoriesElement Categories {
            get { return _categories ?? (_categories = new RadarCategoriesElement()); }
            set { _categories = value; }
        }

        /// <summary>
        /// Category를 추가합니다.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="showLabel"></param>
        public override void AddCategory(string label, bool? showLabel) {
            var category = new RadarCategoryElement
                           {
                               Label = label,
                               ShowLabel = showLabel
                           };
            Categories.Add(category);
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(System.Xml.XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_categories != null)
                _categories.WriteXmlElement(writer);
        }
    }
}