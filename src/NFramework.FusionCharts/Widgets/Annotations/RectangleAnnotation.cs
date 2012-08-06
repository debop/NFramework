using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// 사각형 영역을 가지는 Annotaion
    /// </summary>
    public class RectangleAnnotation : AnnotationElementBase {
        #region Overrides of AnnotationElementBase

        public override AnnotationType Type {
            get { return AnnotationType.Rectangle; }
        }

        #endregion

        /// <summary>
        /// This attribute sets the background fill color for the annotation. You can set any hex color code as the value of this attribute. 
        /// To specify a gradient as background color, separate the hex color codes of each color in the gradient using comma. 
        /// Example: FF5904,FFFFFF. Remember to remove # and any spaces in between. See the gradient specification page for more details.
        /// </summary>
        public string FillColor { get; set; }

        /// <summary>
        /// Sets the alpha (transparency) for the background fill. 
        /// If you've opted for gradient background, you need to set a list of alpha(s) separated by comma. 
        /// If the number of alpha values you have specified is lesser than the number of colors in the gradient, 
        /// then the remaining colors would by default have an alpha of 100.
        /// </summary>
        public int? FillAlpha { get; set; }

        /// <summary>
        /// If you've opted for a gradient background fill, this attribute lets you set the ratio of each color constituent.
        /// Tha ratio for the colors should be separated by commas and the same must sum upto 100. 
        /// For example, if you have 2 colors in your gradient, then fillRatio could be 80,20. There should be no spaces in between the values.
        /// </summary>
        public string FillRatio { get; set; }

        /// <summary>
        /// Angle of the background fill color, in case of a gradient. 
        /// Evey color in the gradient can have a fill angle (which have to be separated by commas without any spaces in between). 
        /// If the number of angle values is less than the number of colors in the gradient, then a default value of 0 is taken for the remaining colors. 
        /// For example, fillAngle='90, 270' for a gradient having 2 colors.
        /// </summary>
        public string FillAngle { get; set; }

        /// <summary>
        /// The pattern of the gradient. It takes a default value of radial for circle and arc and linear for the others.
        /// </summary>
        public FillMethod? FillPattern { get; set; }

        private BorderAttribute _border;

        public virtual BorderAttribute Border {
            get { return _border ?? (_border = new BorderAttribute()); }
            set { _border = value; }
        }

        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(FillColor.IsNotWhiteSpace())
                writer.WriteAttributeString("fillColor", FillColor);
            if(FillAlpha.HasValue)
                writer.WriteAttributeString("FillAlpha", FillAlpha.ToString());
            if(FillRatio.IsNotWhiteSpace())
                writer.WriteAttributeString("FillRatio", FillRatio);
            if(FillAngle.IsNotWhiteSpace())
                writer.WriteAttributeString("FillAngle", FillAngle);
            if(FillPattern.HasValue)
                writer.WriteAttributeString("FillPattern", FillPattern.ToString());

            if(_border != null)
                _border.GenerateXmlAttributes(writer);
        }
    }
}