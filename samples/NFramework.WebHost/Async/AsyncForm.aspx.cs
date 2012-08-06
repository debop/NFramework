using System;
using System.Threading.Tasks;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web;

namespace NSoft.NFramework.WebHost.Async
{
    public partial class AsyncForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
                LoadSiteAsync();
        }

        protected void LoadSiteAsync()
        {
            var task1 = WebClientFactory.Instance.CreateHttpRequest("http://debop.egloos.com").DownloadDataAsync();
            var task2 = WebClientFactory.Instance.CreateHttpRequest("http://www.daum.net").DownloadDataAsync();

            Task.WaitAll(new Task[] { task1, task2 });

            ResultTextBox1.Text = task1.Result.ToText().AsText("에러");
            ResultTextBox2.Text = task2.Result.ToText().AsText("에러");
        }

        protected void btnReload_Click(object sender, EventArgs e)
        {
            LoadSiteAsync();
        }
    }
}