namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// 다각형 영역을 가지는 Annotaion
    /// </summary>
    public class PolygonAnnotation : ArcAnnotation {
        public override AnnotationType Type {
            get { return AnnotationType.Polygon; }
        }
    }
}