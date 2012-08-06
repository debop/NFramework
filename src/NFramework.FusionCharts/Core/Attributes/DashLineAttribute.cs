using System;

namespace NSoft.NFramework.FusionCharts {
    [Serializable]
    public class DashLineAttribute : LineAttribute {
        /// <summary>
        /// Constructor
        /// </summary>
        public DashLineAttribute() : base("line") {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="prefix"></param>
        public DashLineAttribute(string prefix) : base(prefix) {}

        // NOTE : IsDashed와 Dashed가 혼용되어 사용되어도, Chart에서 설정이 가능하도록 했다.
        public bool? Dashed {
            get { return IsDashed; }
            set { IsDashed = value; }
        }

        public int? DashLen { get; set; }
        public int? DashGap { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Dashed.HasValue)
                writer.WriteAttributeString(Prefix + "Dashed", Dashed.GetHashCode().ToString());

            if(DashLen.HasValue)
                writer.WriteAttributeString(Prefix + "DashLen", DashLen.ToString());

            if(DashGap.HasValue)
                writer.WriteAttributeString(Prefix + "DashGap", DashGap.ToString());
        }
    }
}