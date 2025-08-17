using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using OweMe.Api.Benchmarks.Application;
using Shouldly;

namespace OweMe.Api.Benchmarks;

public class Runner(ITestOutputHelper helper)
{
    [Fact]
    public void RunBenchmarks()
    {
        var logger = new AccumulationLogger();

        var config = ManualConfig.Create(DefaultConfig.Instance)
            .AddLogger(logger)
            .WithOptions(ConfigOptions.DisableOptimizationsValidator);

        var results = BenchmarkRunner.Run<PerformanceMiddlewareBenchmarks>(config);
        helper.WriteLine(logger.GetLog());

        results.ShouldNotBeNull();
        results.Reports.ShouldNotBeEmpty();
    }
}