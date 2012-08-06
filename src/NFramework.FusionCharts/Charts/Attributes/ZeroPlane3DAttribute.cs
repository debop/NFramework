using System;
using System.Drawing;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// Zero-Ground Plane의 3D 속성을 나타내는 클래스입니다.
    /// </summary>
    [Serializable]
    public class ZeroPlane3DAttribute : ColorAttribute {
        /// <summary>
        /// 경계 보이기
        /// </summary>
        public virtual bool? ShowBorder { get; set; }

        /// <summary>
        /// 경계 색상
        /// </summary>
        public virtual Color? BorderColor { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(ShowBorder.HasValue)
                writer.WriteAttributeString(Prefix + "ShowBorder", ShowBorder.GetHashCode().ToString());

            if(BorderColor.HasValue)
                writer.WriteAttributeString(Prefix + "BorderColor", BorderColor.Value.ToHexString());
        }
    }
}