using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Chart Style 
    /// </summary>
    public abstract class ChartStyleElementBase : ChartElementBase {
        protected ChartStyleElementBase(string name, string type)
            : base("style") {
            Name = name;
            Type = type;
        }

        public string Name { get; private set; }
        public string Type { get; private set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Name.IsNotWhiteSpace())
                writer.WriteAttributeString("Name", Name);

            if(Type.IsNotWhiteSpace())
                writer.WriteAttributeString("Type", Type);
        }
    }
}