using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.Threading;

namespace NSoft.NFramework.WebHost.Async
{
    public partial class AsyncPage : System.Web.UI.Page
    {
        private static readonly Random Rnd = new ThreadSafeRandom();

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();

            Response.Write("<br/>1. LocalData['ProductName']=" + HttpContext.Current.Items["ProductName"]);

            var context = HttpContext.Current;

            var task1 = Task.Factory.StartNew(() => DoProcessAsync(context));
            var task2 = Task.Factory.StartNew(() => DoProcessAsync2(context, "second task"));

            // 다른 작업...
            // Thread.Sleep(Rnd.Next(10, 20));
            Task.Factory.StartNewDelayed(Rnd.Next(10, 20)).Wait();

            Response.Write("<br/>.................................<br/>");
            Response.Write("<br/>4. LocalData['ProductName']=" + HttpContext.Current.Items["ProductName"]);

            Task.WaitAll(task1, task2);
        }

        /// <summary>
        /// 실제 작업을 처리하는 함수.
        /// </summary>
        protected virtual void DoProcessAsync(HttpContext context)
        {
            Task.Factory.StartNewDelayed(Rnd.Next(100, 200)).Wait();

            Response.Write("<br/>DoProcessTask를 수행하였습니다. ThreadId=" + Thread.CurrentThread.ManagedThreadId);
            Response.Write("<br/>2. LocalData['ProductName']=" + context.Items["ProductName"]);
        }

        /// <summary>
        /// 실제 작업을 처리하는 함수.
        /// </summary>
        protected virtual void DoProcessAsync2(HttpContext context, string state)
        {
            Task.Factory.StartNewDelayed(Rnd.Next(100, 200)).Wait();

            Response.Write("<br/>DoProcessTask2를 수행하였습니다.  state=" + state + ", ThreadId=" + Thread.CurrentThread.ManagedThreadId);
            Response.Write("<br/>3. LocalData['ProductName']=" + context.Items["ProductName"]);
        }
    }
}