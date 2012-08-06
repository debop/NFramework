using System.Drawing;
using System.Linq;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// 프로세스 컬렉션 요소를 나타내는 클래스입니다.
    /// </summary>
    public class ProcessesElement : CollectionElement<ProcessElement> {
        public ProcessesElement() : base("processes") {}

        public string HeaderText { get; set; }
        public HorizontalPosition? PositionInGrid { get; set; }
        public string Width { get; set; }

        public Color? BgColor { get; set; }
        public int? BgAlpha { get; set; }
        public string Label { get; set; }
        public FusionTextAlign? Align { get; set; }
        public FusionVerticalAlign? VAlign { get; set; }

        private FontAttribute _fontAttr;

        public FontAttribute FontAttr {
            get { return _fontAttr ?? (_fontAttr = new FontAttribute()); }
            set { _fontAttr = value; }
        }

        private ItemAttribute _headerAttr;

        public ItemAttribute HeaderAttr {
            get { return _headerAttr ?? (_headerAttr = new ItemAttribute("header")); }
            set { _headerAttr = value; }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(HeaderText.IsNotWhiteSpace())
                writer.WriteAttributeString("HeaderText", HeaderText);
            if(PositionInGrid.HasValue)
                writer.WriteAttributeString("PositionInGrid", PositionInGrid.Value.ToString());
            if(Width.IsNotWhiteSpace())
                writer.WriteAttributeString("Width", Width);

            if(_headerAttr != null)
                _headerAttr.GenerateXmlAttributes(writer);

            if(BgColor.HasValue)
                writer.WriteAttributeString("BgColor", BgColor.Value.ToHexString());
            if(BgAlpha.HasValue)
                writer.WriteAttributeString("BgAlpha", BgAlpha.Value.ToString());
            if(Label.IsNotWhiteSpace())
                writer.WriteAttributeString("Label", Label);
            if(Align.HasValue)
                writer.WriteAttributeString("Align", Align.Value.ToString().ToLower());
            if(VAlign.HasValue)
                writer.WriteAttributeString("VAlign", VAlign.Value.ToString().ToLower());

            if(_fontAttr != null)
                _fontAttr.GenerateXmlAttributes(writer);
        }

        /// <summary>
        /// 새로운 Process를 추가합니다.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public virtual ProcessElement AddProcess(string id, string label) {
            var process = new ProcessElement(id) { ItemAttr = { Label = label } };
            Add(process);
            return process;
        }

        /// <summary>
        /// 지정한 Id를 가진 <see cref="ProcessElement"/>를 찾습니다. 없으면 null을 반환합니다.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual ProcessElement FindProcessById(string id) {
            return this.FirstOrDefault(processElement => processElement.Id.EqualTo(id));
        }
    }
}