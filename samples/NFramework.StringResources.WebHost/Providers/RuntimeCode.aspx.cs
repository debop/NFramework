using System;

namespace NSoft.NFramework.StringResources.WebHost.Providers {
    public partial class RuntimeCode : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            // App_LocalResources or App_GlobalResource 에서 얻기
            //
            lblHelloLocal.Text = GetLocalResourceObject("lblHelloLocalResource1.Text") as string;
            lblHelloGlobal.Text = GetGlobalResourceObject("CommonTerms", "Hello") as string;

            // WebResource ExpressionBuilder 에서 제공하는 Provider를 이용하여 String Resource 정보를 가져온다.
            lblHelloGlobal.Text =
                NSoft.NFramework.StringResources.WebResourceExpressionBuilder.GetGlobalResourceObject("CommonTerms", "Hello") as string;
        }
    }
}