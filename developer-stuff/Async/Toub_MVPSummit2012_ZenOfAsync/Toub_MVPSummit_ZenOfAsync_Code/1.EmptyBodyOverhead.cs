using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#pragma warning disable 1998 // async method lacks 'await'
#pragma warning disable 4014 // async method not awaited

class EmptyBodyOverhead
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void EmptyBody() { }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static async Task EmptyBodyAsync() { }

    internal static void Run()
    {
        var sw = new Stopwatch();
        const int ITERS = 10000000;

        EmptyBody();
        EmptyBodyAsync();

        while (true)
        {
            sw.Restart();
            for (int i = 0; i < ITERS; i++) EmptyBody();
            var emptyBodyTime = sw.Elapsed;

            sw.Restart();
            for (int i = 0; i < ITERS; i++) EmptyBodyAsync();
            var emptyBodyAsyncTime = sw.Elapsed;

            Console.WriteLine("Sync  : {0}", emptyBodyTime);
            Console.WriteLine("Async : {0}", emptyBodyAsyncTime);
            Console.WriteLine("-- {0:F1}x --", emptyBodyAsyncTime.TotalSeconds / emptyBodyTime.TotalSeconds);
        }
    }
}