using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CleanArch.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;
        var sw = Stopwatch.StartNew();

        logger.LogInformation("Handling {Request}", requestName);

        try
        {
            var response = await next();
            sw.Stop();
            logger.LogInformation("Handled {Request} in {Elapsed}ms", requestName, sw.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            logger.LogError(ex, "Error handling {Request} after {Elapsed}ms", requestName, sw.ElapsedMilliseconds);
            throw;
        }
    }
}
