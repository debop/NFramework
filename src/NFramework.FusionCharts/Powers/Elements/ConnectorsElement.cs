using System.Drawing;

namespace NSoft.NFramework.FusionCharts.Powers {
    public class ConnectorsElement : CollectionElement<ConnectorElement> {
        public ConnectorsElement() : base("Connectors") {}

        /// <summary>
        /// Using this value, you can specify a number which will be multiplied by individual connectors strength to get the line thickness. Say, you've a connector whose thickness should be 2 pixels and another whose thickness should be 4. So, you can define this attribute as 2 and then specify the strength of first connector as 1 and second one as 2. This is useful when you're plotting network diagrams and you've to show link strength as connector thickness. 
        /// </summary>
        public int? StdThickness { get; set; }

        public Color? Color { get; set; }
        public int? Alpha { get; set; }

        public bool? Dashed { get; set; }
        public int? DashGap { get; set; }
        public int? DashLen { get; set; }

        public bool? ArrowAtStart { get; set; }
        public bool? ArrowAtEnd { get; set; }
        public int? Strength { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(StdThickness.HasValue)
                writer.WriteAttributeString("StdThickness", StdThickness.Value.ToString());
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

        public ConnectorElement AddConnector(string from, string to, string label) {
            var connector =
                new ConnectorElement
                {
                    From = from,
                    To = to,
                    Label = label
                };

            Add(connector);

            return connector;
        }
    }
}