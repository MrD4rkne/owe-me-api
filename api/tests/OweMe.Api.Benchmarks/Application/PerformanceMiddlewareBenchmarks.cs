using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OweMe.Application.Common;
using OweMe.Application.Common.Middlewares;
using Wolverine;

namespace OweMe.Api.Benchmarks.Application;

[MemoryDiagnoser]
public class PerformanceMiddlewareBenchmarks
{
    private readonly TestMessageContext _context = new();

    private static PerformanceMiddleware CreateMiddleware()
    {
        var logger = new Mock<ILogger<PerformanceMiddleware>>();
        var options = Options.Create(new ApplicationOptions { TooLongRequestThresholdMs = 500 });
        return new PerformanceMiddleware(logger.Object, options);
    }

    [Benchmark]
    public void PerformanceMiddleware_Before()
    {
        CreateMiddleware().Before(_context);
    }

    [Benchmark]
    public void PerformanceMiddleware_Finally()
    {
        CreateMiddleware().Finally(_context);
    }
}