using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Middleware;

public sealed class RequestLoggingAndErrorHandlingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingAndErrorHandlingMiddleware> logger,
    IHostEnvironment environment)
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            logger.LogInformation(
                "HTTP request started {Method} {Path} TraceId={TraceId}",
                context.Request.Method,
                context.Request.Path.Value,
                context.TraceIdentifier);

            await next(context).ConfigureAwait(false);

            stopwatch.Stop();
            logger.LogInformation(
                "HTTP request completed {Method} {Path} StatusCode={StatusCode} ElapsedMs={ElapsedMs} TraceId={TraceId}",
                context.Request.Method,
                context.Request.Path.Value,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                context.TraceIdentifier);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            stopwatch.Stop();
            logger.LogWarning(
                "HTTP request cancelled {Method} {Path} ElapsedMs={ElapsedMs} TraceId={TraceId}",
                context.Request.Method,
                context.Request.Path.Value,
                stopwatch.ElapsedMilliseconds,
                context.TraceIdentifier);
            throw;
        }
        catch (Exception exception)
        {
            stopwatch.Stop();

            logger.LogError(
                exception,
                "Unhandled exception while processing {Method} {Path} ElapsedMs={ElapsedMs} TraceId={TraceId}",
                context.Request.Method,
                context.Request.Path.Value,
                stopwatch.ElapsedMilliseconds,
                context.TraceIdentifier);

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Ocorreu um erro inesperado ao processar a requisição.",
                Detail = environment.IsDevelopment()
                    ? exception.Message
                    : "A operação não pôde ser concluída no momento.",
                Instance = context.Request.Path.Value,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1"
            };

            await JsonSerializer.SerializeAsync(
                context.Response.Body,
                problem,
                SerializerOptions,
                context.RequestAborted).ConfigureAwait(false);
        }
    }
}
