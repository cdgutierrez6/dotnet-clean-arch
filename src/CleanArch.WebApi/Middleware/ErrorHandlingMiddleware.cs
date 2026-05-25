using CleanArch.Domain.Exceptions;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace CleanArch.WebApi.Middleware;

public sealed class ErrorHandlingMiddleware(
    RequestDelegate next,
    ILogger<ErrorHandlingMiddleware> logger
)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning("Validation failed: {Errors}", ex.Message);
            await WriteResponseAsync(context, HttpStatusCode.UnprocessableEntity, new
            {
                type = "validation_error",
                errors = ex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning("Resource not found: {Message}", ex.Message);
            await WriteResponseAsync(context, HttpStatusCode.NotFound, new
            {
                type = "not_found",
                message = ex.Message
            });
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Domain rule violation: {Message}", ex.Message);
            await WriteResponseAsync(context, HttpStatusCode.BadRequest, new
            {
                type = "domain_error",
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await WriteResponseAsync(context, HttpStatusCode.InternalServerError, new
            {
                type = "server_error",
                message = "An unexpected error occurred."
            });
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode status, object body)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;
        await context.Response.WriteAsync(JsonSerializer.Serialize(body));
    }
}
