using System;
using System.Web.UI;
using NSoft.NFramework.Web;
using NSoft.NFramework.WebHost.Domain.Model;

namespace NSoft.NFramework.WebHost.Compression
{
	public partial class CompressiblePage : Page
	{
		#region << logger >>

		private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

		#endregion

		private const string USER_COOKIE = "USER_INFO";

		// 쿠키 조작은 비동기 작업에서 수행할 수 없다.
		protected void Page_Load(object sender, EventArgs e)
		{
			DoPageLoad();
		}

		private void DoPageLoad()
		{
			UserInfo user = null;

			try
			{
				user = CookieTool.GetChuck<UserInfo>(USER_COOKIE);
			}
			catch(Exception ex)
			{
				if(log.IsWarnEnabled)
					log.WarnException("사용자 정보를 담은 쿠키값을 읽는데 실패했습니다.", ex);
			}

			if(user != null)
				LoadUser(user);

			SaveUser();
		}

		private void SaveUser()
		{
			Response.Cookies.Clear();

			var user = new UserInfo("Peter", "Bromberg",
			                        "101 Park Avenue West", "New York", "NY", "10021",
			                        "pbromberg@nospammin.yahoo.com", "petey", "whodunnit");

			// 쿠키 크기를 더 키우면 Browser에서 에러가 발생할 수 있다. 이때에는 Browser의 모든 cookie값을 삭제한 후 재 시도한다.
			for(var i = 0; i < 300; i++)
				// user.FavoriteMovies.Add(i.ToString(), "Favorite Movie Number-" + i.ToString());
				user.FavoriteMovies.Add("Favorite Movie Number-" + i);

			CookieTool.SetChuck(USER_COOKIE, user);
			var size = Request.Cookies[USER_COOKIE].Value.Length;

			lblMessage.Text = string.Format("Save UserInfo to cookie with compression name=[{0}], size=[{1}] bytes.", USER_COOKIE, size);
		}

		private void LoadUser(UserInfo user)
		{
			if(user == null)
				return;

			lblName.Text = user.FirstName + " " + user.LastName;
			lblAddress.Text = user.Address.ToString();
			lblEmail.Text = user.Email;
			lblAccount.Text = string.Format("UserId=[{0}], Password=[{1}]", user.UserName, user.Password);

			rptFavorites.DataSource = user.FavoriteMovies;
			rptFavorites.DataBind();

			lblMessage.Text = @"Load UserInfo from Cookie";
		}
	}
}