using System;
using System.Web;
using System.Web.UI.WebControls;
using NSoft.NFramework.Web;
using NSoft.NFramework.Web.Tools;

namespace RCL.Web.Pages
{
    /// <summary>
    /// 웹 Application에서 사용하는 UserControl (ascx)의 기본 class
    /// </summary>
    public abstract class DefaultControlBase : System.Web.UI.UserControl
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <paramref name="placeHolder"/>에 제목을 설정한다.
        /// </summary>
        /// <param name="placeHolder"></param>
        protected virtual void SetTitle(PlaceHolder placeHolder)
        {
            SetTitle(placeHolder, true);
        }

        /// <summary>
        /// 지정된 <see cref="PlaceHolder"/>에 제목을 설정한다.
        /// </summary>
        /// <param name="placeHolder"></param>
        /// <param name="isContent"></param>
        protected virtual void SetTitle(PlaceHolder placeHolder, bool isContent)
        {
            SiteMapNode currentNode = SiteMap.CurrentNode;
        }

        /// <summary>
        /// 상태 메시지 출력
        /// </summary>
        protected static string GetStatusMessage(int offset, int total)
        {
            if(total > 0)
                return string.Format(WebAppTool.GetGlobalResourceString(AppSettings.ResourceMessages, "Msg_Query_Items_Item"), total, offset + 1);

            return WebAppTool.GetGlobalResourceString(AppSettings.ResourceMessages, "Msg_Query_Items_NoItem");
        }
    }
}