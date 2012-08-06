using System;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.WebHost.PageStatePersisters
{
	public partial class ViewStateForm : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if(IsPostBack == false)
			{
				ViewState["CurrentTime"] = DateTime.Now;

				// 엄청 큰 데이타를 Client로 내려보내지 않고 서버에서 관리합니다.
				//
				ViewState["LargeData"] = ArrayTool.GetRandomBytes(1024 * 128);
				ViewState["LargeStr"] = StringTool.Replicate("동해물과 백두산이 마르고 닳도록", 100);
			}
		}

		protected void btnSubmit_Click(object sender, EventArgs e)
		{
			lblCurrentTime.Text = ViewState["CurrentTime"].AsText("ViewState 정보를 저장소에서 읽어오지 못했습니다.");
			lblLargeStr.Text = ViewState["LargeStr"].AsText().EllipsisChar(80);
		}
	}
}