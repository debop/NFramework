using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

class DictionaryCaching
{
    const int ITERS = 10000000;

    private static ConcurrentDictionary<string, string> s_urlToStringContents = new ConcurrentDictionary<string, string>();
    private static ConcurrentDictionary<string, Task<string>> s_urlToTaskContents = new ConcurrentDictionary<string, Task<string>>();
    private static HttpClient s_httpClient = new HttpClient();

    public static async Task<string> GetContents1Async(string url)
    {
        string contents;
        if (!s_urlToStringContents.TryGetValue(url, out contents))
        {
            contents = await s_httpClient.GetStringAsync(url);
            s_urlToStringContents.TryAdd(url, contents);
        }
        return contents;
    }

    public static Task<string> GetContents2Async(string url)
    {
        Task<string> contents;
        if (!s_urlToTaskContents.TryGetValue(url, out contents))
        {
            contents = s_httpClient.GetStringAsync(url);
            contents.ContinueWith((t, state) => s_urlToTaskContents.TryAdd((string)state, t),
               url,
               CancellationToken.None,
               TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously,
               TaskScheduler.Default);
        }
        return contents;
    }


    internal static void Run()
    {
        string url = "http://www.microsoft.com";
        GetContents1Async(url).Wait();
        GetContents2Async(url).Wait();

        var sw = new Stopwatch();
        while (true)
        {
            sw.Restart();
            for (int i = 0; i < ITERS; i++) GetContents1Async(url).Wait();
            var cacheStringTime = sw.Elapsed;

            sw.Restart();
            for (int i = 0; i < ITERS; i++) GetContents2Async(url).Wait();
            var cacheTaskTime = sw.Elapsed;

            Console.WriteLine("Cache string : {0}", cacheStringTime);
            Console.WriteLine("Cache task   : {0}", cacheTaskTime);
            Console.WriteLine("---- {0:F2}x ----", cacheStringTime.TotalSeconds / cacheTaskTime.TotalSeconds);
            Console.ReadLine();
        }
    }
}