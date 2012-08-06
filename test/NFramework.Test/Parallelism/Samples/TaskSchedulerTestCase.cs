using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NSoft.NFramework.Threading;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.Samples {
    [TestFixture]
    public class TaskSchedulerTestCase {
        [Test]
        [Ignore("Windows.Forms 를 사용하여 테스트할 환경이 안됩니다.")]
        public void Can_CreateForm() {
            using(var form = new MyForm()) {
                form.Show();
                form.Update();

                ThreadTool.Sleep(5000);
            }
        }

        // Unit Test 에서는 동작하지 않는다.
        //
        internal sealed class MyForm : Form {
            public MyForm() {
                Text = @"Synchronoization Context Task Scheduler Demo";
                Visible = true;
                Width = 400;
                Height = 100;
            }

            // NOTE: WinForm, WPF, Silverlight 등 UI Application에서는 TaskScheduler.Default를 사용하는 것이 아니라.
            // NOTE: TaskScheduler.FromCurrentSynchronizationContext()를 사용해야 합니다.
            //
            private readonly TaskScheduler _syncContextTaskScheduler = TaskScheduler.Default;
                                           //TaskScheduler.FromCurrentSynchronizationContext();

            private CancellationTokenSource _cts;

            protected override void OnMouseClick(MouseEventArgs e) {
                if(_cts != null) {
                    _cts.Cancel();
                    _cts = null;
                }
                else {
                    Text = @"Operation running";
                    _cts = new CancellationTokenSource();

                    var task = new Task<int>(() => Sum(2000, _cts.Token), _cts.Token);
                    task.Start();

                    // UI 작업에서는 SynchronizationTaskScheduler를 사용해야 합니다.
                    task.ContinueWith(antecedent => Text = "Result:" + antecedent.Result,
                                      CancellationToken.None,
                                      TaskContinuationOptions.OnlyOnRanToCompletion,
                                      _syncContextTaskScheduler);

                    task.ContinueWith(antecedent => Text = "Operation canceled",
                                      CancellationToken.None,
                                      TaskContinuationOptions.OnlyOnCanceled,
                                      _syncContextTaskScheduler);

                    task.ContinueWith(antecedent => Text = "Operation faulted",
                                      CancellationToken.None,
                                      TaskContinuationOptions.OnlyOnFaulted,
                                      _syncContextTaskScheduler);
                }
                base.OnMouseClick(e);
            }

            private static int Sum(int n, CancellationToken cancelToken) {
                int sum = 0;

                for(; n > 0; n--) {
                    cancelToken.ThrowIfCancellationRequested();
                    checked {
                        sum += n;
                    }
                }
                return sum;
            }
        }
    }
}