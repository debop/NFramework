using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Fusion Chart Context Menu와 관련된 속성들
    /// </summary>
    public class MenuItemAttributeBase : ChartAttributeBase {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="prefix"></param>
        public MenuItemAttributeBase(string prefix) {
            Prefix = prefix;
        }

        protected string Prefix { get; private set; }

        /// <summary>
        /// 메뉴 보기
        /// </summary>
        public bool? Show { get; set; }

        /// <summary>
        /// 메뉴 라벨
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Show.HasValue)
                writer.WriteAttributeString("Show" + Prefix + "MenuItem", Show.GetHashCode().ToString());

            if(Label.IsNotWhiteSpace())
                writer.WriteAttributeString(Prefix + "MenuItemLabel", Label);
        }
    }
}