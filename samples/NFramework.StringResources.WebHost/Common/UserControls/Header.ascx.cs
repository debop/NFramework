using System;
using System.Globalization;
using System.Threading;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web;

namespace NSoft.NFramework.StringResources.WebHost.Common.UserControls {
    public partial class Header : System.Web.UI.UserControl {
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {
                if(Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "ko")
                    localeKeys.SelectedIndex = 0;
                else
                    localeKeys.SelectedIndex = 1;
            }
        }

        protected void LocaleKey_Changed(object sender, EventArgs e) {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(localeKeys.SelectedValue);

            var cookieName = ConfigTool.GetAppSettings("Localization.UserLocale.CookieName", "_LOCALE_KEY_");
            CookieTool.Set(cookieName, localeKeys.SelectedValue);

            Response.Redirect(Request.RawUrl);
        }
    }
}