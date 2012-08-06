using System;
using System.Threading.Tasks;

#pragma warning disable 1998 // async method lacks 'await'

class EmptyBodyGcs
{
    private static async Task EmptyBodyAsync() { }

    internal static void Run()
    {
        const int ITERS = 10000000;

        while (true)
        {
            GC.Collect();
            int gen0 = GC.CollectionCount(0), gen1 = GC.CollectionCount(1), gen2 = GC.CollectionCount(2);

            for (int i = 0; i < ITERS; i++) 
                EmptyBodyAsync().Wait();

            int newGen0 = GC.CollectionCount(0), newGen1 = GC.CollectionCount(1), newGen2 = GC.CollectionCount(2);

            Console.WriteLine("Gen0: {0}    Gen1: {1}    Gen2: {2}", newGen0 - gen0, newGen1 - gen1, newGen2 - gen2);
        }
    }
}