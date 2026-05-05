using System.Net;
using System.Text.Json;
using Kitchen.Core.Domain.Exceptions;

namespace Kitchen.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = exception switch
            {
                // 404 Not Found
                IngredientNotFoundException => HttpStatusCode.NotFound,

                // 400 Bad Request (validation errors & incorrect data)
                InvalidIngredientNameException or
                IncorrectAmountException or
                UnknownLocationException or
                UnknownUnitTypeException => HttpStatusCode.BadRequest,

                // 409 Conflict (already exists)
                IngredientAlreadyExistsException or
                IngredientTypeAlreadyExistsException => HttpStatusCode.Conflict,

                // 400 Bad Request (other from Kitchen.Api)
                KitchenApiException => HttpStatusCode.BadRequest,

                // 500 Internal Server Error (unexpected)
                _ => HttpStatusCode.InternalServerError
            };

            var result = JsonSerializer.Serialize(new
            {
                error = exception.Message,
                code = code.ToString(),
                type = exception.GetType().Name
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }
}