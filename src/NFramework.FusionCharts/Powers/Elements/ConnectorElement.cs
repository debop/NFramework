using System.Drawing;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Powers {
    /// <summary>
    /// Drag Node Chart 에서 Node들을 연결하는 Connector를 나타낸다.
    /// </summary>
    public class ConnectorElement : ChartElementBase {
        public ConnectorElement() : base("connector") {}

        /// <summary>
        /// From Node Id
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// To Node Id
        /// </summary>
        public string To { get; set; }

        public string Label { get; set; }

        public Color? Color { get; set; }
        public int? Alpha { get; set; }

        public bool? Dashed { get; set; }
        public int? DashGap { get; set; }
        public int? DashLen { get; set; }

        public bool? ArrowAtStart { get; set; }
        public bool? ArrowAtEnd { get; set; }
        public double? Strength { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(From.IsNotWhiteSpace())
                writer.WriteAttributeString("From", From);
            if(To.IsNotWhiteSpace())
                writer.WriteAttributeString("To", To);
            if(Label.IsNotWhiteSpace())
                writer.WriteAttributeString("Label", Label);

            if(Color.HasValue)
                writer.WriteAttributeString("Color", Color.Value.ToHexString());
            if(Alpha.HasValue)
                writer.WriteAttributeString("Alpha", Alpha.Value.ToString());
            if(Dashed.HasValue)
                writer.WriteAttributeString("Dashed", Dashed.Value.GetHashCode().ToString());
            if(DashGap.HasValue)
                writer.WriteAttributeString("DashGap", DashGap.Value.ToString());
            if(DashLen.HasValue)
                writer.WriteAttributeString("DashLen", DashLen.Value.ToString());

            if(ArrowAtStart.HasValue)
                writer.WriteAttributeString("ArrowAtStart", ArrowAtStart.Value.GetHashCode().ToString());
            if(ArrowAtEnd.HasValue)
                writer.WriteAttributeString("ArrowAtEnd", ArrowAtEnd.Value.GetHashCode().ToString());

            if(Strength.HasValue)
                writer.WriteAttributeString("Strength", Strength.Value.ToString());
        }
    }
}