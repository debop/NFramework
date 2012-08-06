using System;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;

class FlowingContext
{
    const int ITERS = 100000;

    private static async Task DoWorkAsync()
    {
        for (int i = 0; i < ITERS; i++)
        {
            await Task.Yield();
        }
    }

    internal static void Run()
    {
        var sw = new Stopwatch();
        while (true)
        {
            CallContext.LogicalSetData("Foo", "Bar"); // changes from default context

            sw.Restart();
            DoWorkAsync().Wait();
            var withTime = sw.Elapsed;

            CallContext.FreeNamedDataSlot("Foo"); // back to default context

            sw.Restart();
            DoWorkAsync().Wait();
            var withoutTime = sw.Elapsed;

            Console.WriteLine("With    : {0}", withTime);
            Console.WriteLine("Without : {0}", withoutTime);
            Console.WriteLine("---- {0:F2}x ----", withTime.TotalSeconds / withoutTime.TotalSeconds);
            Console.ReadLine();
        }
    }
}