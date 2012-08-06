using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable 1998 // async method lacks await

class ReadAsync
{
    const int ITERS = 100;

    internal static void Run()
    {
        byte[] data = new byte[0x10000000];
        while (true)
        {
            TrackGcs(new NaiveReadMemoryStream(data));
            TrackGcs(new FasterReadMemoryStream(data));
            Console.ReadLine();
        }
    }

    static void TrackGcs(Stream input)
    {
        int gen0 = GC.CollectionCount(0), gen1 = GC.CollectionCount(1), gen2 = GC.CollectionCount(2);
        for (int i = 0; i < ITERS; i++)
        {
            input.Position = 0;
            input.CopyToAsync(Stream.Null).Wait();
        }
        int newGen0 = GC.CollectionCount(0), newGen1 = GC.CollectionCount(1), newGen2 = GC.CollectionCount(2);
        Console.WriteLine("{0}\tGen0:{1}   Gen1:{2}   Gen2:{3}", input.GetType().Name, newGen0 - gen0, newGen1 - gen1, newGen2 - gen2);
    }

    class NaiveReadMemoryStream : MemoryStream
    {
        public NaiveReadMemoryStream(byte[] data) : base(data) { }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Read(buffer, offset, count);
        }
    }

    class FasterReadMemoryStream : MemoryStream
    {
        public FasterReadMemoryStream(byte[] data) : base(data) { }

        private Task<int> m_lastTask;

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                var tcs = new TaskCompletionSource<int>();
                tcs.SetCanceled();
                return tcs.Task;
            }

            try
            {
                int numRead = Read(buffer, offset, count);
                if (m_lastTask != null && m_lastTask.Result == numRead) return m_lastTask;
                m_lastTask = Task.FromResult(numRead);
                return m_lastTask;
            }
            catch (Exception exc)
            {
                var tcs = new TaskCompletionSource<int>();
                tcs.SetException(exc);
                return tcs.Task;
            }
        }
    }
}