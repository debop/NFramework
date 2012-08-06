using System;
using System.Web;

namespace NSoft.NFramework.StringResources.WebApp {
    public partial class RuntimeCode : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            // App_LocalResources or App_GlobalResource 에서 얻기
            //
            lblHelloLocal.Text = GetLocalResourceObject("lblHelloLocalResource1.Text") as string;
            lblHelloGlobal.Text = GetGlobalResourceObject("CommonTerms", "Hello") as string;

            // Web Application의 App_GlobalResources 에서 정보를 가져옵니다.
            lblHelloGlobal.Text = HttpContext.GetGlobalResourceObject("CommonTerms", "Hello") as string;
        }
    }
}