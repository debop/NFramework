using System;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// 3D Chart Attribute
    /// </summary>
    [Serializable]
    public class ThreeDAttribute : ChartAttributeBase {
        /// <summary>
        /// Since this is a true 3D chart, it's rendered as a 3D model that is viewed from a specific camera angle. The camera can simply be defined as the eye of a real viewer. When you have set animate3D to 0, you can define the camera angle using two attributes - cameraAngX and cameraAngY.
        /// When you use animation, you can also set up the starting camera view from which the camera view starts animating, and the final camera view to which the chart would finally be rotated. For this, you need to use startAngX, startAngY,endAngX and endAngY attributes.
        /// Angle values for both sets (cameraAngY,cameraAngY and endAngX,endAngY) are same
        /// </summary>
        public bool? Animate3D { get; set; }

        /// <summary>
        /// -360 ~ 360
        /// cameraAngX attribute lets you specify the camera angle (for view around the chart vertically) from which the chart is viewed initially in no animation mode. If not specified, the default value is 30.
        /// </summary>
        public int? CameraAngX { get; set; }

        /// <summary>
        /// -360 ~ 360
        /// cameraAngY attribute lets you specify the camera angle (for view around the chart horizontally) from which the chart is viewed initially in no animation mode. If not specified, the default value is -45.
        /// </summary>
        public int? CameraAngY { get; set; }

        /// <summary>
        /// -360 ~ 360
        /// This attribute lets you specify the view angle (for view around the chart vertically) at which rotation of the chart starts (when the chart is initialized). The rotation stops at endAngX. If not specified, the default values for both the attributes are 30.
        /// </summary>
        public int? StartAngX { get; set; }

        /// <summary>
        /// -360 ~ 360
        /// This attribute lets you specify the view angle (for view around the chart vertically) at which rotation of the chart ends (when the chart is initialized). The rotation starts at startAngX. If not specified, the default values for both the attributes are 30.
        /// </summary>
        public int? EndAngX { get; set; }

        /// <summary>
        /// -360 ~ 360
        /// This attribute lets you specify the view angle (for view around the chart horizontally) from which rotation of the chart starts (when the chart is initialized). The rotation stops at endAngY. If not specified, the default values for both the attributes are -45.
        /// </summary>
        public int? StartAngY { get; set; }

        /// <summary>
        /// -360 ~ 360
        /// This attribute lets you specify the view angle (for view around the chart horizontally) at which rotation of the chart ends (when the chart is initialized). The rotation starts at startAngY. If not specified, the default values for both the attributes are -45.
        /// </summary>
        public int? EndAngY { get; set; }

        /// <summary>
        /// The Combination 3D Chart has two lighting systems using which the chart elements are lighted up. The light source may be fixed outside the chart, or you might address it as Chart World. This causes only those sides of the chart to get the light which face the light source. Thus, when the chart is manually rotated, dynamic shades are created on the chart surfaces. This system of lighting system is called Dynamic shading.
        /// Another type of lighting system is there where the light source is fixed with the chart. You may call it world lighting. In this case, the light source rotates with the rotation of the chart. Hence, the surface facing the light source gets lightened and continues to be in the bright side despite any manual rotation of the chart being made, whereas the surface not facing the light keeps remaining in darkness with every chart rotation.
        /// By default, the chart is set in world mode. However, the dynamicShading attribute will let you decide whether to keep the chart in world mode or non-world mode. If you set dynamicShading to 1, the chart will be in dynamic shading/non-world mode.
        /// </summary>
        public bool? DynamicShading { get; set; }

        /// <summary>
        /// Using this, you can specify the angular position of the light source (for X-axis) w.r.t the chart world coordinate system.
        /// </summary>
        public int? LightAngX { get; set; }

        /// <summary>
        /// This attribute allows you to specify the angular position of the light source (for Y-axis) w.r.t the chart world coordinate system.
        /// </summary>
        public int? LightAngY { get; set; }

        /// <summary>
        /// bright2D attribute provides maximum brightness to the chart while being rendered in 2D mode. This is applicable only when you've set dynamicShading to 1. However, once you set this attribute to 1, it won't allow you to use lightAngX and lightAngY attributes. Rather, it automatically sets up the light sources to give the brightest view of the chart in 2D mode.
        /// </summary>
        public bool? Bright2D { get; set; }

        /// <summary>
        /// You can control the intensity of the light that falls on the chart elements. The intensity attribute will enable you to do so. The range of this attribute lies between 0 to 10. 10 would provide light with maximum intensity, and you will get the brightest view of the chart. If you set the value to 0, light will be provided with least intensity. However, the chart will never appear in full darkness even you set intensity to 0. A faint light is always made available. By default, the value is set to 2.5.
        /// </summary>
        public int? Intensity { get; set; }

        /// <summary>
        /// It determines the depth of the YZ wall of 3D chart.
        /// </summary>
        public int? YZWallDepth { get; set; }

        /// <summary>
        /// It determines the depth of the ZX wall of 3D chart.
        /// </summary>
        public int? ZXWallDepth { get; set; }

        /// <summary>
        /// It determines the depth of the XY wall of 3D chart.
        /// </summary>
        public int? XYWallDepth { get; set; }

        /// <summary>
        /// In a 3D combination chart, more than one DATAPLOT types exist due to different datasets. Therefore, to get a distinct view of all the plotted datasets you may want to specify a gap between them. The zGapPlot attribute will let you do so.
        /// </summary>
        public int? ZGapPlot { get; set; }

        /// <summary>
        /// You can set the depth (3D thickness) of each DATAPLOT object using zDepth attribute.
        /// </summary>
        public int? ZDepth { get; set; }

        /// <summary>
        /// In a Combination 3D chart, you can plot multiple number of datasets which can be rendered as Column. These column sets can be arranged in the chart in 2 modes: clustered or manhattan. The clustered attribute will let you choose any one of them. By default, the value of clustered is 0, i.e. the chart appears in non-clustered mode. To change the mode to clustered mode, you need to use clustered='1'.
        /// </summary>
        public bool? Clustered { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML 속성으로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Animate3D.HasValue)
                writer.WriteAttributeString("Animate3D", Animate3D.Value.GetHashCode().ToString());
            if(CameraAngX.HasValue)
                writer.WriteAttributeString("CameraAngX", CameraAngX.Value.ToString());
            if(CameraAngY.HasValue)
                writer.WriteAttributeString("CameraAngY", CameraAngY.Value.ToString());

            if(StartAngX.HasValue)
                writer.WriteAttributeString("StartAngX", StartAngX.Value.ToString());
            if(EndAngX.HasValue)
                writer.WriteAttributeString("EndAngX", EndAngX.Value.ToString());

            if(StartAngY.HasValue)
                writer.WriteAttributeString("StartAngY", StartAngY.Value.ToString());
            if(EndAngY.HasValue)
                writer.WriteAttributeString("EndAngY", EndAngY.Value.ToString());

            if(DynamicShading.HasValue)
                writer.WriteAttributeString("DynamicShading", DynamicShading.Value.GetHashCode().ToString());
            if(LightAngX.HasValue)
                writer.WriteAttributeString("LightAngX", LightAngX.Value.ToString());
            if(LightAngY.HasValue)
                writer.WriteAttributeString("LightAngY", LightAngY.Value.ToString());

            if(Bright2D.HasValue)
                writer.WriteAttributeString("Bright2D", Bright2D.Value.GetHashCode().ToString());

            if(Intensity.HasValue)
                writer.WriteAttributeString("Intensity", Intensity.Value.ToString());

            if(YZWallDepth.HasValue)
                writer.WriteAttributeString("YZWallDepth", YZWallDepth.Value.ToString());
            if(ZXWallDepth.HasValue)
                writer.WriteAttributeString("ZXWallDepth", ZXWallDepth.Value.ToString());
            if(XYWallDepth.HasValue)
                writer.WriteAttributeString("XYWallDepth", XYWallDepth.Value.ToString());

            if(ZGapPlot.HasValue)
                writer.WriteAttributeString("ZGapPlot", ZGapPlot.Value.ToString());
            if(ZDepth.HasValue)
                writer.WriteAttributeString("ZDepth", ZDepth.Value.ToString());

            if(Clustered.HasValue)
                writer.WriteAttributeString("Clustered", Clustered.Value.GetHashCode().ToString());
        }
    }
}