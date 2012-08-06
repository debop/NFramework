using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

class CapturingContext
{
    const int ITERS = 50000;

    private static async Task WithSyncCtx()
    {
        for (int i = 0; i < ITERS; i++)
        {
            var t = Task.Run(() => { });
            await t;
        }
    }

    private static async Task WithoutSyncCtx()
    {
        for (int i = 0; i < ITERS; i++)
        {
            var t = Task.Run(() => { });
            await t.ConfigureAwait(continueOnCapturedContext:false);
        }
    }

    internal static void Run()
    {
        var f = new Form() { Width = 400, Height = 300 };
        var b = new Button() { Text = "Run", Dock = DockStyle.Fill, Font = new Font("Consolas", 18) };
        f.Controls.Add(b);

        b.Click += async delegate
        {
            b.Text = "... Running ... ";
            await Task.WhenAll(WithSyncCtx(), WithoutSyncCtx()); // warm-up

            var sw = new Stopwatch();

            sw.Restart();
            await WithSyncCtx();
            var withTime = sw.Elapsed;

            sw.Restart();
            await WithoutSyncCtx();
            var withoutTime = sw.Elapsed;

            b.Text = string.Format("With    : {0}\nWithout : {1}\n\nDiff    : {2:F2}x", 
                withTime, withoutTime, withTime.TotalSeconds / withoutTime.TotalSeconds);
        };

        f.ShowDialog();
    }
}