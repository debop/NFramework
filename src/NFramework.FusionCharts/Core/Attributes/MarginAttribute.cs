namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Chart의 Margin Attributes
    /// </summary>
    public class MarginAttribute : ChartAttributeBase {
        /// <summary>
        /// Default constructor
        /// </summary>
        public MarginAttribute() : this(string.Empty) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="prefix"></param>
        public MarginAttribute(string prefix) {
            Prefix = (prefix ?? string.Empty).Trim();
        }

        protected string Prefix { get; private set; }

        public int? LeftMargin { get; set; }
        public int? RightMargin { get; set; }
        public int? TopMargin { get; set; }
        public int? BottomMargin { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(LeftMargin.HasValue)
                writer.WriteAttributeString(Prefix + "LeftMargin", LeftMargin.Value.ToString());
            if(RightMargin.HasValue)
                writer.WriteAttributeString(Prefix + "RightMargin", RightMargin.Value.ToString());
            if(TopMargin.HasValue)
                writer.WriteAttributeString(Prefix + "TopMargin", TopMargin.Value.ToString());
            if(BottomMargin.HasValue)
                writer.WriteAttributeString(Prefix + "BottomMargin", BottomMargin.Value.ToString());
        }
    }
}