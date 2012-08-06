using System;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Chart 에 Logo 를 표시하기 위한 Attribute 입니다.
    /// </summary>
    [Serializable]
    public class LogoAttribute : ChartAttributeBase {
        /// <summary>
        /// 로고를 나타낼 이미지의 URL
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// 로고를 나타낼 위치
        /// </summary>
        public RectangleCorner? LogoPosition { get; set; }

        /// <summary>
        /// 투명도
        /// </summary>
        public int? LogoAlpha { get; set; }

        /// <summary>
        /// 배율
        /// </summary>
        public int? LogoScale { get; set; }

        private FusionLink _logoLink;

        /// <summary>
        /// 로고 클릭 시의 링그 정보
        /// </summary>
        public virtual FusionLink LogoLink {
            get { return _logoLink ?? (_logoLink = new FusionLink("logoLink")); }
            set { _logoLink = value; }
        }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(LogoUrl.IsNotWhiteSpace())
                writer.WriteAttributeString("logoUrl", LogoUrl);
            if(LogoPosition.HasValue)
                writer.WriteAttributeString("logoPosition", LogoPosition.Value.ToString());
            if(LogoAlpha.HasValue)
                writer.WriteAttributeString("logoAlpha", LogoAlpha.ToString());
            if(LogoScale.HasValue)
                writer.WriteAttributeString("logoScale", LogoScale.ToString());

            if(_logoLink != null)
                _logoLink.GenerateXmlAttributes(writer);
        }
    }
}