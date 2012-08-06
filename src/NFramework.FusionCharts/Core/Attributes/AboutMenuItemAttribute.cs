using System;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Chart About Menu Item 에 대한 Attributes
    /// </summary>
    /// <example>
    /// &lt;chart yAxisName='Sales Figure' caption='Top 5 Sales Person' numberPrefix='$' useRoundEdges='1' showAboutMenuItem='1' aboutMenuItemLabel='About My Company' aboutMenuItemLink='n-http://www.mycompany.com'&gt; 
    /// </example>
    [Serializable]
    public class AboutMenuItemAttribute : MenuItemAttributeBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public AboutMenuItemAttribute() : base("About") {}

        private FusionLink _link;

        /// <summary>
        /// About에 대한 Link
        /// </summary>
        public virtual FusionLink Link {
            get { return _link ?? (_link = new FusionLink("AboutMenuItemLink")); }
            set { _link = value; }
        }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer"></param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(_link != null)
                _link.GenerateXmlAttributes(writer);
        }
    }
}