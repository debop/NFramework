using System;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// XY Plot Chart에서 사용하는 Set입니다.
    /// </summary>
    [Serializable]
    public class XYSetElement : SetElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public XYSetElement() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public XYSetElement(double x, double y) {
            X = x;
            Y = y;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public XYSetElement(double x, double y, double z)
            : this(x, y) {
            Z = z;
        }

        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(X.HasValue)
                writer.WriteAttributeString("x", X.Value.ToString());
            if(Y.HasValue)
                writer.WriteAttributeString("y", Y.Value.ToString());
            if(Z.HasValue)
                writer.WriteAttributeString("z", Z.Value.ToString());
        }
    }
}