using Humanizer;
using Kitchen.Core.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Kitchen.Infrastructure.Middleware;

internal sealed class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(exception, context);
        }
    }

    private static async Task HandleExceptionAsync(Exception exception, HttpContext context)
    {
        var statusCode = exception switch
        {
            // 404 Not Found
            ProductDefinitionNotFoundException or
            StockItemNotFoundException => StatusCodes.Status404NotFound,

            // 409 Conflict
            ProductDefinitionAlreadyExistsException => StatusCodes.Status409Conflict,

            // 400 Bad Request (domain validation)
            InvalidProductNameException or
            IncorrectAmountException or
            UnknownLocationException or
            UnknownCategoryException or
            UnknownUnitTypeException => StatusCodes.Status400BadRequest,

            // 400 Bad Request (other domain)
            KitchenApiException => StatusCodes.Status400BadRequest,

            // 500 Internal Server Error
            _ => StatusCodes.Status500InternalServerError
        };

        var errorCode = exception.GetType().Name.Replace("Exception", string.Empty).Underscore();

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new Error(errorCode, exception.Message));
    }

    private record Error(string Code, string Message);
}
