using System;
using System.Linq;
using System.Text;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Fusion Link 방식입니다.
    /// </summary>
    public enum FusionLinkMethod {
        /// <summary>
        /// 단순 Link URL (예: link= "../Ajax/Default.aspx", link="javascript:Popup('abc')" )
        /// </summary>
        Local,

        /// <summary>
        /// POP UP	(예: link="P-detailsPopUp,width=400,height=300,toolbar=no,scrollbars=yes,resizable=yes-../Ajax/Default.aspx")
        /// </summary>
        PopUp,

        /// <summary>
        /// Frame  (예: link="F-detailsFrame-../DetailsDataList.aspx%3FMonth%3DJan")
        /// </summary>
        Frame,

        /// <summary>
        /// 새창으로 띄우기 (예: link="n-http://www.realweb21.com")
        /// </summary>
        NewWindow,

        /// <summary>
        /// Javascript (예: link="j-functionName-param1,param2,param3")
        /// NOTE : 이 방법을 사용하기 위해서는 FusionChart의 registerWithJS=1 로 해주어야 한다. <see cref="ChartUtil.RenderChartHtml(string,string,string,string,string,string,System.Nullable{bool},System.Nullable{bool},System.Nullable{bool})"/> 
        /// </summary>
        Javascript
    }

    /// <summary>
    /// Funsion Chart에서 사용하는 Link 정보
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;set label='Apr' value='494' Link="javascript:PopUp('April');" /&gt;
    /// &lt;set label='May' value='761' Link='P-detailsPopUp,width=400,height=300,toolbar=no,scrollbars=yes,resizable=yes-../Ajax/Default.aspx'/&gt;
    /// </code>
    /// </example>
    [Serializable]
    public class FusionLink : ChartAttributeBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string LINK_POPUP = @"P-detailsPopUp";
        public const string LINK_FRAME = @"F-";
        public const string LINK_WINDOW = @"n-";
        public const string LINK_JAVASCRIPT = @"j-";

        /// <summary>
        /// FusionLink 정보를 문자열로 표현합니다. 내부에서 <see cref="Url"/>를 UrlEncoding을 수행합니다.
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// // PopUp
        /// &lt;set label='May' value='761' Link='P-detailsPopUp,width=400,height=300,toolbar=no,scrollbars=yes,resizable=yes-../Ajax/Default.aspx'/&gt;
        /// // Frame
        /// &lt;set label='May' value='761' Link='F-detailsFrame-../Ajax/Default.aspx'/&gt;
        //  // NewWindow
        /// &lt;set label='May' value='761' Link='n-../Ajax/Default.aspx'/&gt;
        /// // javascript
        /// &lt;set label='Apr' value='494' Link="javascript:PopUp('April');" /&gt;
        /// </code>
        /// </example>
        public static string ToLinkString(FusionLink link) {
            Guard.Assert(link != null, @"link is null.");

            var builder = new StringBuilder();

            if(link.Url.IsNotWhiteSpace()) {
                switch(link.LinkMethod) {
                    case FusionLinkMethod.PopUp:
                        builder.Append(LINK_POPUP);

                        if(link.Width.HasValue)
                            builder.AppendFormat(",width={0}", link.Width);
                        if(link.Height.HasValue)
                            builder.AppendFormat(",height={0}", link.Height);
                        if(link.ToolBar.HasValue)
                            builder.AppendFormat(",toolbar={0}", link.ToolBar.Value.ToYesNo().ToLower());
                        if(link.Scrollbars.HasValue)
                            builder.AppendFormat(",scrollbars={0}", link.Scrollbars.Value.ToYesNo().ToLower());
                        if(link.Resizable.HasValue)
                            builder.AppendFormat(",resizable={0}", link.Resizable.Value.ToYesNo().ToLower());

                        builder.Append("-").Append(link.Url.UrlEncode());
                        break;

                    case FusionLinkMethod.Frame:
                        builder.AppendFormat(LINK_FRAME + "{0}-{1}", link.FrameName, link.Url.UrlEncode());
                        break;

                    case FusionLinkMethod.NewWindow:
                        builder.Append(LINK_WINDOW).Append(link.Url.UrlEncode());
                        break;

                    case FusionLinkMethod.Javascript:
                        builder.Append(LINK_JAVASCRIPT).Append(link.Url);
                        break;
                    default:
                        builder.AppendFormat("{0}", link.Url.UrlEncode());
                        break;
                }
            }

            var linkStr = builder.ToString();

            if(IsDebugEnabled)
                log.Debug("Fusion Link = " + linkStr);

            return linkStr;
        }

        public FusionLink() : this("Link") {}

        public FusionLink(string attrName) {
            AttributeName = (attrName ?? string.Empty).Trim();
            Scrollbars = true;
            Resizable = true;
            LinkMethod = FusionLinkMethod.Local;
        }

        protected string AttributeName { get; set; }

        public int? Width { get; set; }
        public int? Height { get; set; }
        public bool? ToolBar { get; set; }
        public bool? Scrollbars { get; set; }
        public bool? Resizable { get; set; }
        public string FrameName { get; set; }
        public FusionLinkMethod? LinkMethod { get; set; }

        /// <summary>
        /// Link click 시 open해야 할 Url (자체적으로 UrlEncoding을 수행합니다)
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Link 정보를 설정합니다.
        /// </summary>
        /// <param name="linkMethod"></param>
        /// <param name="url"></param>
        public virtual void SetLink(FusionLinkMethod linkMethod, string url) {
            LinkMethod = linkMethod;
            Url = url;
        }

        /// <summary>
        /// Frame 을 타겟으로 하는 Link 를 설정한다.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="frameName"></param>
        public virtual void SetFameLink(string url, string frameName) {
            LinkMethod = FusionLinkMethod.Frame;
            Url = url;
            FrameName = frameName;
        }

        /// <summary>
        /// Javascript Fuction Call link (예: link="j-functionName-param1,param2,param3")
        /// NOTE : 이 함수를 사용하기 위해서는 FusionChart의 registerWithJS=1 로 해주어야 한다. <see cref="ChartUtil.RenderChartHtml(string,string,string,string,string,string,System.Nullable{bool},System.Nullable{bool},System.Nullable{bool})"/> 
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="parameters"></param>
        public virtual void SetJavascriptLink(string functionName, params string[] parameters) {
            functionName.ShouldNotBeWhiteSpace("functionName");

            LinkMethod = FusionLinkMethod.Javascript;

            Url = functionName;
            if(parameters != null)
                Url += "-" + string.Join(",", parameters.ToArray());
        }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Url.IsNotWhiteSpace())
                writer.WriteAttributeString(AttributeName, ToLinkString(this));
        }
    }
}