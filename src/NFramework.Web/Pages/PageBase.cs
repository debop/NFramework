using System;
using NSoft.NFramework.IO;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web;
using Telerik.Web.UI;

namespace RCL.Web.Pages
{
    /// <summary>
    /// 최상위 PageBase
    /// </summary>
    public class PageBase : System.Web.UI.Page
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Page의 RadAjaxManager
        /// </summary>
        public virtual RadAjaxManager AjaxManager
        {
            get
            {
                try
                {
                    var ajaxManager = RadAjaxManager.GetCurrent(Page);
                    return ajaxManager;
                }
                catch(Exception ex)
                {
                    if(log.IsWarnEnabled)
                        log.WarnException("RadAjaxManager을 찾지 못하였습니다. Page에 RadAjaxManager를 추가하세요.", ex);
                }
                return null;
            }
        }

        /// <summary>
        /// 페이지를 초기화
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            // 설정할 마스터 페이지가 있다면
            var masterPageFile = GetMasterPageFile();
            if(masterPageFile.IsNotWhiteSpace())
                MasterPageFile = masterPageFile;
        }

        /// <summary>
        /// 마스터 페이지를 리턴한다. OnInit 시 호출되어 할당함
        /// </summary>
        /// <returns>마스터 페이지 경로</returns>
        public virtual string GetMasterPageFile()
        {
            if(MasterPageFile.IsWhiteSpace())
                return string.Empty;

            var masterPageFileName = MasterPageFile.ExtractFileName();
            var themeName = WebAppContext.Current.ThemeName.IsWhiteSpace() ? AppSettings.DefaultThemeAssemblyName : WebAppContext.Current.ThemeName;
            var masterPageFile = string.Format("~/Commons/MasterPages/{0}/{1}", themeName, masterPageFileName);

            return FileTool.GetPhysicalPath(masterPageFile).FileExists() ? masterPageFile : MasterPageFile;
        }
    }
}