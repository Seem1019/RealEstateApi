using RealEstate.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace RealEstate.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            var statusCode = HttpStatusCode.InternalServerError;
            var message = "Internal Server Error";

            switch (ex)
            {
                case NotFoundException _:
                    statusCode = HttpStatusCode.NotFound;
                    message = ex.Message;
                    break;
                case ValidationException _:
                    statusCode = HttpStatusCode.BadRequest;
                    message = ex.Message;
                    break;
                case BusinessRuleException _:
                    statusCode = HttpStatusCode.Conflict;
                    message = ex.Message;
                    break;
            }

            context.Response.StatusCode = (int)statusCode;
            var result = JsonSerializer.Serialize(new { error = message });
            return context.Response.WriteAsync(result);
        }
    }
}