using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using NSoft.NFramework.Parallelism.Tools;

namespace NSoft.NFramework.WebHost.Async
{
	public partial class AsyncEnumeratorTask : System.Web.UI.Page
	{
		private static readonly Uri[] _imageUris = new[]
		                                           {
		                                           	new Uri("http://images.wsdot.wa.gov/nw/520vc00519.jpg"),
		                                           	new Uri("http://images.wsdot.wa.gov/nw/520vc00241.jpg"),
		                                           	new Uri("http://1.1.1.1/NoImage")
		                                           };

		protected void Page_Load(object sender, EventArgs e)
		{
			if(IsPostBack == false)
				LoadImagesByTask();
		}

		protected void m_btnAPMGetImages_Click(object sender, EventArgs e)
		{
			LoadImagesByTask();
		}

		private void LoadImagesByTask()
		{
			try
			{
				DoLoadImagesByTask();
			}
			catch(AggregateException age)
			{
				age.Handle(ex => true);
			}
		}

		private void DoLoadImagesByTask()
		{
			m_lblPic1.Text = m_lblPic2.Text = string.Empty;

			With.TryActionAsync(() =>
			                    {
			                    	var requests = new[]
			                    	               {
			                    	               	WebRequest.Create(_imageUris[0]),
			                    	               	WebRequest.Create(_imageUris[1])
			                    	               };

			                    	Task<WebResponse>[] responseTasks = requests.Select(r => r.GetResponseAsync()).ToArray();

			                    	Task.Factory
			                    		.ContinueWhenAll(responseTasks,
			                    		                 rts =>
			                    		                 {
			                    		                 	for(int i = 0; i < rts.Length; i++)
			                    		                 	{
			                    		                 		using(var response = rts[i].Result)
			                    		                 		{
			                    		                 			Label lbl = (i == 0) ? m_lblPic1 : m_lblPic2;
			                    		                 			lbl.Text = String.Format("Image at {0} is {1:N0} bytes",
			                    		                 			                         response.ResponseUri, response.ContentLength);
			                    		                 		}
			                    		                 	}
			                    		                 },
			                    		                 TaskContinuationOptions.None)
			                    		.Wait();
			                    });
		}
	}
}