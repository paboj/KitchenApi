using Humanizer;
using Kitchen.Core.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Kitchen.Infrastructure.Middleware;

internal sealed class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
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
