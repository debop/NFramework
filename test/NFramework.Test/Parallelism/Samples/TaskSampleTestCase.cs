using System;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.Parallelism.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.Samples {
    [TestFixture]
    public class TaskSampleTestCase {
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

        [Test]
        public void Parent_Child_Task() {
            const int ChildCount = 5;
            var cts = new CancellationTokenSource();

            var parent = new Task<int[]>(() => {
                                             var results = new int[ChildCount];

                                             for(int i = 0; i < ChildCount; i++) {
                                                 var index = i;
                                                 Task.Factory.StartNew(() => results[index] = Sum((index + 1) * 10000, cts.Token),
                                                                       TaskCreationOptions.AttachedToParent);
                                             }

                                             return results;
                                         });

            var succeedTask = parent.ContinueWith(antecedent => Array.ForEach(antecedent.Result, Console.WriteLine));

            parent.Start();

            succeedTask.WaitForCompletionStatus();
        }
    }
}