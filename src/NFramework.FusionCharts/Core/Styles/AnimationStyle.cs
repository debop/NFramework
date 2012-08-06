using System;
using System.Xml;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Using the Animation Style Type, you can virtually animate each and every object on the chart. You can define custom animations and apply them to various objects of the chart. 
    /// </summary>
    /// <remarks>
    /// Each chart object has a list of supported animation properties that can be set through Animation Style Type. Before we get to the parameters supported by the animation style type, let's quickly glance through the properties of chart objects that we can animate using Animation Style Type. 
    /// </remarks>
    [Serializable]
    public class AnimationStyle : ChartStyleElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="name"></param>
        public AnimationStyle(string name) : base(name, "Animation") {}

        [CLSCompliant(false)]
        public AnimationParam? Param { get; set; }

        public bool? Start { get; set; }
        public int? Duration { get; set; }
        public AnimationErasing? Erasing { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Param.HasValue)
                writer.WriteAttributeString("Param", Param.Value.ToString());
            if(Start.HasValue)
                writer.WriteAttributeString("Start", Start.Value.GetHashCode().ToString());
            if(Duration.HasValue)
                writer.WriteAttributeString("Duration", Duration.Value.ToString());
            if(Erasing.HasValue)
                writer.WriteAttributeString("Erasing", Erasing.Value.ToString());
        }
    }
}