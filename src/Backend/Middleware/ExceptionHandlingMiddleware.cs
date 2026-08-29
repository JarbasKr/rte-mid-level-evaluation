using System.Net;
using System.Text.Json;
using FluentValidation;
using Rte.Api.DTOs;
using Rte.Api.Exceptions;

namespace Rte.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleAsync(context, exception);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        var response = exception switch
        {
            AppException app => new ErrorResponse { Message = app.Message, StatusCode = app.StatusCode },
            ValidationException validation => new ErrorResponse
            {
                Message = "Dados inválidos.",
                StatusCode = StatusCodes.Status400BadRequest,
                Errors = validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
            },
            _ => new ErrorResponse
            {
                Message = "Ocorreu um erro interno ao processar a requisição.",
                StatusCode = StatusCodes.Status500InternalServerError
            }
        };

        if (response.StatusCode >= 500)
        {
            _logger.LogError(exception, "Erro não tratado");
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.StatusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
