using NSoft.NFramework.FusionCharts.Charts;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Powers {
    /// <summary>
    /// Drag Node Chart의 Node를 나타내는 Element 입니다.
    /// </summary>
    public class DragNodeSetElement : SetElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public DragNodeSetElement() : base("set") {}

        public virtual string Id { get; set; }
        public virtual bool? AllowDrag { get; set; }
        public virtual DragNodeShapes? Shape { get; set; }

        /// <summary>
        /// Pixels
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Pixels
        /// </summary>
        public int? Width { get; set; }

        public int? Radius { get; set; }

        /// <summary>
        /// More than 3.  
        /// If you've selected Polygon as the shape for node, this attribute lets you define the sides for the same.
        /// </summary>
        public int? NumSides { get; set; }

        public bool? ImageNode { get; set; }

        /// <summary>
        /// If the node contains image, this attribute lets you set the path for it. 
        /// Note that the image should be on the same sub-domain as the chart SWF.
        /// </summary>
        public string ImageURL { get; set; }

        public FusionVerticalAlign? ImageAlign { get; set; }
        public int? ImageWidth { get; set; }
        public int? ImageHeight { get; set; }
        public FusionVerticalAlign? LabelAlign { get; set; }

        /// <summary>
        /// X-axis value for the node. The node will be placed horizontally on the x-axis based on this value. 
        /// </summary>
        public int? X { get; set; }

        /// <summary>
        /// Y-axis value for the node. The node will be placed vertical on the y-axis based on this value. 
        /// </summary>
        public int? Y { get; set; }

        public bool? ShowValue { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Id.IsNotWhiteSpace())
                writer.WriteAttributeString("Id", Id);
            if(AllowDrag.HasValue)
                writer.WriteAttributeString("AllowDrag", AllowDrag.Value.GetHashCode().ToString());
            if(Shape.HasValue)
                writer.WriteAttributeString("Shape", Shape.Value.ToString());

            if(Height.HasValue)
                writer.WriteAttributeString("Height", Height.Value.ToString());
            if(Width.HasValue)
                writer.WriteAttributeString("Width", Width.Value.ToString());
            if(Radius.HasValue)
                writer.WriteAttributeString("Radius", Radius.Value.ToString());

            if(NumSides.HasValue)
                writer.WriteAttributeString("NumSides", NumSides.Value.ToString());
            if(ImageNode.HasValue)
                writer.WriteAttributeString("ImageNode", ImageNode.Value.GetHashCode().ToString());
            if(ImageURL.IsNotWhiteSpace())
                writer.WriteAttributeString("ImageURL", ImageURL);
            if(ImageAlign.HasValue)
                writer.WriteAttributeString("ImageAlign", ImageAlign.Value.ToString());

            if(ImageWidth.HasValue)
                writer.WriteAttributeString("ImageWidth", ImageWidth.Value.ToString());
            if(ImageHeight.HasValue)
                writer.WriteAttributeString("ImageHeight", ImageHeight.Value.ToString());
            if(LabelAlign.HasValue)
                writer.WriteAttributeString("LabelAlign", LabelAlign.Value.ToString());

            if(X.HasValue)
                writer.WriteAttributeString("X", X.Value.ToString());
            if(Y.HasValue)
                writer.WriteAttributeString("Y", Y.Value.ToString());
            if(ShowValue.HasValue)
                writer.WriteAttributeString("ShowValue", ShowValue.Value.GetHashCode().ToString());
        }
    }
}