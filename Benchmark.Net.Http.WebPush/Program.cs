using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using System;

namespace Benchmark.Net.Http.WebPush
{
    class Program
    {
        static void Main(string[] args)
        {
            Summary webPushBenchmarksSummary = BenchmarkRunner.Run<WebPushBenchmarks>();

            Console.ReadKey();
        }
    }
}
