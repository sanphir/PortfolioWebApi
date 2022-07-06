using Portfolio.WebApi.DTO;
using System.Net;
using System.Text.Json;

namespace Portfolio.WebApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Handled exception");

            context.Response.ContentType = "application/json";
            var response = context.Response;
            switch (exception)
            {
                case Exception:
                default:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    var errorResponse = JsonSerializer.Serialize(new ErrorResponseDTO { Error = exception.Message });
                    await response.WriteAsync(errorResponse);
                    break;

            }
        }
    }
}
