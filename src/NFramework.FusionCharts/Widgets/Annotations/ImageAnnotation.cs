using NSoft.NFramework.Tools;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Image Annotation
    /// </summary>
    public class ImageAnnotation : AnnotationElementBase {
        public override AnnotationType Type {
            get { return AnnotationType.Image; }
        }

        /// <summary>
        /// The URL of the image to be used as annotation. It should be in the same domain as the chart.
        /// </summary>
        public string Url { get; set; }

        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Url.IsNotWhiteSpace())
                writer.WriteAttributeString("url", Url.UrlEncode());
        }
    }
}