using System.Diagnostics;
using System.Data.Common;
using System.Text.Json;
using ClinicFlow.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                "Requisição HTTP iniciada {Method} {Path} TraceId={TraceId}",
                context.Request.Method,
                context.Request.Path.Value,
                context.TraceIdentifier);

            await next(context).ConfigureAwait(false);

            stopwatch.Stop();
            logger.LogInformation(
                "Requisição HTTP concluída {Method} {Path} StatusCode={StatusCode} ElapsedMs={ElapsedMs} TraceId={TraceId}",
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
                "Requisição HTTP cancelada {Method} {Path} ElapsedMs={ElapsedMs} TraceId={TraceId}",
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
                "Exceção não tratada ao processar {Method} {Path} ElapsedMs={ElapsedMs} TraceId={TraceId}",
                context.Request.Method,
                context.Request.Path.Value,
                stopwatch.ElapsedMilliseconds,
                context.TraceIdentifier);

            if (context.Response.HasStarted)
            {
                throw;
            }

            var problem = CreateProblemDetails(exception, context);

            context.Response.Clear();
            context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            await JsonSerializer.SerializeAsync(
                context.Response.Body,
                problem,
                SerializerOptions,
                context.RequestAborted).ConfigureAwait(false);
        }
    }

    private ProblemDetails CreateProblemDetails(Exception exception, HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(exception);
        ArgumentNullException.ThrowIfNull(context);

        var baseException = exception.GetBaseException();

        if (exception is DomainValidationException domainValidationException)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Uma ou mais validações de domínio falharam.",
                Detail = string.Join(" ", domainValidationException.Errors),
                Instance = context.Request.Path.Value,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            };
        }

        if (exception is InvalidOperationException)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = exception.Message,
                Detail = baseException.Message,
                Instance = context.Request.Path.Value,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.10"
            };
        }

        if (exception is DbUpdateException)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Não foi possível salvar as alterações no momento.",
                Detail = baseException.Message,
                Instance = context.Request.Path.Value,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1"
            };
        }

        if (exception is DbException)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status503ServiceUnavailable,
                Title = "Não foi possível concluir a operação no momento.",
                Detail = baseException.Message,
                Instance = context.Request.Path.Value,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.4"
            };
        }

        return new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Ocorreu um erro inesperado ao processar a requisição.",
            Detail = environment.IsDevelopment()
                ? baseException.Message
                : "A operação não pôde ser concluída no momento.",
            Instance = context.Request.Path.Value,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1"
        };
    }
}
